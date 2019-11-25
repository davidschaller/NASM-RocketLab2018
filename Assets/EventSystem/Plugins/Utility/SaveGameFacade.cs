using System.Reflection;
using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable()]
public class SaveGameFacade : ISerializable
{
    public System.String saveName; // not guaranteed to be accurate, use as a convenience!
    
    public PCData pcData;
    
    [Serializable()]
    public class PCData : ISerializable
    {
        public string name;
		public Vector3 location;
        public Quaternion rotation;
        public string scene;
        
        public PCData()
        {
        }
        
        public PCData(SerializationInfo info, StreamingContext ctxt)
        {
            name = (string)info.GetValue("Name", typeof(string));
            
			location.x = (float)info.GetValue("Location.x", typeof(float));
			location.y = (float)info.GetValue("Location.y", typeof(float));
			location.z = (float)info.GetValue("Location.z", typeof(float));
			
			Vector3 rot;
            rot.x = (float)info.GetValue("Rotation.x", typeof(float));
			rot.y = (float)info.GetValue("Rotation.y", typeof(float));
			rot.z = (float)info.GetValue("Rotation.z", typeof(float));
			rotation.eulerAngles = rot;
			
            scene = (string)info.GetValue("Scene", typeof(string));
			
            try
            {
				// stuff added after original code written...
            }
            catch(Exception)
            {
                //Debug.Log("No stranger stats to load...");
            }
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Name", name);
            info.AddValue("Location.x", location.x);
			info.AddValue("Location.y", location.y);
			info.AddValue("Location.z", location.z);
			
            info.AddValue("Rotation.x", rotation.eulerAngles.x);
			info.AddValue("Rotation.y", rotation.eulerAngles.y);
			info.AddValue("Rotation.z", rotation.eulerAngles.z);
			
            info.AddValue("Scene", scene);
        }
    }
    
    public SaveGameFacade()
    {
        saveName = "Texas.texas";
    }
    
    public SaveGameFacade(SerializationInfo info, StreamingContext ctxt)
    {
        try
        {
            // new vars go here (so as not to break old saves)
        }
        catch(Exception)
        {
            // eat the exception for old save format
        }
        
        pcData = (PCData)info.GetValue("PCData", typeof(PCData));
    }
    
    public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
    {
        info.AddValue("PCData", pcData);   
    }
    
    
    public static SaveGameFacade NewSave( System.String saveName )
    {
        SaveGameFacade sg = new SaveGameFacade();
        sg.saveName = saveName;
        return sg;
    }
    
    public static bool Serialize<T>(T Data, string Filename)
	{
		string m_FilePath = Filename;
		bool m_Error = false;

		// Using a file stream.			
		FileStream m_FileStream = new FileStream(m_FilePath, FileMode.Create);

		// Construct a BinaryFormatter and use it to serialize the data to the stream.
		BinaryFormatter m_Formatter = new BinaryFormatter();
		try
		{
			m_Formatter.Serialize(m_FileStream, (T)Data);
		}
		catch (SerializationException e)
		{
			Console.WriteLine("Serializer: failed to serialize \"{0}\". Reason: {1}", Filename, e.Message);
			m_Error = true;
		}
		finally
		{
			m_FileStream.Close();
		}

		return !m_Error;
	}
    
    public static T Deserialize<T>(string Filename)
	{
		string m_FilePath = Filename;
		T m_Data = default(T);

		// If the file exists...
		if (System.IO.File.Exists(m_FilePath))
		{
			// Open the file containing the data that you want to deserialize.
			FileStream m_FileStream = null;
			try
			{
				m_FileStream = new FileStream(m_FilePath, FileMode.Open);
			}
			catch(IOException io)
			{
				Console.WriteLine("Failed to open file stream: " + io.Message);
			}
			if(m_FileStream != null)
			{
				try
				{
					BinaryFormatter m_Formatter = new BinaryFormatter();

					// Deserializing data.
					m_Data = (T)m_Formatter.Deserialize(m_FileStream);

				}
				catch (SerializationException e)
				{
					Console.WriteLine("Serializer: failed to deserialize \"{0}\". Reason: {1}", Filename, e.Message);
					return default(T);
				}
				finally
				{
					Console.WriteLine("Closing file stream for " + Filename);
					m_FileStream.Close();
				}
			}
		}

		return m_Data;

	}

    public void Persist()
    {
        try
        {
            Serialize<SaveGameFacade>(this, saveName);
        }
        catch (Exception e)
        {
            Debug.Log("Failed to save game: " + e);
        }
    }
    
    public static SaveGameFacade ReadSave( System.String saveName )
    {
        SaveGameFacade sg;
        //Debug.Log("version:"+Assembly.GetExecutingAssembly().GetName().ToString());

        System.Boolean success = false;
        try
        {
            sg = Deserialize<SaveGameFacade>(saveName);
            success = true;
        }
        catch (FileNotFoundException)
        {
            //Debug.Log("Could not load new format: " + e.FileName + ", trying old one...");
            Debug.Log("Could not load new format, trying old one...");
            sg = null;
        }

        if (!success)
        {
			Stream stream = null;
            try
            {
                stream = new FileStream( saveName, FileMode.Open, FileAccess.Read, FileShare.Read );
                stream.Position =0;
                
                BinaryFormatter OldFormatter = new BinaryFormatter();
                OldFormatter.Binder = new VersionConfigToNamespaceAssemblyObjectBinder();
                sg = (SaveGameFacade) OldFormatter.Deserialize(stream);
				Debug.Log("Success loading " + saveName + ", sg: " + sg.pcData);
            }
            catch (Exception e)
            {
                Debug.Log("Exception loading old format" + saveName + ": " + e);
            }
			finally
			{
				stream.Close();
			}
        }


        return sg;
    }
}
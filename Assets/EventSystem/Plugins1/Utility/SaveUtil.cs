using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Reflection;

public class SaveUtil
{
	static string PSC = "/";

	public static string SavePath
	{
		get
		{
			return GetSavePath(ContextManager.CurrentContext);
		}
		
		private set
		{
			
		}
	}
	
	static string GetSavePath( TexasContext cont )
	{
		string ret = "";
		PSC = System.IO.Path.DirectorySeparatorChar.ToString();

		switch (cont)
		{
			case TexasContext.Development:
				if (Application.platform == RuntimePlatform.WindowsPlayer)    
			    {
			        ret = System.Environment.GetEnvironmentVariable("USERPROFILE")+PSC+"My Documents";
			    }
			    else
			    {
			        ret = System.Environment.GetEnvironmentVariable("HOME");
			    }
			    ret += PSC + "Texas" + PSC;
				if (!System.IO.Directory.Exists(ret))
			    {
			        Debug.Log("CREATING SAVED GAME PATH: " + ret);
			        System.IO.Directory.CreateDirectory(ret);
			    }
				break;
			case TexasContext.WebTestDeployment:
				ret = "http://localhost/TexasSaveTest.php";
				break;
			case TexasContext.ClientDeployment:
				ret = "http://";
				break;
		}
		
		

		return ret;
	}
	
#if !UNITY_WEBPLAYER
	public class CompareFileInfo : IComparer
	{
		int IComparer.Compare(object x, object y)
	    {
	        System.IO.FileInfo file1;
	        System.IO.FileInfo file2;

	        file1 = (System.IO.FileInfo)y;
	        file2 = (System.IO.FileInfo)x;

	        return System.DateTime.Compare(file1.LastWriteTime, file2.LastWriteTime);
	    }
	}
#endif
	
#if !UNITY_WEBPLAYER
	static void PopulateCache ()
	{
		Debug.Log("PopulateCache() at " + Time.time);
	    cachedDescriptors = new List<SaveDescriptor>();

	    System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(SaveUtil.SavePath);
	
		List<System.IO.FileInfo> fileList = new List<System.IO.FileInfo>();
		System.IO.FileInfo[] cachedFiles;
	    cachedFiles = directory.GetFiles("*.texas");
		Dictionary<System.IO.FileInfo, string> dirLookup = new Dictionary<System.IO.FileInfo, string>();
		for (int i=0;i<cachedFiles.Length;i++)
		{
			fileList.Add(cachedFiles[i]);
			dirLookup.Add(cachedFiles[i], SaveUtil.SavePath);
		}
	

		System.IO.FileInfo[] staticList = fileList.ToArray();
		System.Array.Sort(staticList, new CompareFileInfo());
	    for (int i = 0;i<staticList.Length;i++)
	    {
			SaveDescriptor sd = new SaveDescriptor();
			
	        System.IO.FileInfo finfo = staticList[i];
			
			sd.SaveName = finfo.Name;
	        sd.Modified = finfo.LastWriteTime.ToString("g");
			sd.Path = dirLookup[finfo];
			
			sd.SaveFormat = TexasContext.Development;
			
	        cachedDescriptors.Add(sd);
	    }
	
		Debug.Log(cachedDescriptors.Count + " saved games, " + fileList.Count + " file list length");
	}
	
#endif

	static SaveDescriptor[] saves;
	static public SaveDescriptor[] Saves
	{
		get
		{
			if (saves == null)
			{
			 	saves = new SaveDescriptor[cachedDescriptors.Count];
			
				int saveCount = 0;
				foreach (SaveDescriptor sd in cachedDescriptors)
				{
					saves[saveCount]= sd;
					saveCount++;
				}
			}
			
			return saves;
		}
		set
		{
			saves = value;
		}
	}
	
	static public void Init ()
	{
		Saves = null;
#if !UNITY_WEBPLAYER
		PopulateCache();
#endif
	}
	
	static List<SaveDescriptor> cachedDescriptors = new List<SaveDescriptor>();
	static public List<SaveDescriptor> CachedDescriptors
	{
		get
		{
#if !UNITY_WEBPLAYER
			if (cachedDescriptors.Count == 0) PopulateCache();
#endif
			return cachedDescriptors;
		}
		private set
		{
			
		}
	}
}

sealed class VersionConfigToNamespaceAssemblyObjectBinder : SerializationBinder
{
    public override Type BindToType(string assemblyName, string typeName)
    {
        Type typeToDeserialize = null;

        try
        {
            // For each assemblyName/typeName that you want to deserialize to
            // a different type, set typeToDeserialize to the desired type.
            String assemVer1 = Assembly.GetExecutingAssembly().FullName;
            assemblyName = assemVer1;
            //Debug.Log("typenm lio.: " + typeName.LastIndexOf(".") + ", typename: "+ typeName);
            //typeName = "Namespace.Assembly.Object" + typeName.Substring(typeName.LastIndexOf("."),(typeName.Length-typeName.LastIndexOf(".")));

            typeToDeserialize = Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));
        }
        catch (System.Exception ex1)
        {
            throw ex1;
        }
        finally
        {
        }

        return typeToDeserialize;
    }
}

public interface ISaveGame
{
	void Persist();
}

public class SaveDescriptor
{
	string saveName;
	public string SaveName
	{
		get
		{
			return saveName;
		}
		set
		{
			saveName = value;
		}
	}
	
	TexasContext context = TexasContext.Development;
	public TexasContext Context
	{
		get
		{
			return context;
		}
		set
		{
			context = value;
		}
	}
	
	TexasContext saveFormat = TexasContext.Development;
	public TexasContext SaveFormat
	{
		get
		{
			return saveFormat;
		}
		set
		{
			saveFormat = value;
		}
	}
	
	string modified;
	public string Modified
	{
		get
		{
			return modified;
		}
		set
		{
			modified = value;
		}
	}
	
	string path;
	public string Path
	{
		get
		{
			return path;
		}
		set
		{
			path = value;
		}
	}
	
	public string PrettyName
	{
		get
		{
			return SaveName.Replace(".texas", "");
		}
		private set
		{
			
		}
	}
}

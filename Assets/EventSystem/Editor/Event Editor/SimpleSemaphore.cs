using UnityEngine;
using UnityEditor;
using System.Collections;

[System.Serializable]
public class SimpleSemaphore : ScriptableObject
{
	public int semaphore = 0;
	
	public int Value
	{
		get
		{
			int s = semaphore++;
			Debug.Log("Saving/returning semaphore " + s);
			AssetDatabase.SaveAssets();
			
			return s;
		}
		set
		{
			semaphore = value;
		}
	}
	
	public static SimpleSemaphore GetSemaphoreObject (string dataFile)
	{
		SimpleSemaphore sem = (SimpleSemaphore)AssetDatabase.LoadAssetAtPath(dataFile, typeof(SimpleSemaphore));
		if (sem == null)
		{
			sem = ScriptableObject.CreateInstance<SimpleSemaphore>();
			AssetDatabase.CreateAsset(sem, dataFile);
			AssetDatabase.SaveAssets();
		}
		return sem;
	}

}

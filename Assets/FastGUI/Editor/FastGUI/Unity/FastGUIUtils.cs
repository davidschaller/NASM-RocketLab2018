using UnityEngine;
using System.Collections;

public class FastGUIUtils : MonoBehaviour 
{
	
	public static Object selectedObject = null;
	public static string selectedFolderProjectRelative = null;
	public static GameObject rootGameObject = null;
	
	public static string GetProjectRelativePath(string pTarget)
	{
		string tReturn = pTarget.Substring(pTarget.ToString().IndexOf("Assets/"));
		
		return tReturn;
	}
	public static string GetParentFolderPath(string pTarget)
	{
		string tReturn = "";
		
		if(pTarget.IndexOf("\\")>=0)
			pTarget = pTarget.Replace("\\","/");
		
		tReturn = pTarget.Substring(0,pTarget.LastIndexOf("/"))+"/";
		
		return tReturn;
	}
	public static string GetParentFolder(string pTarget)
	{
		string tReturn = "";
		bool bRemoveLast = false;
		if(pTarget.IndexOf('/') >= 0)
		{
			string[] tSliced = pTarget.Split('/');
			
			for(int i = (tSliced.Length-2); i >= 0; i--)
			{
				tReturn = tSliced[i]+"/"+tReturn;
				bRemoveLast = true;
			}
			if(bRemoveLast)
			{
				tReturn = tReturn.Substring(0,tReturn.Length-1);
			}
			else
			{
				if(tReturn == "/")
					tReturn = "";
			}
			return tReturn;
		}
		else
		{
			return tReturn;
		}

	}	
}

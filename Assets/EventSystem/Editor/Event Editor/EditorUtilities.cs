using UnityEngine;
using UnityEditor;
using System.Collections;

public class EditorUtilities : MonoBehaviour
{
	[MenuItem("Event System/Modify items/Disable event player debugs")]
	static void DisableEPDebugs ()
	{
		EventPlayer[] eps = (EventPlayer[])Resources.FindObjectsOfTypeAll(typeof(EventPlayer));
		for(int i=0;i<eps.Length;i++)
		{
			EventPlayer ep = eps[i];
			
			ep.debugPlayer = false;
			ep.deepEventDebug = false;
		}
	}
}

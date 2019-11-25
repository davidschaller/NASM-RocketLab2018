using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class MTIUIBase : MonoBehaviour
{
	static List<MTIUIBase> uiElements = new List<MTIUIBase>();
	
	void Awake ()
	{
		uiElements.Add(this);
		
		//Debug.Log("Awake " + name);
	}
	
	bool wasActive = false;
	public void DeactivateUI (bool toggle)
	{
		//Debug.Log( (toggle ? "A" : "Dea") + "ctivating " + name + " for UI toggle...");
		
		if (!toggle && restoreActiveList != null && gameObject.active)
		{
			restoreActiveList.Add(this);
		}

		gameObject.SetActiveRecursively(toggle);
	}
	
	static List<MTIUIBase> restoreActiveList;
	public static IEnumerator TempToggle (float secondsToRestore, GameObject cameraPostTip, GameObject go)
	{
		restoreActiveList = new List<MTIUIBase>();
		
		foreach(MTIUIBase m in uiElements)
		{
			if (m != null)
				m.DeactivateUI(false);
		}
		
		yield return new WaitForSeconds(secondsToRestore);
		
		foreach(MTIUIBase m in restoreActiveList)
		{
			if (m != null)
				m.gameObject.SetActiveRecursively(true);
		}
		
		restoreActiveList = null;
		
		Debug.Log("Tip should be active...");
		
		if (cameraPostTip)
			cameraPostTip.active = true;
		
		
		yield return new WaitForSeconds(3);
		
		Debug.Log("Tip should be disabled...");
		
		if (cameraPostTip)
			cameraPostTip.active = false;
		Destroy(go);		
	}
	
	public static void ToggleUI (bool toggle)
	{
		foreach(MTIUIBase m in uiElements)
		{
			if (m != null)
				m.DeactivateUI(toggle);
		}
	}
}

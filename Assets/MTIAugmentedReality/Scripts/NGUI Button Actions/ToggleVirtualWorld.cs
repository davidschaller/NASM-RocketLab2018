using UnityEngine;
using System.Collections;

public class ToggleVirtualWorld : MTIUIBase
{
	public bool useUnityGUI = false;
	public bool virtualStartup = true;
	public GameObject[] deactivateWhenVirtual;
	public GameObject[] deactivateWhenReal;
	
	public static bool showVirtual = true;

	void Start ()
	{
		if (!CamTexture.camTextureActiveLastScene && virtualStartup)
		{
			VirtualCamera();
		}
		else if (!virtualStartup || CamTexture.camTextureActiveLastScene)
		{
			if (CamTexture.camTextureActiveLastScene)
				Debug.Log("Cam texture was active last scene load, activating it again...");
			
			RealCamera();
		}
	}

	void OnGUI ()
	{
		if (useUnityGUI)
		{
			string txt = showVirtual ? "Show Real" : "Show Virtual";
			if (GUI.Button(new Rect(Screen.width-100, Screen.height-100, 100, 100), txt))
			{
				ToggleVirtual();
			}
		}
	}
	
	void ToggleVirtual ()
	{
		showVirtual = !showVirtual;
		
		Debug.Log("Virtual world toggled " + (showVirtual ? "on" : "off"));
		
		if (showVirtual)
			VirtualCamera();
		else
			RealCamera();
	}
	
	void VirtualCamera ()
	{
		Camera.main.farClipPlane = UIController.Main.virtualCameraFarClip;
		
		Camera.main.clearFlags = CameraClearFlags.Skybox;
		foreach(GameObject go in deactivateWhenVirtual)
		{
			if (go != null)
				go.SetActiveRecursively(false);
			else
				Debug.LogWarning("Nothing assigned to entry in Deactivate When Virtual when switching to virtual cam", gameObject);
		}
		
		foreach(GameObject go in deactivateWhenReal)
		{
			if (go != null)
			{
				if (go.layer == LayerMask.NameToLayer("Walkable"))
					go.GetComponent<MeshRenderer>().enabled = true;
				else
					go.SetActiveRecursively(true);				
			}
			else
			{
				Debug.LogWarning("Nothing assigned to entry in Deactivate When Real when switching to virtual cam", gameObject);
			}
		}
		
		CamTexture.StopCam();
	}
	
	void RealCamera ()
	{
		CamTexture.StartCam();
		
		Camera.main.farClipPlane = UIController.Main.realCameraFarClip;
		Camera.main.clearFlags = CameraClearFlags.Depth;
		foreach(GameObject go in deactivateWhenVirtual)
		{
			if (go != null)
				go.SetActiveRecursively(true);
			else
				Debug.LogWarning("Nothing assigned to entry in Deactivate When Virtual when switching to real cam", gameObject);
		}
		
		foreach(GameObject go in deactivateWhenReal)
		{
			if (go != null)
			{
				if (go.layer == LayerMask.NameToLayer("Walkable"))
					go.GetComponent<MeshRenderer>().enabled = false;
				else
				go.SetActiveRecursively(false);				
			}
			else
			{
				Debug.LogWarning("Nothing assigned to entry in Deactivate When Real when switching to real cam", gameObject);
			}
		}
	}
	
	void OnClick ()
	{
		ToggleVirtual();
	}
}

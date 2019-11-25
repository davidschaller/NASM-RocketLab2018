using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIController : MonoBehaviour
{
	public GameObject ideTestPrefab;
	
	public GameObject iPhoneSDLandscapeUI;
	public GameObject iPhoneSDPortraitUI;
	public GameObject iPhoneHDLandscapeUI;
	public GameObject iPhoneHDPortraitUI;
	public GameObject iPadSDLandscapeUI;
	public GameObject iPadSDPortraitUI;
	public GameObject iPadHDLandscapeUI;
	public GameObject iPadHDPortraitUI;
	public GameObject androidPortraitUI;
	public GameObject androidLandscapeUI;
	
	GameObject activeUI;
	
	public float virtualCameraFarClip = 1000;
	public float realCameraFarClip = 500;
	
	public Texture2D virtualLeft;
	public Texture2D virtualRight;
	public Texture2D virtualForward;
	public Texture2D virtualBack;
	
	public float iPad3CameraFOV = 44;
	public float otherCameraFOV = 44;
	static float fov = 0;
	public static float FOV
	{
		get
		{
			return fov;
		}
	}
	
	public float lodMaxDistance = 50;
	public float behindLodMaxDistance = 5;
	public float inBuildingLODDistance = 5;
	public float deactivateMaxDistance = 100;
	
	static UIController main;
	public static UIController Main
	{
		get
		{
			if (main == null)
			{
				GameObject go = new GameObject("UI Controller (auto-spawn)");
				main = go.AddComponent<UIController>();
			}
			
			return main;
		}
	}
	
	void Awake ()
	{
		main = this;
		fov = otherCameraFOV;
		
		#if UNITY_IPHONE
		switch (iPhone.generation)
		{
			case iPhoneGeneration.iPhone3G:
			case iPhoneGeneration.iPhone3GS:
			SelectiPhoneSDLandscape();
			break;
			case iPhoneGeneration.iPhone4:
			case iPhoneGeneration.iPhone4S:
			SelectiPhoneHDLandscape();
			break;
			case iPhoneGeneration.iPad2Gen:
			SelectiPadSDLandscape();
			break;
			default:
			if (Application.isEditor)
			{
				iPhoneIDE();				
			}
			else
			{
				fov = iPad3CameraFOV;
				SelectiPadHDLandscape();				
			}
			break;
		}
		#elif UNITY_ANDROID
		SelectAndroidLandscape();
		#endif
	}
	
	void InvertNGUI (bool invert)
	{
		UIAnchor[] anchors = (UIAnchor[])FindObjectsOfType(typeof(UIAnchor));
		foreach(UIAnchor a in anchors)
		{
			if (a != null)
			{
				if (invert)
					a.transform.eulerAngles = new Vector3(a.transform.eulerAngles.x, a.transform.eulerAngles.y, 180);
				else
					a.transform.eulerAngles = new Vector3(a.transform.eulerAngles.x, a.transform.eulerAngles.y, 0);
			}
		}
		
		if (AboutThisAppButton.main)
		{
			Transform r = AboutThisAppButton.main.aboutThisAppRoot.transform.Find("Camera/Anchor");
				
			if (invert)
			{
				r.eulerAngles = new Vector3(r.eulerAngles.x, r.eulerAngles.y, 180);					
			}
			else
				r.eulerAngles = new Vector3(r.eulerAngles.x, r.eulerAngles.y, 0);
		}
	}
	
	DeviceOrientation lastOrientation;
	public static bool inverted = false;
	void Update ()
	{
		if (Application.isEditor && Input.GetKeyDown("r"))
		{
			inverted = !inverted;

			Transform t = activeUI.transform.Find("Camera/Anchor");
			
			if (t.eulerAngles.z == 0)
				InvertNGUI(true);
			else
				InvertNGUI(false);
						
			MoveWithCompass.invertCamera = !MoveWithCompass.invertCamera;
			MovementButton.inverted = !MovementButton.inverted;			

		}
		DeviceOrientation currentOrientation = Input.deviceOrientation;
		if (currentOrientation != DeviceOrientation.LandscapeLeft && currentOrientation != DeviceOrientation.LandscapeRight)
		{
			return;
		}
		
		if (currentOrientation != lastOrientation)
		{
			Transform t = activeUI.transform.Find("Camera/Anchor");
			
			//LR == rotate
			if (currentOrientation == DeviceOrientation.LandscapeRight)
			{
				InvertNGUI(true);
				MovementButton.inverted = true;
				MoveWithCompass.invertCamera = true;
				inverted = true;
			}
			else
			{
				InvertNGUI(false);
				MovementButton.inverted = false;
				MoveWithCompass.invertCamera = false;
				inverted = false;
			}
		}
		
		lastOrientation = currentOrientation;
	}

	void SelectAndroidLandscape ()
	{
		Debug.Log("Android landscape...");
		
		List<GameObject> use = new List<GameObject>();
		use.Add(androidPortraitUI);
		use.Add(androidLandscapeUI);
		
		DestroyIfNotEqualTo(iPhoneSDPortraitUI, use);
		DestroyIfNotEqualTo(iPhoneSDLandscapeUI, use);
		DestroyIfNotEqualTo(iPhoneHDPortraitUI, use);
		DestroyIfNotEqualTo(iPhoneHDLandscapeUI, use);
		DestroyIfNotEqualTo(iPadHDPortraitUI, use);
		DestroyIfNotEqualTo(iPadHDLandscapeUI, use);
		DestroyIfNotEqualTo(iPadSDPortraitUI, use);
		DestroyIfNotEqualTo(iPadSDLandscapeUI, use);
		
		if (androidPortraitUI != null)
			androidPortraitUI.SetActiveRecursively(false);
		androidLandscapeUI.SetActiveRecursively(true);
		
		activeUI = androidLandscapeUI;
	}

	void SelectiPadSDLandscape ()
	{
		Debug.Log("iPad SD...");
		
		List<GameObject> use = new List<GameObject>();
		use.Add(iPadSDPortraitUI);
		use.Add(iPadSDLandscapeUI);
		
		DestroyIfNotEqualTo(iPhoneSDPortraitUI, use);
		DestroyIfNotEqualTo(iPhoneSDLandscapeUI, use);
		DestroyIfNotEqualTo(iPhoneHDPortraitUI, use);
		DestroyIfNotEqualTo(iPhoneHDLandscapeUI, use);

		DestroyIfNotEqualTo(iPadHDPortraitUI, use);
		DestroyIfNotEqualTo(iPadHDLandscapeUI, use);
		
		if (iPadSDPortraitUI != null)
			iPadSDPortraitUI.SetActiveRecursively(false);
		iPadSDLandscapeUI.SetActiveRecursively(true);
		
		activeUI = iPadSDLandscapeUI;
	}
	
	void SelectiPadHDLandscape ()
	{
		Debug.Log("iPad HD...");

		List<GameObject> use = new List<GameObject>();
		use.Add(iPadHDPortraitUI);
		use.Add(iPadHDLandscapeUI);
		
		DestroyIfNotEqualTo(iPhoneSDPortraitUI, use);
		DestroyIfNotEqualTo(iPhoneSDLandscapeUI, use);
		DestroyIfNotEqualTo(iPhoneHDPortraitUI, use);
		DestroyIfNotEqualTo(iPhoneHDLandscapeUI, use);
		
		DestroyIfNotEqualTo(iPadSDPortraitUI, use);
		DestroyIfNotEqualTo(iPadSDLandscapeUI, use);
		
		if (iPadHDPortraitUI != null)
			iPadHDPortraitUI.SetActiveRecursively(false);
		iPadHDLandscapeUI.SetActiveRecursively(true);
		
		activeUI = iPadHDLandscapeUI;
	}

	void SelectiPhoneSDLandscape ()
	{
		Debug.Log("iPhone SD...");
		
		List<GameObject> use = new List<GameObject>();
		use.Add(iPhoneSDPortraitUI);
		use.Add(iPhoneSDLandscapeUI);
		
		DestroyIfNotEqualTo(iPadSDPortraitUI, use);
		DestroyIfNotEqualTo(iPadSDLandscapeUI, use);
		DestroyIfNotEqualTo(iPadHDLandscapeUI, use);
		DestroyIfNotEqualTo(iPadHDPortraitUI, use);
		DestroyIfNotEqualTo(iPhoneHDPortraitUI, use);
		DestroyIfNotEqualTo(iPhoneHDLandscapeUI, use);
		
		if (iPhoneSDPortraitUI != null)
			iPhoneSDPortraitUI.SetActiveRecursively(false);
		iPhoneSDLandscapeUI.SetActiveRecursively(true);
		
		activeUI = iPhoneSDLandscapeUI;
	}
	
	void SelectiPhoneHDLandscape ()
	{
		Debug.Log("iPhone HD...");
		
		List<GameObject> use = new List<GameObject>();
		use.Add(iPhoneHDPortraitUI);
		use.Add(iPhoneHDLandscapeUI);
		
		DestroyIfNotEqualTo(iPadSDPortraitUI, use);
		DestroyIfNotEqualTo(iPadSDLandscapeUI, use);
		DestroyIfNotEqualTo(iPadHDLandscapeUI, use);
		DestroyIfNotEqualTo(iPadHDPortraitUI, use);
		DestroyIfNotEqualTo(iPhoneSDPortraitUI, use);
		DestroyIfNotEqualTo(iPhoneSDLandscapeUI, use);
		
		if (iPhoneHDPortraitUI != null)
			iPhoneHDPortraitUI.SetActiveRecursively(false);
		iPhoneHDLandscapeUI.SetActiveRecursively(true);
		
		activeUI = iPhoneHDLandscapeUI;
	}
	
	void DestroyIfNotEqualTo (GameObject g1, GameObject g2)
	{
		if (g1 == null || g2 == null)
			return;
		
		if (g1.transform != g2.transform)
		{
			Destroy(g1);
		}
	}
	
	void DestroyIfNotEqualTo (GameObject g1, List<GameObject> g2)
	{
		if (g1 == null || g2 == null || g2.Count == 0)
			return;
		
		if (!g2.Contains(g1))
		{
			Destroy(g1);
		}
	}
	
	
	void iPhoneIDE ()
	{
		ideTestPrefab.SetActiveRecursively(true);
		DestroyIfNotEqualTo(iPhoneHDLandscapeUI, ideTestPrefab);
		DestroyIfNotEqualTo(iPhoneHDPortraitUI, ideTestPrefab);
		DestroyIfNotEqualTo(iPadHDLandscapeUI, ideTestPrefab);
		DestroyIfNotEqualTo(iPadHDPortraitUI, ideTestPrefab);

		activeUI = ideTestPrefab;
	}
}

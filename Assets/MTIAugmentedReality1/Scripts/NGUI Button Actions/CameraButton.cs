using UnityEngine;
using System.Collections;

public class CameraButton : MTIUIBase
{	
	GameObject cameraPostTip;
	void Start ()
	{
		Transform ptrans = transform.parent.parent.Find("General GUI Panel/Camera post tip");
		if (!ptrans)
			ptrans = transform.parent.parent.parent.Find("Anchor/General GUI Panel/Camera post tip");
		
		if (ptrans)
		{
			cameraPostTip = ptrans.gameObject;
			cameraPostTip.active = false;
		}
		else
		{
			Debug.LogWarning("Could not find camera post tip...", gameObject);
		}
	}
	
	void OnClick ()
	{
		Debug.LogWarning("Imitating camera shot - no image capture yet.");
		
		TakeScreenShot();
	}
	
	void TakeScreenShot ()
	{
		GameObject go = new GameObject("Toggler");
		ToggleController t = go.AddComponent<ToggleController>();
		
		GameObject go2 = new GameObject("Screenshots");
		go2.AddComponent<Screenshots>();
		
		t.StartCoroutine(MTIUIBase.TempToggle(1, cameraPostTip, go));
	}
}

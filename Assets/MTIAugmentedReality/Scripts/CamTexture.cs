using UnityEngine;
using System.Collections;

public class CamTexture : MonoBehaviour
{
	public bool enableiPhone4Hack = true;
	
	public static bool camTextureActiveLastScene = false;
	
	public static CamTexture main;
	public static WebCamTexture webcamTexture;
	void Start ()
	{
		main = this;
		if (Application.isEditor)
		{
			transform.localEulerAngles = new Vector3(90, 180, 0);
		}
		
	#if UNITY_ANDROID
		transform.localEulerAngles = new Vector3(90, 180, 0);
		transform.localScale = new Vector3(-1 * transform.localScale.x, 1 * transform.localScale.y, 1 * transform.localScale.z);
	#endif
	}
	
	public static void StartCam ()
	{
		webcamTexture = null;


#if UNITY_IPHONE		
		if (main.enableiPhone4Hack && iPhone.generation == iPhoneGeneration.iPhone4)
#else
	    if (1==0)
#endif
		{
			webcamTexture = new WebCamTexture();
			webcamTexture.requestedFPS = 30;
		}
		else
		{
			webcamTexture = new WebCamTexture(Screen.width, Screen.height);
		}

		webcamTexture.Play();

		if(webcamTexture.isPlaying)
		{
			main.GetComponent<Renderer>().material.mainTexture = webcamTexture;
			float heightRatio = (float)webcamTexture.height/(float)webcamTexture.width;
			Debug.Log("Webcam texture is " + webcamTexture.width + " wide and " + webcamTexture.height + " high (" + heightRatio + " ratio)");
			float screenHeightRatio = (float)Screen.height/(float)Screen.width;
			Debug.Log("Viewport size is "+ Screen.width + " wide and " + Screen.height + " high (" + screenHeightRatio + " ratio)");
			
			float widthRatio = 1;
			
			#if UNITY_ANDROID
			heightRatio *= 1.5f;
			
			widthRatio *= 1.15f;
			heightRatio *= 1.15f;
			#endif
			
			main.transform.localScale = new Vector3(main.transform.localScale.x * widthRatio, main.transform.localScale.y, main.transform.localScale.z * heightRatio);
		}				
	}
	
	public static void StopCam ()
	{
		if (webcamTexture != null)
			webcamTexture.Stop();
	}
}

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class Screenshots : MonoBehaviour
{
	[DllImport ("__Internal")]
	private static extern void CaptureScreenshotToCameraRoll (bool isRight);
	
	IEnumerator Start ()
	{
		yield return 0;
		
		if (!Application.isEditor)
		{
			Debug.LogWarning("Capture screenshot to camera roll");
		#if UNITY_IPHONE
			CaptureScreenshotToCameraRoll(UIController.inverted);
		#elif UNITY_ANDROID
			Debug.Log("Screenshot save path " + Application.persistentDataPath);
			
			string path = Application.persistentDataPath;
			string filename = ".png";
			
			filename = System.DateTime.Now.ToString("MM_dd_yy_hh_mm_ss_") + filename;
			
			
			Debug.Log("Capture screenshot...");
			Application.CaptureScreenshot(filename);
			
			Debug.Log("Screenshot name: " + filename);
			
			
			AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
			
			AndroidJavaObject mediaScanner = new AndroidJavaObject("android.media.MediaScannerConnection", jo, null);
			
			mediaScanner.Call("connect");
			
			while (!mediaScanner.Call<bool>("isConnected"))
			{
				Debug.Log("Waiting for connection...");
				yield return 0;
			}
			
			Debug.Log("Connectd, Scanfile...");
			
			if (mediaScanner == null)
			{
				Debug.Log("MediaScanner is null...");
			}

			mediaScanner.Call("scanFile", path + "/" + filename, "image/png");
			
			Debug.Log("Done...");
		#endif
		}
		else
		{
			Debug.LogWarning("Simulated taking picture to camera roll...");
		}
		
		yield return 0;
		
		Destroy(gameObject);
	}
}

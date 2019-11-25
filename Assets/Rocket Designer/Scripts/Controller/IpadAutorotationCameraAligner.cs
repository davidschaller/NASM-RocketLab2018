using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IpadAutorotationCameraAligner : MonoBehaviour
{
    public List<Camera> camerasToAlign;

    void Awake()
    {
        enabled = false;

        if (camerasToAlign == null || camerasToAlign.Count == 0)
        {
            Debug.LogWarning("IpadAutorotationCameraAligner 'Cameras To Align' array is empty, adding all active Cameras");

            Camera[] allCams = (Camera[])Object.FindObjectsOfType(typeof(Camera));

            if (allCams != null)
                camerasToAlign.AddRange(allCams);
        }

#if UNITY_IOS
        enabled = true;
#endif
    }

    ScreenOrientation lastRotation = ScreenOrientation.Unknown;
    void FixedUpdate()
    {
        if (Screen.orientation != lastRotation)
        {
            foreach (Camera cam in camerasToAlign)
            {
                switch (Screen.orientation)
                {
                    case ScreenOrientation.LandscapeLeft:
                    case ScreenOrientation.LandscapeRight:
                        cam.rect = new Rect(0, 0.075f, 1, 0.85f);
                        break;
                    case ScreenOrientation.Portrait:
                    case ScreenOrientation.PortraitUpsideDown:
                        cam.rect = new Rect(0, 0.23f, 1, 0.48f);
                        break;
                }
            }

            lastRotation = Screen.orientation;

            status = "Updating " + camerasToAlign.Count.ToString() + " cameras";
        }
        else
            status = "Doing Nothing";
    }

    string status = string.Empty;
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, Screen.width, 30), "Ipad Autorotation Camera Aligner Status: " + status + ", rotation: " + Screen.orientation.ToString());
    }
}

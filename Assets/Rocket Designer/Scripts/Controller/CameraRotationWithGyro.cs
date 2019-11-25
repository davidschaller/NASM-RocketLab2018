using UnityEngine;
using System.Collections;

public class CameraRotationWithGyro : MonoBehaviour
{

    // Gyroscope-controlled camera for iPhone & Android revised 2.26.12

    // Perry Hoberman <hoberman@bway.net>

    //

    // Usage:

    // Attach this script to main camera.

    // Note: Unity Remote does not currently support gyroscope.

    //

    // This script uses three techniques to get the correct orientation out of the gyroscope attitude:

    // 1. creates a parent transform (camParent) and rotates it with eulerAngles

    // 2. for Android (Samsung Galaxy Nexus) only: remaps gyro.Attitude quaternion values from xyzw to wxyz (quatMap)

    // 3. multiplies attitude quaternion by quaternion quatMult



    // Also creates a grandparent (camGrandparent) which can be rotated with localEulerAngles.y

    // This node allows an arbitrary heading to be added to the gyroscope reading

    // so that the virtual camera can be facing any direction in the scene, no matter what the phone's heading

    static bool gyroBool;

    Gyroscope gyro;

    Quaternion quatMult;

    Quaternion quatMap;

    Vector3 savedEulerAngles;

    Transform originParent;
    GameObject camParent;

/*
    void Awake()
    {
        Init();
    }
*/    

    void Init()
    {
		//Removed Unity version Checks to support unity 4 - dustin.hagen
        gyroBool = SystemInfo.supportsGyroscope;    	

        if (gyroBool)
        {
    	
	        savedEulerAngles = transform.localEulerAngles;

	        // find the current parent of the camera's transform
	        originParent = transform.parent;

    	    // instantiate a new transform
        	camParent = new GameObject("camParent");

	        // match the transform to the camera position
    	    camParent.transform.position = transform.position;

        	// make the original parent the grandparent of the camera transform
	        camParent.transform.parent = originParent;

    	    camParent.transform.localEulerAngles = transform.localEulerAngles;

        	// make the new transform the parent of the camera transform
	        transform.parent = camParent.transform;

        	if (GetComponent<Animation>() && GetComponent<Animation>().enabled)
        	{
        		GetComponent<Animation>().enabled = false;
        	}
        	
            gyro = Input.gyro;

            gyro.enabled = true;

#if UNITY_IPHONE

            camParent.transform.eulerAngles = new Vector3(90, 90, 0);

            if (Screen.orientation == ScreenOrientation.LandscapeLeft)
            {
                quatMult = new Quaternion(0, 0, 1, 0); //**
            }
            else if (Screen.orientation == ScreenOrientation.LandscapeRight)
            {
                quatMult = new Quaternion(0, 0, 1, 0); //**
            }
            else if (Screen.orientation == ScreenOrientation.Portrait)
            {
                quatMult = new Quaternion(0, 0, 1, 0); //**
            }
            else if (Screen.orientation == ScreenOrientation.PortraitUpsideDown)
            {
                quatMult = new Quaternion(0, 0, 1, 0); // Unable to build package on upsidedown
            }
#endif

#if UNITY_ANDROID

            camParent.transform.eulerAngles = new Vector3(90, 90, 0);

            if (Screen.orientation == ScreenOrientation.LandscapeLeft)
            {
                quatMult = new Quaternion(0, 0, 1, 0); //**
            }
            else if (Screen.orientation == ScreenOrientation.LandscapeRight)
            {
                quatMult = new Quaternion(0, 0, 1, 0); //**
            }
            else if (Screen.orientation == ScreenOrientation.Portrait)
            {
                quatMult = new Quaternion(0, 0, 1, 0); //**
            }
            else if (Screen.orientation == ScreenOrientation.PortraitUpsideDown)
            {
                quatMult = new Quaternion(0, 0, 1, 0); // Unable to build package on upsidedown
            }
#endif
            Screen.sleepTimeout = SleepTimeout.NeverSleep;


            enabled = true;
        }
        else
        {

#if UNITY_EDITOR
            print("NO GYRO");
#endif
        }
    }

    void Update()
    {
        if (gyroBool)
        {

#if UNITY_IPHONE
            quatMap = gyro.attitude;
#endif

#if UNITY_ANDROID
//            quatMap = new Quaternion(gyro.attitude.x,gyro.attitude.y,gyro.attitude.z,gyro.attitude.w);
#endif

/*
            quatMap = new Quaternion(qx == "" ? gyro.attitude.x : float.Parse(qx),
            	qy == "" ? gyro.attitude.y : float.Parse(qy),
		            qz == "" ? gyro.attitude.z : float.Parse(qz),
        			    qw == "" ? gyro.attitude.w : float.Parse(qw));
            
            transform.localRotation = quatMap * quatMult;

			camParent.transform.localEulerAngles = new Vector3(ex == "" ? camParent.transform.localEulerAngles.x : savedEulerAngles.x + float.Parse(ex), 
				ey == "" ? camParent.transform.localEulerAngles.y : savedEulerAngles.y + float.Parse(ey), 
					ez == "" ? camParent.transform.localEulerAngles.z : savedEulerAngles.z + float.Parse(ez));
*/
            quatMap = new Quaternion(0, qy == "" ? gyro.attitude.y : float.Parse(qy), qz == "" ? gyro.attitude.z : float.Parse(qz), 0);
            
            transform.localRotation = quatMap * quatMult;

			camParent.transform.localEulerAngles = new Vector3(camParent.transform.localEulerAngles.x, savedEulerAngles.y, camParent.transform.localEulerAngles.z);
        }
    }
    
    string qx = "0", qy = "", qz = "", qw = "0";
	string ex = "", ey = "", ez = "";

    public void Toggle(bool on)
    {
        if (on)
        {
            Init();
        }
        else
        {
            enabled = false;

			if (gyroBool)
	        {

    	        transform.parent = originParent;

        	    if (camParent)
            	{
                	Destroy(camParent.gameObject);
	            }
    	        transform.localEulerAngles = savedEulerAngles;
	        }
        }
    }
/*
    void OnGUI()
    {
        GUI.Box(new Rect(30, 50, 300, 30), transform.localEulerAngles.ToString());
        
        GUILayout.BeginArea(new Rect(30, 90, 50, 500), "");
        
        qx = GUILayout.TextField(qx);
        qy = GUILayout.TextField(qy);
        qz = GUILayout.TextField(qz);
        qw = GUILayout.TextField(qw);        
        GUILayout.Label("euler");
        ex = GUILayout.TextField(ex);
        ey = GUILayout.TextField(ey);
        ez = GUILayout.TextField(ez);        
        
                                                
        GUILayout.EndArea();
    }
*/    
}

using UnityEngine;
using System.Collections;

public class Orientation : MonoBehaviour
{
	bool gyroBool;
	Gyroscope gyro;
	Quaternion rotFix;
	
	void Start()
	{
		Transform originalParent = transform.parent;
		GameObject camParent = new GameObject ("camParent");
		camParent.transform.position = transform.position;
		transform.parent = camParent.transform;
		camParent.transform.parent = originalParent;
	
		gyroBool = SystemInfo.supportsGyroscope;
	
		if (gyroBool)
		{
			gyro = Input.gyro;
			gyro.enabled = true;
		
			/*
			if (Screen.orientation == ScreenOrientation.LandscapeLeft)
			{
				camParent.transform.eulerAngles = new Vector3(90, 90, 0);
			}
			else if (Screen.orientation == ScreenOrientation.Portrait)
			{
				camParent.transform.eulerAngles = new Vector3(90, 180, 0);
			}
			*/
		
			if (Screen.orientation == ScreenOrientation.LandscapeLeft)
			{
				rotFix = new Quaternion(0, 0, 0.7071f, 0.7071f);
			}
			else if (Screen.orientation == ScreenOrientation.Portrait)
			{
				rotFix = new Quaternion(0, 0, 1, 0);
			}
		}
		else
		{
			Debug.LogWarning("No gyroscope detected on this device!");
		}
	}
	
	double lastCompassSyncTime;
	Quaternion camCorrection;
	Quaternion compassOrientation;
	public bool syncToCompass = true;
	Quaternion correction;
	void Update ()
	{
		if (gyroBool)
		{
			Quaternion camRot = gyro.attitude * rotFix;
			
			if (syncToCompass)
			{
				if (Input.compass.timestamp > lastCompassSyncTime)
				{
					lastCompassSyncTime = Input.compass.timestamp;
				
					Vector3 gravity = Input.gyro.gravity.normalized;
					Vector3 flatNorth = Input.compass.rawVector - Vector3.Dot(gravity, Input.compass.rawVector) * gravity;
					compassOrientation = Quaternion.Euler (180, 0, 0) * Quaternion.Inverse(Quaternion.LookRotation(flatNorth, -gravity)) /* * rotFix */ * Quaternion.Euler(0, 0, 90);
				
					camCorrection = compassOrientation * Quaternion.Inverse(camRot);
				}
			
				// Jump straight to the target correction if it's a long way; otherwise, slerp towards it very slowly
				if (Quaternion.Angle(correction, camCorrection) > 45)
					correction = camCorrection;
				else
					correction = Quaternion.Slerp(correction, camCorrection, 0.02f);

				//transform.localRotation = correction * camRot;
				transform.localRotation = compassOrientation; /* * rotFix;*/
			}
			else
			{
				transform.localRotation = camRot;
			}
		}
		else if (Application.isEditor)
		{
			IDEGyroSim();
		}
	}
	
	void IDEGyroSim ()
	{
		if (Input.GetKey("up"))
		{
			transform.Rotate(Vector3.right * 30 * Time.deltaTime);
		}
		else if (Input.GetKey("down"))
		{
			transform.Rotate(Vector3.right * -30 * Time.deltaTime);			
		}
		
		if (Input.GetKey("right"))
		{
			transform.Rotate(Vector3.up * 30 * Time.deltaTime);
		}
		else if (Input.GetKey("left"))
		{
			transform.Rotate(Vector3.up * -30 * Time.deltaTime);
		}
		
	}
	
	void OnGUI ()
	{
		GUI.Label( new Rect(0, 0, 100, 40), "Hdg: " + transform.eulerAngles.y);	
		GUI.Label (new Rect(100, 100, 300, 300), "C: " + Input.compass.rawVector.x.ToString("0.00") + ", " + Input.compass.rawVector.y.ToString("0.00") + ", " + Input.compass.rawVector.z.ToString("0.00"));
	}
}

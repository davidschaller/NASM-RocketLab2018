using UnityEngine;
using System.Collections;

public class GPSActor : MonoBehaviour
{
	public bool smoothPosition = true;
	public float maxMoveSpeed = 1;	
	
	public static GPSActor main;
	
	int walkLayer;
	bool virtualWalkthrough = false;
	bool roomLevel = false;
	IEnumerator Start ()
	{
		if (Application.loadedLevel > 1)
			roomLevel = true;
		if (PersistentData.Main.restorePosition && Application.loadedLevel == 1)
		{
			transform.position = PersistentData.Main.storedPosition;	
			transform.rotation = PersistentData.Main.storedRotation;
			PersistentData.Main.restorePosition = false;
		}
		
		GameObject pd = GameObject.Find("PersistentData");		
		if (PersistentData.Main.virtualWalkthrough)
		{
			virtualWalkthrough = true;
			if (MovementButton.registry.Count == 0)
			{
				Debug.LogWarning("No movement buttons set up in the scene, creating UnityGUI versions...");
				
				PersistentData.Main.unityWalkButtons = true;
				pd.AddComponent<MovementButton>().moveType = MovementButton.MoveType.MoveForward;
				pd.AddComponent<MovementButton>().moveType = MovementButton.MoveType.MoveBack;
			}
		}
		walkLayer = 1 << LayerMask.NameToLayer("Walkable");
		
		main = this;
		if (!Input.location.isEnabledByUser)
		{
			Debug.LogWarning("User has not enabled location services");
			yield break;
		}

	    Input.location.Start (0.1f, 0.1f);
		Input.compass.enabled = true;

	    int maxWait = 20;
	    while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
		{
	        yield return new WaitForSeconds (1);
	        maxWait--;
	    }

	    if (maxWait < 1)
		{
	        Debug.LogWarning ("Location services timed out");
	        yield break;
	    }

	    if (Input.location.status == LocationServiceStatus.Failed)
		{
	        Debug.LogWarning ("Unable to determine device location");
	        yield break;
	    }
	    else
		{
	        Debug.Log ("Location: " + Input.location.lastData.latitude + " " +
	               	Input.location.lastData.longitude + " " +
	               	Input.location.lastData.altitude + " " +
	               	Input.location.lastData.horizontalAccuracy + " " +
	               	Input.location.lastData.timestamp);
	    }
		
		yield return 0;
		Debug.Log("COMPASS ENABLED: " + Input.compass.enabled + " supports gyro: " + SystemInfo.supportsGyroscope);
		if (Application.isEditor || (PersistentData.Main.virtualWalkthrough && !SystemInfo.supportsGyroscope && !Input.compass.enabled))
		{
			Debug.Log("Force left/right buttons");
			PersistentData.Main.unityTurnButtons = true;
			
			pd.AddComponent<MovementButton>().moveType = MovementButton.MoveType.RotateLeft;
			pd.AddComponent<MovementButton>().moveType = MovementButton.MoveType.RotateRight;													
		}
	}
	
	public bool positionLocked = true;
	public bool hideUnityGUI = true;
	void OnGUI ()
	{
		if (hideUnityGUI)
			return;
		
		string txt = positionLocked ? "Unlock GPS" : "Lock GPS";
		
		if (GUI.Button(new Rect(0,Screen.height - 100, 100, 100), txt))
			positionLocked = !positionLocked;
	}

	public void MoveForward ()
	{
		if (!positionLocked)
			transform.position += MoveWithCompass.ghost.TransformDirection(Vector3.forward) * Time.deltaTime * 4;
	}
	
	public void MoveBackward ()
	{
		if (!positionLocked)
			transform.position -= MoveWithCompass.ghost.TransformDirection(Vector3.forward) * Time.deltaTime * 4;
	}
	
	public void RotateRight ()
	{
		transform.Rotate(Vector3.up * 30 * Time.deltaTime);
	}
	
	public void RotateLeft ()
	{
		transform.Rotate(Vector3.up * -30 * Time.deltaTime);
	}
	
	void Update ()
	{
		if (Application.isEditor)
		{
			if (Input.GetKey("w"))
			{
				MoveForward();
			}
			else if (Input.GetKey("s"))
			{
				MoveBackward();
			}
			
			if (Input.GetKey("d"))
			{
				RotateRight();
			}
			else if (Input.GetKey("a"))
			{
				RotateLeft();
			}
			
			
		}
		else if (virtualWalkthrough)
		{
			
		}
		else if (!positionLocked)
		{
			Vector2 uc = GPSScale.UnityCoords(Input.location.lastData.latitude, Input.location.lastData.longitude);
			Vector3 newPos = new Vector3(uc.x, 0, -uc.y);
			
			if (smoothPosition)
			{
				Vector3 oldPos = transform.position;
				float dist = (newPos - oldPos).magnitude;
				float maxMove = maxMoveSpeed*Time.deltaTime;
				float moveRatio = 1;
				if (dist > maxMove)
				{
					moveRatio = maxMove;
				}
				transform.position = Vector3.Lerp(transform.position, newPos, moveRatio);				
			}
			else
			{
				transform.position = newPos;
			}
		}
	}
	
	
	void LateUpdate ()
	{
		if (roomLevel)
			return;
		
		RaycastHit hit;
		//if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, walkLayer))
		if (Physics.Raycast(transform.position +Vector3.up * 2, Vector3.down, out hit, Mathf.Infinity, walkLayer))
		{
			transform.position = new Vector3(transform.position.x, hit.point.y + 0.75f, transform.position.z);			
		}
		/*if (!Application.isEditor)
		{
			transform.rotation = Quaternion.Euler( new Vector3(0, Input.compass.trueHeading, 0));
		}*/
	}
	/*
	void OnGUI ()
	{
		GUI.Label( new Rect(0, 0, 100, 40), "Hdg: " + transform.eulerAngles.y);
		
	}
	*/
}

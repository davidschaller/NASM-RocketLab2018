using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
	public bool cameraLocked = false;
	public string lockedBy = "Test";
	
	static CameraController main;
	
	void Awake ()
	{
		if (!main) main = this;
		main.lockedBy = "";
		main.cameraLocked = false;
	}
	
	public static bool RequestCameraLock (string locker)
	{
        if (main == null && Camera.main != null)
			main = (CameraController)Camera.main.gameObject.AddComponent(typeof(CameraController));
			
		if (main != null && !main.cameraLocked)
		{
			print(locker + " acquired camera lock");
			main.lockedBy = locker;
			main.cameraLocked = true;
			return true;
		}
		
        if (main != null)
		    print("Denying camera lock to " + locker + ", locked by " + main.lockedBy);
        
		return false;
	}

	public static string LockedBy ()
	{
        if (main != null)
		    return main.lockedBy;
        else
            return string.Empty;
	}
	
	public static void ReleaseLock(string locker)
	{
		if (main != null && main.cameraLocked && locker == main.lockedBy)
		{
			main.cameraLocked = false;
			main.lockedBy = "";
			main.watchPoint = Vector3.zero;
			main.watchTransform = null;
		}
	}
	
	string lastBreaker = "";
	public static void BreakLock(string breaker)
	{
		main.cameraLocked = false;
		main.lastBreaker = breaker;
		main.lockedBy = "";
	}
	
	public static string LastBreaker ()
	{
		return main.lastBreaker;
	}
	
	public static bool Locked ()
	{
		return main && main.cameraLocked;
	}
	
	void Update ()
	{
		if (Input.GetKeyDown("space"))
		{
			BreakLock("space key");
		}
	}

	Transform watchTransform;
	public static Transform WatchTransform
	{
		get
		{
			return main.watchTransform;
		}
		set
		{
			main.watchTransform = value;
		}
	}

	Vector3 watchPoint;
	public static Vector3 WatchPoint
	{
		get
		{
			return main.watchPoint;
		}
		set
		{
			main.watchPoint = value;
		}
	}
	
	void LateUpdate ()
	{
		if (cameraLocked && (watchTransform != null || watchPoint != Vector3.zero))
		{
			if (watchTransform != null)
				GetComponent<Camera>().transform.LookAt(watchTransform);
			else if (watchPoint != Vector3.zero)
				GetComponent<Camera>().transform.LookAt(watchPoint);
		}
	}
}

using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class PCCamera : MonoBehaviour
{
    public enum Views
    {
        FirstPerson,
        ThirdPerson,
        Focused,
		Rotate
    }

    public bool StandMode;

    public float backWeight = 1,
                 leftWeight = 1;

    public float focusedFieldOfVew = 35;
    float NormalFieldOfVew = 70;

    Views currentView = Views.ThirdPerson,
          lastCurrentView = Views.ThirdPerson;

    public Views CurrentView { get { return currentView; } }

	public Transform target;

    public Transform myCameraTr;
    Camera myCamera;

    public Vector3 focusCorrection = new Vector3(-0.3f, -0.1f, 0);
	
	public Texture cursorTexture;
    public float maxCam3DDistance = 10f;
    public float minCam3DDistance = 10f;
    public float zoomSpeed = 3f;

	public float distance = 4.0f;
	//public float height = 1.0f;
	float heightDamping = 2.0f;
	public float rotationDamping = 3.0f;
	public bool lockFirstPersonView = false;

    public float lodMaxDistance = 30f;
    public float deactivateMaxDistance = 30f;
    public float behindLodMaxDistance = 10f;
    public float inBuildingLODDistance = 10f;
    public float heightOffSet = 1.8f;

    private float x = 0.0f;
    private float y = 10.0f;
    public float xSpeed = 240.0f;
    public float ySpeed = 60.0f;

    public int yMinLimit = 5;
    public int yMaxLimit = 80;

    private static PCCamera main;
    public static PCCamera Main
    {
        get
        {
            return main;
        }
        private set { }
    }

    private bool isLocked = false;
    public bool IsLocked
    {
        get
        {
            return isLocked;
        }
    }
	
	public float RotateAngleStart = 100f;
	public float RotateHeight = -0.8f;
	public float RotateDistance = 2f;
	private float rotateHeight = -0.8f;
	private float rotateDistance = 2f;
	
    void Awake()
    {
        if (!main)
        {
            main = this;
        }

        if (lockFirstPersonView)
            FirstPersonView();

        myCameraTr = Camera.main.transform;
        myCamera = Camera.main;

        // Add Igbore Raycast layer to mask
		
        mask |= 1 << LayerMask.NameToLayer("Ignore Raycast");
        mask |= 1 << LayerMask.NameToLayer("Key");
        mask |= 1 << LayerMask.NameToLayer("FortCrew");
        // Invert mask
        mask = ~mask;
    }

    void Start()
    {
        smoothPlayerPos = transform.position;
    }
    
    Vector3 TargetPosWithOffset
	{
        get
        {
            if (target != null)
            {
                return target.position + Vector3.up * heightOffSet;
            }
            else
                return Vector3.zero;
        }
    }
		
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360.0f)
            angle += 360.0f;
        if (angle > 360.0f)
            angle -= 360.0f;

        return Mathf.Clamp(angle, min, max);
    }
    
	Vector3 CalculateCameraPositionAngle(float distance, float height)
	{
		Vector3 result = Vector3.zero;
        y = ClampAngle(y, yMinLimit, yMaxLimit);
		float tx = x;
		
		if(StandMode)
			tx += target.eulerAngles.y;
			
		Quaternion rotation = Quaternion.Euler(y, tx, 0.0f);
		result = rotation * new Vector3(0.0f, 0f, -distance) + TargetPosWithOffset;
		result = new Vector3(result.x, result.y+height, result.z);

        Vector3 result1 = rotation * new Vector3(0.0f, 0f, -0.2f) + TargetPosWithOffset;;
		result1 = new Vector3(result1.x, result1.y+height, result1.z);
		
		return result;
	}
	
	public float maxCamDist = 1;
	public float minCamDistance = 0.5f;
	private LayerMask mask;
	Vector3 CheckIntersect(Vector3 farCamPoint,Vector3 closeCamPoint)
	{
		Vector3 result;
		// Find far and close position for the camera
		//Vector3 farCamPoint = myCameraTr.position;
		//Vector3 closeCamPoint = myCameraTr.position;
		float farDist = Vector3.Distance(farCamPoint, closeCamPoint);
 
		// Smoothly increase maxCamDist up to the distance of farDist
		maxCamDist = Mathf.Lerp(maxCamDist, farDist, 5 * Time.deltaTime);
		//maxCamDist = farDist;
 
		// Make sure camera doesn't intersect geometry
		// Move camera towards closeOffset if ray back towards camera position intersects something 
		RaycastHit hit;
		Vector3 closeToFarDir = (farCamPoint - closeCamPoint) / farDist;
		float padding = 0.1f;
		if (Physics.Raycast(closeCamPoint, closeToFarDir, out hit, maxCamDist + padding, mask))
		{
			maxCamDist = hit.distance - padding;
		}
		result = closeCamPoint + closeToFarDir * maxCamDist;
		return result;
	}

    void LateUpdate()
    {
        if (target == null)
        {
            target = transform;
        }

        if (!isLocked)
        {
            CheckForScroll();

            switch (currentView)
            {
                case Views.ThirdPerson:
                    UpdateThirdPersonCamera();
                    break;
                case Views.FirstPerson:
                    UpdateFirstPersonCamera();
                    break;
                case Views.Focused:
                    UpdateFocusedView();
                    break;
                case Views.Rotate:
                    UpdateRotateView();
                    break;
            }
        }
    }

    public float angleH = 0;
    public float angleV = -10;

    NormalCharacterMotor motor;

    public float horizontalAimingSpeed = 270f;
    public float verticalAimingSpeed = 270f;
    public float maxVerticalAngle = 10f,
                 maxVerticalAngleWhenClose = 5f;
    public float minVerticalAngle = -10f;
    public Vector3 smoothPlayerPos;
    public float smoothingTime = 0.5f;
    public Vector3 pivotOffset = Vector3.zero;
    public Vector3 camOffset = new Vector3(0.0f, 0.7f, -1.7f);
    public Vector3 closeOffset = Vector3.zero;

    public bool showHit = false;
    public Transform currentHit;

    void UpdateThirdPersonCamera()
    {
        if (motor == null)
        {
            motor = target.GetComponent<NormalCharacterMotor>();
            return;
        }

        if (Time.deltaTime == 0 || Time.timeScale == 0)
            return;

        if (StandMode)
        {
            angleH += Mathf.Clamp(Input.GetAxis("Mouse X"), -1, 1) * horizontalAimingSpeed * Time.deltaTime;

            angleV += Mathf.Clamp(Input.GetAxis("Mouse Y"), -1, 1) * verticalAimingSpeed * Time.deltaTime;
            // limit vertical angle
            angleV = Mathf.Clamp(angleV, minVerticalAngle, maxCamDist > 1 ? maxVerticalAngle : maxVerticalAngleWhenClose);
        }
        else if (!motor.IsAutoMoving)
        {
            angleH += Mathf.Clamp(Input.GetAxis("Horizontal"), -1, 1) * horizontalAimingSpeed * Time.deltaTime;

            motor.desiredFacingDirection = myCameraTr.TransformDirection(Vector3.forward);
        }
        else if (motor.desiredVelocity.magnitude > .2f)
        {
            motor.desiredFacingDirection = Vector3.zero;
            angleH = transform.eulerAngles.y;
        }

        // Before changing camera, store the prev aiming distance.
        // If we're aiming at nothing (the sky), we'll keep this distance.
        float prevDist = (target.position - myCameraTr.position).magnitude;

        Quaternion aimRotation = Quaternion.Euler(-angleV, angleH, 0);
        Quaternion camYRotation = Quaternion.Euler(0, angleH, 0);

        myCameraTr.rotation = aimRotation;

        // Find far and close position for the camera
        smoothPlayerPos = Vector3.Lerp(smoothPlayerPos, target.position, smoothingTime * Time.deltaTime);
        smoothPlayerPos.x = target.position.x;
        smoothPlayerPos.z = target.position.z;
        
        Vector3 farCamPoint = smoothPlayerPos + camYRotation * pivotOffset + aimRotation * camOffset;
        Vector3 closeCamPoint = target.position + Vector3.up * camOffset.y +camYRotation * closeOffset;
        float farDist = Vector3.Distance(farCamPoint, closeCamPoint);

        // Smoothly increase maxCamDist up to the distance of farDist
        maxCamDist = Mathf.Lerp(maxCamDist, Mathf.Min(farDist, distance), 5 * Time.deltaTime);

        RaycastHit hit;
        Vector3 closeToFarDir = (farCamPoint - closeCamPoint) / farDist;

        float padding = 0.3f;

        if (Physics.Raycast(closeCamPoint, closeToFarDir, out hit, maxCamDist + padding, mask))
        {
            /*
            if (showHit)
                currentHit = hit.transform;
             */

            maxCamDist = hit.distance - padding;
        }

        maxCamDist = Mathf.Clamp(maxCamDist, .5f, maxCam3DDistance);

        myCameraTr.position = closeCamPoint + closeToFarDir * maxCamDist;
    }

    void CheckForScroll()
    {
        // Zoom out
        if (Input.GetAxis("Mouse ScrollWheel") < 0 || (Input.GetKey("-") || Input.GetKey("_")))
        {
            distance += zoomSpeed * Time.deltaTime;
        }
        // Zoom in
        if (Input.GetAxis("Mouse ScrollWheel") > 0 || (Input.GetKey("+") || Input.GetKey("=")))
        {
            distance -= zoomSpeed * Time.deltaTime;
        }

        distance = Mathf.Clamp(distance, minCam3DDistance, maxCam3DDistance);
    }

    void OnGUI()
    {
        ShowDragCursor(StandMode && Input.GetMouseButton(0));
    }

    void ShowDragCursor(bool show)
    {
        Cursor.visible = !show;
        if (show)
        {
            Vector3 mousePos = Input.mousePosition;

            Rect pos = new Rect(mousePos.x, Screen.height - mousePos.y, cursorTexture.width, cursorTexture.height);
            GUI.Label(pos, cursorTexture);
        }
    }

    Vector3 UpdateFirstPersonCamera()
    {
        myCameraTr.position = FPSPos;

        return myCameraTr.position;
    }

    public Vector3 lastCamPos = Vector3.zero,
                   lastLookAt = Vector3.zero;
    Vector3 LookAt3dCam()
    {
        if (myCameraTr.position != lastCamPos)
        {
            Vector3 direction = TargetPosWithOffset - myCameraTr.position;

            /*
            RaycastHit hit;
            if (Physics.Raycast(myCameraTr.position, direction, out hit))
            {
                lastLookAt = hit.point;
            }
             */
            lastCamPos = myCameraTr.position;
        }

        if (lastLookAt == Vector3.zero)
        {
            return TargetPosWithOffset;
        }
        else
            return lastLookAt;
    }

    void UpdateFocusedView()
    {
        if (CameraFocus)
        {
            myCameraTr.position = WatchTarget.position + Vector3.up * WatchTargetHeight + WatchTarget.TransformDirection(Vector3.forward) * 2;
        }
        else
            myCameraTr.position = FPSPos;

        if (WatchTarget != null)
        {
            myCameraTr.LookAt(WatchTarget.position + Vector3.up * WatchTargetHeight +
                watchTarget.TransformDirection(Vector3.right) * focusCorrection.x +
                    watchTarget.TransformDirection(Vector3.up) * focusCorrection.y);
        }

        if (myCamera.fieldOfView != focusedFieldOfVew)
        {
            myCamera.fieldOfView = focusedFieldOfVew;
        }
    }

    void UpdateRotateView()
    {
        Vector3 lookTarget = TargetPosWithOffset;
		rotateDistance += 0.005f;
		rotateHeight += 0.005f;
        Vector3 targetPos = Vector3.zero;
		x =x+0.5f;
        targetPos = CalculateCameraPositionAngle(rotateDistance, rotateHeight);

        myCameraTr.position = targetPos;

        // Always look at the target
        myCameraTr.LookAt(lookTarget);
    }
	float FocusLenght = 2;
	bool CameraFocus = false;
	public Transform watchTarget;
	public Transform WatchTarget
	{
		get
		{
			return watchTarget;
		}
		set
		{
			WatchTargetHeight = 1.7f;
			watchTarget = value;
		}
	}

	float watchTargetHeight = 1.7f;
	public float WatchTargetHeight
	{
		get
		{
			return watchTargetHeight;
		}
		set
		{
			watchTargetHeight = value;
		}
	}

    public Transform Target
    {
        get
        {
            return target;
        }
        set
        {
            SetTarget(value);
        }
    }

    void SetTarget(Transform t)
    {
        Transform temp = t == null ? null : t.Find("CameraTarget");
        if (temp != null)
            target = temp;
        else
        {
            target = t;
        }
    }

    public void ToggleViewPoint()
    {
        if (!lockFirstPersonView)
        {
            if (currentView == Views.ThirdPerson)
            {
                currentView = Views.FirstPerson;
            }
            else if (currentView == Views.FirstPerson)
            {
                currentView = Views.ThirdPerson;
            }
        }
    }
	
	bool visible = true;
	Vector3 lastpoint;
	private void VisibleTarget(bool vis)
	{
        if (!vis)
        {
            myCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Key"));
        }
        else
        {
            myCamera.cullingMask |= 1 << LayerMask.NameToLayer("Key");
        }

        /*
		if(visible == vis)
			return;
		visible = vis;
		if(visible)
			target.position = lastpoint;
		else
		{
			lastpoint = target.position;
			target.position = target.position+Vector3.up*-10;
		}
         */
	}
	
    public Vector3 FirstPersonView()
    {
		VisibleTarget(true);
		myCamera.fieldOfView = NormalFieldOfVew;
        myCamera.nearClipPlane = .3f;

        currentView = Views.FirstPerson;

        return UpdateFirstPersonCamera();
    }

    public void FocusOnTargetView(Transform tr, float focuslenght)
    {
        if (currentView != Views.Focused)
        {
            lastCurrentView = currentView;
            oldFieldOfView = myCamera.fieldOfView;
        }

        VisibleTarget(false);
        FocusLenght = focuslenght;
        CameraFocus = true;

        watchTarget = tr;
        currentView = Views.Focused;
    }

    public void DefocusView()
    {
        VisibleTarget(true);
        currentView = lastCurrentView;
        myCamera.fieldOfView = oldFieldOfView;
    }

	public void FocusOnTargetView(Transform tr)
    {
        FocusLenght = 0;

		VisibleTarget(true);
		CameraFocus = false;
        watchTarget = tr;
        currentView = Views.Focused;
        oldFieldOfView = myCamera.fieldOfView;
        myCamera.nearClipPlane = .3f;
    }


    public void ThirdPersonView()
    {
        angleH = transform.eulerAngles.y;

		VisibleTarget(true);
        if (!lockFirstPersonView)
            currentView = Views.ThirdPerson;

        myCamera.fieldOfView = NormalFieldOfVew;

        myCamera.nearClipPlane = .1f;
		angleV = minVerticalAngle;
    }

    public void RotateView()
    {
		VisibleTarget(true);
		
		x = RotateAngleStart+target.rotation.eulerAngles.y;
		rotateDistance = RotateDistance;
		rotateHeight = RotateHeight;
        if (!lockFirstPersonView)
            currentView = Views.Rotate;

        myCamera.nearClipPlane = .1f;

        myCamera.fieldOfView = focusedFieldOfVew;
    }
	
	Views tmpViev = Views.ThirdPerson;
    public void RotateView(float time)
    {
		tmpViev = currentView;
		RotateView();
		Invoke("ReturnCurrenView", time);
    }

    public void ReturnCurrenView()
    {
        currentView = tmpViev;
        myCamera.fieldOfView = NormalFieldOfVew;
    }
	
	bool indoors = false;
	public bool Indoors
	{
		set
		{
			indoors = value;
		}
		get
		{
			return indoors;
		}
	}

    float oldFieldOfView = 0;
    Vector3 zoomTarget;

    public void ZoomIn(Vector3 target)
    {
        oldFieldOfView = myCamera.fieldOfView;

        zoomTarget = target;
        StartCoroutine(ZoomProcess(target, 20, true));
	}

    internal void ZoomOut(Vector3 target)
    {
        StartCoroutine(ZoomProcess(zoomTarget, 20, false));
    }

    IEnumerator ZoomProcess(Vector3 target, float fieldOfView, bool zoomIn)
    {
        isLocked = true;
        float timer = 0;

        while (timer < 1)
        {
            timer += Time.deltaTime;
            if (zoomIn)
            {
                myCameraTr.LookAt(Vector3.Lerp(TargetPosWithOffset, target, timer));
                myCamera.fieldOfView = Mathf.Lerp(oldFieldOfView, fieldOfView, timer);
            }
            else
            {
                myCameraTr.LookAt(Vector3.Lerp(target, TargetPosWithOffset, timer));
                myCamera.fieldOfView = Mathf.Lerp(fieldOfView, oldFieldOfView, timer);
            }
            yield return 0;
        }

        if (!zoomIn)
            isLocked = false;
    }

    public bool SetStandMode()
    {
        if (motor.desiredVelocity.magnitude == 0)
        {
            StandMode = true;
        }
        else
            StandMode = false;

        return StandMode;
    }
	
	public void ShowCameraMode()
	{
		Debug.Log("Camera mode: "+CurrentView);
		if(target != null)
			Debug.Log("target.name: "+target.name+", heightOffSet: "+heightOffSet);
		else
			Debug.Log("target is null, heightOffSet: "+heightOffSet);
		if(WatchTarget != null)
			Debug.Log("WatchTarget.name: "+WatchTarget.name+", WatchTargetHeight: "+WatchTargetHeight);
		else
			Debug.Log("WatchTarget is null, WatchTargetHeight: "+WatchTargetHeight);

		Debug.Log("angleH: "+angleH+", angleV: "+angleV);
		
		Debug.Log("CameraFocus: "+StandMode);
		Debug.Log("StandMode: "+StandMode);
	}

    public Vector3 FPSPos
    {
        get
        {
            return target.position + Vector3.up * WatchTargetHeight;
        }
    }
}


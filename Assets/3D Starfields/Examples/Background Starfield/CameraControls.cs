using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour
{
    public Transform myLookTarget;

    public float myDistance = 500;
    public float myMinDistance = 100;
    public float myMaxDistance = 5000;
    public float myMaxZoomSpeed = 500;

    public float myYawSpeed = 100;
    public float myPitchSpeed = 100;

    public float myPitchMin = -20;
    public float myPitchMax = 80;

    public float myInitialPitch = 0;
    public float myInitialYaw = 20;

    public float myAngularSmoothSpeed = 1;
    public float myZoomSmoothSpeed = 1;

	public bool myEnableControls = true;
	public float myAutoYawSpeed = 20;

    private float myYaw = 0;
    private float myPitch = 0;
    private Vector3 myAngularVelocity;

    private float myZoomSpeed = 0.0f;

    void Awake()
    {
        myPitch = myInitialPitch;
        myYaw = myInitialYaw;
    }

    public void LateUpdate()
    {
        if (myLookTarget)
        {
            // If moving the mouse around, rotate the camera
            Vector3 desiredAngularVelocity = Vector3.zero;

			if (Input.GetKey(KeyCode.Mouse1) && myEnableControls)
            {
                desiredAngularVelocity.x = -Input.GetAxis("Mouse Y") * myPitchSpeed;
                desiredAngularVelocity.y = Input.GetAxis("Mouse X") * myYawSpeed;
                myAngularVelocity = desiredAngularVelocity;
            }

			if(myAutoYawSpeed > 0 && !myEnableControls)
			{
				desiredAngularVelocity.y = myAutoYawSpeed;
				myAngularVelocity = desiredAngularVelocity;
			}
            
            myAngularVelocity = Vector3.Lerp(myAngularVelocity, Vector3.zero, Time.deltaTime * myAngularSmoothSpeed);

            myPitch += myAngularVelocity.x * Time.unscaledDeltaTime;
            myYaw += myAngularVelocity.y * Time.unscaledDeltaTime;
            myPitch = ClampAngle(myPitch, myPitchMin, myPitchMax);

			float desiredZoomSpeed = 0.0f;
			if(myEnableControls)
				desiredZoomSpeed = Input.GetAxis("Mouse ScrollWheel") * myMaxZoomSpeed * -1;

            if (Mathf.Abs(desiredZoomSpeed) > 0)
                myZoomSpeed = desiredZoomSpeed;
            else
                myZoomSpeed = Mathf.Lerp(myZoomSpeed, 0, Time.deltaTime * myZoomSmoothSpeed);

            myDistance += myZoomSpeed * Time.unscaledDeltaTime;
            myDistance = Mathf.Clamp(myDistance, myMinDistance, myMaxDistance);

            Quaternion rotation = Quaternion.Euler(myPitch, myYaw, 0);
            Vector3 position = rotation * (Vector3.forward * -myDistance) + myLookTarget.position;

            transform.rotation = rotation;
            transform.position = position;
        }
    }

    private float ClampAngle(float anAngle, float aMin, float aMax)
    {
        if (anAngle < -360)
            anAngle += 360;
        if (anAngle > 360)
            anAngle -= 360;
        return Mathf.Clamp(anAngle, aMin, aMax);
    }
}

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class IpadTiltController : MonoBehaviour
{
    Vector3 lastAcceleration;
    public Vector3 accelerationDebug;

    bool isLookingUp = false,
         isLookingUpLast = false;

    void Update()
    {
        if (Application.isEditor)
        {
        }
        else
        {
#if UNITY_IOS
            accelerationDebug = Input.acceleration;
#endif
        }

        accelerationDebug.z = Mathf.Clamp(accelerationDebug.z, -1, 1);

        float difference = Mathf.Abs(lastAcceleration.z - accelerationDebug.z);

        isLookingUp = lastAcceleration.z < accelerationDebug.z;

        /*
        if (isLookingUpLast != isLookingUp)
        {
            isLookingUpLast = isLookingUp;
            StopAllCoroutines();
            busy = false;
        }
         */

        if (difference > .1f && !busy)
        {
            StartCoroutine(LerpCamera(lastAcceleration.z < accelerationDebug.z, difference, accelerationDebug));

            /*
            if (lastAcceleration.z < accelerationDebug.z)
            {
                // Looking up
                camera.transform.RotateAround(transform.TransformDirection(Vector3.left), difference);
            }
            else
            {
                // Looking down
                camera.transform.RotateAround(transform.TransformDirection(Vector3.right), difference);
            }
             */
        }
    }

    bool busy = false;
    IEnumerator LerpCamera(bool isUp, float diff, Vector3 acceleration)
    {
        busy = true;

        Quaternion from = transform.rotation;
        Vector3 rotateAround = transform.TransformDirection(Vector3.right);
        if (isUp)
            rotateAround = transform.TransformDirection(Vector3.left);
        Vector3 camForward = transform.TransformDirection(Vector3.forward);
        camForward = Quaternion.AngleAxis(diff * Mathf.Rad2Deg * 1.5f, rotateAround) * camForward;
        Quaternion to = Quaternion.LookRotation(camForward);

        float timer = 0;
        while (timer < 1)
        {
            Quaternion curr = Quaternion.Lerp(from, to, timer);

            GetComponent<Camera>().transform.rotation = curr;
            GetComponent<Camera>().transform.localEulerAngles = new Vector3(GetComponent<Camera>().transform.localEulerAngles.x, 0, 0);

            timer += Time.deltaTime * 4;
            yield return 0;
        }

        lastAcceleration = acceleration;
        busy = false;
    }
    

    void OnGUI()
    {
        GUI.Box(new Rect(10, 100, 100, 30), accelerationDebug.ToString());

        GUI.Box(new Rect(10, 140, 300, 30), "Input.gyro.rotationRate = " + Input.gyro.rotationRate.ToString());

        GUI.Box(new Rect(10, 180, 300, 30), "Input.gyro.rotationRateUnbiased = " + Input.gyro.rotationRateUnbiased.ToString());
    }
}

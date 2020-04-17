using UnityEngine;
using System.Collections;

public class MatchRotation : MonoBehaviour
{
    public Transform myTransform;
	
	void LateUpdate ()
    {
        if(myTransform != null)
            transform.rotation = myTransform.rotation;
	}
}

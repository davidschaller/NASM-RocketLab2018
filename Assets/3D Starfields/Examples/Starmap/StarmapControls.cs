using UnityEngine;
using System.Collections;

public class StarmapControls : MonoBehaviour
{
	public GameObject myShip;
	public LayerMask myPickingMask;
	public float myTravelRange = 500;
	public float myShipMoveSpeed = 100;

    private Camera myCamera;
	private ObjectFieldGenerator myObjectField;
	private LineRenderer myLineRenderer;
	private Transform myCurrentLocation;

    void Awake()
    {
        myCamera = Camera.main;
		myObjectField = FindObjectOfType<ObjectFieldGenerator> ();
		myLineRenderer = GetComponent<LineRenderer> ();
    }

	void Start()
	{
        // Find object nearest to the center, to pick as starting point
        Transform startingLocation = null;

        float closestSqDistance = Mathf.Infinity;
        int childCount = myObjectField.transform.childCount;
        for(int i = 0; i < childCount; ++i)
        {
            Transform child = myObjectField.transform.GetChild(i);
            float sqDist = (child.transform.position - myObjectField.transform.position).sqrMagnitude;
            if(sqDist < closestSqDistance)
            {
                closestSqDistance = sqDist;
                startingLocation = child;
            }
        }

		if(startingLocation != null)
			MoveShipInstantly(startingLocation);
	}

	void Update ()
    {
        myLineRenderer.enabled = false;

        if (Input.GetMouseButton(1)) // Rotating with the mouse, skip picking
            return;

        Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitInfo;
		if( Physics.Raycast(ray, out hitInfo, 5000, myPickingMask) )
        {
			Transform hoveredTransform = hitInfo.collider.transform;
			Transform currentLocation = myCurrentLocation != null ? myCurrentLocation : myShip.transform;

			if(hoveredTransform != currentLocation && (hoveredTransform.position - currentLocation.position).sqrMagnitude < Mathf.Pow(myTravelRange, 2))
			{
				myLineRenderer.enabled = true;
				myLineRenderer.SetPosition(0, currentLocation.position);
				myLineRenderer.SetPosition(1, hoveredTransform.position);

				if (Input.GetMouseButtonDown (0)) 
				{
					MoveShip(hoveredTransform);
				}
			}
        }
    }

	void MoveShipInstantly(Transform aDestTransform)
	{
		myShip.transform.position = aDestTransform.position + Vector3.up * 10;
		myCurrentLocation = aDestTransform;
	}

	void MoveShip(Transform aDestTransform)
	{
		StopAllCoroutines ();
		StartCoroutine (MoveShipOverTime (aDestTransform));
	}

	IEnumerator MoveShipOverTime(Transform aDestTransform)
	{
		myCurrentLocation = null;
		Vector3 destinationPos = aDestTransform.position + Vector3.up * 10;

		bool arrived = false;
		while (!arrived) 
		{
			Vector3 toDestination = destinationPos - myShip.transform.position;
			float distanceThisFrame = Time.deltaTime * myShipMoveSpeed;
			if(toDestination.sqrMagnitude < Mathf.Pow(distanceThisFrame, 2))
			{
				distanceThisFrame = toDestination.magnitude;
				arrived = true;
				myCurrentLocation = aDestTransform;
			}

			toDestination.Normalize();
			myShip.transform.position = myShip.transform.position + toDestination * distanceThisFrame;

			Vector3 forward = toDestination;
			Vector3 upVector = Vector3.up;
			if(!arrived)
			{
				Vector3 left = Vector3.Cross(toDestination, Vector3.up);
				upVector = Vector3.Cross(left, toDestination);
			}
			else 
			{
				Vector3 left = Vector3.Cross(toDestination, myShip.transform.up);
				forward = Vector3.Cross(Vector3.up, left);
			}

			myShip.transform.rotation = Quaternion.LookRotation(forward, upVector);

			yield return new WaitForEndOfFrame();
		}
	}
}

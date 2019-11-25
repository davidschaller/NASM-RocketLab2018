using UnityEngine;
using System.Collections;

public class PlatformTrigger : MonoBehaviour
{
	public bool unevenPlatform = true;
	public bool switchesPCToFirstPerson = false;
	public Collider platformCollider;
	
	float floorLevel = 0;
	void Start ()
	{
		if (!unevenPlatform)
		{
			RaycastHit hit;
			Ray ray = new Ray(platformCollider.transform.position + Vector3.up * 10, Vector3.down);
			if (platformCollider.Raycast(ray, out hit, 100))
			{
				floorLevel = hit.point.y;
			}
			else
			{
				floorLevel = platformCollider.transform.position.y;
			}
		}
	}
	
	public float GetFloorLevel ()
	{
		float lvl = 0;
		if (unevenPlatform)
		{
			RaycastHit hit;
			Ray ray = new Ray(platformCollider.transform.position + Vector3.up * 10, Vector3.down);
			if (platformCollider.Raycast(ray, out hit, 100))
			{
				lvl = hit.point.y;
			}
		}
		else
		{
			lvl = floorLevel;
		}
		
		return lvl;
	}
}

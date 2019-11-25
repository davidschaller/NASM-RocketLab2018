using UnityEngine;
using System.Collections;

public class Waypoint : MonoBehaviour
{
    public bool groundWithLayer;
    public string layerName = "Walkable";

	public float speedHint = 0;
	
	public WaypointGroup GetWaypointGroup ()
	{
		if (transform.parent)
		{
			return (WaypointGroup)transform.parent.GetComponent(typeof(WaypointGroup));
		}
		
		return null;
	}
	
	float tolerance = 0.05f;
	public bool PositionIsClose (Vector3 pos)
	{
		if ((pos - transform.position).magnitude < tolerance)
			return true;
		return false;
	}

	static TerrainData cachedData;
	static Transform cachedTransform;
	bool grounded = false;
	public bool Grounded
	{
		get
		{
			return grounded;
		}
	}
	
	public void Ground ()
	{
        if (groundWithLayer && !string.IsNullOrEmpty(layerName))
        {
            RaycastHit hit;

            Ray ray = new Ray(transform.position + Vector3.up * 40, Vector3.down);

            

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer(layerName)))
            {
                //Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);

                transform.position = hit.point;
            }

            grounded = true;
            return;
        }

        if (transform.root.name.Contains("Music") || transform.root.name.Contains("Banjo") && (cachedData == null || cachedTransform == null))
        {
            if (transform && transform.root && transform.root.Find("Terrain"))
            {
                Terrain musicTerrain = transform.root.Find("Terrain").GetComponent<Terrain>();
                cachedData = musicTerrain.terrainData;
                cachedTransform = musicTerrain.transform;
            }
        }

		if ((cachedData == null || cachedTransform == null)  && TerrainManager.MainTerrain != null)
		{
			cachedData = TerrainManager.MainTerrain.terrainData;
			cachedTransform = TerrainManager.MainTerrain.transform;
		}
		else if (TerrainManager.MainTerrain == null)
		{
			Debug.Log("TerrainManager has no terrain", gameObject);
		}
		
		

		if (cachedData == null)
			Debug.LogWarning("active terrainData is null, instability is coming...");
		if (cachedTransform == null)
			Debug.LogWarning("active terrain transform is null, instability is coming...");


		if (cachedData != null && cachedTransform != null)
		{
			transform.position = new Vector3(
											 transform.position.x, 
											 cachedData.GetInterpolatedHeight(transform.position.x/cachedData.size.x, transform.position.z/cachedData.size.z) + cachedTransform.position.y, 
											 transform.position.z);
			grounded = true;
		}
	}
}

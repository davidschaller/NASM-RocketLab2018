using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreasureMiniMap : MonoBehaviour
{
	public Camera cam;
	public GameObject npcMarkerPrefab;
	public GameObject playerMarkerPrefab;

	bool mapEnabled = false;
	public bool MapEnabled
	{
		get
		{
			return mapEnabled;
		}
		set
		{
			mapEnabled = value;
			if (!value)
			{
				cam.enabled = false;
			}
			else
			{
				cam.enabled = true;
			}
		}
	}

	static TreasureMiniMap main;
	void Awake ()
	{
		main = this;
	}
	
	List<NPCController> trackedNPCs = new List<NPCController>();
	public List<NPCController> TrackedNPCs
	{
		get
		{
			return trackedNPCs;
		}
		set
		{
			trackedNPCs = value;
		}
	}

	public static void RemoveMarkerForNPC(string nm, bool removeAll)
	{
		NPCController rem = null;
		foreach (NPCController n in main.trackedNPCs)
		{
			if (n.npcName == nm || removeAll)
			{
				rem = n;
			}
		}

		if (rem != null)
		{
			Destroy(main.markers[rem].gameObject);
			main.markers.Remove(rem);
			main.trackedNPCs.Remove(rem);
		}
	}

	Dictionary<NPCController, Transform> markers = new Dictionary<NPCController, Transform>();
	void AddMarker (NPCController npc, Color c)
	{
		GameObject marker = (GameObject)Instantiate(npcMarkerPrefab,
													npc.transform.position + Vector3.up * 50,
													npc.transform.rotation);
		marker.GetComponent<Renderer>().material.color = c;
		marker.layer = LayerMask.NameToLayer("Minimap Camera Only");
		markers.Add(npc, marker.transform);
		marker.transform.parent = npc.transform;
	}

	public void Reset ()
	{
		RemoveMarkerForNPC(null, true);
	}
	
	Transform playerMarker;
	public void RegisterMystery(TreasureHuntItem def)
	{
		foreach(TreasureNPCPackage npc in def.npcs)
		{
			TrackedNPCs.Add(npc.npcController);
			AddMarker(npc.npcController, npc.minimapColor);
		}

		if (playerMarker == null)
		{
			GameObject playerC = GameObject.FindWithTag("Player");
			GameObject player = playerC.gameObject;
			playerMarker = ((GameObject)Instantiate(playerMarkerPrefab,
													player.transform.position + Vector3.up * 55,
													player.transform.rotation)).transform;
			playerMarker.transform.parent = player.transform;
			
		}
	}
}

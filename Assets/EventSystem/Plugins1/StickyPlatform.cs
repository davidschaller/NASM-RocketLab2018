using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StickyPlatform : MonoBehaviour
{
	void Awake ()
	{
		if (GetComponent<Rigidbody>() == null)
			gameObject.AddComponent(typeof(Rigidbody));

		GetComponent<Rigidbody>().isKinematic = true;
		GetComponent<Rigidbody>().useGravity = false;
	}

	void Attach(Transform t)
	{
		Debug.Log("Triggered by " + t.gameObject.name);
		savedParents[t] = t.parent;
		t.parent = transform;
	}
	
	
	Dictionary<Transform, Transform> savedParents = new Dictionary<Transform, Transform>();
	void OnTriggerEnter (Collider coll)
	{
		ActorBase npc = (ActorBase)coll.GetComponent(typeof(ActorBase));
		if (npc != null)
			Attach(npc.transform);
	}

	void OnTriggerStay (Collider coll)
	{
		ActorBase npc = (ActorBase)coll.GetComponent(typeof(ActorBase));
		if (npc != null && !savedParents.ContainsKey(npc.transform))
		{
			Debug.Log("Unregistered character present: " + npc.transform.name);
		}
	}

	void Detach(Transform t)
	{
		Debug.Log("Character leaving sticky platform: " + t.gameObject.name);
		t.parent = savedParents[t];
		savedParents[t] = null;
	}

	void OnTriggerExit (Collider coll)
	{
		ActorBase npc = (ActorBase)coll.GetComponent(typeof(ActorBase));
		if (npc != null)
			Detach(npc.transform);
	}
}

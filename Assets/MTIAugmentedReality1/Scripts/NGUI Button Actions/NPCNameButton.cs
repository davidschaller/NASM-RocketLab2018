using UnityEngine;
using System.Collections;

public class NPCNameButton : MonoBehaviour
{
	public static NPCNameButton main;
	public float defaultLabelHeight = 2;
	
	NPCController npc;
	static Camera uiCam;
	static Camera gameCam;
	
	IEnumerator Start ()
	{
		if (main == null)
		{
			main = this;
		}
		//yield return 0;
		gameObject.SetActiveRecursively(false);			
		
		if (this != main)
		{
//			active = true;
			enabled = true;
		}
		yield return 0;
		
		buttonActive = false;
	}
	
	public static void FindMain ()
	{
		NPCNameButton[] ns = (NPCNameButton[])Resources.FindObjectsOfTypeAll(typeof(NPCNameButton));
		//Debug.Log("Found NPCName button", n.gameObject);
		
		if (ns != null && ns.Length != 0)
		{
			main = ns[0];

			Debug.LogWarning("NPC Nameplate main replaced with " + main.gameObject);
			//Debug.Break();
		}
	}
	
	void OnClick ()
	{
		if (GPSActor.main)
			GPSActor.main.positionLocked = true;
		//MoveWithCompass.lockOrientation = true;
		MoveWithCompass.ignoreCompass = true;
		
		float y = 0;
		Transform player = MoveWithCompass.main.transform;
		bool reposition = false;
		if (MoveWithCompass.main.transform.parent)
		{
			y = MoveWithCompass.main.transform.transform.parent.position.y;
			player = MoveWithCompass.main.transform.parent;
			reposition = true;
		}
		else
		{
			y = MoveWithCompass.main.transform.position.y;
			player = MoveWithCompass.main.transform;
		}
		
		if (reposition)
		{
			player.position = npc.transform.position + (player.position - npc.transform.position).normalized * 1.5f;
			MoveWithCompass.main.transform.LookAt(npc.Npc.transform.position + Vector3.up * 1.1f);			
		}
		if (Application.isEditor)
			Debug.LogWarning("Start dialog with npc: " + npc.npcName + " at " + Time.time);
		NPCDialogButton.main.ShowDialog(npc);
		npc.OnTalkToClick();
		Destroy(gameObject);
	}
	
	public static NPCNameButton CreateInstance (NPCController n)
	{
		if (Application.isEditor)
			Debug.LogWarning("Create NPC Name button instance for NPC " + n.name, n.gameObject);
		
		NPCNameButton newInstance = (NPCNameButton)Instantiate(main);
		newInstance.name = main.name + " " + n.npcName;
		newInstance.gameObject.SetActiveRecursively(true);
		newInstance.transform.parent = main.transform.parent;
        newInstance.GetComponentInChildren<UILabel>().text = n.npcName;		
		if (n.talkLabelHeight == 0)
			n.talkLabelHeight = main.defaultLabelHeight;
		gameCam = Camera.main;
		
		Vector3 tpos = gameCam.WorldToViewportPoint(n.transform.position + Vector3.up*n.talkLabelHeight);
		
		GameObject uiRoot = main.transform.root.gameObject;
		uiCam = uiRoot.transform.Find("Camera").GetComponent<Camera>();
		Vector3 mPos = uiCam.ViewportToWorldPoint(tpos);
		mPos.z = 0f;
		
		newInstance.transform.position = mPos;
		newInstance.transform.localScale = Vector3.one;
		newInstance.transform.localRotation = Quaternion.identity;

		newInstance.lastPos = n.transform.position;
		
		newInstance.npc = n;
		
		return newInstance;
	}
	
	void DeactivateButton ()
	{
		Debug.LogWarning("Deactivate button", gameObject);
		
		gameObject.SetActiveRecursively(false);
		enabled = true;
//		active = true;
		
		buttonActive = false;
	}
	
	void ActivateButton ()
	{
		Debug.LogWarning("Reactivate button", gameObject);
		gameObject.SetActiveRecursively(true);
		
		buttonActive = true;
	}
	
	Vector3 lastPos;
	Quaternion lastRot;
	bool buttonActive = false;
	void Update ()
	{
		if (npc != null)
		{
			float dist = (Camera.main.transform.position - npc.transform.position).magnitude;
			bool inFrontAndClose = false;
			
			if (dist < npc.maxTalkDistance)
			{
				Vector3 toNPC = npc.transform.position - Camera.main.transform.position;
				//Debug.Log("Dot: " + Vector3.Dot(Camera.main.transform.TransformDirection(Vector3.forward), toNPC));
				if (Vector3.Dot(Camera.main.transform.TransformDirection(Vector3.forward), toNPC) > 0)
				{
					//Debug.LogWarning("In front and close to " + npc.name);
					inFrontAndClose = true;
				}
			}
			
			if (!buttonActive && inFrontAndClose)
			{
				//Debug.Log("Should activate button for " + npc.name);
				ActivateButton();
			}
			else if (inFrontAndClose)
			{
				//Debug.Log("Button active? " + buttonActive + " for " + npc.name);
			}
			
			if(inFrontAndClose)// && (lastPos != gameCam.transform.position || lastRot != gameCam.transform.rotation))
			{
				Vector3 tpos = gameCam.WorldToViewportPoint(npc.transform.position + Vector3.up*npc.talkLabelHeight);
				Vector3 mPos = uiCam.ViewportToWorldPoint(tpos);
		
				transform.position = mPos;
				if (transform.localPosition.z != 0)
					transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
				
				lastPos = gameCam.transform.position;
				lastRot = gameCam.transform.rotation;
				
				//Debug.Log("Update position to " + mPos);
			}
			else
			{
				//Debug.Log("Not updating position for " + name, gameObject);
			}
			
			if (!inFrontAndClose)		
			{
				if (buttonActive)
				{
					DeactivateButton();
				}
				else
				{
					//Debug.LogWarning("Leaving this button alone: " + name, gameObject);
				}
			}
		}
	}
}

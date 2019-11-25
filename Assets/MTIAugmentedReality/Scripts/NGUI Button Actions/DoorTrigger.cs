using UnityEngine;
using System.Collections;

public class DoorTrigger : MTIUIBase
{
	public string sceneToLoad;
	public string buildingDescription;
	
	bool overriden = true;
	bool triggered = false;
	
	IEnumerator Start ()
	{
		yield return 5;
		overriden = false;
	}
	
	void OnTriggerStay (Collider c)
	{
		if (overriden || triggered)
			return;
		
		if (c.transform.root.CompareTag("Player"))
		{
			DoTrigger();
		}		
	}

	void DoTrigger ()
	{
		triggered = true;
		bool showButton = true;
		if (sceneToLoad == "")
			showButton = false;
		BuildingButton.Show(this, showButton);		
	}
	
	void OnTriggerEnter (Collider c)
	{
		if(overriden)
			return;
				
		if (c.transform.root.CompareTag("Player"))
		{
			DoTrigger();
		}
	}
	
	void OnTriggerExit (Collider c)
	{
		if (c.transform.root.CompareTag("Player"))
		{
			triggered = false;
			bool showButton = true;
			if (sceneToLoad == "")
				showButton = false;
				
			BuildingButton.Hide();
		}
	}
	
	public void LoadRoom ()
	{
		SceneLoaderIndication.LoadLevel(sceneToLoad);
	}
	
	void OnClick ()
	{
		LoadRoom();
	}
}

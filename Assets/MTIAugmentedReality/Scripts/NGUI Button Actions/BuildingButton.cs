using UnityEngine;
using System.Collections;

public class BuildingButton : MTIUIBase
{
	UIToggleStub buildingLabelBox;
	UIToggleStub buildingLabelText;
	
	public static BuildingButton main;
	IEnumerator Start ()
	{
		//yield return 0;
		
		main = this;
		
//		Debug.Log("MAIN BUILDING BUTTON " + transform.root.name + " active? " + active, gameObject);
		buildingLabelBox = transform.parent.Find("Building label box").GetComponent<UIToggleStub>();
		buildingLabelText = transform.parent.Find("Building label text").GetComponent<UIToggleStub>();
		
		Hide(true);
		
		yield return 0;
	}

	public static void Hide ()
	{
		if (Application.isEditor)
			Debug.Log("Hide building button " + main.name);
		
		main.DeactivateUI(false);
		main.buildingLabelBox.DeactivateUI(false);
		main.buildingLabelText.DeactivateUI(false);
	}
	
	public static void Hide (bool showButton)
	{
		if (Application.isEditor)
			Debug.Log("Hide building button " + main.name);

		if (showButton)
			main.DeactivateUI(false);
		main.buildingLabelBox.DeactivateUI(false);
		main.buildingLabelText.DeactivateUI(false);
	}

	public static void Show ()
	{
		if (Application.isEditor)
			Debug.Log("Show building button " + main.name);
		
		main.DeactivateUI(true);
		main.buildingLabelBox.DeactivateUI(true);
		main.buildingLabelText.DeactivateUI(true);
	}

	static DoorTrigger door;
	public static void Show (DoorTrigger door, bool showButton)
	{
		if (Application.isEditor)
			Debug.Log("Show building button " + main.name);

		BuildingButton.door = door;
		if (showButton)
			main.DeactivateUI(true);
		main.buildingLabelBox.DeactivateUI(true);
		main.buildingLabelText.DeactivateUI(true);
		
		main.buildingLabelText.GetComponent<UILabel>().text = door.buildingDescription;
		main.transform.Find("Label").GetComponent<UILabel>().text = "Enter " + door.sceneToLoad;
	}


	static RoomController room;
	public static void Show (RoomController room)
	{
		if (Application.isEditor)
			Debug.Log("Show building button " + main.name);
		
		BuildingButton.room = room;
		main.DeactivateUI(true);
		main.buildingLabelBox.DeactivateUI(true);
		main.buildingLabelText.DeactivateUI(true);
		
		main.buildingLabelText.GetComponent<UILabel>().text = room.roomDescription;
		main.transform.Find("Label").GetComponent<UILabel>().text = "Exit " + Application.loadedLevelName;
	}

	
	void OnClick ()
	{
		if (!ToggleVirtualWorld.showVirtual)
		{
			CamTexture.camTextureActiveLastScene = true;
			CamTexture.StopCam();			
		}
		else
		{
			CamTexture.camTextureActiveLastScene = false;
		}
		
		
		if (main.transform.Find("Label").GetComponent<UILabel>().text.StartsWith("Enter"))
		{
			PersistentData.Main.restorePosition = true;
			PersistentData.Main.storedPosition = GPSActor.main.transform.position;
			PersistentData.Main.storedRotation = GPSActor.main.transform.rotation;
		
			door.LoadRoom();
		}
		else
			room.ExitRoom();
		
		Hide();
	}
}

using UnityEngine;
using System.Collections;

public class RoomController : MTIUIBase
{
	public bool useUnityGUI = false;
	public string loadLevelOnExit;
	public string roomDescription;
	
	IEnumerator Start ()
	{
		yield return new WaitForSeconds(0.5f);
		
		BuildingButton.Show(this);
	}
		
	public void ExitRoom ()
	{
		SceneLoaderIndication.LoadLevel(loadLevelOnExit);
	}
	
	void OnClick ()
	{
		ExitRoom();
	}
}

using UnityEngine;
using System.Collections;

public class LoadHomeLevel : MTIUIBase
{
	public string levelToLoad;
	
	void OnClick ()
	{
		Debug.Log("Loading home screen level: " + levelToLoad);
		
		Application.LoadLevel(levelToLoad);
	}
}

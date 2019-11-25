using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovementButton : MonoBehaviour
{
	public enum MoveType
	{
		MoveForward,
		MoveBack,
		RotateRight,
		RotateLeft,
	}
	
	public MoveType moveType;

	int textureWidth = 64;
	bool unityGUI = false;
	public Texture2D tex;

	public static List<MovementButton> registry = new List<MovementButton>();
	public static void RemoveAllButtons ()
	{
		foreach(MovementButton b in registry)
		{
			Destroy(b);
		}
		registry.Clear();
	}
	
	void Start ()
	{
		if (PersistentData.Main.unityWalkButtons && (moveType == MoveType.MoveForward || moveType == MoveType.MoveBack))
			unityGUI = true;
		else if (PersistentData.Main.unityTurnButtons && (moveType == MoveType.RotateRight || moveType == MoveType.RotateLeft))
			unityGUI = true;
		
		registry.Add(this);
		switch(moveType)
		{
			case MoveType.MoveForward:
				tex = PersistentData.Main.virtualForward;
			break;
			case MoveType.MoveBack:
				tex = PersistentData.Main.virtualBack;
			break;
			case MoveType.RotateRight:
				tex = PersistentData.Main.virtualRight;
			break;
			case MoveType.RotateLeft:
				tex = PersistentData.Main.virtualLeft;
			break;
		}								
		
		if (tex != null)
			textureWidth = tex.width;
	}
	
	void AddActorToCompass ()
	{
		if (Application.loadedLevel != 0)
		{
			GPSActor a = (GPSActor)FindObjectOfType(typeof(GPSActor));
			if (a == null)
			{
				MoveWithCompass c = (MoveWithCompass)FindObjectOfType(typeof(MoveWithCompass));
				a = c.gameObject.AddComponent<GPSActor>();
			}
		}		
	}
	
	void OnLevelWasLoaded (int lvl)
	{
		Invoke("AddActorToCompass", 0.5f);
	}

	void SendMoveEvent ()
	{
		switch(moveType)
		{
			case MoveType.MoveForward:
				GPSActor.main.MoveForward();
			break;
			case MoveType.MoveBack:
				GPSActor.main.MoveBackward();
			break;
			case MoveType.RotateRight:
				GPSActor.main.RotateRight();
			break;
			case MoveType.RotateLeft:
				GPSActor.main.RotateLeft();
			break;
		}						
	}
	
	void Update ()
	{
		if (mouseDown)
		{
			SendMoveEvent();
		}
	}
	
	bool mouseDown = false;
	void OnPress (bool down)
	{
		if (down)
			mouseDown = true;
		else
			mouseDown = false;
	}
	
	GUIStyle buttonStyle;
	Rect rect = new Rect(0,0,0,0);
	public static bool inverted = false;
	void OnGUI ()
	{
		if (!unityGUI)
			return;
		
		if (inverted)
		{
			Vector2 pivotPoint = new Vector2(Screen.width/2, Screen.height/2);
			GUIUtility.RotateAroundPivot(180, pivotPoint);
		}
		
		int centerX = Screen.width/2;
		int y = Screen.height - textureWidth - 10;

		switch(moveType)
		{
			// Left (25pixel gap) Right  (460 pixel gap)  Forward (25pixel gap) Backwards
			case MoveType.RotateLeft:
				rect = new Rect(centerX - 230 - textureWidth - 15 - textureWidth, y, textureWidth, textureWidth);
			break;
			case MoveType.RotateRight:
				rect = new Rect(centerX - 230 - textureWidth, y, textureWidth, textureWidth);
			break;
			case MoveType.MoveForward:
				rect = new Rect(centerX + 230, y, textureWidth, textureWidth);
			break;
			case MoveType.MoveBack:
				rect = new Rect(centerX + 230 + textureWidth + 15, y, textureWidth, textureWidth);
			break;
		}

		if (buttonStyle == null)
		{
			buttonStyle = new GUIStyle();
			buttonStyle.normal.background = tex;
		}
		if (GUI.RepeatButton(rect, "", buttonStyle))
			SendMoveEvent();
	}
}

using UnityEngine;
using System.Collections;

public class TreasureHuntNextGUI : MonoBehaviour
{
	public string buttonText = "Continue";
	
	public string itemNameStyle = "Label";
	public string buttonStyle = "Button";
	public Texture2D nextItemImage;
	public string nextItemImageStyle = "my collection small card";
	
	
	Texture2D itemThumbnail;
	public Texture2D ItemThumbnail
	{
		get
		{
			return itemThumbnail;
		}
		set
		{
			itemThumbnail = value;
		}
	}
	
	void OnGUI ()
	{
		GUI.depth = 100;
		
		GUI.skin = TreasureHuntController.GUISkin;

		GUIStyle boxStyle = GUI.skin.GetStyle("next item box");
		
		Rect boxRect = new Rect (Screen.width/2 - boxStyle.fixedWidth/2,
								 Screen.height/2 - boxStyle.fixedHeight/2,
								 boxStyle.fixedWidth,
								 boxStyle.fixedHeight);
		
		GUI.Box(boxRect, "Your Next Mystery Object", boxStyle);
		boxRect.y += boxStyle.padding.top + boxStyle.margin.top;
		boxRect.height -= boxStyle.padding.top + boxStyle.margin.top + boxStyle.padding.bottom + boxStyle.margin.bottom;
		
		GUILayout.BeginArea(boxRect);
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUIStyle cardStyle = new GUIStyle(GUI.skin.GetStyle(nextItemImageStyle));
		if (TreasureHuntController.CurrentMystery != null)
			itemThumbnail = nextItemImage;
		if (itemThumbnail != null)
			cardStyle.normal.background = itemThumbnail;
		GUILayout.Label("", cardStyle);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.FlexibleSpace();
		GUIStyle nameStyle = GUI.skin.GetStyle(itemNameStyle);
		if (TreasureHuntController.CurrentMystery == null)
			GUILayout.Label("George's belt buckle", nameStyle);
		else
			GUILayout.Label(TreasureHuntController.CurrentMystery.itemName, nameStyle);
		
		GUILayout.FlexibleSpace();

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button(buttonText))
		{
			enabled = false;
			GUIManager.PlayButtonSound();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.EndArea();
	}
}

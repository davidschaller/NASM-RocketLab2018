using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreasureHuntCollectionGUI : MonoBehaviour
{
	GUISkin skin;
	public Rect screenRect;

	public string bonusDescriptionStyle = "Label";

	static TreasureHuntCollectionGUI main;
	public static TreasureHuntCollectionGUI Main
	{
		get
		{
			return main;
		}
		private set
		{
		}
	}
	
	void Awake ()
	{
		main = this;
		
		if (screenRect.width <= 0)
			screenRect.width = Screen.width;
		if (screenRect.height <= 0)
			screenRect.height = Screen.height;
	}

	void Start ()
	{
		skin = TreasureHuntController.GUISkin;
	}

	enum TabState
	{
		MyCollection,
		EasterEggs,
	}
	TabState displayState = TabState.MyCollection;
	public void DisplayEasterEggs ()
	{
		displayState = TabState.EasterEggs;
	}

	void DrawCard(int cardIndex)
	{		
		bool found = cardIndex < TreasureMysteryScrollGUI.CollectedCount;
		
		GUIStyle smallCardStyle = skin.GetStyle("my collection small card");
		GUIStyle s = new GUIStyle(smallCardStyle);

		GUILayout.BeginVertical();
		if (found)
		{
			s.normal.background = TreasureHuntController.main.TreasureHuntOrder[cardIndex].thumbnailImage;
			GUILayout.Label("", s, GUILayout.Width(s.fixedWidth));
		}
		else
		{
			
			GUILayout.Label("", "my collection small card ?");
		}
		
		if (found)
		{
			GUIStyle buttonStyle = "my collection small card button";
			string buttonName = TreasureHuntController.main.TreasureHuntOrder[cardIndex].name;
			Vector2 buttonSize = buttonStyle.CalcSize(new GUIContent(buttonName));
			
			GUILayout.BeginHorizontal(GUILayout.MaxWidth(s.fixedWidth +
														 s.padding.left +
														 s.padding.right +
														 s.margin.left +
														 s.margin.right));
			
			GUILayout.Space(s.padding.left + s.margin.left + (s.fixedWidth/2 - buttonSize.x/2));
			if (GUILayout.Button(buttonName, buttonStyle))
			{
				TreasureHuntController.main.TreasureHuntOrder[cardIndex].DisplayBasic();
				GUIManager.PlayButtonSound();
			}			
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
		
		GUILayout.EndVertical();
	}

	void DrawEggCard(int bucketIndex)
	{
		bool found = bucketIndex < bonusObjects.Count;
		
		GUIStyle smallCardStyle = skin.GetStyle("my collection small card");

		GUILayout.BeginVertical();
		if (found)
		{
			GUIStyle s = new GUIStyle(smallCardStyle);
			s.normal.background = bonusObjects[bucketIndex].itemImage;


			GUIStyle buttonStyle = "my collection small card button";
			string buttonName = bonusObjects[bucketIndex].shortDescription;
			Vector2 buttonSize = buttonStyle.CalcSize(new GUIContent(buttonName));

			
			GUILayout.Label("", s);

			GUILayout.BeginHorizontal(GUILayout.MaxWidth(s.fixedWidth +
														 s.padding.left +
														 s.padding.right +
														 s.margin.left +
														 s.margin.right));
			GUILayout.Space(s.padding.left + s.margin.left + (s.fixedWidth/2 - buttonSize.x/2));
			if (GUILayout.Button(buttonName, "my collection small card button"))
			{
				bonusObjects[bucketIndex].DisplayFromCollection();
				GUIManager.PlayButtonSound();
			}
			
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
		else
		{
			
			GUILayout.Label("", "my collection small card ?");
		}
		
		GUILayout.EndVertical();
	}


	public float tabOverlap = 5;
	Vector2 eggsScroll;
	void OnGUI ()
	{
		GUI.depth = -100;
		
		GUI.skin = skin;

		GUILayout.BeginArea(screenRect);
		GUIStyle tabStyle = skin.GetStyle("my collection tab button");
		Vector2 tabSize = tabStyle.CalcSize( new GUIContent("My Collection"));
		float tabHeight = tabSize.y;
		GUILayout.EndArea();

		Rect bottomRect = new Rect(screenRect);
		bottomRect.y += tabHeight;
		
		GUILayout.BeginArea(bottomRect);
		GUIStyle collectionBoxStyle = skin.GetStyle("my collection box");
		GUIStyle smallCardStyle = skin.GetStyle("my collection small card");
		
		GUILayout.Box("", collectionBoxStyle);
		if (displayState == TabState.MyCollection)
		{
			GUILayout.BeginHorizontal();
			for(int i=0;i<5;i++)
			{
				DrawCard(i);
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			for(int i=5;i<10;i++)
			{
				DrawCard(i);
			}
			GUILayout.EndHorizontal();
		}
		else if (displayState == TabState.EasterEggs)
		{
			eggsScroll = GUILayout.BeginScrollView(eggsScroll,
												   GUILayout.Width(screenRect.width - 40),
												   GUILayout.Height(smallCardStyle.fixedHeight * 2.5f));
			
			GUILayout.BeginHorizontal();
			for(int i=0;i<5;i++)
			{
				DrawEggCard(i);
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			for(int i=5;i<10;i++)
			{
				DrawEggCard(i);
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			for(int i=10;i<15;i++)
			{
				DrawEggCard(i);
			}
			GUILayout.EndHorizontal();
			
			
			GUILayout.BeginHorizontal();
			for(int i=15;i<20;i++)
			{
				DrawEggCard(i);
			}
			GUILayout.EndHorizontal();
			
			GUILayout.EndScrollView();
		}
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Back to Treasure Hunt", "back to hunt button"))
		{
			displayState = TabState.MyCollection;
			enabled = false;
			GUIManager.PlayButtonSound();
		}
		
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.EndArea();


		Rect topRect = new Rect(screenRect);
		topRect.height = tabHeight + tabOverlap;
		
		GUILayout.BeginArea(topRect);
		GUILayout.BeginHorizontal();

		tabHeight = displayState == TabState.MyCollection ? tabSize.y + tabOverlap : tabSize.y;
		if(GUILayout.Button("My Collection", tabStyle, GUILayout.Height(tabHeight)))
		{
			displayState = TabState.MyCollection;
			GUIManager.PlayButtonSound();
		}

		tabHeight = displayState == TabState.EasterEggs ? tabSize.y + tabOverlap : tabSize.y;
		if (GUILayout.Button("Bonus Objects", tabStyle, GUILayout.Height(tabHeight)))
		{
			displayState = TabState.EasterEggs;
			GUIManager.PlayButtonSound();
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	List<TreasureBonusObject> bonusObjects = new List<TreasureBonusObject>();
	public static void FoundBonusObject(TreasureBonusObject obj)
	{
		main.bonusObjects.Add(obj);
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreasureMysteryScrollGUI : MonoBehaviour
{
	TreasureHuntCollectionGUI collectionGUI;
	
	static TreasureMysteryScrollGUI main;
	void Awake ()
	{
		collectionGUI = GetComponent<TreasureHuntCollectionGUI>();
		main = this;
	}

	void Start ()
	{
		skin = TreasureHuntController.GUISkin;
	}
	
	GUISkin skin;
	public float bottomOffset = 20;
	public float leftOffset = 20;

	public float sliderQuestionIncrement = 40;

	public static void IncrementSlider()
	{
		main.sliderOffset += main.sliderQuestionIncrement;
	}
	
	float sliderOffset = 0;
	int collectedCount = 0;
	public static int CollectedCount
	{
		get
		{
			return main.collectedCount;
		}
		set
		{
			main.collectedCount = value;
			TreasureHuntController.CollectedItemCount = main.collectedCount;
		}
	}
	
	int totalCount = 0;
	public int TotalCount
	{
		get
		{
			return totalCount;
		}
		set
		{
			totalCount = value;
		}
	}
	void OnGUI ()
	{
		GUI.skin = skin;

		GUIStyle scrollStyle = skin.GetStyle("mystery object scroll");
		GUIStyle sliderStyle = skin.GetStyle("mystery object scroll background slider");
		
		Rect scrollRect = new Rect(leftOffset,
								   Screen.height - scrollStyle.fixedHeight - bottomOffset,
								   scrollStyle.fixedWidth,
								   scrollStyle.fixedHeight);

		Rect sliderRect = scrollRect;
		sliderRect.y -= sliderOffset;
		sliderStyle.normal.background = sliderImage;
		GUI.Box( sliderRect, "", sliderStyle);
		GUI.Box( scrollRect, "", scrollStyle);
		GUILayout.BeginArea( scrollRect );
		GUILayout.Label(scrollTitle, "mystery scroll title");
		GUILayout.Label("Who to Ask?", "mystery scroll question");

		for (int i=0;i<names.Count;i++)
		{			
			GUILayout.BeginHorizontal();
			// The logic of this got inverted due to misunderstanding
			// the spec
			if (checkmarks[names[i]])
			{
				GUIStyle noCheck = new GUIStyle(skin.GetStyle("mystery scroll no-check"));
				noCheck.normal.background = TreasureHuntController.CurrentMystery.NPCDotByName(names[i]);
				GUILayout.Box("", noCheck);	
			}
			else
			{
				GUILayout.Box("", "mystery scroll checkmark");
			}
			GUILayout.Label(names[i], "mystery scroll name");
			GUILayout.EndHorizontal();
			GUILayout.Label(locations[i], "mystery scroll location");
		}
		if (GUILayout.Button("My Collection", "my collection button"))
		{
			collectionGUI.enabled = true;
			GUIManager.PlayButtonSound();
		}
		GUILayout.EndArea();

		float cStart = scrollRect.y + scrollRect.height;
		Rect collectedCountRect = new Rect( scrollRect.x, cStart, scrollRect.width, Screen.height - cStart);
		
		GUILayout.BeginArea(collectedCountRect);
		GUILayout.Label("Objects collected: " + collectedCount + " out of " + totalCount, "objects collected");
		GUILayout.EndArea();
		
	}

	string scrollTitle = "No More Objects!";
	public string ScrollTitle
	{
		get
		{
			return scrollTitle;
		}
		set
		{
			scrollTitle = value;
		}
	}

	List<string> names = new List<string>();
	List<string> locations = new List<string>();
	Dictionary<string, bool> checkmarks = new Dictionary<string, bool>();
	public void AddNPCInfo(string nm, string loc)
	{
		names.Add(nm);
		locations.Add(loc);
		checkmarks.Add(nm, true);
	}

	public static void UncheckNPC(string nm)
	{
		main.checkmarks[nm] = false;
	}

	Texture2D sliderImage;
	void SetupSlider(Texture2D sliderImage)
	{
		this.sliderImage = sliderImage;
	}

	public void Reset ()
	{
		checkmarks.Clear();
		names.Clear();
		locations.Clear();
		sliderOffset = 0;
	}
	
	public void RegisterMystery(TreasureHuntItem def)
	{		
		if (def == null) Debug.LogError("Empty Treasure Hunt.  Perhaps you forgot to assign one to the Treasure Controller?");

		ScrollTitle = def.itemName;
		
		for(int i=0;i<def.npcs.Length;i++)
		{
			AddNPCInfo(def.npcs[i].npcController.npcName, def.npcs[i].npcLocationName);
		}
		
		if (def && def.thumbnailImage)
			SetupSlider(def.thumbnailImage);
	}
}

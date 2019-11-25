using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreasureHuntController : MonoBehaviour
{
	public bool displayBordersFromStart = true;
	public string quitURL = "http://google.com";
	
	public GUISkin treasureHuntSkin;
	public static GUISkin GUISkin
	{
		get
		{
			return main.treasureHuntSkin;
		}
		private set
		{
			main.treasureHuntSkin = value;
		}
	}

	public string collectButtonStyle = "Button";
	public static string CollectButtonStyle
	{
		get
		{
			return main.collectButtonStyle;
		}
		private set
		{
		}
	}

	TreasureHuntItem[] treasureHuntOrder;
	public TreasureHuntItem[] TreasureHuntOrder
	{
		get
		{
			return treasureHuntOrder;
		}
		private set
		{
		}
	}

	public void ShuffleToFront (TreasureHuntItem f)
	{
		List<TreasureHuntItem> items = new List<TreasureHuntItem>();
		foreach(TreasureHuntItem i in treasureHuntOrder)
		{
			if (i == f)
				items.Add(i);
		}

		foreach(TreasureHuntItem i in treasureHuntOrder)
		{
			if (i != f)
				items.Add(i);
		}

		for(int i=0;i<items.Count;i++)
		{
			treasureHuntOrder[i] = items[i];
		}
	}
	
	TreasureMysteryScrollGUI scrollGUI;
	TreasureMiniMap minimap;
	TreasureHuntNextGUI nextItemGUI;
	TreasureHuntCollectionGUI collectionGUI;
	public static TreasureHuntCollectionGUI CollectionGUI
	{
		get
		{
			return main.collectionGUI;
		}
		set
		{
			main.collectionGUI = value;
		}
	}

	public static void ShuffleInPlace(List<TreasureHuntItem> source)
	{
		System.Random rnd = new System.Random();
		for (int inx = source.Count-1; inx > 0; --inx)
		{
			int position = rnd.Next(inx);
			TreasureHuntItem temp = source[inx];
			source[inx] = source[position];
			source[position] = temp;
		}
	}
	
	public static TreasureHuntController main;
	void Awake ()
	{
		displayBanners = displayBordersFromStart;
		main = this;
		scrollGUI = GetComponent<TreasureMysteryScrollGUI>();
		minimap = GetComponent<TreasureMiniMap>();
		collectionGUI = GetComponent<TreasureHuntCollectionGUI>();
		nextItemGUI = GetComponent<TreasureHuntNextGUI>();

		List<TreasureHuntItem> tmp = new List<TreasureHuntItem>();
		TreasureHuntItem[] items = (TreasureHuntItem[])FindObjectsOfType(typeof(TreasureHuntItem));
		foreach (TreasureHuntItem i in items)
		{
			tmp.Add(i);
		}

		ShuffleInPlace(tmp);
		treasureHuntOrder = new TreasureHuntItem[items.Length];
		for(int i=0;i<tmp.Count;i++)
		{
			treasureHuntOrder[i] = tmp[i];
		}

		Debug.Log("First item: " + treasureHuntOrder[0].itemName);
		
		DisableGame();
	}

	bool displayBanners = false;
	void DisableGame()
	{
		displayBanners = displayBordersFromStart;
		scrollGUI.enabled = false;
		minimap.MapEnabled = false;
		collectionGUI.enabled = false;
		nextItemGUI.enabled = false;
	}

	int mysteryIndex = 0;
	public int MysteryIndex
	{
		get
		{
			return mysteryIndex;
		}
		set
		{
			mysteryIndex = value;
		}
	}

	static Dictionary<NPCController, EventPlayerBase> savedTalkEvents = new Dictionary<NPCController, EventPlayerBase>();
	void StartMystery(TreasureHuntItem myst)
	{
		npcTalkCount = 0;
		currentMystery = myst;
		scrollGUI.RegisterMystery(currentMystery);
		minimap.RegisterMystery(currentMystery);
		foreach (TreasureNPCPackage n in myst.npcs)
		{
			if (n.npcController.talkEventPlayer != null)
			{
				savedTalkEvents.Add(n.npcController, n.npcController.talkEventPlayer);
			}
			n.npcController.canTalk = true;
			n.npcController.talkEventPlayer = n.npcTreasureDialogEvent;
		}
	}

	public static void FinishTreasureHunt ()
	{
		main.DisableGame();
		main.Finished = true;
	}

	public static void NextItem ()
	{
		GUIManager.PlayButtonSound();
		main.nextItemGUI.enabled = true;
	}

	public static void RestoreSavedTalkEvents ()
	{
		foreach(NPCController npc in savedTalkEvents.Keys)
		{
            // Oleg: NPC reconstruction
			//npc.talkEventPlayer = savedTalkEvents[npc];
			//npc.canTalk = true;
		}

		savedTalkEvents.Clear();
	}

	public static void RegisterNPCTalked(NPCController npc)
	{
		if (savedTalkEvents.ContainsKey(npc))
		{
            // Oleg: NPC reconstruction
			//npc.talkEventPlayer = savedTalkEvents[npc];
			//npc.canTalk = true;
			savedTalkEvents.Remove(npc);
		}
	}
	
	public static void StartNextMysterySilent ()
	{
		RestoreSavedTalkEvents();
		
		main.minimap.Reset();
		main.scrollGUI.Reset();

		main.mysteryIndex++;

		if (main.mysteryIndex >= main.treasureHuntOrder.Length)
		{
			Debug.Log("All mystery objects accounted for, finishing treasure hunt.");
			FinishTreasureHunt();
		}
		else
		{
			Debug.Log("Start next mystery " + (main.mysteryIndex+1) + "/" + (main.treasureHuntOrder.Length) + "...");
			main.StartMystery(main.treasureHuntOrder[main.mysteryIndex]);
		}
	}
	
	public static void StartNextMystery ()
	{
		RestoreSavedTalkEvents();
		main.minimap.Reset();
		main.scrollGUI.Reset();

		main.mysteryIndex++;

		if (main.mysteryIndex >= main.treasureHuntOrder.Length)
		{
			Debug.Log("All mystery objects accounted for, finishing treasure hunt.");
			FinishTreasureHunt();
		}
		else
		{
			Debug.Log("Start next mystery " + (main.mysteryIndex+1) + "/" + (main.treasureHuntOrder.Length) + "...");
			main.StartMystery(main.treasureHuntOrder[main.mysteryIndex]);
			NextItem();
		}
	}
	
	TreasureHuntItem currentMystery;
	public static TreasureHuntItem CurrentMystery
	{
		get
		{
			return main.currentMystery;
		}
		set
		{
			main.currentMystery = value;
		}
	}

	public Rect askPlayerPrefsRect = new Rect(100, 100, 200, 200);
	public bool askPlayerPrefsCentered = false;
	public string askPlayerPrefsBox = "Box";
	public string askPlayerPrefsTitle = "Do you want to continue your previous game, or start a new one?";
	public string askPlayerContinue = "Continue";
	public string askPlayerNew = "New Game";
	void OnGUI ()
	{
		if (displayBanners)
		{
			GUI.skin = treasureHuntSkin;

			GUIStyle hStyle = GUI.skin.GetStyle("header");
			Rect headerRect = new Rect(0, 0, Screen.width, hStyle.fixedHeight);
			GUI.Box(headerRect, "", hStyle);

			GUIStyle fStyle = GUI.skin.GetStyle("footer");
			Rect footerRect = new Rect(0, Screen.height - fStyle.fixedHeight, Screen.width, fStyle.fixedHeight);
			GUI.Box(footerRect, "", fStyle);

			GUILayout.BeginArea(headerRect);
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if(GUILayout.Button("Quit"))
			{
				Application.OpenURL(quitURL);
				GUIManager.PlayButtonSound();
			}
			GUILayout.Space(10);
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.EndArea();
		}

		if (askPlayerPrefs)
		{
			GUI.skin = treasureHuntSkin;

			if (askPlayerPrefsCentered)
			{
				askPlayerPrefsRect.x = Screen.width/2 - askPlayerPrefsRect.width/2;
				askPlayerPrefsRect.y = Screen.height/2 - askPlayerPrefsRect.height/2;
			}
			GUI.Box(askPlayerPrefsRect, askPlayerPrefsTitle, askPlayerPrefsBox);
			GUILayout.BeginArea(askPlayerPrefsRect);
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			if (GUILayout.Button(askPlayerContinue))
			{
				TreasureHuntItem[] items = (TreasureHuntItem[])FindObjectsOfType(typeof(TreasureHuntItem));
				TreasureBonusObject[] bos = (TreasureBonusObject[])FindObjectsOfType(typeof(TreasureBonusObject));
				foreach(TreasureHuntItem it in items)
				{
					it.CollectIfInPrefs();
				}
				foreach(TreasureBonusObject it in bos)
				{
					it.CollectIfInPrefs();
				}
				ReallyStartGame();
				GUIManager.PlayButtonSound();
			}
			else if (GUILayout.Button(askPlayerNew))
			{
				TreasureHuntItem[] items = (TreasureHuntItem[])FindObjectsOfType(typeof(TreasureHuntItem));
				TreasureBonusObject[] bos = (TreasureBonusObject[])FindObjectsOfType(typeof(TreasureBonusObject));
				foreach(TreasureHuntItem it in items)
				{
					it.ClearPrefs();
				}
				foreach(TreasureBonusObject it in bos)
				{
					it.ClearPrefs();
				}
				ReallyStartGame();
				GUIManager.PlayButtonSound();
			}
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
	}

	void ReallyStartGame ()
	{
		askPlayerPrefs = false;
		displayBanners = true;
		scrollGUI.enabled = true;
		minimap.MapEnabled = true;
		finished = false;
		scrollGUI.TotalCount = treasureHuntOrder.Length;
		
		StartMystery(treasureHuntOrder[mysteryIndex]);
	}

	bool askPlayerPrefs = false;
	public void StartGame ()
	{
		TreasureHuntItem[] items = (TreasureHuntItem[])FindObjectsOfType(typeof(TreasureHuntItem));
		TreasureBonusObject[] bos = (TreasureBonusObject[])FindObjectsOfType(typeof(TreasureBonusObject));
		bool prefsExist = false;
		foreach(TreasureHuntItem it in items)
		{
			if (it.IsInPrefs)
				prefsExist = true;
		}
		foreach(TreasureBonusObject it in bos)
		{
			if (it.IsInPrefs)
				prefsExist = true;
		}
		
		if (!prefsExist)
			ReallyStartGame();
		else
			askPlayerPrefs = true;
	}

	void CompletedMystery ()
	{
		Debug.Log("Completed current mystery");
	}

	void RevealMysteryObject ()
	{
		Debug.Log("Revealing mystery object");
		currentMystery.Reveal();
	}

	public static int CollectedItemCount
	{
		get
		{
			return TreasureMysteryScrollGUI.CollectedCount;
		}
		set
		{
			if (value == main.scrollGUI.TotalCount)
				main.CompletedMystery();
		}
	}
	
	int npcTalkCount = 0;
	public static int NPCTalkCount
	{
		get
		{
			return main.npcTalkCount;
		}
		set
		{
			main.npcTalkCount = value;
			if (main.npcTalkCount == 3)
				main.RevealMysteryObject();
		}
	}

	bool finished = false;
	public bool Finished
	{
		get
		{
			return finished;
		}
		set
		{
			finished = value;
		}
	}
}

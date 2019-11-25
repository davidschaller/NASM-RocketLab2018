using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

public enum NotebookState
{
	Collection,
	LetterToMa,
}

public class NotebookManager : MonoBehaviour
{
	static NotebookManager main;
	public static NotebookManager Main
	{
		get
		{
			return main;
		}
		set
		{
			main = value;
		}
	}

	static NotebookState state = NotebookState.Collection;
	public static NotebookState State
	{
		get
		{
			return state;
		}
		set
		{
			state = value;
		}
	}

	public static bool HasSomeInfoCards
	{
		get
		{
			return collectedNotes.Count > 0 || collectedInfos.Count > 0;
		}
	}
	
	static List<string> collectedNotes = new List<string>();
	public static List<string> CollectedNotes
	{
		get
		{
			return collectedNotes;
		}
	}
	
	public static string InfoCardSaveData
	{
		get
		{
			string ret = "";
			foreach(string s in collectedNotes)
			{
				ret += SaveManager.cardEntryXML.Replace("*ENTRY*", s);
			}

			return ret;
		}
	}

	public static void SetInfoCardData( XmlDocument xml )
	{
		collectedNotes.Clear();
		
		XPathNavigator navigator = xml.CreateNavigator();
		XPathNodeIterator nodes = navigator.Select("/root/cards");
		nodes.MoveNext();
		XPathNavigator nodesNavigator = nodes.Current;
		
		XPathNodeIterator nodesText = nodesNavigator.SelectDescendants(XPathNodeType.Text, false);
		
		while (nodesText.MoveNext())
		{
			string note = nodesText.Current.Value;
			if (!collectedNotes.Contains(note))
			{
				Debug.Log("Adding note: " +note);	
				collectedNotes.Add(note);
			}
		}
	}

	int nextCollectedIndex = 0;
	Rect leftColumn;
	Rect rightColumn;
	public static void Init ()
	{
        canShowDoneButton = false;

        MovementController.IsWritingLetter = true;

		state = NotebookState.Collection;
		GameObject go = new GameObject();
		Main = (NotebookManager)go.AddComponent(typeof(NotebookManager));
		DialogPair[] dialogs = (DialogPair[])GameObject.FindObjectsOfType(typeof(DialogPair));
		foreach(DialogPair pair in dialogs)
			pair.Grabbed = false;

		Main.nextCollectedIndex = collectedNotes.Count-1;

		Main.leftColumn = new Rect(GUIManager.Main.firstNoteOffset.x,
								   GUIManager.Main.firstNoteOffset.y,
								   GUIManager.Main.notebookBackgroundRect.width/2,
								   GUIManager.Main.notebookBackgroundRect.height);
		
		Main.rightColumn = new Rect(GUIManager.Main.notebookBackgroundRect.width/2 + GUIManager.Main.firstNoteOffset.x,
									GUIManager.Main.firstNoteOffset.y,
									GUIManager.Main.notebookBackgroundRect.width/2,
									GUIManager.Main.notebookBackgroundRect.height);
	} 

	public static void InitLetterToMa ()
	{
		Init();
		State = NotebookState.LetterToMa;
	}

	void DrawNotebookLeftColumn (DialogPair[] dialogs, Rect leftColumn, out float summaryHeight)
	{
		summaryHeight = 0;
		GUILayout.BeginArea( leftColumn );
		
		foreach(DialogPair pair in dialogs)
		{
			Rect areaRect = GUILayoutUtility.GetRect (new GUIContent(pair.notebookSummaryText), "Notebook  note" );
			areaRect.width = GUIManager.Main.notebookNoteWidth;
			summaryHeight = areaRect.height; // cache this for grabbed
											 // summary later

			if (!pair.Grabbed && !pair.Summarized)
			{
				GUI.Label(areaRect, pair.notebookSummaryText, "Notebook  note");

				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && areaRect.Contains(Event.current.mousePosition))
				{
					Debug.Log("Pickup Info " + pair.notebookSummaryText);
					pair.Grabbed = true;
					grabbedSummary = pair;

                    dx = Mathf.Abs(Event.current.mousePosition.x - areaRect.x);
                    dy = Mathf.Abs(Event.current.mousePosition.y - areaRect.y);
				}
			}
			
			else
			{
				GUIStyle invisible = new GUIStyle(GUI.skin.GetStyle("Notebook  note"));
				invisible.normal.background = null;
				GUI.Label(areaRect, "", invisible);
			}
		}

		GUILayout.EndArea();

	}

    private static bool canShowDoneButton = false;

    private float dx, dy; // Use this to fit label position

	void DrawGrabbedSummaries (DialogPair[] dialogs, Rect rightColumn, float summaryHeight)
	{
		if (grabbedSummary != null)
		{
			Rect areaRect = 
                new Rect(Event.current.mousePosition.x - dx, 
                            Event.current.mousePosition.y - dy, 
                                GUIManager.Main.notebookNoteWidth, summaryHeight);
			
			GUI.Label(areaRect, grabbedSummary.notebookSummaryText, "Notebook  note");
		

		    if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
		    {
                if (rightColumn.Contains(Event.current.mousePosition)
                    || (rightColumn.x < (areaRect.x + areaRect.width)))
			    {
				    if (collectedNotes.Count > nextCollectedIndex+1)
				    {
					    foreach(DialogPair pair in dialogs)
					    {
						    if (pair.notebookSummaryText == collectedNotes[collectedNotes.Count-1])
							    pair.Summarized = false;
					    }

					    collectedNotes.RemoveAt(collectedNotes.Count-1);
				    }
                    canShowDoneButton = true;
				    collectedNotes.Add(grabbedSummary.notebookSummaryText);
				    grabbedSummary.Summarized = true;
			    }

			    grabbedSummary.Grabbed = false;
			    grabbedSummary = null;
		    }
        }
	}

	void CollectionStateGUI ()
	{
		float summaryHeight = 0;
		GUI.skin = GUIManager.Skin;
		DialogPair[] dialogs = (DialogPair[])GameObject.FindObjectsOfType(typeof(DialogPair));


		GUI.Box( GUIManager.Main.notebookBackgroundRect, "", "Notebook Background");


		GUILayout.BeginArea( GUIManager.Main.notebookBackgroundRect );
		

		DrawNotebookLeftColumn(dialogs, leftColumn, out summaryHeight);


		GUILayout.BeginArea( rightColumn );
		
		foreach(string s in collectedNotes)
		{
			GUILayout.Label(s, "Notebook-dropped note", GUILayout.Width(GUIManager.Main.notebookNoteWidth));
		}

		GUILayout.EndArea();

        if (dialogs.Length == 0)
        {
            canShowDoneButton = true;
        }

		
		string done = "Done";
		float bwidth = GUI.skin.GetStyle("Button").CalcSize(new GUIContent(done)).x;
		float bx = GUIManager.Main.notebookBackgroundRect.x + GUIManager.Main.notebookBackgroundRect.width/2 + GUIManager.Main.notebookBackgroundRect.width/4 - bwidth/2;

        if (canShowDoneButton && GUI.Button(new Rect(bx, GUIManager.Main.notebookBackgroundRect.y + 550, bwidth, 50), done))
		{
			Finished();
		}
		else if (Application.isEditor && collectedNotes.Count > 0 && GUI.Button(new Rect(bx + bwidth + 50, GUIManager.Main.notebookBackgroundRect.y + 400, bwidth, 50), "(Ma)"))
		{
			state = NotebookState.LetterToMa;
		}

		GUILayout.EndArea();


		DrawGrabbedSummaries(dialogs, rightColumn, summaryHeight);
	}

	void Finished ()
	{
		if (GameObject.Find("Flashback Root"))
			FlashbackManager.NotebookManagerFinished = true;
		else
			NotebookManager.Destroy();
		GUIManager.PlayButtonSound();
        MovementController.IsWritingLetter = false;
	}

	void FinishedLetterToMa ()
	{
		LetterWritingRunner.FinishLetter();
		Finished();
	}

    void DrawGrabbedInfo(List<string> notes, Dictionary<int, Rect> p_noteAreas, float summaryHeight)
	{
        if (grabbedInfo != null)
        {
            Rect areaRect = new Rect(Event.current.mousePosition.x - dx,
                                        Event.current.mousePosition.y - dy,
                                            GUIManager.Main.notebookNoteWidth, summaryHeight);

            GUI.Label(areaRect, grabbedInfo, "Notebook  note");


            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                foreach (KeyValuePair<int, Rect> item in p_noteAreas)
                {
                    if (item.Value.Contains(Event.current.mousePosition))
                    {
                        if (!string.IsNullOrEmpty(collectedInfos[item.Key]))
                        {
                            collectedNotes.Add(collectedInfos[item.Key]);   
                        }

                        collectedInfos.RemoveAt(item.Key);
                        collectedInfos.Insert(item.Key, grabbedInfo);
                        collectedNotes.Remove(grabbedInfo);
                        grabbedInfo = null;
                    }
                }
            }
        }
	}

	void DrawStaticLetterElements ()
	{
		GUILayout.BeginArea( rightColumn );
		GUILayout.Label("Test");
		GUILayout.EndArea();
	}
	
	string grabbedInfo = null;
	static List<string> collectedInfos = new List<string>();
	public static List<string> CollectedInfos
	{
		get
		{
			return collectedInfos;
		}
	}
	
	public static string LetterSaveData
	{
		get
		{
			string ret = "";
			foreach(string s in collectedInfos)
			{
                if (!string.IsNullOrEmpty(s))
                {
                    ret += SaveManager.letterEntryXML.Replace("*ENTRY*", s);
                }
			}

			return ret;
		}
	}

	public static void SetLetterToMaData( XmlDocument xml )
	{
		collectedInfos.Clear();
		
		XPathNavigator navigator = xml.CreateNavigator();
		XPathNodeIterator nodes = navigator.Select("/root/letter");
		nodes.MoveNext();
		XPathNavigator nodesNavigator = nodes.Current;
		
		XPathNodeIterator nodesText = nodesNavigator.SelectDescendants(XPathNodeType.Text, false);
		
		while (nodesText.MoveNext())
		{
			string note = nodesText.Current.Value;
			
			if (!collectedInfos.Contains(note))
				collectedInfos.Add(note);
		}
	}


	
	float summaryHeight = 0;
    string yourThoughts = "Type Your Own Thoughts Here";

    Dictionary<int, Rect> noteAreas;

	void LetterToMaStateGUI ()
	{

		GUI.skin = GUIManager.Skin;

		GUIStyle bgStyle = GUI.skin.FindStyle("Letter to Ma Background");
		if (bgStyle == null)
		{
			GUIManager.DebugText(this, "Can't find 'Letter to Ma Background' custom style for notebook background, using 'Notebook Background' instead.");
			
			bgStyle = GUI.skin.GetStyle("Notebook Background");
		}
		
		GUI.Box( GUIManager.Main.notebookBackgroundRect, "", bgStyle);


		GUILayout.BeginArea( GUIManager.Main.notebookBackgroundRect );
		//DrawStaticLetterElements();




		/*       LEFT COLUMN   */
		GUILayout.BeginArea(leftColumn);
		GUIManager.DebugText(this, collectedNotes.Count + " collected notes");
		
		foreach(string s in collectedNotes)
		{
			Rect areaRect = GUILayoutUtility.GetRect (new GUIContent(s), "Notebook  note" );
			areaRect.width = GUIManager.Main.notebookNoteWidth;

			if (grabbedInfo == null)
			{		
				GUI.Label(areaRect, s, "Notebook  note");
				summaryHeight = areaRect.height; // cache this for grabbed
				// summary later
				
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && areaRect.Contains(Event.current.mousePosition))
				{
					grabbedInfo = s;
					Debug.Log("Pickup Ma Info " + grabbedInfo);

                    dx = Mathf.Abs(Event.current.mousePosition.x - areaRect.x);
                    dy = Mathf.Abs(Event.current.mousePosition.y - areaRect.y);
				}
                
			}
            else if (grabbedInfo != s)
            {
                GUI.Label(areaRect, s, "Notebook  note");
            }
                // need this part?
            else
            {
                GUIStyle invisible = new GUIStyle(GUI.skin.GetStyle("Notebook  note"));
                invisible.normal.background = null;
                GUI.Label(areaRect, "", invisible);
            }
		}
		GUILayout.EndArea();



		
		/*        RIGHT COLUMN  */
        noteAreas = new Dictionary<int, Rect>();

		GUILayout.BeginArea( rightColumn );
		GUILayout.Space(GUIManager.Main.dragNoteTopOffset);

        // prefill with empty values
        while (collectedInfos.Count < 3)
        {
            collectedInfos.Add(string.Empty);
        }

		for (int i = 0; i < 3; i++)
		{
            Rect areaRect;

            if (string.IsNullOrEmpty(collectedInfos[i]))
            {
                areaRect = GUILayoutUtility.GetRect(new GUIContent(""), "Notebook-dropped note");
                areaRect.width = GUIManager.Main.notebookNoteWidth;
                GUI.DrawTexture(areaRect, GUIManager.Main.dragNoteHere);
            }
            else
            {
                areaRect = GUILayoutUtility.GetRect(new GUIContent(collectedInfos[i]), "Notebook-dropped note");
                areaRect.width = GUIManager.Main.notebookNoteWidth;
                GUI.Label(areaRect, collectedInfos[i], "Notebook-dropped note");
            }
            areaRect.x += rightColumn.x;
            areaRect.y += rightColumn.y;

            noteAreas.Add(i, areaRect);
			GUILayout.Space(GUIManager.Main.dragNoteVerticalSpacing);
		}
		string yourThoughtsStyle = "your thoughts";
		GUIStyle thoughtsStyle = GUI.skin.FindStyle(yourThoughtsStyle);
		if (thoughtsStyle == null)
			thoughtsStyle = "TextArea";

        Rect thoughtsRect = GUILayoutUtility.GetRect(new GUIContent(yourThoughts), thoughtsStyle);

        thoughtsRect.width = GUIManager.Main.notebookNoteWidth;

        if (thoughtsRect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseUp)
        {
            if (yourThoughts == "Type Your Own Thoughts Here")
            {
                yourThoughts = string.Empty;
            }
        }

        yourThoughts = GUI.TextArea(thoughtsRect, yourThoughts, thoughtsStyle);
		GUILayout.EndArea();


		
		string done = "Send Letter to Ma";
		float bwidth = GUI.skin.GetStyle("Button").CalcSize(new GUIContent(done)).x;
		float bx = GUIManager.Main.notebookBackgroundRect.x + GUIManager.Main.notebookBackgroundRect.width/2 + GUIManager.Main.notebookBackgroundRect.width/4 - bwidth/2;
		
		if(GUI.Button(new Rect(bx, GUIManager.Main.notebookBackgroundRect.y + GUIManager.Main.toMaButtonVOffset, bwidth, 50), done))
		{
			FinishedLetterToMa();
		}

		GUILayout.EndArea();
        

		if (grabbedInfo != null)
		{
			GUIManager.DebugText(this, "Grabbed: " + grabbedInfo);
            DrawGrabbedInfo(collectedInfos, noteAreas, summaryHeight);
		}
		else
		{
			GUIManager.DebugText(this, "NO GRAB");
		}
	}
	
	DialogPair grabbedSummary = null;
	void OnGUI ()
	{
		leftColumn.x = GUIManager.Main.firstNoteOffset.x;
		leftColumn.y = GUIManager.Main.firstNoteOffset.y;

		rightColumn.x = GUIManager.Main.notebookBackgroundRect.width/2 + GUIManager.Main.firstNoteOffset.x;
		rightColumn.y = GUIManager.Main.firstNoteOffset.y;
		
		switch(state)
		{
			case NotebookState.Collection:
				CollectionStateGUI();
				break;
			case NotebookState.LetterToMa:
				LetterToMaStateGUI();
				break;
		}
	}
	
	public static void Destroy()
	{
		Debug.Log("Destroy notebook manager");
		
		NotebookManager d = (NotebookManager)GameObject.FindObjectOfType(typeof(NotebookManager));
		GameObject.Destroy(d.gameObject);
		Main = null;
	}
}


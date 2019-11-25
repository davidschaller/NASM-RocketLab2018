using UnityEngine;

public class DialogPair : MonoBehaviour
{
	public string flashbackDateText = "March 5, 1836";
	public Vector2 flashbackDateLocation = Vector2.zero;
		
	public string buttonText = "Listen to conversation";
	public Vector2 listenLocation = Vector2.zero;
	public Vector2 buttonWH = new Vector2(150, 15);
	public float buttonHeight = 2.5f;
	public EventPlayerBase listenScenario;
	
	public Vector2 dialogWH = new Vector2(400,200);
	public float nameSpacing = 100;
	public string dialogTitle;
	
	public NPCController npc1;
	public string npc1Name;
	public string npc1DialogText;
	public Vector2 npc1TextLocation = Vector2.zero;

	public NPCController npc2;
	public string npc2Name;
	public string npc2DialogText;
	public Vector2 npc2TextLocation = Vector2.zero;

	public string notebookSummaryText = "This is a summarization of this dialog pair";
	bool grabbed = false;
	public bool Grabbed
	{
		get
		{
			return grabbed;
		}
		set
		{
			grabbed = value;
		}
	}

	bool summarized = false;
	public bool Summarized
	{
		get
		{
			return summarized;
		}
		set
		{
			summarized = value;
		}
	}
	
	public EventPlayerBase endScenario;
	
	Vector3 center;
	bool listening = false;
	static bool anyListening = false;
	void Awake ()
	{
		center = Vector3.Lerp(npc1.transform.position, npc2.transform.position, 0.5f);
	}
	
	bool IntroScenarioFinished ()
	{
		return (listening && (listenScenario == null || listenScenario.Finished));
	}

	bool EndScenarioFinished ()
	{
		return ((listenScenario == null || listenScenario.Finished) && (endScenario == null || endScenario.Finished));
	}
	
	void Update ()
	{
		Debug.DrawRay(center, Vector3.up * buttonHeight, Color.blue);
	}
	
	bool dialogFinished = false;
	public bool DialogFinished
	{
		get
		{
			return dialogFinished && EndScenarioFinished();
		}
		private set {}
	}

	public Vector2 doneLocation = Vector2.zero;
	
	void OnGUI ()
	{
		if (GUIManager.Skin != null)
		{
			GUI.skin = GUIManager.Skin;
		}

		Vector2 dateDim = GUI.skin.GetStyle("Speech Title").CalcSize(new GUIContent(flashbackDateText));
		
		GUI.Label(new Rect(flashbackDateLocation.x, Screen.height - flashbackDateLocation.y - dateDim.y, dateDim.x, dateDim.y), flashbackDateText, "Speech Title");

        /*
         * Have to test if CameraWaypointEvent is still executing, so hide 'talk to' buttons while zooming
         */

        if (!anyListening && !listening && !dialogFinished && !CameraWaypointRunner.InProccess)
		{
			int buttonWidth = (int)buttonWH.x;
			int bHeight = (int)buttonWH.y;

			GUIStyle bstyle = GUI.skin.GetStyle("Button Tall");

			if (GUIManager.Skin != null)
			{
				GUIContent cont = new GUIContent(buttonText);
				
				Vector2 bsize = bstyle.CalcSize(cont);
				
				buttonWidth = (int)bsize.x;
				bHeight = (int)bstyle.CalcHeight(cont, buttonWidth);
			}
			
			
			Vector2 screenPos = listenLocation;
			if (screenPos == Vector2.zero)
			{
				screenPos = GUIUtility.ScreenToGUIPoint(Camera.main.WorldToScreenPoint(center + Vector3.up*buttonHeight));
				screenPos.x -= buttonWidth/2;
				screenPos.y = Screen.height - screenPos.y;
			}

			if (GUI.Button(new Rect(screenPos.x, screenPos.y, buttonWidth, bHeight), buttonText, bstyle))
			{
				Debug.Log("LISTEN");
				listening = true;
				anyListening = true;
				if (listenScenario != null) listenScenario.PlayerTriggered();
				GUIManager.PlayButtonSound();
			}
		}
		else if (listening && (listenScenario == null || listenScenario.Finished))
		{
			GUIStyle tStyle = GUI.skin.GetStyle("Dialog Text");
			Vector2 v1 = npc1TextLocation;
			int w = 0;
			if (v1 == Vector2.zero)
			{
				w = (int)tStyle.CalcSize(new GUIContent(npc1DialogText)).x/2;
                // Oleg: NPC reconstruction
				//v1 = new Vector2(npc1.ScreenPosition.x - w/2, npc1.ScreenPosition.y);
			}
			GUIManager.RenderAnswer(npc1Name, npc1DialogText, v1);

			Vector2 v2 = npc2TextLocation;
			if (v2 == Vector2.zero)
			{
				w = (int)tStyle.CalcSize(new GUIContent(npc2DialogText)).x/2;
                // Oleg: NPC reconstruction
				//v2 = new Vector2(npc2.ScreenPosition.x - w/2, npc2.ScreenPosition.y);
			}
			GUIManager.RenderAnswer(npc2Name, npc2DialogText, v2);

			
			Rect doneRect = new Rect(doneLocation.x, doneLocation.y, 50, buttonWH.y);
			if (doneLocation == Vector2.zero)
			{
				doneRect.x = (v1 + v2).x/2;
				doneRect.y = (v1 + v2).y/2 + 100;
			}
			
			if (GUI.Button(doneRect, "Done"))
			{
				listening = false;
				anyListening = false;
				dialogFinished = true;
				
				if (endScenario != null) endScenario.PlayerTriggered();
				GUIManager.PlayButtonSound();
			}
		}
	}
}
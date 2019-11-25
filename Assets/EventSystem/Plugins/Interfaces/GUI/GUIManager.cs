using UnityEngine;
using System.Collections.Generic;

public class GUIManager : MonoBehaviour
{
    public string quitURL = "http://google.com";
    public float targetFPS = 40f;

	public enum Projects
	{
		Texas,
		MtVernon,
		ArchitectStudio,
		Crypto,
	}

	public Projects project;
	
	public static Projects Project
	{
		get
		{
			return main.project;
		}
	}
	
	static GUIManager main;
	public static GUIManager Main
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
	
	public GUISkin skin;
	public static GUISkin Skin
	{
		get
		{
			if (main != null)
				return main.skin;
			else
				return null;
		}
		private set
		{
		}
	}

	public string loaderText = "Please wait";
	public string loaderTextStyle = "Label";
	public string loaderBGStyle = "Loader BG";
	public string loaderFGStyle = "Loader FG";

	public string npcNameButtonStyle = "Button";
	public static string NPCNameButtonStyle
	{
		get
		{
			return main.npcNameButtonStyle;
		}
		private set
		{
		}
	}

	public AudioClip buttonSound;
	public float buttonVolume = 1;
	public static AudioClip ButtonSound
	{
		get
		{
			return main.buttonSound;
		}
		private set
		{
		}
	}

	public static void PlayButtonSound ()
	{
		if (!main)
			Debug.LogWarning("Trying to play button sound, but there's no GUI Manager in the scene.  Errors will ensue.");
		
		if (Camera.main)
		{
			if (!Camera.main.GetComponent<AudioSource>())
                Common.AddAudioTo(Camera.main.gameObject);

			if (ButtonSound != null)
				Camera.main.GetComponent<AudioSource>().PlayOneShot(ButtonSound, main.buttonVolume);
			else
				Debug.LogWarning("No button sound assigned to GUI Manager", main.gameObject);
		}
		else
		{
			Debug.LogWarning("Trying to play button sound, but cannot find main camera");
		}
	}

	//public Texture2D arrow;

	public Rect notebookBackgroundRect = new Rect(80, 200,800, 600);
	public int notebookNoteWidth = 300;
	public Vector2 firstNoteOffset = new Vector2(40,40);
	public Texture2D dragNoteHere;
	public float dragNoteTopOffset = 10;
	public float dragNoteVerticalSpacing = 20;
	public float toMaButtonVOffset = 550;

	public MapCamera mapCameraPrefab;
	MapCamera mapCamera;
	
	void Awake ()
	{
		if (fadeIn)
		{
			fadeState = FadeState.FadeIn;
		}
		
		GameObject go = GameObject.Find("Map Camera");
		if (go == null && mapCameraPrefab != null)
		{
			mapCamera = (MapCamera)Instantiate(mapCameraPrefab);
			mapCamera.name = "Map Camera";
		}
		
		Main = this;
		if (Application.isEditor)
		{
			musicMuted = PlayerPrefs.GetInt("MusicMute", 0) == 1 ? true : false;
			doDebugWindow = PlayerPrefs.GetInt("Show Debug Window", 0) == 1 ? true : false;
			disableSubstitutionWarnings = PlayerPrefs.GetInt("Disable SubVar Warnings", 0) == 1 ? true : false;
		}
	}

	public static void RenderAnswer(string answer, int bottomOffset)
	{
		GUIStyle dialogText = Skin.GetStyle("Dialog Text");
		
		float height = dialogText.CalcHeight(new GUIContent(answer), dialogText.fixedWidth);
			
		Rect answerRect = new Rect(Screen.width/2 - dialogText.fixedWidth/2,
								   bottomOffset - height,
								   dialogText.fixedWidth,
								   height);
		
		GUILayout.BeginArea(answerRect);
		GUILayout.Label(answer, dialogText);
		GUILayout.EndArea();
	}

	public static void RenderAnswer(string title, string answer, Vector2 loc)
	{
		GUIStyle dialogText = Skin.GetStyle("Dialog Text");
		GUIStyle lbl = Skin.GetStyle("Speech Title");
		lbl.alignment = TextAnchor.UpperLeft;
		
		float height = dialogText.CalcHeight(new GUIContent(answer), dialogText.fixedWidth);
			
		Rect answerRect = new Rect(loc.x,
								   loc.y,
								   dialogText.fixedWidth,
								   height);
		
		GUILayout.BeginArea(answerRect);
		GUILayout.Label("\n" + answer, dialogText);
		GUILayout.EndArea();

		answerRect.x += 20;
		GUILayout.BeginArea(answerRect);
		GUILayout.Label(title, lbl);
		GUILayout.EndArea();

	}

	Rect headerRect;
	public static Rect HeaderRect
	{
		get
		{
			return Main.headerRect;
		}
		private set
		{
		}
	}

	static Dictionary<object, string> debugTextContributors = new Dictionary<object, string>();
	static Dictionary<object, float> contTimes = new Dictionary<object, float>();
	public static void DebugText( object sender, string msg )
	{
		if (!debugTextContributors.ContainsKey(sender))
		{
			debugTextContributors.Add(sender, msg + "\n");
		}
		else
		{
			debugTextContributors[sender] = msg + "\n";
		}
		contTimes[sender] = Time.time;
	}
	
	bool doDebugWindow = false;
	public bool disableHeader = false;
	void OnGUI ()
	{
		GUI.skin = skin;

		if (project == Projects.Texas && !disableHeader)
		{
			GUIStyle header = Skin.GetStyle("header");
			headerRect = new Rect(0, 0, Screen.width, header.fixedHeight);
			GUI.Box(headerRect, "", header);
			//if (!FlashbackManager.FlashbackActive)
			//{
			GUILayout.BeginArea(headerRect);
			GUILayout.FlexibleSpace();
			
			GUILayout.BeginHorizontal();
			//GUILayout.FlexibleSpace();
			string goalText = string.Empty;
			if (GoalManager.CurrentGoal != null)
			{
				goalText = GoalManager.CurrentGoal.GoalTargetName;
				goalText = (goalText == null ? string.Empty : "Goal: " + goalText);
			}
            

			GUILayout.Space(Screen.width/2 - Skin.GetStyle("Label").CalcSize(new GUIContent(goalText)).x/2);
			GUILayout.Label(goalText);
			GUILayout.FlexibleSpace();
			GUILayout.Label("User name", skin.GetStyle("Username"));
			if (GUILayout.Button("Notes"))
            {
                NotebookManager.Init();


            }
            if (GUILayout.Button("Quit"))
            {
                Application.OpenURL(quitURL);
            }
			GUILayout.Space(40);
			GUILayout.EndHorizontal();

			GUILayout.FlexibleSpace();
			GUILayout.EndArea();
			//}
		}
		else if (project == Projects.MtVernon)
		{
		}


		if (Application.isEditor)
		{
			GUI.skin = null;
			
			GUI.depth = -10000; // keep this on top of other stuff

			GUI.Box(new Rect(0, Screen.height-25, 110, 25), "");
			
			doDebugWindow = GUI.Toggle (new Rect (5,Screen.height-23,100,20), doDebugWindow, "Debug Window");
			if (doDebugWindow)
				debugWindowRect = GUI.Window(10000, debugWindowRect, DebugWindow, "Debug Window");

			if (GUI.changed)
			{
				PlayerPrefs.SetInt("Show Debug Window", doDebugWindow ? 1 : 0);
			}

		}


		if (fadeTexture == null)
		{
			fadeTexture = new Texture2D(1,1);
			fadeTexture.SetPixels(new Color[1] {Color.black});
			fadeTexture.Apply(false);
			
			fadeStyle = new GUIStyle();
			fadeStyle.normal.background = fadeTexture;
		}

		if ((fadeIn && fadeState == FadeState.FadeIn) || (fadeOut && fadeState == FadeState.FadeOut))
		{
			GUI.color = new Color(1, 1, 1, fadeAlpha);
			
			GUI.Box(new Rect(0,0, Screen.width, Screen.height), "", fadeStyle);
		}
	}
	GUIStyle fadeStyle;
	Texture2D fadeTexture;
	float fadeAlpha = 1;
	public float fadeInSpeed = 0.5f;
	public float fadeOutSpeed = 0.5f;
	enum FadeState
	{
		None,
		FadeIn,
		FadeOut,
	}
	FadeState fadeState = FadeState.None;
	public bool fadeIn = false;
	public bool fadeOut = false;

	public static void FadeOut ()
	{
		Debug.LogWarning("DO FADE OUT");

        if (main.fadeOut)
        {
            main.fadeAlpha = 0;
            main.fadeState = FadeState.FadeOut;
        }
	}
	
	public static bool musicMuted = false;
	public static bool disablePlayerWarnings = false;
	public static bool disableSubstitutionWarnings = false;
	public static bool webSaveMode = true;
	static string debugText = "";
	Rect debugWindowRect = new Rect(0, 60, 300, 500);
	void DebugWindow (int id)
	{
		if (id != 10000)
			return;
			
		if (GUILayout.Button( musicMuted ? "Unmute Music" : "Mute Music"))
		{
			musicMuted = !musicMuted;
			AudioRunner.MuteSoundtrack(musicMuted);
			PlayerPrefs.SetInt("MusicMute", musicMuted ? 1 : 0);
		}
		else if (GUILayout.Button( disablePlayerWarnings ? "Enable EP Warnings" : "Disable EP Warnings" ))
		{
			disablePlayerWarnings = !disablePlayerWarnings;
		}
		else if (GUILayout.Button( disableSubstitutionWarnings ? "Enable SubVar Warnings" : "Disable SubVar Warnings" ))
		{
			disableSubstitutionWarnings = !disableSubstitutionWarnings;
			PlayerPrefs.SetInt("Disable SubVar Warnings", disableSubstitutionWarnings ? 1 : 0);
		}
		else if (GUILayout.Button( webSaveMode ? "Switch to Local Saves" : "Switch to Web Saves" ))
		{
			webSaveMode = !webSaveMode;
			SaveManager.SetSaveMode(webSaveMode);
		}

		if (project == Projects.Texas)
		{
			if (NotebookManager.HasSomeInfoCards &&
				GUILayout.Button("(Ma: " + NotebookManager.CollectedNotes.Count + ", " + NotebookManager.CollectedInfos.Count + ")"))
			{
				NotebookManager.InitLetterToMa();
			}
			else if (!NotebookManager.HasSomeInfoCards)
			{
				DebugText(this, "No info cards for letter (" + NotebookManager.CollectedNotes.Count + ", " + NotebookManager.CollectedInfos.Count + ")");
			}

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Load"))
			{
				SaveManager.DoLoad();
			}
			else if (GUILayout.Button("Save"))
			{
				SaveManager.DoSave();
			}
			else if (webSaveMode && GUILayout.Button("Create"))
			{
				SaveManager.CreateProfile();
			}
			else if (webSaveMode && GUILayout.Button("Login"))
			{
				SaveManager.Authenticate();
			}
			else if (webSaveMode && GUILayout.Button( SaveManager.DoPost ? "Switch to Debug" : "Switch to Post" ))
			{
				SaveManager.DoPost = !SaveManager.DoPost;
			}
		
			GUILayout.EndHorizontal();
		}
		
		debugText = "";
		
		foreach( KeyValuePair<object, string> entry in debugTextContributors )
		{
			if (entry.Value != "" && entry.Value != "\n")
			{
				debugText += entry.Value;
			}
		}
		
		if (debugText != "")
		{
			GUILayout.Label("GameTime " + Time.time.ToString("0.0")); // + debugText);
			foreach( KeyValuePair<object, string> entry in debugTextContributors )
			{
				if (entry.Value != "" && entry.Value != "\n" && focusEntry == null)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Label(entry.Value);
					if(GUILayout.Button("Focus on\n" + entry.Key))
					{
						focusEntry = entry.Key;
					}
					GUILayout.EndHorizontal();
				}
				else if (focusEntry != null && focusEntry == entry.Key)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Label(entry.Value);
					if(GUILayout.Button("De-Focus"))
					{
						focusEntry = null;
					}
					GUILayout.EndHorizontal();					
				}
				
			}
		}
				 
		
		GUI.DragWindow();
	}
	object focusEntry;
	
	GameObject extraStuff;
	void Update ()
	{
		if (fadeIn && fadeState == FadeState.FadeIn)
		{
			fadeAlpha -= fadeInSpeed * Time.deltaTime;
			if (fadeAlpha <= 0)
			{
				fadeIn = false;
				fadeState = FadeState.None;
			}
		}
		else if (fadeOut && fadeState == FadeState.FadeOut)
		{
			if (fadeAlpha <= 1)
				fadeAlpha += fadeOutSpeed * Time.deltaTime;
			// don't cancel fade out so it stays black
		}

		
		if (Input.GetKeyDown("f1"))
		{
			if (!extraStuff)
				extraStuff = GameObject.Find("Extra Stuff");

			if (extraStuff)
			{
				Debug.Log(extraStuff.active ? "Disabling extra stuff" : " Enabling extra stuff");
				extraStuff.SetActiveRecursively( !extraStuff.active );

                if (extraStuff.active)
                {
                    EventPlayer[] list = (EventPlayer[])GameObject.FindObjectsOfType(typeof(EventPlayer));
                    foreach (EventPlayer ev in list)
                    {
                        if (ev.transform.IsChildOf(extraStuff.transform) && ev.NeedRestart)
                        {
                            Debug.Log(ev.name);
                            ev.StartOnSceneStart();
                        }
                    }
                }
			}
			else
			{
				Debug.Log("No Extra Stuff to toggle");
			}
		}
		
		if (Application.isEditor)
		{
			List<object> removeEntries = new List<object>();
			foreach( KeyValuePair<object, string> entry in debugTextContributors )
			{
				if (contTimes[entry.Key] < (Time.time - 5))
				{
					removeEntries.Add(entry.Key);
				}
			}
			foreach(object o in removeEntries)
			{
				debugTextContributors[o] = "";
			}
		}
	}
}

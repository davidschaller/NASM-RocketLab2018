using UnityEngine;
using System;
using System.Xml;
using System.Xml.XPath;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

public enum SavingState
{
	None,
	Saving,
	Loading,
	Authenticating,
	CreatingProfile,
	SubmittingProfile,
	SubmittingAuthentication,
	SubmittingSave,
	SubmittingLoad,
}

public class SaveManager : MonoBehaviour
{
	static SavingState savingState = SavingState.None;
	public static SavingState SavingState
	{
		get
		{
			return savingState;
		}
	}
	
	public static bool Saving
	{
		get
		{
			return savingState == SavingState.Saving;
		}
	}
	
	public static bool Loading
	{
		get
		{
			return savingState == SavingState.Loading;
		}
	}

	public static bool CreatingProfile
	{
		get
		{
			return savingState == SavingState.CreatingProfile;
		}
	}

	static bool loadingFromSave = false;
	public static bool LoadingFromSave
	{
		get
		{
			return loadingFromSave;
		}
		set
		{
			loadingFromSave = value;
		}
	}

	static bool authenticated = false;
	public static bool Authenticated
	{
		get
		{
			return authenticated;
		}
		set
		{
			authenticated = value;
		}
	}
	
	static SaveManager main;
	static bool stoleControl = false;
	static void Init ()
	{
		if (main == null)
		{
			GameObject go = new GameObject();
			go.name = "Load & Save";
			main = (SaveManager)go.AddComponent(typeof(SaveManager));
			MovementController mc = MovementController.GetMovementController();
			if (mc && mc.PCControlled())
			{
				mc.LockPCControl();
				stoleControl = true;
			}
		}
	}

	void Awake ()
	{
		DontDestroyOnLoad(gameObject);
	}
	
	static void Finish()
	{
		Debug.Log("Finishing up SaveManager...");
		
		if (stoleControl)
		{
			MovementController mc = MovementController.GetMovementController();
			mc.SetPCControl();
		}
		savingState = SavingState.None;
		SaveUtil.Saves = null;
		main.StopAllCoroutines();
		Destroy(main);
	}

	static bool CanSave
	{
		get
		{
			return savingState == SavingState.None;
		}
	}
	static bool CanLoad
	{
		get
		{
			return savingState == SavingState.None;
		}
	}
	static bool CanCreateProfile
	{
		get
		{
			return savingState == SavingState.None;
		}
	}

	static bool CanAuthenticate
	{
		get
		{
			return savingState == SavingState.None;
		}
	}

	
	public static void DoSave ()
	{
		if (CanSave)
		{
			Init();
			main.sceneToSave = "";	
			savingState = SavingState.Saving;
		}
	}

	string sceneToSave = "";
	public static void DoSave (string sceneName)
	{
		if (CanSave)
		{
			Init();
			main.sceneToSave = sceneName;
			savingState = SavingState.Saving;
		}
		else
		{
			Debug.Log("Cannot save: " + savingState);
		}
	}

	
	public static void DoLoad ()
	{
		if (CanLoad)
		{
			Init();
			
			savingState = SavingState.Loading;
			if (!webSaving)
				SaveUtil.Init();
		}
	}

	public static void Authenticate ()
	{
		if (CanAuthenticate)
		{
			Init();
			main.authMessage = "";
			savingState = SavingState.Authenticating;
		}
	}
	
	public static void CreateProfile ()
	{
		if (CanCreateProfile)
		{
			Init();
			main.createProfileMessage = "Please write down your username and password and keep handy.  We do not collect email addresses, so we cannot send you a reminder of your login information.";
			savingState = SavingState.CreatingProfile;
		}
	}
	
	static void CreateSave(string nm)
	{
		Debug.Log("DO SAVE: " + nm);
		
		GameObject player=(GameObject)GameObject.FindWithTag("Player");
		ActorBase actor = (ActorBase)player.GetComponent(typeof(ActorBase));
				
		if (!nm.EndsWith(".texas")) nm += ".texas";
		SaveGameFacade sg = SaveGameFacade.NewSave(SaveUtil.SavePath+nm);
		
		SaveGameFacade.PCData pc = new SaveGameFacade.PCData();
		pc.name = actor.Name;
		
		pc.location = player.transform.position;
		pc.rotation = player.transform.rotation;
		pc.scene = Application.loadedLevelName;
		
		sg.pcData = pc;
		sg.Persist();
		Finish();
	}

	public static IEnumerator WaitForMan ()
	{
		Debug.LogWarning("Waiting for man");
		
		GameObject man = null;
		while(man == null)
		{
			man=(GameObject)GameObject.FindWithTag("Player");

			yield return new WaitForSeconds(0);
		}

		Debug.Log("web saving? " + webSaving);
		
		if (webSaving)
		{
			/* The decision was made not to load positions or goals...
			   Just start the scene from its normal starting point
			man.transform.position = savePosition;
			man.transform.eulerAngles = saveRotation;
			*/
		}
		else
		{
			man.transform.position = loadedGame.pcData.location;
			man.transform.rotation = loadedGame.pcData.rotation;
		
			ActorBase actor = (ActorBase)man.GetComponent(typeof(ActorBase));
			actor.Name = loadedGame.pcData.name;
		}
		
		
		loadedGame = null;
		Finish();
	}
	
	void OnLevelWasLoaded(int ind)
	{
		Debug.LogWarning("Level was loaded (index: " + ind + "), name " + Application.loadedLevelName);
		
		if (!FlashbackManager.FlashbackActive)
		{
			Debug.Log("No flashback active, level loaded");
			if (loadedGame != null || webSaving)
			{
				if (loadedGame != null)
					Debug.Log("Loaded game from save game: " + loadedGame.pcData.scene);
				else
					Debug.Log("Loaded from web saved game");
				
				StartCoroutine(WaitForMan());
			}
		}
		else
		{
			Debug.Log("Flashback active, level loaded");
		}
		loadingFromSave = false;
	}
	
	static SaveGameFacade loadedGame;
	static void LoadSave (SaveDescriptor desc)
	{
		Debug.Log("Load: " + desc.PrettyName);
		DontDestroyOnLoad(main.gameObject);
		loadedGame = SaveGameFacade.ReadSave(desc.Path+desc.SaveName);
		Debug.Log("Loaded pc name: " + loadedGame.pcData.name);
		if (loadedGame.pcData == null) Debug.Log("No PC DATA!");
		if (loadedGame.pcData.scene != null)
			Application.LoadLevel(loadedGame.pcData.scene);
		else
		{
			Debug.Log("No Scene specified for load, loading 0...");
			Application.LoadLevel(0);
		}
	}

	static bool webSaving = true;
	public static void SetSaveMode (bool webSave)
	{
		webSaving = webSave;
	}
	
	string saveName = "savename";
	Vector2 saveWH = new Vector2(200, 60);
	Vector2 loadWH = new Vector2(200, 200);
	Vector2 loadScrollPosition;
	void LocalSaving ()
	{
		if (Saving)
		{
			Rect saveRect = new Rect(Screen.width/2 - saveWH.x/2, Screen.height/2 - saveWH.y/2, saveWH.x, saveWH.y);
			GUI.Box(saveRect, "Save");
			GUILayout.BeginArea(saveRect);
			GUILayout.FlexibleSpace();
			saveName = GUILayout.TextField(saveName);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Cancel"))
			{
				Finish();
				GUIManager.PlayButtonSound();
			}
			if (GUILayout.Button("Save"))
			{
				CreateSave(saveName);
				GUIManager.PlayButtonSound();
			}
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
		else if (Loading)
		{
			Rect loadRect = new Rect(Screen.width/2 - loadWH.x/2, Screen.height/2 - loadWH.y/2, loadWH.x, loadWH.y);
			GUI.Box(loadRect, "Load");
			loadRect.y += 20;
			loadRect.height -= 20;
			GUILayout.BeginArea(loadRect);
			GUILayout.FlexibleSpace();
			loadScrollPosition = GUILayout.BeginScrollView(loadScrollPosition, GUILayout.Width(loadWH.x));
			foreach(SaveDescriptor desc in SaveUtil.Saves)
			{
				if (GUILayout.Button(desc.PrettyName, GUILayout.Width(loadWH.x-20)))
				{
					LoadSave(desc);
					GUIManager.PlayButtonSound();
				}
			}
			GUILayout.EndScrollView();
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Cancel"))
			{
				Finish();
				GUIManager.PlayButtonSound();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
	}


	/* Example load data response:
	  <?xml version="1.0" encoding="UTF-8"?>
	  <root>
	  <game_id>2</game_id>
	  <gender>male</gender>
	  <scene>Chapter 1</scene>
	  <goal>None</goal>
	  <location>p: (727.1, 56.0, 580.0); 
	  r: (0.0, 297.2, 0.0)</location>
	  <cards>
	  </cards>
	  <letter>
	  </letter>
	  </root>
	 */

	//static Vector3 savePosition;
	static Vector3 saveRotation;
	
	void ParseLoadData (XmlDocument xml)
	{
		Debug.Log("Parsing load data");
		string gameId = "";
		string gender = "";
		string scene = "";
		string goal = "";
		string location = "";
		
		XPathNavigator navigator = xml.CreateNavigator();
		XPathNodeIterator iterator = navigator.Select("/root/game_id");
		iterator.MoveNext(); gameId = iterator.Current.Value;
		iterator = navigator.Select("/root/gender"); iterator.MoveNext(); gender = iterator.Current.Value;
		profileGender = gender;
		iterator = navigator.Select("/root/scene"); iterator.MoveNext(); scene = iterator.Current.Value;
		iterator = navigator.Select("/root/goal"); iterator.MoveNext(); goal = iterator.Current.Value;
		iterator = navigator.Select("/root/location"); iterator.MoveNext(); location = iterator.Current.Value;

		Debug.Log("Game id: " + gameId);
		Debug.Log("Gender: " + gender);
		Debug.Log("Scene: " + scene);
		Debug.Log("Goal: " + goal);
		Debug.Log("Location: " + location);

		/* Location data looks like:
		   p: (727.1, 56.0, 580.0);
		   r: (0.0, 297.2, 0.0) */
		//string position = "";
		//string[] xyz = null;

		//if (location.Length > 10)
		//{
			//position = location.Substring(4, location.IndexOf(";")-5);
			//xyz = position.Split(',');
            //savePosition = new Vector3(float.Parse(xyz[0], CultureInfo.InvariantCulture.NumberFormat),
            //                           float.Parse(xyz[1], CultureInfo.InvariantCulture.NumberFormat),
            //                           float.Parse(xyz[2], CultureInfo.InvariantCulture.NumberFormat));

			//int start = location.IndexOf(";")+7;
			//int length = location.Substring(start).Length - 1;
			//string rotation = location.Substring(start, length);
			
			//xyz = rotation.Split(',');
            //saveRotation = new Vector3(float.Parse(xyz[0], CultureInfo.InvariantCulture.NumberFormat),
            //                           float.Parse(xyz[1], CultureInfo.InvariantCulture.NumberFormat),
            //                           float.Parse(xyz[2], CultureInfo.InvariantCulture.NumberFormat));
		//}

		NotebookManager.SetInfoCardData(xml);
		NotebookManager.SetLetterToMaData(xml);
		
		loadingFromSave = true;
		LevelLoader.LoadLevelProgress(scene, null, null);
	}

	public static int sessionUserId = -1;
	void PostSucceeded (XmlDocument responseXML)
	{
		Debug.Log("Post succeeded stub...");
		switch(savingState)
		{
			case SavingState.SubmittingAuthentication:
				//XmlNodeList userId = responseXML.GetElementsByTagName("user_id");
				//sessionUserId = Int32.Parse(userId[0].InnerText);
				//ParseLoadData(responseXML);
				Debug.Log("Valid login.");
				authMessage = "Login is valid";
				authenticated = true;
				break;
			case SavingState.SubmittingProfile:
				Debug.Log("Created profile.");
				createProfileMessage = "Profile created successfully";
				authenticated = true;
				break;
			case SavingState.SubmittingLoad:
				ParseLoadData(responseXML);
				break;
			case SavingState.SubmittingSave:
				Debug.Log("Data should have saved to web successfully");
				Finish();
				break;
		}

	}

	void PostFailed (XmlDocument responseXML, string msg)
	{
		switch(savingState)
		{
			case SavingState.SubmittingProfile:
				savingState = SavingState.CreatingProfile;
				createProfileMessage = msg;
				authenticated = false;
				break;
			case SavingState.SubmittingAuthentication:
				savingState = SavingState.Authenticating;
				main.authMessage = "That login is incorrect. Please try again";
				authenticated = false;
				break;
			default:
				savingState = SavingState.None;
				break;
		}

		//if (savingState != SavingState.SubmittingProfile)
		//	Finish();
	}

	static bool doPost = true;
	public static bool DoPost
	{
		get
		{
			return doPost;
		}
		set
		{
			doPost = value;
		}
	}
	
	static string saveURL = "http://www.tx-independence.org/unity/";
	//static bool postInProcess = false;
	IEnumerator PostXML (string xml)
	{
		//postInProcess = true;
		
		WWWForm form = new WWWForm();
		form.AddField("request", xml);

		if (!doPost)
		{
			Debug.LogWarning("POST turned off for debugging, would have posted request: " + xml);
			if (savingState == SavingState.CreatingProfile)
				createProfileMessage = "simulating server response delay...";

			yield return new WaitForSeconds(2);
			savingState = SavingState.None;
			Finish();
		}
		else
		{
			WWW www = new WWW(saveURL, form);
			
			yield return www;
			
			if (www.error != null)
			{
				Debug.Log(www.error);
			}
			else
			{
				Debug.Log("Received www data: "+  www.text);
				
				XmlDocument responseXML = new XmlDocument();
				responseXML.LoadXml(www.text);
				XmlNodeList success = responseXML.GetElementsByTagName( "success" );
				XmlNodeList msg = responseXML.GetElementsByTagName("msg");;

				/*if (success.Count == 0)
				{
					// assume success for now...
					Debug.LogWarning("Assuming success since some server responses don't tell us so...");
					PostSucceeded(responseXML);
				}
				else*/
				{
					for ( int i = 0; i < success.Count; ++i )
					{
						int res = Int32.Parse(success[i].InnerText);
						if (res == 1)
						{
							Debug.Log("Success: " + res + " msg: " + msg[i].InnerText);
							PostSucceeded(responseXML);
						}
						else
						{
							Debug.Log("Failed: " + res + " msg: " + msg[i].InnerText);
							PostFailed(responseXML, msg[i].InnerText);
							Debug.LogWarning("Failed: " + res + " msg: " + msg[i].InnerText + " form: " + xml);
						}
					}
				}
			}
		}
		//postInProcess = false;
	}
	
	const string createXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
		"<root>" +
		"<action>create</action>" +
		"<firstname>*NAME*</firstname>" +
		"<lastname>*LASTNAME*</lastname>" +
		"<username>*USERNAME*</username>" +
		"<password>*PASSWORD*</password>" +
		"</root>";

	static string profileFirstName = "";
	static string profileLastName = "";
	static string profileUsername = "";
	static string profilePassword = "";
	public static string profileGender = "";
	public static Gender Gender
	{
		get
		{
			return (profileGender == "male" || profileGender == "") ? Gender.Male : Gender.Female;
		}
	}
	void SubmitProfileData ()
	{
		savingState = SavingState.SubmittingProfile;
		
		string xml = createXML;
		
		xml = xml.Replace("*NAME*", profileFirstName);
		xml = xml.Replace("*LASTNAME*", profileLastName);
		xml = xml.Replace("*USERNAME*", profileUsername);
		xml = xml.Replace("*PASSWORD*", profilePassword);

		Debug.Log("Submit XML: " + xml);
		main.StartCoroutine(main.PostXML(xml));
	}

    Rect createProfileArea = new Rect(0, 0, 400, 250);
	string createProfileMessage = "";
	void CreateWebProfile ()
	{
		if (ProfileManager.Skin)
			GUI.skin = ProfileManager.Skin;
		createProfileArea.x = Screen.width - createProfileArea.width - ProfileManager.BoxInset.x;
		createProfileArea.y = Screen.height - createProfileArea.height - ProfileManager.BoxInset.y;
		GUI.Box(createProfileArea, "");
		GUILayout.BeginArea(createProfileArea);

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("Sign Up!", ProfileManager.BoxTitleStyle);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("First Name: ");
		profileFirstName = GUILayout.TextArea(profileFirstName, GUILayout.Width(createProfileArea.width/2));
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Last Name: ");
		profileLastName = GUILayout.TextArea(profileLastName, GUILayout.Width(createProfileArea.width/2));
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("User Name: ");
		profileUsername = GUILayout.TextArea(profileUsername, GUILayout.Width(createProfileArea.width/2));
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Password: ");
		//profilePassword = GUILayout.TextArea(profilePassword,
		//GUILayout.Width(createProfileArea.width/2));
		profilePassword = GUILayout.PasswordField (profilePassword, "*"[0], GUILayout.Width(createProfileArea.width/2));
		GUILayout.EndHorizontal();

		//GUILayout.Label(createProfileMessage);
		GUILayout.Label(createProfileMessage, ProfileManager.CreateProfileStyle);
		
		//GUILayout.FlexibleSpace();
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Cancel"))
		{
			savingState = SavingState.None;
			Finish();
		}
		else if (savingState != SavingState.SubmittingProfile && GUILayout.Button("Create"))
		{
			SubmitProfileData();
			createProfileMessage = "Submitting profile data...";
		}
		else if (savingState == SavingState.SubmittingProfile && GUILayout.Button(authenticated ? "OK" : "Create"))
		{
			if (authenticated)
			{
				savingState = SavingState.None;
				Finish();
			}
			else
			{
				savingState = SavingState.CreatingProfile;
				SubmitProfileData();
				createProfileMessage = "Submitting profile data...";
			}
		}

		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	string loadXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
		"<root>" +
		"<action>load</action>" +
		"<username>*UNAME*</username>" +
		"</root>";
	void WebLoad ()
	{
		if (profileUsername == "")
		{
			Debug.LogWarning("NO USERNAME IS SET, YOU MUST LOGIN FIRST");
			GUIManager.DebugText(this, "NO USERNAME SET, you must login before loading or saving.");
			savingState = SavingState.None;
			Finish();
			
			return;
		}

		
		savingState = SavingState.SubmittingLoad;

		
		string loadData = loadXML.Replace("*UNAME*", profileUsername);

		StartCoroutine(PostXML(loadData));
	}


	string authXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
		"<root>" +
		"<action>authenticate</action>" +
		"<username>*UNAME*</username>" +
		"<password>*PASSWORD*</password>" +
		"</root>";
	void SubmitAuthentication ()
	{
		savingState = SavingState.SubmittingAuthentication;
		
		string authData = authXML.Replace("*UNAME*", profileUsername);
		authData = authData.Replace("*PASSWORD*", profilePassword);

		StartCoroutine(PostXML(authData));
	}


	string authMessage = "";
	void WebAuthenticate ()
	{
		if (ProfileManager.Skin)
			GUI.skin = ProfileManager.Skin;
		
		createProfileArea.x = Screen.width - createProfileArea.width - ProfileManager.BoxInset.x;
		createProfileArea.y = Screen.height - createProfileArea.height - ProfileManager.BoxInset.y;
		GUI.Box(createProfileArea, "");
		GUILayout.BeginArea(createProfileArea);
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("Log In!", ProfileManager.BoxTitleStyle);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Username: ");
		profileUsername = GUILayout.TextArea(profileUsername, GUILayout.Width(createProfileArea.width/2));
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Password: ");
		//profilePassword = GUILayout.TextArea(profilePassword,
		//GUILayout.Width(createProfileArea.width/2));
		profilePassword = GUILayout.PasswordField (profilePassword, "*"[0], GUILayout.Width(createProfileArea.width/2));
		GUILayout.EndHorizontal();

		GUILayout.Label(authMessage);
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Cancel"))
		{
			savingState = SavingState.None;
			Finish();
		}
		else if (savingState != SavingState.SubmittingAuthentication && GUILayout.Button("Submit"))
		{
			SubmitAuthentication();
			authMessage = "Submitting profile data...";
		}
		else if (savingState == SavingState.SubmittingAuthentication && GUILayout.Button("OK"))
		{
			savingState = SavingState.None;
			Finish();
		}

		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}



	
	public const string cardEntryXML = "  <blurb>*ENTRY*</blurb>";
	const string cardsXML = "<cards>" +
		"*CARDENTRIES*" +
		"</cards>";

	public const string letterEntryXML = "<text>*ENTRY*</text>";
	const string letterXML = "<letter>" +
		"*LETTERENTRIES*" +
		"</letter>";
	
	const string saveXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
		"<root>" +
		"<version>*VERSION*</version>" +
		"<username>*UNAME*</username>" +
		"<password>*PASSWORD*</password>" +
		"<action>save</action>" +
		"<gender>*GENDER*</gender>" +
		"<scene>*SCENE*</scene>" +
		"<goal>*GOAL*</goal>" +
		"<location>*LOCATION*</location>" +
		"*CARDS*" +
		"*LETTERS*" +
		"</root>";
	void WebSave ()
	{
		if (profileUsername == "")
		{
			Debug.LogWarning("NO USERNAME IS SET, YOU MUST LOGIN FIRST");
			GUIManager.DebugText(this, "NO USERNAME SET, you must login before loading or saving.");
			savingState = SavingState.None;
			Finish();
			
			return;
		}

		savingState = SavingState.SubmittingSave;
		GameObject player=(GameObject)GameObject.FindWithTag("Player");
		string playerLocation = "";
		string playerRotation = "";
		if (player)
		{
			playerLocation = "p: " + player.transform.position;
			playerRotation = "\nr: " + player.transform.rotation.eulerAngles;
		}
		
		string saveData = saveXML.Replace("*VERSION*", "1");
		saveData = saveData.Replace("*UNAME*", profileUsername);
		saveData = saveData.Replace("*PASSWORD*", profilePassword);
		saveData = saveData.Replace("*GENDER*", profileGender);
		saveData = saveData.Replace("*SCENE*", main.sceneToSave == "" ? Application.loadedLevelName : main.sceneToSave);
		saveData = saveData.Replace("*GOAL*", GoalManager.CurrentGoal == null ? "None" : GoalManager.CurrentGoal.name);
		saveData = saveData.Replace("*LOCATION*", (playerLocation + "; " + playerRotation));

		string cardData = cardsXML.Replace("*CARDENTRIES*", NotebookManager.InfoCardSaveData);
		string letterData = letterXML.Replace("*LETTERENTRIES*", NotebookManager.LetterSaveData);

		saveData = saveData.Replace("*CARDS*", cardData);
		saveData = saveData.Replace("*LETTERS*", letterData);

		StartCoroutine(PostXML(saveData));
	}
	
	void WebSaving ()
	{
		switch(savingState)
		{
			case SavingState.CreatingProfile:
				CreateWebProfile();
				break;
			case SavingState.SubmittingProfile:
				CreateWebProfile();
				break;
			case SavingState.Loading:
				WebLoad();
				break;
			case SavingState.Saving:
				WebSave();
				break;
			case SavingState.Authenticating:
				WebAuthenticate();
				break;
			case SavingState.SubmittingAuthentication:
				WebAuthenticate();
				break;
		}
	}
	
	void OnGUI ()
	{
		if (webSaving)
		{
			WebSaving();
		}
		else
		{
			LocalSaving();
		}
	}
}
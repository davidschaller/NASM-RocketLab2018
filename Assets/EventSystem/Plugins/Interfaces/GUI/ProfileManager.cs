using UnityEngine;
using System.Collections;

public class ProfileManager : MonoBehaviour
{
	enum State
	{
		StartOrContinue,
		ChooseGender,
	}

	State state = State.StartOrContinue;
	
	public GUISkin skin;
	public string introTitle = "Experience the days of revolution in Washington Town, March 1836";
	public string introText = "You can play the game without an account--but if you are logged in, your game will auto-save so you can return later to keep playing.";
	public string startNewButton = "Start New Game";
	public string continueButton = "Continue Saved Game";
	public string createAccountButton = "Sign Up";
	public string loginButton = "Login";
	public string youreLoggedIn = "You're logged in!";

	public string boxTitleStyle = "Label";
	public string introTextStyle = "Label";
	public string startNewStyle = "Button";
	public string continueStyle = "Button";
	public string createAccountStyle = "Button";
	public string createMessageStyle = "Label";
	public string loginStyle = "Button";
	public string youreLoggedInStyle = "Label";

	public EventPlayer startNewGameEP;

	public Vector2 boxInset = new Vector2(10, 10);
	public static Vector2 BoxInset
	{
		get
		{
			return main.boxInset;
		}
	}

	public static GUISkin Skin
	{
		get
		{
			if (main != null)
				return main.skin;

			return null;
		}
	}

	public static GUIStyle BoxTitleStyle
	{
		get
		{
			if (Skin)
			{
				GUIStyle style = Skin.FindStyle(main.boxTitleStyle);
				if (style == null)
					style = Skin.FindStyle("Box");
				return style;
			}

			return GUI.skin.GetStyle("Label");
		}
	}

	public static GUIStyle CreateProfileStyle
	{
		get
		{
			if (Skin)
			{
				GUIStyle style = Skin.FindStyle(main.createMessageStyle);
				if (style == null)
					style = Skin.FindStyle("Label");

                style.stretchHeight = true; ;
				return style;
			}

			return GUI.skin.GetStyle("Label");
		}
	}

	
	static ProfileManager main;
	void Awake ()
	{
		main = this;
	}
	
	public Rect backgroundRect = new Rect(0, 0, 400, 250);
    public Rect whoAreYouRect = new Rect(0, 0, 400, 250);
	public string whoAreYouBoxStyle = "RedBox";
	public string whoAreYouTitle = "Who Are You?";
	public string whoAreYouText = "You are Jessie Watson, a 14 year-old living with your family in Texas.  Click on the player character you prefer.";
	public string whoAreYouStyle = "BodyTextWhite";
	public Texture chooseBG;
	public Texture boyImage;
	public Texture girlImage;
	public Texture selectedImage;
	public Texture unselectedImage;
	public Rect boyRect = new Rect(400, 100, 200, 600);
	public Rect girlRect = new Rect(400 + 240, 100, 200, 600);
	public Rect continueButtonRect = new Rect(520, 750, 100, 40);
	int selection = 0;
	void OnGUI ()
	{
		if (SaveManager.SavingState != SavingState.None)
			return;
		
		if (skin)
			GUI.skin = skin;

		//GUI.depth = -1000;


		if (state == State.StartOrContinue)
		{
			backgroundRect.x = Screen.width - backgroundRect.width - boxInset.x;
			backgroundRect.y = Screen.height - backgroundRect.height - boxInset.y;
			
		
		
			GUI.Box(backgroundRect, "");
			GUILayout.BeginArea(backgroundRect);
			GUILayout.Label(introTitle, boxTitleStyle);
			GUILayout.Label(SaveManager.Authenticated ? youreLoggedIn : introText, introTextStyle);

			GUILayout.BeginHorizontal();
			if (GUILayout.Button(startNewButton, startNewStyle))
			{
				state = State.ChooseGender;
			}

			if (!SaveManager.Authenticated)
			{
				GUILayout.BeginVertical();
				if (GUILayout.Button(createAccountButton, createAccountStyle))
					SaveManager.CreateProfile();
			
				if (GUILayout.Button(loginButton, loginStyle))
					SaveManager.Authenticate();		
				GUILayout.EndVertical();
			}
			else
			{
				if (GUILayout.Button(continueButton, continueStyle))
				{
					SaveManager.DoLoad();
				}
			}
		
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
		else if (state == State.ChooseGender)
		{
			if (chooseBG)
			{
				GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), chooseBG);
			}
			GUI.Box(whoAreYouRect, whoAreYouTitle);
			GUILayout.BeginArea(whoAreYouRect);
			GUILayout.Label(whoAreYouText, whoAreYouStyle);
			GUILayout.EndArea();


			if (Event.current.type == EventType.MouseDown)
			{
				if (boyRect.Contains(Event.current.mousePosition))
				{
					selection = 1;
					Event.current.Use();
				}
				else if (girlRect.Contains(Event.current.mousePosition))
				{
					selection = 2;
					Event.current.Use();
				}
			}


            if (boyImage)
            {
                GUI.DrawTexture(boyRect, boyImage);
                if (selection == 0 || selection == 2)
                {
                    GUI.DrawTexture(boyRect, unselectedImage);
                }
                else if (selection == 1)
                {
                    GUI.DrawTexture(boyRect, selectedImage);
                }
            }
            if (girlImage)
            {
                GUI.DrawTexture(girlRect, girlImage);
                if (selection == 0 || selection == 1)
                {
                    GUI.DrawTexture(girlRect, unselectedImage);
                }
                else if (selection == 2)
                {
                    GUI.DrawTexture(girlRect, selectedImage);
                }
            }
			
			if (selection != 0 && GUI.Button(continueButtonRect, "Continue"))
			{
				if (selection == 1)
					SaveManager.profileGender = "male";
				else if (selection == 2)
					SaveManager.profileGender = "female";
				
				startNewGameEP.PlayerTriggered();
				Destroy(gameObject);
			}
		}
	}
}

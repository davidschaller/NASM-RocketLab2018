using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class NewGameGUI : GUIBase
{
#if Crypto
    private string loadLevelName = string.Empty;

    private GUIStyle bigHeaderStyle,
                     box2Style,
                     greenHeaderStyle,
                     labelStyle,
                     textFieldStyle,
                     buttonStyle,
                     smallButtonStyle,
                     toggleStyle,
					 BtoggleStyle,
                     box4Style,
                     blackLabelStyle;

/*    public string headerText = "NEW GAME",
                  title1 = "Set Up Game Progression",
                  title2 = "Chouse Ciper Method",
                  title3 = "Chouse the Type of Game",
                  backText = "Go Back",
                  startText = "Start Game",
                  q1Text = "?",
				  tg1 = "One round per day (all players do not have to complete a round for game to progress)",
				  tg2 = "Game progresses when all players have completed each round",
                  popupDescr = "Each game takes 4-5 rounds of clue-gathering and analysis before a plaer wins. You can set up your game in two ways:",
                  popupTg1 = "A new round is opened each day (so the game will be played out over 4-5 days). Each player must return daily to play the new round, but the game does not depend on every player doing so. New rounds will open daily whether or not all players have completed the previous round. That way? no one will hold up the game.",
                  popupTg2 = "Game progresses when players complete each round. All players must complete each round for the game to progress. If all players are participating? you can play through the game in one day. But one player can hold up the entire game if they do not participate.";
*/
    private float paddingTop = 20,
                    textFieldWidth = 130;
    private bool timedToggle = false;
    private bool regularToggle = true;
    private bool CCMTg1 = true, CCMTg2 = false;
    private bool CTGTg1 = true, CTGTg2 = false;

    private int popup_id = 0;
    private string Password = "";
    private Rect windowRect = new Rect(20, 20, 120, 50);

    #region core functions

    new void Awake()
    {
        base.Awake();
        MainGUI.RegisterStateScript(MainGUI.States.NewGame, GetComponent<NewGameGUI>());
    }

    private void OnEnable()
    {
        loadLevelName = string.Empty;
    }

    private void SetGUIStyles()
    {
        if (SkinManager.IsActive)
        {
            bigHeaderStyle = SkinManager.Main.GetStyle("BigHeader", "Begin");
            box2Style = SkinManager.Main.GetStyle("Box2", "Begin");
            greenHeaderStyle = SkinManager.Main.GetStyle("GreenHeader", "Begin");
            greenHeaderStyle.padding.left = 0;
            greenHeaderStyle.padding.right = 0;
            labelStyle = SkinManager.Main.GetStyle("Label", "Begin");
            labelStyle.wordWrap = true;
            textFieldStyle = SkinManager.Main.GetStyle("textfield", "Begin");
            buttonStyle = SkinManager.Main.GetStyle("button", "Begin");
            smallButtonStyle = SkinManager.Main.GetStyle("SmallButton", "Begin");
            toggleStyle = SkinManager.Main.GetStyle("toggle", "Begin");
			//toggleStyle.fontSize = 13;
            BtoggleStyle = SkinManager.Main.GetStyle("ToggleButton", "Begin");
            box4Style = SkinManager.Main.GetStyle("Box4", "Begin");
            blackLabelStyle = SkinManager.Main.GetStyle("BlackLabel", "Begin");
        }
        else
            Debug.LogWarning("SkinManager is not active");
    }

    private void OnDisable()
    {
    }

    private void Start()
    {
        SetGUIStyles();
    }

    private void OnGUI()
    {
        if (string.IsNullOrEmpty(loadLevelName))
        {
            RenderNewGame();
        }
        else
        {
            if (Application.CanStreamedLevelBeLoaded(loadLevelName))
                Application.LoadLevel(loadLevelName);
        }
    }

    #endregion

    #region rendering

    private int roundLimit = 10;
    private bool isBeginner = false;


    private void RenderNewGame()
    {
        Rect newGameRect = new Rect(Screen.width / 2 - box2Style.fixedWidth / 2, Screen.height / 2 - box2Style.fixedHeight / 2, box2Style.fixedWidth, box2Style.fixedHeight);

        GUI.Box(newGameRect, GUIContent.none, box2Style);

        GUI.BeginGroup(newGameRect);

        Rect headerRect = new Rect((box2Style.fixedWidth - bigHeaderStyle.fixedWidth) / 2, paddingTop, box2Style.fixedWidth, box2Style.fixedHeight);
        GUI.Label(headerRect, new GUIContent(GetText("headerText")), bigHeaderStyle);

        Rect leftRect = new Rect(0, paddingTop + bigHeaderStyle.fixedHeight, newGameRect.width, newGameRect.height);
             //rightRect = new Rect(leftRect.width - 20, paddingTop + bigHeaderStyle.fixedHeight + 15, newGameRect.width * 0.3f, newGameRect.height * 0.5f);

        GUI.BeginGroup(leftRect);

        Vector2 labSize = greenHeaderStyle.CalcSize(new GUIContent(GetText("title1")));
        Rect nameRect = new Rect(10, 10, labSize.x, labSize.y);
        GUI.Label(nameRect, GetText("title1"), greenHeaderStyle);

        //Vector2 q1ButtonSize = smallButtonStyle.CalcSize(new GUIContent(q1Text));
        if (GUI.Button(new Rect(nameRect.x + nameRect.width + 10, nameRect.y, 18, 18), GetText("q1Text"), smallButtonStyle))
        {
            if (popup_id != 1)
                popup_id = 1;
            else
                popup_id = 0;
        }

        Rect progressionToggleRect = new Rect(20, nameRect.y + nameRect.height, box2Style.fixedWidth - 40, toggleStyle.CalcHeight(new GUIContent(GetText("tg1")),box2Style.fixedWidth - 40));
        timedToggle = !regularToggle;
        timedToggle = GUI.Toggle(progressionToggleRect, timedToggle, GetText("tg1"), toggleStyle);
        regularToggle = !timedToggle;
        Rect Toggle2 = new Rect(20, progressionToggleRect.y + progressionToggleRect.height+5, box2Style.fixedWidth - 40, toggleStyle.CalcHeight(new GUIContent(GetText("tg2")),box2Style.fixedWidth - 40));
        regularToggle = GUI.Toggle(Toggle2, regularToggle, GetText("tg2"),toggleStyle);

        labSize = greenHeaderStyle.CalcSize(new GUIContent(GetText("title2")));
        Rect CCMRect = new Rect(10, Toggle2.y + Toggle2.height + 10, labSize.x, labSize.y);
        GUI.Label(CCMRect, GetText("title2"), greenHeaderStyle);

        //Vector2 q2ButtonSize = smallButtonStyle.CalcSize(new GUIContent(q1Text));
        GUI.Button(new Rect(CCMRect.x + CCMRect.width + 10, CCMRect.y, 18, 18), GetText("q1Text"), smallButtonStyle);
        CCMTg1 = !CCMTg2;
        Rect CCMToggle1 = new Rect(20, CCMRect.y + CCMRect.height, box2Style.fixedWidth - 20, 20);
        CCMTg1 = GUI.Toggle(CCMToggle1, CCMTg1, GetText("tg3"), toggleStyle);

        CCMTg2 = !CCMTg1;
        Rect CCMToggle2 = new Rect(20, CCMToggle1.y + CCMToggle1.height, box2Style.fixedWidth - 20, 20);
        CCMTg2 = GUI.Toggle(CCMToggle2, CCMTg2, GetText("tg4"), toggleStyle);

        labSize = greenHeaderStyle.CalcSize(new GUIContent(GetText("title3")));
        Rect CTGRect = new Rect(10, CCMToggle2.y + CCMToggle2.height + 10, labSize.x, labSize.y);
        GUI.Label(CTGRect, GetText("title3"), greenHeaderStyle);

        //Vector2 q3ButtonSize = smallButtonStyle.CalcSize(new GUIContent(q1Text));
        GUI.Button(new Rect(CTGRect.x + CTGRect.width + 10, CTGRect.y, 18, 18), GetText("q1Text"), smallButtonStyle);
        CTGTg1 = !CTGTg2;
        Rect CTGToggle1 = new Rect(20, CTGRect.y + CTGRect.height, 200, 20);
        CTGTg1 = GUI.Toggle(CTGToggle1, CTGTg1, GetText("tg5"), toggleStyle);

        CTGTg2 = !CTGTg1;
        Rect CTGToggle2 = new Rect(20, CTGToggle1.y + CTGToggle1.height, 200, 20);
        CTGTg2 = GUI.Toggle(CTGToggle2, CTGTg2, GetText("tg6"), toggleStyle);

        Password = GUI.PasswordField(new Rect(CTGToggle2.x + CTGToggle2.width, CTGToggle2.y, textFieldWidth, CTGToggle2.height), Password, '*', textFieldStyle);
		
		Rect RLrect = new Rect(20, CTGToggle2.y + CTGToggle2.height+5, 170, 20);
        GUI.Label(RLrect, GetText("RoundLimit"));
		Rect RLTrect = new Rect(20, RLrect.y + RLrect.height, 170, 20);
        string roundLimitText = GUI.TextField(RLTrect, roundLimit.ToString());
        int.TryParse(roundLimitText, out roundLimit);
		Rect IsBrect = new Rect(20, RLTrect.y + RLTrect.height, 170, 20);
        isBeginner = GUI.Toggle(IsBrect, isBeginner, GetText("IsBeginner"));

        GUI.EndGroup();
        Vector2 backSize = buttonStyle.CalcSize(new GUIContent(GetText("backText")));
        Rect backRect = new Rect(20, newGameRect.height - backSize.y - 20, backSize.x, backSize.y);
        if (GUI.Button(backRect, GetText("backText"), buttonStyle))
        {
            MainGUI.SetState(MainGUI.States.Home);
        }

        Vector2 startSize = buttonStyle.CalcSize(new GUIContent(GetText("startText")));
        Rect startRect = new Rect(newGameRect.width - startSize.x - 20, backRect.y, startSize.x, startSize.y);
        if (GUI.Button(startRect, GetText("startText"), buttonStyle))
        {
            CryptoPlayerManager.State = CryptoPlayerManager.StartState.NewGame;
            if (timedToggle)
            {
                CryptoPlayerManager.IsTimedRounds = true;
                CryptoPlayerManager.RoundLimit = roundLimit;
            }
            CryptoPlayerManager.IsBeginner = isBeginner;

            if (!CryptoNet.GotUserData)
                StartCoroutine(CryptoNet.GetUserData());

            loadLevelName = "Dressingroom";
        }
        GUI.EndGroup();

        if (popup_id != 0)
        {

            windowRect = new Rect(newGameRect.x + newGameRect.height-40, newGameRect.y + newGameRect.width/6, 300, 350);
            //GUI.BeginGroup(windowRect);
            //GUI.Box(windowRect, GUIContent.none);
            windowRect = GUI.Window(popup_id, windowRect, ShowPopup, GetText("popupHeader"), box4Style);
            //GUI.EndGroup();
        }
        
    }

    private void ShowPopup(int windowID)
    {
        //GUI.BeginGroup(windowRect);
        //blackLabelStyle.fontSize = 13;
        Rect labrec = new Rect(10, 30, windowRect.width - 15, blackLabelStyle.CalcHeight(new GUIContent(GetText("popupDescr")), windowRect.width - 15));
        GUI.Label(labrec, GetText("popupDescr"), blackLabelStyle);

        //BtoggleStyle.fontSize = 10;
        timedToggle = !regularToggle;
        Rect tg1rec = new Rect(10, labrec.y + labrec.height + 10, windowRect.width - 15, BtoggleStyle.CalcHeight(new GUIContent(GetText("popupTg1")), windowRect.width - 15));
        timedToggle = GUI.Toggle(tg1rec, timedToggle, GetText("popupTg1"), BtoggleStyle);

        Rect tg2rec = new Rect(10, tg1rec.y + tg1rec.height + 10, windowRect.width - 15, BtoggleStyle.CalcHeight(new GUIContent(GetText("popupTg2")), windowRect.width - 15));
        regularToggle = !timedToggle;
        regularToggle = GUI.Toggle(tg2rec, regularToggle, GetText("popupTg2"), BtoggleStyle);

        Vector2 closeSize = smallButtonStyle.CalcSize(new GUIContent(GetText("Close")));
        Rect closeRect = new Rect((windowRect.width - closeSize.x) / 2, windowRect.height - 30, closeSize.x, closeSize.y);
        if (GUI.Button(closeRect, GetText("Close"), smallButtonStyle))
            popup_id = 0;
        //GUI.EndGroup();
    }

    #endregion
#endif
}
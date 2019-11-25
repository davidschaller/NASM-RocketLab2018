#if Crypto
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class JoinGameGUI : GUIBase
{
    private string loadLevelName = string.Empty;

    private GUIStyle box2Style,
                     greenHeaderStyle,
                     labelStyle,
                     textFieldStyle,
                     buttonStyle,
                     bigHeaderStyle,
                     orangeStyle,
                     listButtonStyle,
                     smallButtonStyle,
                     blackLabelStyle,
                     highLighted_style;

/*    public string headerText = "JOIN A GAME",
                  backText = "Go Back",
                  startText = "Start Game",
                  green1Text ="Current Public Games:",
                  GHText = "Game Host",
                  PText = "Progression",
                  CText = "Ciphers",
                  green2Text = "Private Games:",
                  TPText = "Type password:",
                  orangeText = "Click on a public game to select it,\nor type the password of a private game.";*/

    private float paddingTop = 10;

    public int selPosition = -1;
    public string selkey = "";
    private int RowHeight = 18;
    private int MaxI = 1;
    private string Password="";
    private int rejoincount = 0;
    #region core functions

    new void Awake()
    {
    	base.Awake();
        MainGUI.RegisterStateScript(MainGUI.States.JoinGame, GetComponent<JoinGameGUI>());
    }

    private void SetGUIStyles()
    {
        if (SkinManager.IsActive)
        {
            box2Style = SkinManager.Main.GetStyle("Box2", "Begin");
            greenHeaderStyle = SkinManager.Main.GetStyle("GreenHeader", "Begin");
            //greenHeaderStyle.fontSize = 20;
            labelStyle = SkinManager.Main.GetStyle("Label", "Begin");
            textFieldStyle = SkinManager.Main.GetStyle("textfield", "Begin");
            buttonStyle = SkinManager.Main.GetStyle("button", "Begin");
            bigHeaderStyle = SkinManager.Main.GetStyle("BigHeader", "Begin");
            orangeStyle = SkinManager.Main.GetStyle("OrangeText", "Begin");
            listButtonStyle = SkinManager.Main.GetStyle("ListButton", "Begin");
            smallButtonStyle = SkinManager.Main.GetStyle("SmallQButton", "Begin");
            blackLabelStyle = SkinManager.Main.GetStyle("BlackLabel", "Begin");

            highLighted_style = new GUIStyle();
            highLighted_style.normal = listButtonStyle.active;

        }
        else
            Debug.LogWarning("SkinManager is not active");
    }

    private void OnEnable()
    {
        loadLevelName = string.Empty;
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

            RenderGameList();

            if (TimeToRefresh())
            {
                if (!CryptoNet.RefreshingGamesForPlayer)
                    StartCoroutine(CryptoNet.RefreshGameNamesForPlayer(CryptoPlayerManager.Username));

                if (!CryptoNet.RefreshingGameNames)
                    StartCoroutine(CryptoNet.RefreshGameNames());
            }
        }
        else
        {
            if (Application.CanStreamedLevelBeLoaded(loadLevelName))
                Application.LoadLevel(loadLevelName);
        }
    }

    #endregion

    Vector2 scrollPosition = Vector2.zero;

    #region rendering

    private void RenderGameList()
    {
        if (box2Style != null && greenHeaderStyle != null && labelStyle != null && textFieldStyle != null)
        {
            Rect newGameRect = new Rect(Screen.width / 2 - box2Style.fixedWidth / 2, Screen.height / 2 - box2Style.fixedHeight / 2, box2Style.fixedWidth, box2Style.fixedHeight);

            GUI.Box(newGameRect, GUIContent.none, box2Style);

            GUI.BeginGroup(newGameRect);

            Rect headerRect = new Rect((box2Style.fixedWidth - bigHeaderStyle.fixedWidth) / 2, paddingTop, bigHeaderStyle.fixedWidth, bigHeaderStyle.fixedHeight);
            GUI.Label(headerRect, new GUIContent(GetText("headerText")), bigHeaderStyle);

            Vector2 orangeSize = orangeStyle.CalcSize(new GUIContent(GetText("orangeText")));
            Rect orangeRect = new Rect((box2Style.fixedWidth - orangeSize.x) / 2, headerRect.height + 20 + paddingTop, orangeSize.x, orangeSize.y);
            GUI.Label(orangeRect, GetText("orangeText"), orangeStyle);

            Vector2 greenSize = greenHeaderStyle.CalcSize(new GUIContent(GetText("green1Text")));
            Rect greenRect = new Rect(20, orangeRect.y + orangeRect.height+10, greenSize.x, greenSize.y);
            GUI.Label(greenRect, GetText("green1Text"), greenHeaderStyle);

            //labelStyle.fontSize = 12;
            Vector2 GHSize = labelStyle.CalcSize(new GUIContent(GetText("GHText")));
            Rect GHRect = new Rect(20, greenRect.y + greenRect.height + 5, GHSize.x, GHSize.y);
            GUI.Label(GHRect, GetText("GHText"), labelStyle);
            GUI.Button(new Rect(GHRect.x + GHRect.width+5, GHRect.y -3, 18, 18), "?", smallButtonStyle);

            Vector2 PSize = labelStyle.CalcSize(new GUIContent(GetText("PText")));
            Rect PRect = new Rect((box2Style.fixedWidth - 20)/3 +20, greenRect.y + greenRect.height + 5, PSize.x, PSize.y);
            GUI.Label(PRect, GetText("PText"), labelStyle);
            GUI.Button(new Rect(PRect.x + PRect.width + 5, PRect.y - 3, 18, 18), "?", smallButtonStyle);
            
            Vector2 CSize = labelStyle.CalcSize(new GUIContent(GetText("CText")));
            Rect CRect = new Rect((box2Style.fixedWidth - 20) / 6*4 +20, greenRect.y + greenRect.height + 5, CSize.x, CSize.y);
            GUI.Label(CRect, GetText("CText"), labelStyle);
            GUI.Button(new Rect(CRect.x + CRect.width + 5, CRect.y - 3, 18, 18), "?", smallButtonStyle);

            Rect outsideRect = new Rect(15, CRect.y + CRect.height, newGameRect.height - 40, System.Math.Min(160, MaxI * RowHeight));
            Rect outsideRect1 = new Rect(0, 0, outsideRect.width-16, MaxI*RowHeight);

            if (CryptoNet.GameNamesForPlayer != null && CryptoNet.gameNames != null)
            {
                scrollPosition = GUI.BeginScrollView(outsideRect, scrollPosition, outsideRect1,false,true);
                int i = 0;
                
                foreach (string key in CryptoNet.GameNamesForPlayer.Keys)
                {
                    string[] gridStrings = new string[3];
                    gridStrings[0] = "Rejoin Starter: " + CryptoNet.GameNamesForPlayer[key];
                    gridStrings[1] = key;
                    gridStrings[2] = "Caeser";
                    if (DrawListRecord(outsideRect1, i, gridStrings))
                    {
                        selPosition = i;
                        selkey = key;
                    }
                    i++;
                    if (rejoincount < i)
                        rejoincount = i;
                }
                
                foreach (string key in CryptoNet.gameNames.Keys)
                {
                    if (string.IsNullOrEmpty(CryptoNet.gameNames[key]))
                        continue;

                    if (CryptoNet.GameNamesForPlayer.ContainsKey(key))
                        continue;

                    string gameInfo = CryptoNet.gameNames[key];

                    gameInfo = gameInfo.Replace("(full or started)", "");

                    string gameName = gameInfo.Split(':')[0];
                    string gameLevel = gameInfo.Split(':')[1];

                    string[] gridStrings = new string[3];
                    gridStrings[0] = "Starter: " + gameName;
                    gridStrings[1] = gameLevel;
                    gridStrings[2] = "Caeser";

                    if (DrawListRecord(outsideRect1, i, gridStrings))
                    {
                        selPosition = i;
                        selkey = key;
                    }
                    i++;
                    if (MaxI < i)
                        MaxI = i;
                    
                }
                GUI.EndScrollView();
            }
            Vector2 green2Size = greenHeaderStyle.CalcSize(new GUIContent(GetText("green2Text")));
            Rect green2Rect = new Rect(20, outsideRect.y + outsideRect.height + 5, green2Size.x, green2Size.y);
            GUI.Label(green2Rect, GetText("green2Text"), greenHeaderStyle);

            Vector2 TPSize = labelStyle.CalcSize(new GUIContent(GetText("TPText")));
            Rect TPRect = new Rect(20, green2Rect.y + green2Rect.height, TPSize.x, TPSize.y);
            GUI.Label(TPRect, GetText("TPText"), labelStyle);
            Password = GUI.TextField(new Rect(TPRect.x + TPRect.width, TPRect.y, TPSize.x, TPRect.height), Password, textFieldStyle);

            Vector2 backSize = buttonStyle.CalcSize(new GUIContent(GetText("backText")));
            Rect backRect = new Rect(20, newGameRect.height - backSize.y - 20, backSize.x, backSize.y);
            if (GUI.Button(backRect, GetText("backText"), buttonStyle))
            {
                MainGUI.SetState(MainGUI.States.Home);
            }

            Vector2 startSize = buttonStyle.CalcSize(new GUIContent(GetText("startText")));
            Rect startRect = new Rect(newGameRect.width - 20 - startSize.x, newGameRect.height - startSize.y - 20, startSize.x, startSize.y);
            if (GUI.Button(startRect, GetText("startText"), buttonStyle))
            {
                if (selPosition > -1)
                {
                    if(selPosition < rejoincount)
                    {
                        CryptoPlayerManager.KeyToJoin = selkey;
                        CryptoPlayerManager.State = CryptoPlayerManager.StartState.RejoinGame;
                        loadLevelName = "tajnia-original";
                    }
                    else if (CryptoNet.GameNamesForPlayer != null && !CryptoNet.GameNamesForPlayer.ContainsKey(selkey))
                    {
                        CryptoPlayerManager.KeyToJoin = selkey;
                        CryptoPlayerManager.State = CryptoPlayerManager.StartState.JoinGame;
                        loadLevelName = "Dressingroom";
                    }
                }
            }

            GUI.EndGroup();
        }
    }

    private bool DrawListRecord(Rect rect,int i, string[] data)
    {
        bool res = false;
        Rect btnrec = new Rect(rect.x, i * RowHeight, rect.width - 10, RowHeight);
        Rect labrec = new Rect((box2Style.fixedWidth - 20) / 3 + 25, i * RowHeight - 3, 300, RowHeight);
        Rect labrec2 = new Rect((box2Style.fixedWidth - 20) / 6 * 4 + 25, i * RowHeight - 3, 300, RowHeight);
        res = GUI.Button(btnrec, data[0], selPosition == i ? highLighted_style : listButtonStyle);
        //blackLabelStyle.fontSize = 16;
        GUI.Label(labrec, data[1], blackLabelStyle);
        GUI.Label(labrec2, data[2], blackLabelStyle);
        return res;
    }

    #endregion

    private float timer = 1;
    private float refreshDelay = 1;

    private bool TimeToRefresh()
    {
        timer += Time.deltaTime;

        if (timer >= refreshDelay)
        {
            timer = 0;
            return true;
        }
        else
            return false;
    }
}
#endif
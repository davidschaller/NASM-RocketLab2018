using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class HomeGUI : GUIBase
{
    private GUIStyle bigHeaderStyle,
                     labelStyle,
                     textFieldStyle,
                     buttonStyle,
                     box3Style,
                     greenTextboxStyle,
                     smallButtonStyle,
                     bigButtonStyle,
                     //orangeTextboxStyle,
                     boxGrayStyle,
                     greenHeaderStyle;

    /*private string headerText = "SPY CARD",
                  changeText = "Change",
                  rankText = "Rank:",
                  pointsText = "Points:",
                  affiliationText = "Affiliation:",
                  notificationsText = "Email Notifications:",
                  onText = "On",
                  offText = "Off",
                  completedText = "Games Completed:",
                  wonText = "Games Won:",
                  crackedText = "Historical Plagues Cracked:",
                  returnText = "Join or Resume Game",
                  newGameText = "New Game",
                  shopText = "Spy Shop",
                  leaderboardsText = "Leaderboards",
                  backText = "Go Back",
                  logoutText = "Logout",
                  archievementsText = "Archievements:";*/

    private float paddingTop = 10;
    private int affiliationValue = 1;
                

    #region core functions

    new void Awake()
    {
    	base.Awake();
        MainGUI.RegisterStateScript(MainGUI.States.Home, GetComponent<HomeGUI>());
    }

    private void SetGUIStyles()
    {
        if (SkinManager.IsActive)
        {
            bigHeaderStyle = SkinManager.Main.GetStyle("BigHeader", "Begin");
            labelStyle = SkinManager.Main.GetStyle("Label", "Begin");
            textFieldStyle = SkinManager.Main.GetStyle("textfield", "Begin");
            buttonStyle = SkinManager.Main.GetStyle("button", "Begin");
            box3Style = SkinManager.Main.GetStyle("box3", "Begin");
            greenTextboxStyle = SkinManager.Main.GetStyle("GreenTextbox", "Begin");

            smallButtonStyle = SkinManager.Main.GetStyle("SmallButton", "Begin");
            bigButtonStyle = SkinManager.Main.GetStyle("BigButton", "Begin");

            //orangeTextboxStyle = SkinManager.Main.GetStyle("OrangeTextbox", "Begin");

            boxGrayStyle = SkinManager.Main.GetStyle("BoxGray", "Begin");

            greenHeaderStyle = SkinManager.Main.GetStyle("GreenHeader", "Begin");
        }
        else
            Debug.LogWarning("SkinManager is not active");
    }

    private void OnEnable()
    {
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
        RenderSpyCard();
    }

    #endregion

    #region rendering

    private void RenderSpyCard()
    {
        if (box3Style != null && bigHeaderStyle != null && labelStyle != null && textFieldStyle != null)
        {
            Rect spyCardRect = new Rect(Screen.width / 2 - box3Style.fixedWidth / 2, Screen.height / 2 - box3Style.fixedHeight / 2, box3Style.fixedWidth, box3Style.fixedHeight);

            GUI.Box(spyCardRect, GUIContent.none, box3Style);

            GUI.BeginGroup(spyCardRect);

            Rect headerRect = new Rect((box3Style.fixedWidth - bigHeaderStyle.fixedWidth) / 2, paddingTop, box3Style.fixedWidth, box3Style.fixedHeight);
            GUI.Label(headerRect, new GUIContent(GetText("headerText")), bigHeaderStyle);

            Rect leftRect = new Rect(0, paddingTop + bigHeaderStyle.fixedHeight, spyCardRect.width * 0.7f, spyCardRect.height * 0.5f),
                 rightRect = new Rect(leftRect.width - 20, paddingTop + bigHeaderStyle.fixedHeight + 15, spyCardRect.width * 0.3f, spyCardRect.height * 0.5f);

            GUI.BeginGroup(leftRect);

            Rect nameRect = new Rect(10, 10, leftRect.width / 2, textFieldStyle.lineHeight);
            GUI.TextField(nameRect, "MyUser Name", greenTextboxStyle);
            Vector2 changeButtonSize = smallButtonStyle.CalcSize(new GUIContent(GetText("changeText")));

            GUI.Button(new Rect(nameRect.x + nameRect.width, nameRect.y, changeButtonSize.x, changeButtonSize.y), GetText("changeText"), smallButtonStyle);

            
            Vector2 rankSize = labelStyle.CalcSize(new GUIContent(GetText("rankText")));
            Rect rankRect = new Rect(20, nameRect.y + nameRect.height, rankSize.x, rankSize.y);
            GUI.Label(rankRect, GetText("rankText"), labelStyle);

            Vector2 pointsSize = labelStyle.CalcSize(new GUIContent(GetText("pointsText")));
            Rect pointsRect = new Rect(20, rankRect.y + rankRect.height, pointsSize.x, pointsSize.y);
            GUI.Label(pointsRect, GetText("pointsText"), labelStyle);
            
            Vector2 affSize = labelStyle.CalcSize(new GUIContent(GetText("affiliationText")));
            Rect affRect = new Rect(20, pointsRect.y + pointsRect.height, affSize.x, affSize.y);
            GUI.Label(affRect, GetText("affiliationText"), labelStyle);

            
            Vector2 notifSize = labelStyle.CalcSize(new GUIContent(GetText("notificationsText")));
            Rect notifRect = new Rect(20, affRect.y + affRect.height + 10, notifSize.x, notifSize.y);
            GUI.Label(notifRect, GetText("notificationsText"), labelStyle);

            
            //Vector2 onSize = buttonStyle.CalcSize(new GUIContent(onText));
            Rect selectionGridRect = new Rect(notifRect.x + notifRect.width + 10, notifRect.y, 73, 18);
            affiliationValue = GUI.SelectionGrid(selectionGridRect, affiliationValue, new GUIContent[2] { new GUIContent(GetText("onText")), new GUIContent(GetText("offText"))}, 2, smallButtonStyle);

           
            Vector2 completedSize = buttonStyle.CalcSize(new GUIContent(GetText("completedText")));
            Rect completedRect = new Rect(85, selectionGridRect.y + selectionGridRect.height, completedSize.x, completedSize.y);
            GUI.Label(completedRect, GetText("completedText"), labelStyle);

            Vector2 wonSize = buttonStyle.CalcSize(new GUIContent(GetText("wonText")));
            Rect wonRect = new Rect(85, completedRect.y + completedRect.height, wonSize.x, wonSize.y);
            GUI.Label(wonRect, GetText("wonText"), labelStyle);

            Vector2 crackedSize = buttonStyle.CalcSize(new GUIContent(GetText("crackedText")));
            Rect crackedRect = new Rect(85, wonRect.y + wonRect.height, crackedSize.x, crackedSize.y);
            GUI.Label(crackedRect, GetText("crackedText"), labelStyle);
            
            GUI.EndGroup();

            
            GUI.BeginGroup(rightRect, boxGrayStyle);

            float archiHeaderHeight = bigHeaderStyle.CalcHeight(new GUIContent(GetText("archievementsText")), greenHeaderStyle.fixedHeight);
            Rect archievRect = new Rect(0, 10, rightRect.width, archiHeaderHeight);
            GUI.Label(archievRect, new GUIContent(GetText("archievementsText")), greenHeaderStyle);

            GUI.EndGroup();

            Vector2 returnSize = bigButtonStyle.CalcSize(new GUIContent(GetText("returnText")));
            Rect returnRect = new Rect(spyCardRect.width / 2 - returnSize.x - 20, leftRect.y + leftRect.height + 20, returnSize.x, returnSize.y);
            if (GUI.Button(returnRect, GetText("returnText"), bigButtonStyle))
            {
                MainGUI.SetState(MainGUI.States.JoinGame);
            }

            Vector2 newGameSize = bigButtonStyle.CalcSize(new GUIContent(GetText("newGameText")));
            Rect newGameRect = new Rect(spyCardRect.width / 2 + 20, returnRect.y, newGameSize.x + 20, newGameSize.y);
            if (GUI.Button(newGameRect, GetText("newGameText"), bigButtonStyle))
            {
                MainGUI.SetState(MainGUI.States.NewGame);
            }

            Vector2 shopSize = buttonStyle.CalcSize(new GUIContent(GetText("shopText")));
            Rect shopRect = new Rect(spyCardRect.width / 2 - shopSize.x - 20, newGameRect.y + newGameRect.height + 10, shopSize.x, shopSize.y);
            GUI.Button(shopRect, GetText("shopText"), buttonStyle);

            Vector2 leaderSize = buttonStyle.CalcSize(new GUIContent(GetText("leaderboardsText")));
            Rect leaderRect = new Rect(spyCardRect.width / 2 + 20, shopRect.y, leaderSize.x, leaderSize.y);
            GUI.Button(leaderRect, GetText("leaderboardsText"), buttonStyle);


            Vector2 backSize = smallButtonStyle.CalcSize(new GUIContent(GetText("backText")));
            Rect backRect = new Rect(20, spyCardRect.height - backSize.y - 20, backSize.x, backSize.y);
            if (GUI.Button(backRect, GetText("backText"), smallButtonStyle))
            {
                MainGUI.SetState(MainGUI.States.Login);
            }

            Vector2 logoutSize = smallButtonStyle.CalcSize(new GUIContent(GetText("logoutText")));
            Rect logoutRect = new Rect(spyCardRect.width - logoutSize.x - 20, backRect.y, logoutSize.x, logoutSize.y);
            if (GUI.Button(logoutRect, GetText("logoutText"), smallButtonStyle))
            {
                MainGUI.SetState(MainGUI.States.Login);
            }
            
            GUI.EndGroup();
        }
    }

    #endregion
}


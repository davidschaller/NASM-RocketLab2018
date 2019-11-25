#if Crypto
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoginGUI : GUIBase
{
    private float rectWidth = 280,
                  rectHeight = 165;

    /*
    public string headerText = "You Must Login to Play the Game:",
                  usernameText = "Username:",
                  passwordText = "Password:",
                  registerText = "Register",
                  loginText = "Login";
    */


    private float margin = 5,
                  textFieldWidth = 130;

    private GUIStyle boxStyle,
                     greenHeaderStyle,
                     labelStyle,
                     textFieldStyle,
                     buttonStyle,
                     introStyle;

    //private const string EMPTY_LOGIN_ERROR = "Login is empty";

    #region core functions

    new void Awake()
    {
        base.Awake();
        MainGUI.RegisterStateScript(MainGUI.States.Login, GetComponent<LoginGUI>());
        CryptoPlayerManager.Username = string.Empty;
        CryptoPlayerManager.Password = string.Empty;
    }

    private void SetGUIStyles()
    {
        if (SkinManager.IsActive)
        {
            boxStyle = SkinManager.Main.GetStyle("Box", "Begin");
            greenHeaderStyle = SkinManager.Main.GetStyle("GreenHeader", "Begin");
            labelStyle = SkinManager.Main.GetStyle("Label", "Begin");
            textFieldStyle = SkinManager.Main.GetStyle("textfield", "Begin");
            buttonStyle = SkinManager.Main.GetStyle("button", "Begin");
            introStyle = SkinManager.Main.GetStyle("Intro", "Begin");
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
        RenderDescriptionText();
        RenderLoginPanel();
    }

    #endregion

    #region rendering

    private void RenderLoginPanel()
    {
        if (boxStyle != null && greenHeaderStyle != null && labelStyle != null && textFieldStyle != null)
        {
            Rect loginRect = new Rect(Screen.width / 2 - rectWidth / 2, Screen.height - rectHeight - margin * 7, rectWidth, rectHeight);

            GUI.Box(loginRect, GUIContent.none, boxStyle);
            GUI.BeginGroup(loginRect);

            float headerHeight = greenHeaderStyle.CalcHeight(new GUIContent(GetText("header")), rectWidth);

            Rect headerRect = new Rect(0, 20, loginRect.width, headerHeight);
            GUI.Label(headerRect, new GUIContent(GetText("header")), greenHeaderStyle);

            Vector2 usernameSize = labelStyle.CalcSize(new GUIContent(GetText("username")));

            Rect usernameRect = new Rect(rectWidth / 2 - (textFieldWidth + usernameSize.x) / 2, headerRect.y + headerRect.height + margin, usernameSize.x, usernameSize.y);
            GUI.Label(usernameRect, GetText("username"), labelStyle);

			GUI.SetNextControlName ("Username");
			CryptoPlayerManager.Username = GUI.TextField(new Rect(usernameRect.x + usernameRect.width + margin, usernameRect.y, textFieldWidth, usernameRect.height), CryptoPlayerManager.Username, textFieldStyle);
            

            Rect passwordRect = new Rect(usernameRect.x, usernameRect.y + usernameRect.height + margin, usernameSize.x, usernameSize.y);
            GUI.Label(passwordRect, GetText("password"), labelStyle);

			GUI.SetNextControlName ("Password");
            CryptoPlayerManager.Password = GUI.TextField(new Rect(passwordRect.x + passwordRect.width + margin, passwordRect.y, textFieldWidth, passwordRect.height), CryptoPlayerManager.Password, textFieldStyle);

            Vector2 registerSize = buttonStyle.CalcSize(new GUIContent(GetText("register")));
            registerSize.x *= 1.5f;

            Rect registerRect = new Rect(rectWidth / 2 - registerSize.x - margin, rectHeight - registerSize.y - margin * 4, registerSize.x, registerSize.y);

            if (GUI.Button(registerRect, GetText("register"), buttonStyle))
            {
                
            }

            bool keyenter = Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Return);


	        if (keyenter || GUI.Button(new Rect(registerRect.x + registerRect.width + margin * 2, registerRect.y, registerSize.x, registerSize.y), GetText("login"), buttonStyle))
            {
                if (!string.IsNullOrEmpty(CryptoPlayerManager.Username))
                {
                    MainGUI.SetState(MainGUI.States.Home);
                }
                else
				{
#if Crypto
                	ModalController.Main.AddMessage(GetText("EMPTY_LOGIN_ERROR"),"OK",ModalProperties.Alignment.MiddleCenter,true,true,null);
                    //MessageGUI.ShowMessage(EMPTY_LOGIN_ERROR);
#endif
				}
            }

            GUI.EndGroup();
            if(GUI.GetNameOfFocusedControl()=="")
            	GUI.FocusControl("Username"); 
        }
    }
    
    private void RenderDescriptionText()
    {
        if (introStyle != null)
            GUI.DrawTexture(new Rect(50, 80, introStyle.fixedWidth, introStyle.fixedHeight), introStyle.normal.background);        
    }

    #endregion
}

#endif
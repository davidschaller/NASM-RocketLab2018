using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MainGUI : MonoBehaviour
{
    public enum States
    {
        Login,
        Home,
        NewGame,
        JoinGame
    }
	
	public enum RunMode
	{
		Deployment,
		Test,
	}


    public static States CurrentState { get; private set; }
    public static MainGUI Main { get; private set; }

    public RunMode runMode;

    private static Dictionary<States, MonoBehaviour> stateScripts = new Dictionary<States, MonoBehaviour>();
    //private static States lastState;

    private GUIStyle backgroundStyle,
                     upperLeftCornerStyle,
                     upperRightCornerStyle,
                     lowerLeftCornerStyle,
                     lowerRightCornerStyle,
                     mainHeaderStyle;

    #region core functions

    private void Awake()
    {
        if (Main == null)
            Main = this;
		
#if Crypto
        CryptoNet.SetMode(runMode);

        CryptoPlayerManager.RunMode = runMode;
#endif
    }

    private void SetGUIStyles()
    {
        if (SkinManager.IsActive)
        {
            backgroundStyle = SkinManager.Main.GetStyle("Background", "Begin");
            upperLeftCornerStyle = SkinManager.Main.GetStyle("UpperLeftCorner", "Begin");
            upperRightCornerStyle = SkinManager.Main.GetStyle("UpperRightCorner", "Begin");
            lowerLeftCornerStyle = SkinManager.Main.GetStyle("LowerLeftCorner", "Begin");
            lowerRightCornerStyle = SkinManager.Main.GetStyle("LowerRightCorner", "Begin");
            mainHeaderStyle = SkinManager.Main.GetStyle("MainHeader", "Begin");
        }
        else
            Debug.LogWarning("SkinManager is not active");
    }

    private void Start()
    {
        SetGUIStyles();

        SetState(States.Login);
    }

    private void OnGUI()
    {
        GUI.depth = 3;
        RenderBackground();
        RenderCorners();
        RenderTexts();
    }

    #endregion

    #region rendering

    private void RenderBackground()
    {
        if (backgroundStyle != null)
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), backgroundStyle.normal.background);
    }

    private void RenderCorners()
    {
        if (upperLeftCornerStyle != null)
            GUI.DrawTexture(new Rect(0, 0, upperLeftCornerStyle.fixedWidth, upperLeftCornerStyle.fixedHeight),
                upperLeftCornerStyle.normal.background);

        if (upperRightCornerStyle != null)
            GUI.DrawTexture(new Rect(Screen.width - upperRightCornerStyle.fixedWidth, 0, upperRightCornerStyle.fixedWidth, upperRightCornerStyle.fixedHeight),
                upperRightCornerStyle.normal.background);

        if (lowerLeftCornerStyle != null)
            GUI.DrawTexture(new Rect(0, Screen.height - lowerLeftCornerStyle.fixedHeight, lowerLeftCornerStyle.fixedWidth, lowerLeftCornerStyle.fixedHeight),
                lowerLeftCornerStyle.normal.background);

        if (lowerRightCornerStyle != null)
            GUI.DrawTexture(new Rect(Screen.width - lowerRightCornerStyle.fixedWidth, Screen.height - lowerRightCornerStyle.fixedHeight, lowerRightCornerStyle.fixedWidth, lowerRightCornerStyle.fixedHeight),
                lowerRightCornerStyle.normal.background);
    }

    private void RenderTexts()
    {
        if (mainHeaderStyle != null)
            GUI.DrawTexture(new Rect(200, 10, mainHeaderStyle.fixedWidth, mainHeaderStyle.fixedHeight), mainHeaderStyle.normal.background);
    }

    #endregion

    #region static functions

    public static void RegisterStateScript(States state, MonoBehaviour script)
    {
        if (!stateScripts.ContainsKey(state))
            stateScripts.Add(state, script);
        else
            stateScripts[state] = script;
    }

    public static void SetState(States newState)
    {
        if (CurrentState != newState && stateScripts.ContainsKey(CurrentState))
        {
            stateScripts[CurrentState].enabled = false;
        }

        //lastState = CurrentState;
        CurrentState = newState;

        if (stateScripts.ContainsKey(CurrentState))
        {
            if (stateScripts[CurrentState] != null)
            {
                stateScripts[CurrentState].enabled = true;
            }
        }
    }

    #endregion

}


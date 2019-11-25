using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BeginGUI : MonoBehaviour
{
    public GUISkin footerSkin,
                   buttonSkin,
                   hintSkin;

    public string non3DLink = "http://architectstudio3d.org/AS3d/design_studiolite.html";

    public Texture2D flwLarge;

    public float loadWindowWidth = 360,
                 loadWindowHeight = 220;

    public string headerText = "With Frank Lloyd Wright as your guide, design a house online:";
    public string postHeadertext = "Choose a client and a location, the apply your imagination to the challenges of architectural design. You can see your design take shape, then walk through a virtual 3D model of it. How well will your house satisfy the cliet's needs? How well does it suit its location? Submit your house to the design gallery so the world can see it - and rate it!";
    public string waitText = "Please wait while the Design Studio loads...";
    public string errorLoadingText = "Coudn't load the game. Try again";

    private enum BeginStates
    {
        main,
        begin,
        processing,
        error,
        hint
    }

    BeginStates beginState = BeginStates.main;
    private LoadAS3DManager oLoadAS3DManager;

    Rect flwRect, lineRect;

    float flwWidth, flwHeight, flwX;

    GameObject goBackground;

    private GUIStyle needsBoxStyle,
                     labelStyle,
                     headerStyle,
                     centeredHeaderStyle,
                     buttonStyle;

    void Awake()
    {
        enterContent = new GUIContent(enterText);
        non3DContent = new GUIContent(on3DVersionText);
        beginContent = new GUIContent(beginText);

        flwRect = new Rect(60, 211, flwLarge.width, flwLarge.height);
        flwWidth = flwRect.width;
        flwHeight = flwRect.height;
        flwX = flwRect.x;

        lineRect = new Rect(10, flwRect.y + flwRect.height, 577, 1);
        goBackground = GameObject.Find("Background");

        SetGUIStyles();
    }

    private void SetGUIStyles()
    {
        needsBoxStyle = hintSkin.GetStyle("needsbox");
        labelStyle = buttonSkin.GetStyle("label");
        headerStyle = hintSkin.GetStyle("label");

        centeredHeaderStyle = new GUIStyle(headerStyle);
        centeredHeaderStyle.alignment = TextAnchor.MiddleCenter;
        buttonStyle = buttonSkin.GetStyle("button");
    }

    private void Start()
    {
        oLoadAS3DManager = (LoadAS3DManager)GameObject.FindObjectOfType(typeof(LoadAS3DManager));
    }

    private void OnGUI()
    {
        switch (beginState)
        {
            case BeginStates.main:
                IntroText();
                ShowMain();
                break;
            case BeginStates.begin:
                IntroText();
                ShowBegin();
                break;
            case BeginStates.processing:
                IntroText();
                ShowProcessing();
                break;
            case BeginStates.error:
                IntroText();
                ShowError();
                break;
            case BeginStates.hint:
                ShowHint();
                break;
        }
    }

    private void ShowError()
    {
        Rect errorRect = new Rect(Screen.width / 2 - loadWindowWidth / 2, Screen.height / 2 - loadWindowHeight / 2, loadWindowWidth, loadWindowHeight);
        GUI.Box(errorRect, GUIContent.none, needsBoxStyle);

        Vector2 textSize = labelStyle.CalcSize(new GUIContent(errorLoadingText));

        GUI.BeginGroup(errorRect);
        GUI.Label(new Rect(errorRect.width / 2 - textSize.x / 2, 50, textSize.x, textSize.y), errorLoadingText, labelStyle);

        if (GUI.Button(new Rect(errorRect.width / 2 - 50, errorRect.height - 100, 100, 30), "OK", buttonStyle))
        {
            beginState = BeginStates.main;
        }

        GUI.EndGroup();
    }

    private int progress = 0;

    private void ShowProcessing()
    {
        Rect processingRect = new Rect(Screen.width / 2 - loadWindowWidth / 2, Screen.height / 2 - loadWindowHeight / 2, loadWindowWidth, loadWindowHeight);

        Rect insideProcessingRect = processingRect;
        insideProcessingRect.y += 50;
        insideProcessingRect.height -= 50;

        GUI.Box(processingRect, GUIContent.none, needsBoxStyle);
        GUILayout.BeginArea(insideProcessingRect);
        GUILayout.Label("Loading...", labelStyle);

        if (oLoadAS3DManager.RequestFinished)
        {
            if (LoadAS3DManager.oGameData != null)
            {
                if (LocationManager.PickedLocation == null)
                {
                    LocationManager.Init();
                    if (LoadAS3DManager.oGameData.Location >= 0)
                        LocationManager.PickedLocation = LocationManager.Locations[LoadAS3DManager.oGameData.Location];
                    else
                        LocationManager.PickedLocation = LocationManager.Locations[0];
                }

                if (LocationManager.StreamList[LoadAS3DManager.oGameData.Location] != null)
                {
                    LoadGame();
                }
                else
                {
                    if (!LocationManager.PickedLocation.IsDownloading() && !LocationManager.PickedLocation.IsDownloaded())
                    {
                        Debug.Log("Downloading started");
                        LocationManager.PickedLocation.Download();
                    }
                    else
                    {
                        progress = Mathf.RoundToInt(LocationManager.PickedLocation.GetDownloadProgress() * 100);

                        GUILayout.Label(string.Format("{0}%", progress), centeredHeaderStyle);

                        if (LocationManager.PickedLocation.IsDownloaded())
                        {
                            LocationManager.StreamList[LoadAS3DManager.oGameData.Location] = LocationManager.PickedLocation.Stream;
                            LoadGame();
                        }
                    }
                }
            }
            else
            {
                Debug.Log("Loading failed");
                beginState = BeginStates.error;
            }
        }
        else
            GUILayout.Label("Requesting game info...", labelStyle);

        GUILayout.EndArea();
    }

    private string enterText = "Enter Design Studio",
                   on3DVersionText = "Non-3D Version",
                   beginText = "Begin your design",
                   returningText = "Returning to a saved design? Enter your Design Code:",
                   confirmCode = "ENTER",
                   illbeHere = "I'll be here to help you out as you design your house";

    GUIContent enterContent,
               non3DContent,
               beginContent;

    private string key = string.Empty;

    private void ShowBegin()
    {
        GUI.DrawTexture(flwRect, flwLarge);

        GUIStyle footerStyle = footerSkin.GetStyle("bigbutton");
        GUIStyle buttonStyle = buttonSkin.GetStyle("button");
        GUIStyle labelStyle = footerSkin.GetStyle("label");
        GUIStyle textBoxStyleOriginal = buttonSkin.GetStyle("textfield");
        GUIStyle textBoxStyle = new GUIStyle(textBoxStyleOriginal);

        textBoxStyle.alignment = TextAnchor.MiddleRight;


        Rect beginButton = new Rect(500, 390, 140, footerStyle.CalcSize(beginContent).y);
        Rect returningLabel = new Rect(410, 450, 180, 60);
        Rect keyRect = new Rect(590, 455, 90, 23);
        Rect confirmCodeRect = new Rect(700, 455, 100, 23);

        // Begin your design
        if (GUI.Button(beginButton, beginContent, footerStyle))
        {
            beginState = BeginStates.hint;
        }

        GUI.Label(returningLabel, returningText, labelStyle);

        key = GUI.TextField(keyRect, key, 8, textBoxStyle);

        if (GUI.Button(confirmCodeRect, confirmCode, buttonStyle))
        {
            oLoadAS3DManager.JoinGame(key);
            beginState = BeginStates.processing;
        }
    }

    float delayTime = 0,
          resizeTime = 0;

    private void ShowHint()
    {
        if (goBackground.active == true)
            goBackground.active = false;

        GUI.DrawTexture(flwRect, flwLarge);

        if (delayTime < 3)
        {
            GUI.Label(new Rect(flwRect.x + flwRect.width - 30, flwRect.y + 40, 500, 30), illbeHere, buttonSkin.GetStyle("label"));
            delayTime += Time.deltaTime;
        }
        else if (flwRect.width > 120 && flwRect.height > 136)
        {
            flwRect.width = Mathf.Lerp(flwWidth, 120, resizeTime);
            flwRect.height = Mathf.Lerp(flwHeight, 136, resizeTime);
            flwRect.x = Mathf.Lerp(flwX, 10, resizeTime);
            flwRect.y = lineRect.y - flwRect.height;

            resizeTime += Time.deltaTime;
        }
        else
        {
            LoadGame();
        }
    }

    private void LoadGame()
    {
        if (Application.CanStreamedLevelBeLoaded("FloorPlans"))
        {
            Application.LoadLevel("FloorPlans");
        }

        Rect loadRect = new Rect(Screen.width / 2 - loadWindowWidth / 2, Screen.height / 2 - loadWindowHeight / 2,
                                        loadWindowWidth, loadWindowHeight);

        GUI.Box(loadRect, GUIContent.none, needsBoxStyle);
        GUI.BeginGroup(loadRect);
        

        Rect textRect = new Rect(20, 40, loadRect.width - 40, loadRect.y);
        GUI.Label(textRect, waitText, labelStyle);

        string percents = (Application.GetStreamProgressForLevel("FloorPlans") * 100).ToString() + "%";

        Vector2 textSize = needsBoxStyle.CalcSize(new GUIContent(percents));

        GUI.Label(new Rect(loadRect.width / 2 - textSize.x / 2 - 10, 100, 50, 30), percents, headerStyle);
        GUI.EndGroup();

    }

    private void ShowMain()
    {
        GUIStyle buttonStyle = footerSkin.GetStyle("bigbutton");

        GUI.skin = footerSkin;

        GUI.DrawTexture(flwRect, flwLarge);

        Rect enterButton = new Rect(500, 390, 140, buttonStyle.CalcSize(enterContent).y);
        Rect non3DButton = new Rect(550, 430, 120, buttonStyle.CalcSize(non3DContent).y);

        if (GUI.Button(enterButton, enterContent, buttonStyle))
        {
            key = string.Empty;
            beginState = BeginStates.begin;
        }

        if (GUI.Button(non3DButton, non3DContent))
        {
            Application.OpenURL(non3DLink);
        }
    }

    private void IntroText()
    {
        GUIStyle headerStyle = hintSkin.GetStyle("label");
        Vector2 headerSize = headerStyle.CalcSize(new GUIContent(headerText));
        GUI.Label(new Rect(280, 185, headerSize.x, headerSize.y), headerText, headerStyle);
        GUIStyle beginTextStyle = hintSkin.GetStyle("beginText");
        GUI.Label(new Rect(280, 80, headerSize.x, 400), postHeadertext, beginTextStyle);
    }
}

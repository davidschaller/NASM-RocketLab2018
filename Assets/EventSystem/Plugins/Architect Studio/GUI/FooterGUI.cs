using UnityEngine;
using System;

public class FooterGUI : MonoBehaviour
{
    public GUISkin footerSkin,
                   hintSkin,
                   buttonSkin;

    public Texture2D footerLogo,
                     footerLine;

    public string aboutSiteUrl = "http://architectstudio3d.org/AS3d/about_site.html",
                  forTeachersUrl = "http://architectstudio3d.org/AS3d/for_teachers.html",
                  relatedLinksUrl = "http://architectstudio3d.org/AS3d/related_links.html",
                  logoUrl = "http://www.gowright.org/";

    public string errorSaveMessage = "Couldn't save your design. Server return error";


    public float saveWindowWidth = 360,
                 saveWindowHeight = 220;

    private GUIStyle footerButton,
                     longFooterButton,
                     headerStyle,
                     centeredHeaderStyle,
                     needsTextStyle,
                     centerendNeedsText,
                     buttonStyle,
                     selectedButtonStyle;

    private SaveAS3DManager oSaveAS3DManager = null;

    private void Awake()
    {
        oSaveAS3DManager = (SaveAS3DManager)GameObject.FindObjectOfType(typeof(SaveAS3DManager));
        SetGUIStyles();
    }

    private void SetGUIStyles()
    {
        footerButton = footerSkin.GetStyle("button");
        longFooterButton = footerSkin.GetStyle("longButton");

        headerStyle = hintSkin.GetStyle("label");
        centeredHeaderStyle = new GUIStyle(headerStyle);
        centeredHeaderStyle.alignment = TextAnchor.MiddleCenter;

        needsTextStyle = hintSkin.GetStyle("needstext");
        centerendNeedsText = new GUIStyle(needsTextStyle);
        centerendNeedsText.alignment = TextAnchor.MiddleCenter;

        buttonStyle = buttonSkin.GetStyle("button");
        selectedButtonStyle = new GUIStyle(buttonStyle);
        selectedButtonStyle.normal = selectedButtonStyle.active;
    }

    private void Footer()
    {
        Vector2 flwSmallPos = new Vector2(10, Screen.height - 190),
                flwLinePos = new Vector2(flwSmallPos.x, flwSmallPos.y + 136);

        Common.DrawTextureAt(flwLinePos, footerLine);

        Vector2 logoLocation = new Vector2(Screen.width - footerLogo.width, Screen.height - footerLogo.height);

        if (GUI.Button(new Rect(logoLocation.x, logoLocation.y, footerLogo.width, footerLogo.height), footerLogo, GUIStyle.none))
        {
            Application.OpenURL(logoUrl);
        }

        GUILayout.BeginArea(new Rect(0,
                                      Screen.height - footerButton.normal.background.height - 5,
                                      Screen.width,
                                      footerButton.normal.background.height));
        GUILayout.BeginHorizontal();

        if (ArchitectStudioGUI.IsActive && ArchitectStudioGUI.Mode == ArchitectStudioGUI.modes.Design)
        {
            GUILayout.Space(20);
            if (GUILayout.Button("Save Your Design", footerButton))
            {
                if (oSaveAS3DManager.CurrentSaveState == SaveAS3DManager.SaveWindowState.hide)
                    oSaveAS3DManager.CurrentSaveState = SaveAS3DManager.SaveWindowState.show;
            }
            if (BuildingManager.Picked != null && BuildingManager.Picked.PickedRoof != null)
            {
                GUILayout.Space(20);
                if (GUILayout.Button("Print", footerButton))
                {
                    ShowPrintPreview();
                }
            }
        }

        if (ArchitectStudioGUI.Mode != ArchitectStudioGUI.modes.Print)
        {
            GUILayout.Space(20);
            if (GUILayout.Button("About This Site", footerButton))
            {
                Application.OpenURL(aboutSiteUrl);
            }
            GUILayout.Space(20);
            if (GUILayout.Button("For Teachers and Librarians", longFooterButton))
            {
                Application.OpenURL(forTeachersUrl);
            }
            GUILayout.Space(20);
            if (GUILayout.Button("Related Links", footerButton))
            {
                Application.OpenURL(relatedLinksUrl);
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    private void ShowPrintPreview()
    {
        CameraSwitcher.Is3DView = false;
        ArchitectStudioGUI.Mode = ArchitectStudioGUI.modes.Print;
        ArchitectStudioGUI.SetState(ArchitectStudioGUI.States.Interior);
    }

    private void OnGUI()
    {
        if (ArchitectStudioGUI.CurrentState < ArchitectStudioGUI.States.GalleryTour)
            Footer();

        if (oSaveAS3DManager != null && oSaveAS3DManager.CurrentSaveState != SaveAS3DManager.SaveWindowState.hide)
            SaveWindow();
    }

    private void SaveWindow()
    {
        Rect saveRect = new Rect(Screen.width / 2 - saveWindowWidth / 2, Screen.height / 2 - saveWindowHeight / 2,
                                        saveWindowWidth, saveWindowHeight);

        GUI.Box(saveRect, GUIContent.none, hintSkin.GetStyle("needsbox"));

        Rect insideSaveRect = new Rect(saveRect.x + saveRect.width / 4,
            saveRect.y + saveRect.height / 4, saveRect.width / 2, saveRect.height / 1.5f);

        GUILayout.BeginArea(insideSaveRect);
        GUILayout.BeginVertical();

        switch (oSaveAS3DManager.CurrentSaveState)
        {
            case SaveAS3DManager.SaveWindowState.show:
                GUILayout.Label("Save Your Design", centeredHeaderStyle);
                GUILayout.Label("You can save your house design\nand return later to work it more",
                    centerendNeedsText);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("SAVE", buttonSkin.GetStyle("button"), GUILayout.Width(80)))
                {
                    if (LoadAS3DManager.oGameData == null || LoadAS3DManager.oGameData.GameId < 0)
                    {
                        // Register new game
                        oSaveAS3DManager.RegisterNewGame(new Info(string.Empty, 0, 0, DateTime.Now, string.Empty, string.Empty));
                    }
                    oSaveAS3DManager.CurrentSaveState = SaveAS3DManager.SaveWindowState.processing;

                    oSaveAS3DManager.Save();
                }
                if (GUILayout.Button("CANCEL", buttonSkin.GetStyle("button"), GUILayout.Width(80)))
                {
                    oSaveAS3DManager.CurrentSaveState = SaveAS3DManager.SaveWindowState.hide;
                }
                GUILayout.EndHorizontal();
                break;
            case SaveAS3DManager.SaveWindowState.processing:
                if (oSaveAS3DManager.RequestFinished)
                    oSaveAS3DManager.CurrentSaveState = SaveAS3DManager.SaveWindowState.success;

                GUILayout.Label("Please wait while we save your design", centeredHeaderStyle);
                break;
            case SaveAS3DManager.SaveWindowState.error:
                GUILayout.Label(errorSaveMessage, centeredHeaderStyle);
                if (GUILayout.Button("Ok", buttonSkin.GetStyle("button")))
                {
                    oSaveAS3DManager.CurrentSaveState = SaveAS3DManager.SaveWindowState.hide;
                }
                break;
            case SaveAS3DManager.SaveWindowState.success:
                if (LoadAS3DManager.oGameData != null)
                {
                    GUILayout.Label("Your design", centerendNeedsText);
                    GUILayout.Label("has been saved.", centerendNeedsText);
                    GUILayout.Label("Your Design Code is:", centerendNeedsText);
                    GUILayout.TextField(LoadAS3DManager.oGameData.SecurityKey, centeredHeaderStyle);
                    GUILayout.Label("Write down it - you'll need it to", centerendNeedsText);
                    GUILayout.Label("work on your design later", centerendNeedsText);

                    Rect okRect = new Rect(insideSaveRect.width / 2 - 40,
                            insideSaveRect.height - 26, 80, 26);

                    if (GUI.Button(okRect, "OK", buttonSkin.GetStyle("button")))
                    {
                        oSaveAS3DManager.CurrentSaveState = SaveAS3DManager.SaveWindowState.hide;
                    }
                }
                else
                    oSaveAS3DManager.CurrentSaveState = SaveAS3DManager.SaveWindowState.error;

                break;
            default:
                oSaveAS3DManager.CurrentSaveState = SaveAS3DManager.SaveWindowState.hide;
                break;
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIGeneralController : GeneralControllerBase
{
    const string COMPLETE_BKG = "complete-bkg",
                 LAUNCH_BUTTON = "launch-button",
                 ENTER_NAME_BUTTON = "enter-name-button",
                 COMPLETE_TEXTFIELD = "textfield";

    public GUISkin guiSkin;

    GUIStyle launchButtonStyle,
             enterNameButtonStyle,
             completeBkgStyle,
             completeTextFieldStyle;

    public AudioClip clickSound;

    public Vector2 consoleSize = new Vector2(800, 300),
                   completeSize = new Vector2(400, 139),
                   closeCompareButtonSize = new Vector2(200, 30);

    public Rect tabRect = new Rect(350, 20, 600, 50),
                buttonsRect = new Rect(70, 90, 150, 200),
                screenRect = new Rect(250, 100, 500, 250),
                componentRect = new Rect(580, 100, 150, 150),
                compareMyRocketRect = new Rect(50, 50, 400, 500);

    public float propertyWidth = 100,
                 sliderWidth = 150,
                 chooseButtonWidth = 250,
                 rocketNameTextFiledWidth = 150;

    public float completeMarginButton = 50,
                 completePaddingTop = 65,
                 completeEnterNamePaddingLeft = 130,
                 completeSpaceAfterName = 30;

    string rocketName = string.Empty;

    RenderTexture rtex,
                  rtexCompare;
    public Camera ComponentCamera,
                  compareCamera;

    private RocketComponentsController.RocketConfig Rocket;
    private RealRocket RealRocket;

    public override void ShowComparedRocket(RocketComponentsController.RocketConfig inRocket, RealRocket inRealRocket, GetComponentTypeText typeTextCallback, GetComponentNameText nameTextCallback, bool isAlert)
    {
        Rocket = inRocket;
        RealRocket = inRealRocket;

        if (compareCamera)
        {
            //ComponentCamera.enabled = false;

            compareCamera.enabled = true;
            compareCamera.targetTexture = rtexCompare;
            compareCamera.Render();
        }
    }

    void OnEnable()
    {
        if (guiSkin)
        {
            launchButtonStyle = guiSkin.GetStyle(LAUNCH_BUTTON);
            enterNameButtonStyle = guiSkin.GetStyle(ENTER_NAME_BUTTON);
            completeBkgStyle = guiSkin.GetStyle(COMPLETE_BKG);
            completeTextFieldStyle = guiSkin.GetStyle(COMPLETE_TEXTFIELD);
        }

        if (xml)
        {
            enterText = DialogXMLParser.GetText(xml.text, enterButtonSection, ActiveLanguage.English);
            compareButtonText = DialogXMLParser.GetText(xml.text, compareButtonSection, ActiveLanguage.English);
            launchButtonText = DialogXMLParser.GetText(xml.text, prepareForLaunchButtonSection, ActiveLanguage.English);
            completeButtonText = DialogXMLParser.GetText(xml.text, completeButtonSection, ActiveLanguage.English);
            closecompareButtonText = DialogXMLParser.GetText(xml.text, closecompareButtonSection, ActiveLanguage.English);
            compareTitleText = DialogXMLParser.GetText(xml.text, compareTitleSection, ActiveLanguage.English);
        }

		rtex = new RenderTexture(512, 512, 32);
		RenderTexture.active = rtex;

        rtexCompare = new RenderTexture(512, 512, 32);
        RenderTexture.active = rtexCompare;
    }

    bool renderComparison = false,
         renderNameRocket = false;

    void OnGUI()
    {
        if (guiSkin)
        {
            if (renderComparison)
                RenderComparison();

            if (renderNameRocket)
                RenderComplete();
        }
    }

    void RenderComparison()
    {
        GUI.Box(compareMyRocketRect, GUIContent.none);
        //Rect closeComparisonButtonRect = new Rect(Screen.width / 2 - closeCompareButtonSize.x / 2, Screen.height - closeCompareButtonSize.y, closeCompareButtonSize.x, closeCompareButtonSize.y);

        GUILayout.BeginArea(compareMyRocketRect);
        GUILayout.Label(rocketName);
        GUILayout.Box(rtexCompare, GUILayout.Width(compareMyRocketRect.width / 3), GUILayout.Height(compareMyRocketRect.height));

        RenderComparisonCenter();

        GUILayout.BeginArea(new Rect(2 * compareMyRocketRect.width / 3, 0, compareMyRocketRect.width / 3, compareMyRocketRect.height));
        GUILayout.Label(RealRocket == null ? "" : RealRocket.NameText);
        GUILayout.Box(RealRocket == null ? null : RealRocket.Img, GUILayout.Width(compareMyRocketRect.width / 3), GUILayout.Height(compareMyRocketRect.height));
        GUILayout.EndArea();


        GUILayout.EndArea();
    }

    void RenderComparisonCenter()
    {
        GUILayout.BeginArea(new Rect(compareMyRocketRect.width / 3, 0, compareMyRocketRect.width / 3, compareMyRocketRect.height));
        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        GUILayout.Label(compareTitleText);
        GUILayout.EndHorizontal();
        
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Space(80);
        GUILayout.Label(getComponentTypeText(RocketComponentBase.ComponentType.Propellant));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label(getComponentNameText(Rocket.pickedPropellant));
        GUILayout.Label(getComponentNameText(RealRocket.pickedPropellant));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(80);
        GUILayout.Label(getComponentTypeText(RocketComponentBase.ComponentType.Control));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label(getComponentNameText(Rocket.pickedControl));
        GUILayout.Label(getComponentNameText(RealRocket.pickedControl));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(80);
        GUILayout.Label(getComponentTypeText(RocketComponentBase.ComponentType.Shape));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label(getComponentNameText(Rocket.pickedShape));
        GUILayout.Label(getComponentNameText(RealRocket.pickedShape));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(80);
        GUILayout.Label(getComponentTypeText(RocketComponentBase.ComponentType.Stages));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label(getComponentNameText(Rocket.pickedStage));
        GUILayout.Label(getComponentNameText(RealRocket.pickedStage));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(80);
        GUILayout.Label(RealRocket.DescriptionTitle);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label(RealRocket.Description);
        GUILayout.EndHorizontal();

        if (GUILayout.Button(closecompareButtonText, launchButtonStyle))
        {
            PlayTap();
        }
        GUILayout.EndArea();
    }

    void RenderComplete()
    {
        Rect completeRect = new Rect(Screen.width / 2 - completeSize.x / 2, Screen.height - completeSize.y - completeMarginButton, completeSize.x, completeSize.y);

        GUI.Box(completeRect, GUIContent.none, completeBkgStyle);

        GUILayout.BeginArea(new Rect(completeRect.x, completeRect.y, completeRect.width, completeRect.height + completeMarginButton));
        {
            GUILayout.Space(completePaddingTop);
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(completeEnterNamePaddingLeft);
                rocketName = GUILayout.TextField(rocketName, completeTextFieldStyle, GUILayout.Width(rocketNameTextFiledWidth));
                if (GUILayout.Button(enterText, enterNameButtonStyle))
                {
                    if (confirmRocketName != null)
                    {
                        confirmRocketName(rocketName);
                    }

                    PlayTap();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(completeSpaceAfterName);

            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(compareButtonText, launchButtonStyle))
                {
                    if (compareRocketsCallback != null)
                    {
                        compareRocketsCallback();
                    }
                    PlayTap();
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(launchButtonText, launchButtonStyle))
                {
                    if (prepareForLaunchCallback != null)
                    {
                        prepareForLaunchCallback();
                    }
                    PlayTap();
                } 
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndArea();
    }

    void PlayTap()
    {
        if (clickSound)
        {
            if (!GetComponent<AudioSource>())
                gameObject.AddComponent<AudioSource>();

            GetComponent<AudioSource>().PlayOneShot(clickSound);
        }
    }

    public override void ShowRocketNamePanel(GeneralControllerBase.ConfirmRocketName nameCallback, ControllerOfGUIBase.SimpleCallback compareCallback, ControllerOfGUIBase.SimpleCallback launchCallback, ControllerOfGUIBase.SimpleCallback backCallback, SimpleCallback compareBackCallback)
    {
        throw new System.NotImplementedException();
    }

    public override void HideNamePanel()
    {
        throw new System.NotImplementedException();
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIConsoleController : ConsoleControllerBase
{
    const string CONSOLE_TAB = "console-tab",
                 CONSOLE_BUTTON = "console-button",
                 CONSOLE_BKG = "console-bkg",
                 SCREEN_HEADER = "screen-header",
                 SCREEN_DESCRIPTION = "screen-description",
                 SCREEN_PROPERTY = "screen-property",
                 CONSOLE_CHOOSE_BUTTON = "screen-choose-button",
                 COMPLETE_BKG = "complete-bkg",
                 LAUNCH_BUTTON = "launch-button",
                 ENTER_NAME_BUTTON = "enter-name-button",
                 COMPLETE_TEXTFIELD = "textfield";

    public GUISkin guiSkin;

    GUIStyle consoleTabStyle,
             consoleTabActiveStyle,
             consoleButtonStyle,
             consoleButtonActiveStyle,
             consoleButtonPickedStyle,
             consoleButtonPickedAndActiveStyle,
             consoleBkgStyle;

    GUIStyle screenHeaderStyle,
             screenDescriptionStyle,
             screenPropertyStyle,
             chooseButtonStyle;

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

    RenderTexture rtex;

    public Camera ComponentCamera;

    private RocketComponentsController.RocketConfig Rocket;
    private RealRocket RealRocket;

    public override void SetState(ConsoleControllerBase.ConsoleStates newState)
    {
        ConsoleState = newState;

        if (activeTab == null)
        {
            SwitchTab(currentTabs[0]);
        }

        if (newState == ConsoleStates.Play || newState == ConsoleStates.CanBeComplete)
        {
            if (ComponentCamera)
            {
                ComponentCamera.targetTexture = rtex;
                ComponentCamera.Render();
            }
        }
    }

    void OnEnable()
    {
        if (guiSkin)
        {
            consoleTabStyle = guiSkin.GetStyle(CONSOLE_TAB);
            consoleTabActiveStyle = new GUIStyle(consoleTabStyle);
            consoleTabActiveStyle.normal = consoleTabStyle.active;
            consoleTabActiveStyle.hover = consoleTabStyle.active;

            consoleButtonStyle = guiSkin.GetStyle(CONSOLE_BUTTON);
            consoleButtonActiveStyle = new GUIStyle(consoleButtonStyle);
            consoleButtonActiveStyle.normal = consoleButtonStyle.active;
            consoleButtonActiveStyle.hover = consoleButtonStyle.active;

            consoleButtonPickedStyle = new GUIStyle(consoleButtonStyle);
            consoleButtonPickedStyle.normal = consoleButtonStyle.onNormal;
            consoleButtonPickedStyle.hover = consoleButtonStyle.onHover;
            consoleButtonPickedStyle.active = consoleButtonStyle.onActive;

            consoleButtonPickedAndActiveStyle = new GUIStyle(consoleButtonStyle);
            consoleButtonPickedAndActiveStyle.normal = consoleButtonStyle.onActive;
            consoleButtonPickedAndActiveStyle.hover = consoleButtonStyle.onActive;
            consoleButtonPickedAndActiveStyle.active = consoleButtonStyle.onActive;

            consoleBkgStyle = guiSkin.GetStyle(CONSOLE_BKG);

            screenHeaderStyle = guiSkin.GetStyle(SCREEN_HEADER);
            screenDescriptionStyle = guiSkin.GetStyle(SCREEN_DESCRIPTION);
            screenPropertyStyle = guiSkin.GetStyle(SCREEN_PROPERTY);

            chooseButtonStyle = guiSkin.GetStyle(CONSOLE_CHOOSE_BUTTON);

            launchButtonStyle = guiSkin.GetStyle(LAUNCH_BUTTON);
            enterNameButtonStyle = guiSkin.GetStyle(ENTER_NAME_BUTTON);
            completeBkgStyle = guiSkin.GetStyle(COMPLETE_BKG);
            completeTextFieldStyle = guiSkin.GetStyle(COMPLETE_TEXTFIELD);
        }

        if (xml)
        {
            chooseButtonText = DialogXMLParser.GetText(xml.text, chooseButtonSection, ActiveLanguage.English);
            nextTabText = DialogXMLParser.GetText(xml.text, nextTabButtonSection, ActiveLanguage.English);
            enterText = DialogXMLParser.GetText(xml.text, enterButtonSection, ActiveLanguage.English);
            compareButtonText = DialogXMLParser.GetText(xml.text, compareButtonSection, ActiveLanguage.English);
            launchButtonText = DialogXMLParser.GetText(xml.text, prepareForLaunchButtonSection, ActiveLanguage.English);
            completeButtonText = DialogXMLParser.GetText(xml.text, completeButtonSection, ActiveLanguage.English);
            closecompareButtonText = DialogXMLParser.GetText(xml.text, closecompareButtonSection, ActiveLanguage.English);
            compareTitleText = DialogXMLParser.GetText(xml.text, compareTitleSection, ActiveLanguage.English);
        }

		rtex = new RenderTexture(512, 512, 32);
		RenderTexture.active = rtex;
    }

    void OnGUI()
    {
        if (guiSkin)
        {
            RenderMainConsole();
        }
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
        GUILayout.Label(GetComponentTypeText(RocketComponentBase.ComponentType.Propellant));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label(GetComponentNameText(Rocket.pickedPropellant));
        GUILayout.Label(GetComponentNameText(RealRocket.pickedPropellant));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(80);
        GUILayout.Label(GetComponentTypeText(RocketComponentBase.ComponentType.Control));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label(GetComponentNameText(Rocket.pickedControl));
        GUILayout.Label(GetComponentNameText(RealRocket.pickedControl));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(80);
        GUILayout.Label(GetComponentTypeText(RocketComponentBase.ComponentType.Shape));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label(GetComponentNameText(Rocket.pickedShape));
        GUILayout.Label(GetComponentNameText(RealRocket.pickedShape));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(80);
        GUILayout.Label(GetComponentTypeText(RocketComponentBase.ComponentType.Stages));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label(GetComponentNameText(Rocket.pickedStage));
        GUILayout.Label(GetComponentNameText(RealRocket.pickedStage));
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
            if (completeRocketDesignClick != null)
                completeRocketDesignClick();

            PlayTap();
        }
        GUILayout.EndArea();
    }

    void RenderMainConsole()
    {
        GUI.enabled = isClickable;

        Rect consoleRect = new Rect(Screen.width / 2 - consoleSize.x / 2, Screen.height - consoleSize.y, consoleSize.x, consoleSize.y);

        GUI.Box(consoleRect, GUIContent.none, consoleBkgStyle);

        GUI.BeginGroup(consoleRect);
        {
            RenderTabs();
            RenderButtons();
            RenderScreen();
			RenderComponent();
        }
        GUI.EndGroup();

        GUI.enabled = true;
    }

    void RenderTabs()
    {
        GUILayout.BeginArea(tabRect);
        {
            GUILayout.BeginHorizontal();
            {
                foreach (TabDetails tabDetails in currentTabs)
                {
                    if (GUILayout.Button(tabDetails.componentTypeText, activeTab == tabDetails ? consoleTabActiveStyle : consoleTabStyle))
                    {
                        if (activeTab != tabDetails)
                        {
                            SwitchTab(tabDetails);
                        }
                    }
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndArea();
    }

    void RenderButtons()
    {
        if (activeTab != null && activeTab.activeButton != null)
        {
            GUILayout.BeginArea(buttonsRect);
            {
                foreach (ButtonDetails button in activeTab.buttons)
                {
                    GUIStyle currentButtonStyle = consoleButtonStyle;

                    if (button.Picked)
                    {
                        if (activeTab.activeButton == button)
                        {
                            currentButtonStyle = consoleButtonPickedAndActiveStyle;
                        }
                        else
                        {
                            currentButtonStyle = consoleButtonPickedStyle;
                        }
                    }
                    else if (activeTab.activeButton == button)
                    {
                        currentButtonStyle = consoleButtonActiveStyle;
                    }

                    if (GUILayout.Button(button.componentNameText, currentButtonStyle))
                    {
                        if (activeTab.activeButton != button)
                        {
                            activeTab.activeButton = button;

                            if (buttonClickCallback != null)
                            {
                                buttonClickCallback(button, false);
                            }

                            PlayTap();
                        }
                    }
                }
            }
            GUILayout.EndArea();
        }
    }

    void RenderScreen()
    {
        if (activeTab != null && activeTab.activeButton != null && activeTab.activeButton.screen != null)
        {
            GUILayout.BeginArea(screenRect);
            {
                GUILayout.Label(activeTab.activeButton.screen.header, screenHeaderStyle);
                GUILayout.Label(activeTab.activeButton.screen.description, screenDescriptionStyle);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(activeTab.activeButton.screen.costName, screenPropertyStyle, GUILayout.Width(propertyWidth));
                    GUILayout.HorizontalSlider(activeTab.activeButton.screen.cost, 0, 10, GUILayout.Width(sliderWidth));
                }
                GUILayout.EndHorizontal();

                if (activeTab.activeButton.screen.property1 > 0)
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(activeTab.activeButton.screen.property1Name, screenPropertyStyle, GUILayout.Width(propertyWidth));
                        GUILayout.HorizontalSlider(activeTab.activeButton.screen.property1, 0, 10, GUILayout.Width(sliderWidth));

                        if (activeTab.activeButton.screen.improve > 0)
                        {
                            GUILayout.Label("+" + activeTab.activeButton.screen.improve, screenPropertyStyle, GUILayout.Width(propertyWidth));
                        }
                    }
                    GUILayout.EndHorizontal();
                }

                if (activeTab.activeButton.screen.property2 > 0)
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(activeTab.activeButton.screen.property2Name, screenPropertyStyle, GUILayout.Width(propertyWidth));
                        GUILayout.HorizontalSlider(activeTab.activeButton.screen.property2, 0, 10, GUILayout.Width(sliderWidth));
                    }
                    GUILayout.EndHorizontal();
                }

                GUILayout.FlexibleSpace();

                GUILayout.BeginHorizontal();
                {
                    if (activeTab.activeButton.Picked)
                    {
                        ConsoleControllerBase.TabDetails nextTab = GetNextTab();

                        if (nextTab.componentType != activeTab.componentType)
                        {
                            if (GUILayout.Button(nextTabText, chooseButtonStyle, GUILayout.Width(chooseButtonWidth)))
                            {
                                NextTab();
                                PlayTap();
                            }
                        }
                    }
                    else
                    {
                        if (GUILayout.Button(chooseButtonText + activeTab.activeButton.componentNameText, chooseButtonStyle, GUILayout.Width(chooseButtonWidth)))
                        {
                            if (buttonClickCallback != null)
                            {
                                buttonClickCallback(activeTab.activeButton, true);

                                PlayTap();
                            }
                        }
                    }

                    if (ConsoleState == ConsoleStates.CanBeComplete)
                    {
                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button(completeButtonText, launchButtonStyle))
                        {
                            if (completeRocketDesignClick != null)
                            {
                                completeRocketDesignClick();
                            }

                            PlayTap();
                        }
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.FlexibleSpace();
				
            }
            GUILayout.EndArea();
        }
        else
            Debug.LogError("Nothing to render. This should not happen");
    }

    void RenderComponent()
    {
        GUI.Box(componentRect, rtex);
    }
	
    void SwitchTab(TabDetails to)
    {
        activeTab = to;

        if (tabClickCallback != null)
        {
            tabClickCallback(to, false);
        }

        if (to.buttons != null)
        {
            if (to.activeButton != null)
            {
                buttonClickCallback(to.activeButton, false);
            }
            else
            {
                buttonClickCallback(to.buttons[0], false);
            }
        }
        else
            Debug.LogError("Buttons are NULL, That shouldn't be");

        PlayTap();
    }

    void NextTab()
    {
        ConsoleControllerBase.TabDetails nextTab = GetNextTab();

        SwitchTab(nextTab);
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

    public override void InitTabs(Dictionary<RocketComponentBase.ComponentType, string> tabs, TabClickCallback callback, SimpleCallback completeCallback)
    {
        currentTabs = new List<TabDetails>();
        foreach (KeyValuePair<RocketComponentBase.ComponentType, string> pair in tabs)
        {
            currentTabs.Add(new TabDetails()
            {
                activeButton = null,
                buttons = null,
                componentType = pair.Key,
                componentTypeText = pair.Value
            });
        }

        tabClickCallback = callback;
        completeRocketDesignClick = completeCallback;
    }

    public override void InitButtons(Dictionary<RocketComponentBase.ComponentName, string> names, ConsoleControllerBase.TabDetails forTab,  ConsoleControllerBase.ButtonClickCallback callback, ConsoleControllerBase.ButtonClickCallback afterCallback)
    {
        foreach (ConsoleControllerBase.TabDetails item in currentTabs)
        {
            if (forTab == item)
                item.InitButtons(names);
        }

        buttonClickCallback = callback;
        afterButtonClickCallback = afterCallback;
    }

    public override void ConfirmSelection(RocketComponentBase.ComponentType componentType, RocketComponentBase.ComponentName componentName)
    {
        foreach (TabDetails tabItem in currentTabs)
        {
            if (tabItem.componentType == componentType)
            {
                foreach (ButtonDetails buttonItem in tabItem.buttons)
                {
                    buttonItem.Picked = buttonItem.componentName == componentName;
                }
                break;
            }
        }

        NextTab();
    }

    bool isClickable = true;
    public override void ToggleClickable(bool state)
    {
        isClickable = state;
    }

    public override void SetCameraTo(Transform transform, bool fast)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdatePanelButtonState(RocketComponentsController.RocketConfig Rocket_Config)
    {
    }
}

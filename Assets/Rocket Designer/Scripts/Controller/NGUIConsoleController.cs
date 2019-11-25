using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class NGUIConsoleController : ConsoleControllerBase
{
    public AudioClip tabClickAudio,
                     buttonAudioClip;

    public bool disableInactivePanels = false;

    public class NGUIConsoleDetails
    {
        public RocketComponentBase.ComponentType subPanelType = RocketComponentBase.ComponentType.Propellant;
        public Transform trPanel,
                         trDisplay;

        public class NGUITabButton
        {
            public UIImageButton tab;
            public RocketComponentBase.ComponentType tabType;

            public void ToggleTab(bool status)
            {
                tab.GetComponent<Collider>().enabled = status;

                if (!status)
                {
                    tab.OnPress(true);
                }
            }
        }

        public NGUITabButton[] tabs;

        public NGUIButton[] buttons;
        public class NGUIButton
        {
            public UIImageButton button;
            public RocketComponentBase.ComponentName componentName = RocketComponentBase.ComponentName.None;

            public void ToggleComponentButton(bool picked)
            {
                button.GetComponent<Collider>().enabled = !picked;
            }

            public bool subscribed = false;
        }


        public UIImageButton chooseButton,
                             finishButton;
        public UISprite componentImagePlaceholder;
        public UITexture componentPlaceholder;

        public UILabel title,
                       description,
                       chooseButtonLabel;

        public Transform cameraPlaceholder;

        public Meters[] meterLabels;
        public class Meters
        {
            public UILabel label;
            public UISlider slider;

            public void ToggleMeter(bool state)
            {
                label.gameObject.SetActive(state);
                slider.gameObject.SetActive(state);
            }
        }

        public NGUIConsoleDetails(Transform consoleItem, AudioClip buttonClickClip, bool disableInactive)
        {
            trPanel = consoleItem;

            meterLabels = new Meters[3] { new Meters(), new Meters(), new Meters() };
            buttons = new NGUIButton[3] { new NGUIButton(), new NGUIButton(), new NGUIButton() };
            tabs = new NGUITabButton[4] { new NGUITabButton(), new NGUITabButton(), new NGUITabButton(), new NGUITabButton() };

            foreach (Transform child in consoleItem)
            {
                switch (child.name)
                {
                    case "Button: Propellant":
                        tabs[0].tab = RocketDesignerCommon.MakeButton(child, buttonClickClip);
                        tabs[0].tabType = RocketComponentBase.ComponentType.Propellant;
                        break;
                    case "Button: Controls":
                        tabs[1].tab = RocketDesignerCommon.MakeButton(child, buttonClickClip);
                        tabs[1].tabType = RocketComponentBase.ComponentType.Control;
                        break;
                    case "Button: stages":
                        tabs[2].tab = RocketDesignerCommon.MakeButton(child, buttonClickClip);
                        tabs[2].tabType = RocketComponentBase.ComponentType.Stages;
                        break;
                    case "Button: Shape":
                        tabs[3].tab = RocketDesignerCommon.MakeButton(child, buttonClickClip);
                        tabs[3].tabType = RocketComponentBase.ComponentType.Shape;
                        break;
                    case "Camera Placeholder":
                        cameraPlaceholder = child;
                        break;
                }
            }

            switch (consoleItem.name)
            {
                case "Control Console Panel":
                    subPanelType = RocketComponentBase.ComponentType.Control;

                    foreach (Transform child in consoleItem)
                    {
                        switch (child.name)
                        {
                            case "Button: controls: exhaustvanes":
                                buttons[0].button = RocketDesignerCommon.MakeButton(child, buttonClickClip);
                                buttons[0].componentName = RocketComponentBase.ComponentName.ExhaustVanes;
                                break;
                            case "Button: controls: gimbaled":
                                buttons[1].button = RocketDesignerCommon.MakeButton(child, buttonClickClip);
                                buttons[1].componentName = RocketComponentBase.ComponentName.GimbaledEngines;
                                break;
                            case "Button: controls: moveablefins":
                                buttons[2].button = RocketDesignerCommon.MakeButton(child, buttonClickClip);
                                buttons[2].componentName = RocketComponentBase.ComponentName.MoveableFins;
                                break;
                        }
                    }

                    foreach (Transform item in consoleItem.parent)
                    {
                        if (item.name == "Control Display Screen Panel")
                        {
                            SetItemsFrom(item, buttonClickClip);
                            trDisplay = item;
                            break;
                        }
                    }
                    break;
                case "Propellant Console Panel":
                    subPanelType = RocketComponentBase.ComponentType.Propellant;

                    foreach (Transform child in consoleItem)
                    {
                        switch (child.name)
                        {
                            case "Button: Propel: kerosene":
                                buttons[0].button = RocketDesignerCommon.MakeButton(child, buttonClickClip);
                                buttons[0].componentName = RocketComponentBase.ComponentName.Kerosene;
                                break;
                            case "Button: Propel: liqnitrogen":
                                buttons[1].button = RocketDesignerCommon.MakeButton(child, buttonClickClip);
                                buttons[1].componentName = RocketComponentBase.ComponentName.LiquidHydrogen;
                                break;
                            case "Button: Propel: solidfuel":
                                buttons[2].button = RocketDesignerCommon.MakeButton(child, buttonClickClip);
                                buttons[2].componentName = RocketComponentBase.ComponentName.SolidFuel;
                                break;
                        }
                    }

                    foreach (Transform item in consoleItem.parent)
                    {
                        if (item.name == "Propellant Display Screen Panel")
                        {
                            SetItemsFrom(item, buttonClickClip);
                            trDisplay = item;
                            break;
                        }
                    }
                    break;
                case "Stages Console Panel":
                    subPanelType = RocketComponentBase.ComponentType.Stages;

                    foreach (Transform child in consoleItem)
                    {
                        switch (child.name)
                        {
                            case "Button: stages: onestage":
                                buttons[0].button = RocketDesignerCommon.MakeButton(child, buttonClickClip);
                                buttons[0].componentName = RocketComponentBase.ComponentName.OneStage;
                                break;
                            case "Button: stages: twostage":
                                buttons[1].button = RocketDesignerCommon.MakeButton(child, buttonClickClip);
                                buttons[1].componentName = RocketComponentBase.ComponentName.TwoStage;
                                break;
                            case "Button: stages: threestage":
                                buttons[2].button = RocketDesignerCommon.MakeButton(child, buttonClickClip);
                                buttons[2].componentName = RocketComponentBase.ComponentName.ThreeStage;
                                break;
                        }
                    }

                    foreach (Transform item in consoleItem.parent)
                    {
                        if (item.name == "Stages Display Screen Panel")
                        {
                            SetItemsFrom(item, buttonClickClip);
                            trDisplay = item;
                            break;
                        }
                    }
                    break;
                case "Shape Console Panel":
                    subPanelType = RocketComponentBase.ComponentType.Shape;

                    foreach (Transform child in consoleItem)
                    {
                        switch (child.name)
                        {
                            case "Button: shape: cylinderical":
                                buttons[0].button = RocketDesignerCommon.MakeButton(child, buttonClickClip);
                                buttons[0].componentName = RocketComponentBase.ComponentName.Cylindrical;
                                break;
                            case "Button: shape: flared":
                                buttons[1].button = RocketDesignerCommon.MakeButton(child, buttonClickClip);
                                buttons[1].componentName = RocketComponentBase.ComponentName.Flared;
                                break;
                            case "Button: shape: tapered":
                                buttons[2].button = RocketDesignerCommon.MakeButton(child, buttonClickClip);
                                buttons[2].componentName = RocketComponentBase.ComponentName.Tapered;
                                break;
                        }
                    }

                    foreach (Transform item in consoleItem.parent)
                    {
                        if (item.name == "Shape Display Screen Panel")
                        {
                            SetItemsFrom(item, buttonClickClip);
                            trDisplay = item;
                            break;
                        }
                    }
                    break;
            }

            foreach (NGUITabButton tab in tabs)
            {
                if (tab.tabType == subPanelType)
                {
                    tab.ToggleTab(false);
                }
            }

            Toggle(disableInactive, false);
        }

        void SetItemsFrom(Transform item, AudioClip buttonClickClip)
        {
            foreach (Transform child in item)
            {
                switch (child.name)
                {
                    case "Button: Choose This":
                        chooseButton = RocketDesignerCommon.MakeButton(child, buttonClickClip);

                        foreach (Transform chooseButtonChild in child)
                        {
                            if (chooseButtonChild.name == "idle")
                            {
                                foreach (Transform trSprite in chooseButtonChild)
                                {
                                    if (trSprite.GetComponent<UILabel>())
                                    {
                                        chooseButtonLabel = trSprite.GetComponent<UILabel>();
                                        break;
                                    }
                                }
                            }
                        }
                        break;
                    case "Component Image":
                        componentImagePlaceholder = child.GetChild(0).GetComponent<UISprite>();
                        componentPlaceholder = child.Find("TextureComponent").GetComponent<UITexture>();
                        break;
                    case "Component Text":
                        foreach (Transform txt in child)
                        {
                            if (txt.name == "Component Description")
                            {
                                description = txt.GetComponent<UILabel>();
                            }
                            else if (txt.name == "Component TItle")
                            {
                                title = txt.GetComponent<UILabel>();
                            }
                        }
                        break;
                    case "Meter Labels":
                        foreach (Transform lbl in child)
                        {
                            switch (lbl.name)
                            {
                                case "Label 1":
                                    meterLabels[0].label = lbl.GetComponent<UILabel>();
                                    break;
                                case "Label 2":
                                    meterLabels[1].label = lbl.GetComponent<UILabel>();
                                    break;
                                case "Label 3":
                                    meterLabels[2].label = lbl.GetComponent<UILabel>();
                                    break;
                            }
                        }
                        break;
                    case "Meters":
                        foreach (Transform meter in child)
                        {
                            switch (meter.name)
                            {
                                case "Meter 1":
                                    meterLabels[0].slider = meter.GetComponent<UISlider>();
                                    break;
                                case "Meter 2":
                                    meterLabels[1].slider = meter.GetComponent<UISlider>();
                                    break;
                                case "Meter 3":
                                    meterLabels[2].slider = meter.GetComponent<UISlider>();
                                    break;
                            }
                        }
                        break;
                    case "Tooltip":
                        Destroy(child.gameObject);
                        break;
                    case "Button: Finish":
                        finishButton = RocketDesignerCommon.MakeButton(child, buttonClickClip);
                        finishButton.gameObject.SetActive(false);
                        break;
                }
            }
        }

        public bool subcribedChooseButton = false,
                    subscribedTabs = false;

        public void Toggle(bool useIt, bool on)
        {
            if (useIt)
            {
                trPanel.gameObject.SetActive(on);
                trDisplay.gameObject.SetActive(on);
            }
        }
    }

    List<NGUIConsoleDetails> nGUIConsoles;

    Camera GUI3DCamera;

    RenderTexture rtexComponent, rtexCompare;
    public Camera componentCamera;

    void Start()
    {
        GameObject goCameras = GameObject.Find("Cameras");
        foreach (Transform tr in goCameras.transform)
        {
            if (tr.name == "Main Camera")
            {
                trMainCamera = tr;
                break;
            }
        }

        nGUIConsoles = new List<NGUIConsoleDetails>();

        Transform anchor = ResolutionDetector.Main.Gui3D.transform;

        foreach (Transform consoleItem in anchor.GetChild(0))
        {
            if (!consoleItem.name.Contains("Display") && consoleItem.name != "Camera")
            {
                NGUIConsoleDetails panel = new NGUIConsoleDetails(consoleItem, buttonAudioClip, disableInactivePanels);
                nGUIConsoles.Add(panel);
            }
        }

        foreach (Transform tr in anchor.GetChild(0))
        {
            if (tr.name == "Camera")
            {
                GUI3DCamera = tr.GetComponent<Camera>();
                break;
            }
        }
    }

    void OnEnable()
    {
        if (xml)
        {
            chooseButtonText = DialogXMLParser.GetText(xml.text, chooseButtonSection, ActiveLanguage.English);
            chooseButtonTextChange = DialogXMLParser.GetText(xml.text, chooseButtonSectionChange, ActiveLanguage.English);
            nextTabText = DialogXMLParser.GetText(xml.text, nextTabButtonSection, ActiveLanguage.English);
            enterText = DialogXMLParser.GetText(xml.text, enterButtonSection, ActiveLanguage.English);
            compareButtonText = DialogXMLParser.GetText(xml.text, compareButtonSection, ActiveLanguage.English);
            launchButtonText = DialogXMLParser.GetText(xml.text, prepareForLaunchButtonSection, ActiveLanguage.English);
            completeButtonText = DialogXMLParser.GetText(xml.text, completeButtonSection, ActiveLanguage.English);
            closecompareButtonText = DialogXMLParser.GetText(xml.text, closecompareButtonSection, ActiveLanguage.English);
            compareTitleText = DialogXMLParser.GetText(xml.text, compareTitleSection, ActiveLanguage.English);
        }
        rtexComponent = new RenderTexture(512, 512, 32);
        RenderTexture.active = rtexComponent;

        rtexCompare = new RenderTexture(256, 512, 32);
        RenderTexture.active = rtexCompare;

        componentCamera.targetTexture = rtexComponent;
        componentCamera.Render();

    }

    Transform trMainCamera;
    public override void SetState(ConsoleControllerBase.ConsoleStates newState)
    {
        if (newState == ConsoleStates.Complete)
        {
            StartCoroutine(FlyCamera(trMainCamera, null));
        }
        else if (newState == ConsoleStates.CanBeComplete && ConsoleState == ConsoleStates.Complete)
        {
            FlyCamera(activeTab);
        }

        ConsoleState = newState;

        if (activeTab == null)
        {
            SwitchTab(currentTabs[3]);
        }
        else    
            UpdateConsole(activeTab.activeButton, activeTab);
    }

    void SwitchTab(TabDetails to)
    {
        if (ConsoleState == ConsoleStates.Play || ConsoleState == ConsoleStates.CanBeComplete)
        {
            // This is just buttons initialization
            if (to.buttons == null)
                if (tabClickCallback != null)
                    tabClickCallback(to, true);

            if (to.buttons != null)
            {
                ButtonDetails buttonDetails = null;
                if (to.activeButton != null)
                {
                    buttonDetails = to.activeButton;
                }
                else
                    buttonDetails = to.buttons[0];

                if (buttonClickCallback(buttonDetails, false))
                {
                    if (to != activeTab)
                    {
                        if (tabClickCallback != null)
                            tabClickCallback(to, false);

                        FlyCamera(to);
                    }
                    else
                    {
                        afterButtonClickCallback(buttonDetails, false);
                    }

                    UpdateConsole(buttonDetails, to);
                }
            }
            else
                Debug.LogError("Buttons are NULL, That shouldn't be");
        }
    }

    void SwitchComponentView(TabDetails to)
    {
        if (componentCamera)
            foreach (NGUIConsoleDetails nGUIConsole in nGUIConsoles)
            {
                if (nGUIConsole.subPanelType == activeTab.componentType)
                {
                    nGUIConsole.componentPlaceholder.mainTexture = rtexComponent;
                    nGUIConsole.componentPlaceholder.pivot = UIWidget.Pivot.Center;
                    break;
                }
            }
    }

    void FlyCamera(TabDetails to)
    {
        StopAllCoroutines();

        foreach (NGUIConsoleDetails nGUIConsole in nGUIConsoles)
        {
            if (nGUIConsole.subPanelType == to.componentType)
            {
                nGUIConsole.Toggle(disableInactivePanels, true);
                StartCoroutine(FlyCamera(nGUIConsole.cameraPlaceholder, to));
                break;
            }
        }
    }

    public float flyCameraDelay = 1;
    IEnumerator FlyCamera(Transform consoleTo, TabDetails newActiveTab)
    {
        Vector3 startPos = GUI3DCamera.transform.position,
                endPos = consoleTo.position;

        Quaternion startRot = GUI3DCamera.transform.rotation,
                   endRot = consoleTo.rotation;

        float timer = 0;
        while (timer < 1)
        {
            GUI3DCamera.transform.position = Vector3.Lerp(startPos, endPos, timer);
            GUI3DCamera.transform.rotation = Quaternion.Lerp(startRot, endRot, timer);

            timer += Time.deltaTime / flyCameraDelay;

            yield return 0;
        }

        GUI3DCamera.transform.position = endPos;
        GUI3DCamera.transform.rotation = endRot;

        if (newActiveTab != null && newActiveTab != activeTab)
        {
            if (activeTab != null)
            {
                foreach (NGUIConsoleDetails nGUIConsole in nGUIConsoles)
                {
                    if (nGUIConsole.subPanelType == activeTab.componentType)
                    {
                        nGUIConsole.Toggle(disableInactivePanels, false);
                        break;
                    }
                }
            }

            activeTab = newActiveTab;
            SwitchComponentView(activeTab);

            ButtonDetails buttonDetails = null;
            if (newActiveTab.activeButton != null)
            {
                buttonDetails = newActiveTab.activeButton;
            }
            else
                buttonDetails = newActiveTab.buttons[0];
            afterButtonClickCallback(buttonDetails, false);
        }
    }

    void UpdateConsole(ButtonDetails buttonDetails, TabDetails activeTab)
    {
        foreach (NGUIConsoleDetails nGUIConsole in nGUIConsoles)
        {
            if (nGUIConsole.subPanelType == activeTab.componentType)
            {
                if (!nGUIConsole.subscribedTabs)
                {
                    foreach (NGUIConsoleDetails.NGUITabButton item in nGUIConsole.tabs)
                    {
                        UIEventListener.Get(item.tab.gameObject).onClick = TabClick;
                    }

                    nGUIConsole.subscribedTabs = true;
                }

                foreach (NGUIConsoleDetails.NGUIButton nguiButton in nGUIConsole.buttons)
                {
                    if (!nguiButton.subscribed)
                    {
                        UIEventListener.Get(nguiButton.button.gameObject).onClick = ConsoleButtonClick;
                        nguiButton.subscribed = true;
                    }

                    nguiButton.ToggleComponentButton(nguiButton.componentName == buttonDetails.componentName);
                    StartCoroutine(ToggleComponentButton(nguiButton.button, nguiButton.componentName == buttonDetails.componentName));
                }

                nGUIConsole.title.text = buttonDetails.screen.header;
                nGUIConsole.description.text = buttonDetails.screen.description;

                nGUIConsole.meterLabels[0].label.text = buttonDetails.screen.costName;
                nGUIConsole.meterLabels[0].slider.sliderValue = buttonDetails.screen.cost / 10;

                nGUIConsole.meterLabels[1].label.text = buttonDetails.screen.property1Name;
                nGUIConsole.meterLabels[1].slider.sliderValue = (buttonDetails.screen.property1 + buttonDetails.screen.improve) / 10;

                if (!string.IsNullOrEmpty(buttonDetails.screen.property2Name))
                {
                    nGUIConsole.meterLabels[2].label.text = buttonDetails.screen.property2Name;
                    nGUIConsole.meterLabels[2].slider.sliderValue = buttonDetails.screen.property2 / 10;
                }
                else
                {
                    nGUIConsole.meterLabels[2].ToggleMeter(false);
                }

                if (ConsoleState == ConsoleStates.CanBeComplete)
                {
                    nGUIConsole.chooseButtonLabel.text = chooseButtonTextChange;
                }
                else
                    nGUIConsole.chooseButtonLabel.text = chooseButtonText;

                nGUIConsole.chooseButton.gameObject.SetActive(true);

                if (!nGUIConsole.subcribedChooseButton)
                {
                    UIEventListener.Get(nGUIConsole.chooseButton.gameObject).onClick = ChooseComponentClick;
                    nGUIConsole.subcribedChooseButton = true;

                    UIEventListener.Get(nGUIConsole.finishButton.gameObject).onClick = CompleteDesignClick;
                }
                break;
            }
        }
    }

    IEnumerator ToggleComponentButton(UIImageButton uIImageButton, bool picked)
    {
        float timer = 0;

        while (timer < .3f)
        {
            uIImageButton.OnPress(picked);

            timer += Time.deltaTime;
            yield return 0;
        }
    }

    public override void InitTabs(Dictionary<RocketComponentBase.ComponentType, string> tabs, ConsoleControllerBase.TabClickCallback callback, ConsoleControllerBase.SimpleCallback completeCallback)
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

    public override void InitButtons(Dictionary<RocketComponentBase.ComponentName, string> names, ConsoleControllerBase.TabDetails forTab, ConsoleControllerBase.ButtonClickCallback callback, ConsoleControllerBase.ButtonClickCallback afterCallback)
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

        //NextTab();
    }

    void NextTab()
    {
        ConsoleControllerBase.TabDetails nextTab = GetNextTab();

        SwitchTab(nextTab);
    }

    bool isClickable = true;
    public override void ToggleClickable(bool state)
    {
        isClickable = state;

        if (state)
        {
            foreach (NGUIConsoleDetails nGUIConsole in nGUIConsoles)
            {
                nGUIConsole.finishButton.gameObject.SetActive(ConsoleState == ConsoleStates.CanBeComplete);
            }
        }
        else
        {
            foreach (NGUIConsoleDetails nGUIConsole in nGUIConsoles)
            {
                nGUIConsole.finishButton.gameObject.SetActive(false);
            }
        }
    }

    void TabClick(GameObject go)
    {
        if (!isClickable)
            return;

        foreach (NGUIConsoleDetails nGUIConsole in nGUIConsoles)
        {
            foreach (NGUIConsoleDetails.NGUITabButton item in nGUIConsole.tabs)
            {
                if (item.tab.gameObject == go)
                {
                    foreach (TabDetails tabDetails in currentTabs)
                    {
                        if (tabDetails.componentType == item.tabType)
                        {
                            SwitchTab(tabDetails);
                            break;
                        }
                    }

                    break;
                }
            }
        }
    }

    void ConsoleButtonClick(GameObject go)
    {
        if (!isClickable)
            return;

        if (ConsoleState == ConsoleStates.Play || ConsoleState == ConsoleStates.CanBeComplete)
        {
            foreach (NGUIConsoleDetails nGUIConsole in nGUIConsoles)
            {
                foreach (NGUIConsoleDetails.NGUIButton nGUIButton in nGUIConsole.buttons)
                {
                    if (nGUIButton.button.gameObject == go)
                    {
                        foreach (ButtonDetails button in activeTab.buttons)
                        {
                            if (button.componentName == nGUIButton.componentName)
                            {
                                if (activeTab.activeButton != button)
                                {
                                    if (buttonClickCallback != null)
                                    {
                                        if (buttonClickCallback(button, false))
                                        {
                                            activeTab.activeButton = button;
                                            UpdateConsole(button, activeTab);
                                            afterButtonClickCallback(button, false);
                                        }
                                    }
                                }
                                break;
                            }
                        }
                        break;
                    }
                }
            }
        }
    }

    void ChooseComponentClick(GameObject go)
    {
        if (!isClickable)
            return;

        if (ConsoleState == ConsoleStates.Play || ConsoleState == ConsoleStates.CanBeComplete)
        {
            NextTab();
        }
    }

    void CompleteDesignClick(GameObject go)
    {
        if (!isClickable)
            return;

        if (ConsoleState == ConsoleStates.CanBeComplete)
        {
            if (completeRocketDesignClick != null)
            {
                completeRocketDesignClick();
            }

            foreach (NGUIConsoleDetails nGUIConsole in nGUIConsoles)
            {
                nGUIConsole.finishButton.gameObject.SetActive(false);
            }
        }
    }

    public override void SetCameraTo(Transform placeholder, bool fast)
    {
        StopAllCoroutines();
        if (placeholder == null)
            placeholder = trMainCamera;

        if (fast)
        {
            GUI3DCamera.transform.position = placeholder.transform.position;
            GUI3DCamera.transform.rotation = placeholder.transform.rotation;
        }
        else
            StartCoroutine(FlyCamera(placeholder, null));
    }

    public override void UpdatePanelButtonState(RocketComponentsController.RocketConfig Rocket_Config)
    {
        foreach (NGUIConsoleDetails nGUIConsole in nGUIConsoles)
        {
            foreach (NGUIConsoleDetails.NGUITabButton item in nGUIConsole.tabs)
            {
                if (item.tabType == RocketComponentBase.ComponentType.Propellant)
                {
                    Transform child = item.tab.transform.Find("completed");
                    bool child_active  = Rocket_Config.pickedPropellant != RocketComponentBase.ComponentName.None;
                    NGUITools.SetActive(child.gameObject, child_active);
                }
                if (item.tabType == RocketComponentBase.ComponentType.Control)
                {
                    Transform child = item.tab.transform.Find("completed");
                    bool child_active = Rocket_Config.pickedControl != RocketComponentBase.ComponentName.None;
                    NGUITools.SetActive(child.gameObject, child_active);
                }
                if (item.tabType == RocketComponentBase.ComponentType.Shape)
                {
                    Transform child = item.tab.transform.Find("completed");
                    bool child_active = Rocket_Config.pickedShape != RocketComponentBase.ComponentName.None;
                    NGUITools.SetActive(child.gameObject, child_active);
                }
                if (item.tabType == RocketComponentBase.ComponentType.Stages)
                {
                    Transform child = item.tab.transform.Find("completed");
                    bool child_active = Rocket_Config.pickedStage != RocketComponentBase.ComponentName.None;
                    NGUITools.SetActive(child.gameObject, child_active);
                }
            }
        }
    }
}

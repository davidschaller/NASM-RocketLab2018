using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class NGUIGeneralController : GeneralControllerBase
{
    public AudioClip buttonAudioClip;

    public class RocketNameDetails
    {
        public UIImageButton backToDesignButton,
                             compareButton,
                             enterNameButton,
                             prepareForLaunch;

        public Transform tr;
    }

    public class CompareRocketDetails
    {
        public UIImageButton ClosecompareButton;

        public Transform tr;
    }

    RocketNameDetails rocketNameDetails;
    CompareRocketDetails compareRocketDetails;

    RenderTexture rtexCompare;
    public Camera compareCamera;

    UIInput rocketNameInput;

	OptionsData optionsData;

    void Start()
    {
		optionsData = FindObjectOfType<OptionsData>();

		Transform anchor = ResolutionDetector.Main.Gui2D.transform;

        foreach (Transform tr in anchor.GetChild(0))
        {
            if (tr.name == "Anchor-General")
            {
                foreach (Transform panel in tr)
                {
                    switch (panel.name)
                    {
                        case "Name Rocket":
                            rocketNameDetails = new RocketNameDetails();
                            rocketNameDetails.tr = panel;

                            foreach (Transform child in panel)
                            {
                                switch (child.name)
                                {
                                    case "Button: Back to Design":
                                        rocketNameDetails.backToDesignButton = RocketDesignerCommon.MakeButton(child, buttonAudioClip);
                                        UIEventListener.Get(rocketNameDetails.backToDesignButton.gameObject).onClick = BackToDesignClick;
                                        break;
                                    case "Button: Compare to Real Rocket":
                                        rocketNameDetails.compareButton = RocketDesignerCommon.MakeButton(child, buttonAudioClip);
                                        UIEventListener.Get(rocketNameDetails.compareButton.gameObject).onClick = CompareClick;
                                        break;
                                    case "Button: enter name":
                                        rocketNameDetails.enterNameButton = RocketDesignerCommon.MakeButton(child, buttonAudioClip);
                                        UIEventListener.Get(rocketNameDetails.enterNameButton.gameObject).onClick = EnterNameClick;
                                        break;
                                    case "Button: Prepare for Launch":
                                        rocketNameDetails.prepareForLaunch = RocketDesignerCommon.MakeButton(child, buttonAudioClip);
                                        UIEventListener.Get(rocketNameDetails.prepareForLaunch.gameObject).onClick = PrepareForLaunchClick;
                                        break;
                                    case "Name Rocket Popup":
                                        foreach (Transform nameRocketChild in child)
                                        {
                                            if (nameRocketChild.name == "Input")
                                            {
                                                rocketNameInput = nameRocketChild.GetComponent<UIInput>();
                                            }
                                        }
                                        break;
                                }
                            }
                            break;
                        case "Comparison Panel":
                            compareRocketDetails = new CompareRocketDetails();
                            compareRocketDetails.tr = panel;

                            foreach (Transform child in panel)
                            {
                                switch (child.name)
                                {
                                    case "Button: Close Comparison":
                                        compareRocketDetails.ClosecompareButton = RocketDesignerCommon.MakeButton(child, buttonAudioClip);
                                        UIEventListener.Get(compareRocketDetails.ClosecompareButton.gameObject).onClick = CompareCloseClick;
                                        break;
                                }
                            }
                            break;
                    }
                }
            }
            break;
        }
    }

    void OnEnable()
    {
        if (xml)
        {
            enterText = DialogXMLParser.GetText(xml.text, enterButtonSection, ActiveLanguage.English);
            compareButtonText = DialogXMLParser.GetText(xml.text, compareButtonSection, ActiveLanguage.English);
            launchButtonText = DialogXMLParser.GetText(xml.text, prepareForLaunchButtonSection, ActiveLanguage.English);
            completeButtonText = DialogXMLParser.GetText(xml.text, completeButtonSection, ActiveLanguage.English);
            closecompareButtonText = DialogXMLParser.GetText(xml.text, closecompareButtonSection, ActiveLanguage.English);
            compareTitleText = DialogXMLParser.GetText(xml.text, compareTitleSection, ActiveLanguage.English);
        }

        rtexCompare = new RenderTexture(356, 512, 32);
        RenderTexture.active = rtexCompare;
    }

    private RocketComponentsController.RocketConfig Rocket;
    private RealRocket RealRocket;
    private bool IsAlert = false;

    string rocketName = string.Empty;

    public override void ShowComparedRocket(RocketComponentsController.RocketConfig inRocket, RealRocket inRealRocket, GetComponentTypeText typeTextCallback, GetComponentNameText nameTextCallback, bool isAlert)
    {
        Rocket = inRocket;
        RealRocket = inRealRocket;
        IsAlert = isAlert;
        getComponentTypeText = typeTextCallback;
        getComponentNameText = nameTextCallback;

        SetCompareText();

//        if (compareCamera)
//        {
//            compareCamera.enabled = true;
//            compareCamera.targetTexture = rtexCompare;
//            compareCamera.Render();
//        }

        compareRocketDetails.tr.gameObject.SetActive(true);
    }

    void BackToDesignClick(GameObject go)
    {
        if (backToDesignCallback != null)
            backToDesignCallback();

        rocketNameDetails.tr.gameObject.SetActive(false);
    }

    void CompareClick(GameObject go)
    {
        if (!IsAlert)
        {
            rocketNameDetails.tr.gameObject.SetActive(false);

            if (compareRocketsCallback != null)
            {
                compareRocketsCallback();
            }
        }
    }

    void EnterNameClick(GameObject go)
    {
        if (confirmRocketName != null)
            confirmRocketName(rocketNameInput.text);
    }

    void PrepareForLaunchClick(GameObject go)
    {
		if (compareCamera)
		{
			compareCamera.enabled = true;
			compareCamera.targetTexture = rtexCompare;
			compareCamera.Render();
		}

        if (prepareForLaunchCallback != null)
        {
            prepareForLaunchCallback();
        }

        rocketNameDetails.tr.gameObject.SetActive(false);
    }

	void CompareCloseClick(GameObject go)
	{
		if (compareCloseCallback != null)
			compareCloseCallback();

		compareRocketDetails.tr.gameObject.SetActive(false);
		if (!IsAlert) {
			rocketNameDetails.tr.gameObject.SetActive(true);
			if (optionsData != null) {
				GameObject nameInput = rocketNameDetails.tr.Find("Name Rocket Popup/Input").gameObject;
				GameObject nameLabel = rocketNameDetails.tr.Find("Name Rocket Popup/Rocketname label").gameObject;
				GameObject nameButton = rocketNameDetails.tr.Find("Button: enter name").gameObject;
				if (optionsData.TreatAsMobile) {
					if(nameInput != null) {
						nameInput.SetActive(false);
					}
					if(nameLabel != null) {
						nameLabel.SetActive(false);
					}
					if(nameButton != null) {
						nameButton.SetActive(false);
					}
				} else {
					if (nameInput != null) {
						nameInput.SetActive(true);
					}
					if (nameLabel != null) {
						nameLabel.SetActive(true);
					}
					if (nameButton != null) {
						nameButton.SetActive(true);
					}
				}
			}
		}
	}

		void SetCompareText()
    {
        //RealRocket.Img

        foreach (Transform child in compareRocketDetails.tr)
        {
            if (child.name == "Museum Map")
                NGUITools.SetActive(child.gameObject, RealRocket.inMuseum);

            if (child.name == "Tooltip")
                NGUITools.SetActive(child.gameObject, RealRocket.inMuseum);

            foreach (Transform child2 in child)
            {
                if (child.name == "Tooltip")
                    if (child2.name == "Tooltip text")
                    {
                        child2.GetComponent<UILabel>().text = RealRocket.Tooltip;
                    }
                    
                if (child.name == "Comparison Rocket")
                    switch (child2.name)
                    {
                        case "Rocketname Text":
                            UILabel label = child2.GetComponent<UILabel>();
                            label.text = RealRocket.NameText;
                            break;
                        case "Rocket Comparison Texture":
                            child2.GetComponent<UITexture>().mainTexture = RealRocket.Img;
                            child2.GetComponent<UITexture>().pivot = UIWidget.Pivot.Top;
                            break;
                    }

                if (child.name == "User Rocket")
                    switch (child2.name)
                    {
                        case "Rocketname Text":
                            UILabel label = child2.GetComponent<UILabel>();
                            label.text = Rocket.rocketName;
                            break;
                        case "Rocket Comparison Texture":
                            child2.GetComponent<UITexture>().mainTexture = rtexCompare;
                            child2.GetComponent<UITexture>().pivot = UIWidget.Pivot.Top;
                            child2.GetComponent<UITexture>().MakePixelPerfect();
                            break;
                    }

                switch (child2.name)
                {
                    case "Propellant header":
                        child2.GetComponent<UILabel>().text = getComponentTypeText(RocketComponentBase.ComponentType.Propellant);
                        break;
                    case "Propel-Left":
                        child2.GetComponent<UILabel>().text = getComponentNameText(Rocket.pickedPropellant);
                        break;
                    case "Propel-Right":
                        child2.GetComponent<UILabel>().text = getComponentNameText(RealRocket.pickedPropellant);
                        break;

                    case "Control header":
                        child2.GetComponent<UILabel>().text = getComponentTypeText(RocketComponentBase.ComponentType.Control);
                        break;
                    case "Control-Left":
                        child2.GetComponent<UILabel>().text = getComponentNameText(Rocket.pickedControl);
                        break;
                    case "Control-Right":
                        child2.GetComponent<UILabel>().text = getComponentNameText(RealRocket.pickedControl);
                        break;

                    case "Stages header":
                        child2.GetComponent<UILabel>().text = getComponentTypeText(RocketComponentBase.ComponentType.Stages);
                        break;
                    case "Stages-Left":
                        child2.GetComponent<UILabel>().text = getComponentNameText(Rocket.pickedStage);
                        break;
                    case "Stages-Right":
                        child2.GetComponent<UILabel>().text = getComponentNameText(RealRocket.pickedStage);
                        break;

                    case "Shape header":
                        child2.GetComponent<UILabel>().text = getComponentTypeText(RocketComponentBase.ComponentType.Shape);
                        break;
                    case "Shape-Left":
                        child2.GetComponent<UILabel>().text = getComponentNameText(Rocket.pickedShape);
                        break;
                    case "Shape-Right":
                        child2.GetComponent<UILabel>().text = getComponentNameText(RealRocket.pickedShape);
                        break;

                    case "Rocket Name":
                        child2.GetComponent<UILabel>().text = RealRocket.DescriptionTitle;
                        break;
                    case "Rocket Description":
                        child2.GetComponent<UILabel>().text = RealRocket.Description;
                        break;
                }
            }
        }
    }

    public override void ShowRocketNamePanel(ConfirmRocketName nameCallback, SimpleCallback compareCallback, SimpleCallback launchCallback, SimpleCallback backCallback, SimpleCallback compareBackCallback)
    {
        IsAlert = false;
        confirmRocketName = nameCallback;
        compareRocketsCallback = compareCallback;
        prepareForLaunchCallback = launchCallback;
        backToDesignCallback = backCallback;
        compareCloseCallback = compareBackCallback;

        rocketNameDetails.tr.gameObject.SetActive(true);
		if (optionsData != null) {
			GameObject nameInput = rocketNameDetails.tr.Find("Name Rocket Popup/Input").gameObject;
			GameObject nameLabel = rocketNameDetails.tr.Find("Name Rocket Popup/Rocketname label").gameObject;
			GameObject nameButton = rocketNameDetails.tr.Find("Button: enter name").gameObject;
			if (optionsData.TreatAsMobile) {
				if (nameInput != null) {
					nameInput.SetActive(false);
				}
				if (nameLabel != null) {
					nameLabel.SetActive(false);
				}
				if (nameButton != null) {
					nameButton.SetActive(false);
				}
			} else {
				if (nameInput != null) {
					nameInput.SetActive(true);
				}
				if (nameLabel != null) {
					nameLabel.SetActive(true);
				}
				if (nameButton != null) {
					nameButton.SetActive(true);
				}
			}
		}
	}

	public override void HideNamePanel()
    {
        rocketNameDetails.tr.gameObject.SetActive(false);
    }
}

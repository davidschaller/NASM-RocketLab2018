using UnityEngine;
using System.Collections;

public class NGUIHeaderController : HeaderControllerBase
{
    public AudioClip buttonClickClip;

    const string ANCHOR_HEADER_PATH = "Camera/Anchor-Header";

    const string HEADER_SHORT = "Header Middle: Short",
                 HEADER_TALL = "Header Middle: Tall",
                 MISSION_INTRO= "Header Middle: Mission Intro",
                 HEADER_MAIN = "Header Main Panel";

    const string FOREGROUND = "foreground";

        // Missions Slider
    const string MISSIONS = "bg_missions",
                 MISSION_THUMB = "ibtn_mission",
        // Other Sliders
                 SUCCESS = "pgsb_success",
                 BUDGET = "pgsb_budget",
                 RISK = "pgsb_risk_short",
        // Buttons
                 GALLERY = "ibtn_gallery",
                 HOME = "ibtn_home";

    UISlider missionsSlider,
             riskSlider;

    const float missionsBkgXOffset = 15;

    class ShortHeaderDetails
    {
        public Transform tr;

        public UILabel missionDescription,
                       missionSubDescription,
                       risk,
                       twRatio,
                       cost,
                       twRatioValue;

        public UISlider riskProgressBar, costValue;

        public ShortHeaderDetails(Transform header)
        {
            tr = header;

            foreach (Transform child in header)
            {
                switch (child.name)
                {
                    case "Mission Title":
                        foreach (Transform missionItem in child)
                        {
                            switch (missionItem.name)
                            {
                                case "Mission Description":
                                    missionDescription = child.GetComponent<UILabel>();
                                    break;
                                case "Mission subDescription":
                                    missionSubDescription = child.GetComponent<UILabel>();
                                    break;
                            }
                        }
                        break;
                    case "Readout Text":
                        foreach (Transform readoutChild in child)
                        {
                            switch (readoutChild.name)
                            {
                                case "Cost":
                                    foreach (Transform costItem in readoutChild)
                                    {
                                        switch (costItem.name)
                                        {
                                            case "Progress Bar":
                                                costValue = costItem.GetComponent<UISlider>();
                                                break;
                                        }
                                    }
                                    break;
                                case "Cost Label":
                                    cost = readoutChild.GetChild(0).GetComponent<UILabel>();
                                    break;
                                case "Risk text":
                                    risk = readoutChild.GetComponent<UILabel>();
                                    break;
                                case "Risk Progress Bar":
                                    riskProgressBar = readoutChild.GetComponent<UISlider>();
                                    break;
                                case "TW Ratio":
                                    twRatio = readoutChild.GetComponent<UILabel>();
                                    break;
                                case "TW Ratio #":
                                    twRatioValue = readoutChild.GetComponent<UILabel>();
                                    break;
                            }
                        }
                        break;

                }
            }
        }
    }

    class TallHeaderDetails
    {
        public Transform tr;

        public UILabel missionHeader,
                       missionNumber,
                       missionDescription,
                       missionSubDescription,
                       risk,
                       cost,
                       thrust,
                       weight,
                       twRatio,
                       twRatioValue;

        public UISlider riskProgressBar,
                        costProgresssBar;

        public UISprite thrustArrow,
                        weightArrow;

        float thrustWeightMaxWidth = 0;

        public TallHeaderDetails(Transform header)
        {
            tr = header;

            foreach (Transform child in header)
            {
                switch (child.name)
                {
                    case "Cost Label":
                        cost = child.GetChild(0).GetComponent<UILabel>();
                        break;
                    case "Cost Meter":
                        costProgresssBar = child.GetChild(0).GetComponent<UISlider>();
                        break;
                    case "Mission Info":
                        foreach (Transform missionItem in child)
                        {
                            switch (missionItem.name)
                            {
                                case "Mission #":
                                    missionNumber = missionItem.GetComponent<UILabel>();
                                    break;
                                case "Mission Description":
                                    missionDescription = missionItem.GetComponent<UILabel>();
                                    break;
                                case "Mission header":
                                    missionHeader = missionItem.GetComponent<UILabel>();
                                    break;
                                case "Mission subDescription":
                                    missionSubDescription = missionItem.GetComponent<UILabel>();
                                    break;
                            }
                        }
                        break;
                    case "Risk Label":
                        risk = child.GetChild(0).GetComponent<UILabel>();
                        break;
                    case "Risk Meter":
                        riskProgressBar = child.GetChild(0).GetComponent<UISlider>();
                        break;
                    case "Thrust Arrows":
                        foreach (Transform arrow in child)
                        {
                            thrustWeightMaxWidth = child.GetChild(0).localScale.x;
                            switch (arrow.name)
                            {
                                case "arrow_thrust ( Sprite )":
                                    thrustArrow = arrow.GetComponent<UISprite>();
                                    break;
                                case "arrow_weight ( Sprite )":
                                    weightArrow = arrow.GetComponent<UISprite>();
                                    
                                    break;
                            }
                        }
                        break;
                    case "Thrust Label":
                        thrust = child.GetChild(0).GetComponent<UILabel>();
                        break;
                    case "TW Ratio":
                        foreach (Transform twRationChild in child)
                        {
                            switch (twRationChild.name)
                            {
                                case "5.3":
                                    twRatioValue = twRationChild.GetComponent<UILabel>();
                                    break;
                                case "Ratio label":
                                    twRatio = twRationChild.GetComponent<UILabel>();
                                    break;
                            }
                        }
                        break;
                    case "Weight Label":
                        weight = child.GetChild(0).GetComponent<UILabel>();
                        break;
                }
            }
        }

        public void SetArrows(float thrustValue, float weightValue)
        {
            Vector3 thrustScale = thrustArrow.transform.localScale,
                    weightScale = weightArrow.transform.localScale;

            if (thrustValue == 0)
            {
                thrustScale.x = 0;
            }
            else
            {
                thrustScale.x = thrustValue / 10 * thrustWeightMaxWidth;
            }

            if (weightValue == 0)
            {
                weightScale.x = 0;
            }
            else
                weightScale.x = weightValue / 10 * thrustWeightMaxWidth;

            thrustArrow.transform.localScale = thrustScale;
            weightArrow.transform.localScale = weightScale;
        }
    }

    class HeaderMainDetails
    {
        public Transform tr;

        public UILabel missions,
                       successRate,
                       successRateValue,
                       budget,
                       budgetValue;

        public UIImageButton[] missionButtons;

        public UIImageButton galleryButton,
                             homeButton;

        public UISlider successProgressBar,
                        budgetProgressBar;

        public HeaderMainDetails(Transform header, AudioClip buttonClickClip)
        {
            tr = header;

            missionButtons = new UIImageButton[4];

            foreach (Transform child in header)
            {
                switch (child.name)
                {
                    case "Budget Label":
                        foreach (Transform budgetItem in child)
                        {
                            switch (budgetItem.name)
                            {
                                case "Budget Header":
                                    budget = budgetItem.GetComponent<UILabel>();
                                break;
                                case "Budget Readout":
                                    budgetValue = budgetItem.GetComponent<UILabel>();
                                break;
                            }
                        }
                        break;
                    case "Budget Meter":
                        budgetProgressBar = child.GetChild(0).GetComponent<UISlider>();
                        break;
                    case "Button: gallery":
                        galleryButton = RocketDesignerCommon.MakeButton(child, buttonClickClip);
                        break;
                    case "Button: home":
                        homeButton = RocketDesignerCommon.MakeButton(child, buttonClickClip);
                        break;
                    case "Button: Mission1":
                        missionButtons[0] = RocketDesignerCommon.MakeButton(child, null);
                        break;
                    case "Button: Mission2":
                        missionButtons[1] = RocketDesignerCommon.MakeButton(child, null);
                        break;
                    case "Button: Mission3":
                        missionButtons[2] = RocketDesignerCommon.MakeButton(child, null);
                        break;
                    case "Button: Mission4":
                        missionButtons[3] = RocketDesignerCommon.MakeButton(child, null);
                        break;
                    case "Mission Bar":
                        foreach (Transform missionsItem in child)
                        {
                            if (missionsItem.name == "Missions text")
                            {
                                missions = missionsItem.GetComponent<UILabel>();
                            }
                        }
                        break;
                    case "Success Label":
                        foreach (Transform successItem in child)
                        {
                            switch (successItem.name)
                            {
                                case "Success rate #":
                                    successRateValue = successItem.GetComponent<UILabel>();
                                    break;
                                case "Success Title1":
                                    successRate = successItem.GetComponent<UILabel>();
                                    break;
                            }
                        }

                        foreach (Transform successItem in child)
                        {
                            if (successItem.name == "Success Title2")
                            {
                                Destroy(successItem.gameObject);
                                break;
                            }
                        }
                        break;
                    case "Success Meter":
                        successProgressBar = child.GetChild(0).GetComponent<UISlider>();
                        break;
                }
            }
        }

        public void RemoveButtonColliders()
        {
            foreach (UIImageButton btn in missionButtons)
            {
                if (btn && btn.GetComponent<Collider>())
                {
                    Destroy(btn.GetComponent<Collider>());
                }
            }
        }
    }

    IEnumerator SetRound(int round)
    {
        headerMainDetails.RemoveButtonColliders();

        yield return null;

		if (headerMainDetails.missionButtons != null && headerMainDetails.missionButtons[round - 1] != null) {
			headerMainDetails.missionButtons[round - 1].OnHover(true);
		}
    }

    class HeaderIntroDetails
    {
        public Transform tr;

        public UILabel missionHeader,
                       missionNumber,
                       missionDescription,
                       missionSubDescription,
                       risk,
                       cost,
                       thrust,
                       weight,
                       twDescription,
                       costValue;

        public UISlider riskProgressBar;

        public UIImageButton galleryButton,
                             homeButton;
        public UIImageButton startDesigningButton;

        public HeaderIntroDetails(Transform header, AudioClip buttonClickClip)
        {
            tr = header;

            foreach (Transform child in header)
            {
                switch (child.name)
                {
                    case "Button: gallery":
                        galleryButton = RocketDesignerCommon.MakeButton(child, buttonClickClip);
                        break;
                    case "Button: home":
                        homeButton = RocketDesignerCommon.MakeButton(child, buttonClickClip);
                        break;
                    case "Button: Start Designing":
                        startDesigningButton = RocketDesignerCommon.MakeButton(child, buttonClickClip);
                        break;
                    case "Cost Label":
                        cost = child.GetChild(0).GetComponent<UILabel>();
                        break;
                    case "Cost Meter":
                        foreach (Transform costItem in child)
                        {
                            switch (costItem.name)
                            {
                                case "foreground":
                                    costValue = costItem.GetChild(0).GetComponent<UILabel>();
                                    break;
                            }
                        }
                        break;
                    case "Mission Info":
                        foreach (Transform missionItem in child)
                        {
                            switch (missionItem.name)
                            {
                                case "Mission #":
                                    missionNumber = missionItem.GetComponent<UILabel>();
                                    break;
                                case "Mission Description":
                                    missionDescription = missionItem.GetComponent<UILabel>();
                                    break;
                                case "Mission header":
                                    missionHeader = missionItem.GetComponent<UILabel>();
                                    break;
                                case "Mission subDescription":
                                    missionSubDescription = missionItem.GetComponent<UILabel>();
                                    break;
                            }
                        }
                        break;
                    case "Risk Label":
                        risk = child.GetChild(0).GetComponent<UILabel>();
                        break;
                    case "Risk Meter":
                        riskProgressBar = child.GetChild(0).GetComponent<UISlider>();
                        break;
                    case "Thrust Label":
                        foreach (Transform thrustItem in child)
                        {
                            switch (thrustItem.name)
                            {
                                case "THRUST label":
                                    thrust = thrustItem.GetComponent<UILabel>();
                                    break;
                                case "Thrust text":
                                    twDescription = thrustItem.GetComponent<UILabel>();
                                    break;
                            }
                        }
                        break;
                    case "Weight Label":
                        weight = child.GetChild(0).GetComponent<UILabel>();
                        break;
                }
            }
        }
    }

    ShortHeaderDetails shortHeaderDetails;
    TallHeaderDetails tallHeaderDetails;
    HeaderMainDetails headerMainDetails;
    HeaderIntroDetails headerIntroDetails;

    void Start()
    {
        Transform anchor = ResolutionDetector.Main.Gui2D.transform;

        Transform trAnchorHedaer = anchor.Find(ANCHOR_HEADER_PATH).transform;

        foreach (Transform header in trAnchorHedaer)
        {
            switch (header.name)
            {
                case HEADER_SHORT:
                    shortHeaderDetails = new ShortHeaderDetails(header);
                    break;
                case HEADER_TALL:
                    tallHeaderDetails = new TallHeaderDetails(header);
                    break;
                case HEADER_MAIN:
                    headerMainDetails = new HeaderMainDetails(header, buttonClickClip);
                    break;
                case MISSION_INTRO:
                    headerIntroDetails = new HeaderIntroDetails(header, buttonClickClip);
                    break;
            }
        }
    }

    void OnEnable()
    {
        if (xml)
        {
            startDesignButtonText = DialogXMLParser.GetText(xml.text, startDesignSection, ActiveLanguage.English);
            missionsText = DialogXMLParser.GetText(xml.text, missionsSection, ActiveLanguage.English);
            successText = DialogXMLParser.GetText(xml.text, successSection, ActiveLanguage.English);
            missionTitleText = DialogXMLParser.GetText(xml.text, missionTitleSection, ActiveLanguage.English);
            missionDescriptionText = DialogXMLParser.GetText(xml.text, missionDescriptionSection, ActiveLanguage.English);
            riskText = DialogXMLParser.GetText(xml.text, riskSection, ActiveLanguage.English);
            thrustText = DialogXMLParser.GetText(xml.text, thrustSection, ActiveLanguage.English);
            weightText = DialogXMLParser.GetText(xml.text, weightSection, ActiveLanguage.English);
            ratioText = DialogXMLParser.GetText(xml.text, rationSection, ActiveLanguage.English);
            costText = DialogXMLParser.GetText(xml.text, costSection, ActiveLanguage.English);
            budgetText = DialogXMLParser.GetText(xml.text, budgetSection, ActiveLanguage.English);
            galleryButtonText = DialogXMLParser.GetText(xml.text, galleryButtonSection, ActiveLanguage.English);
            homeButtonText = DialogXMLParser.GetText(xml.text, homeButtonSection, ActiveLanguage.English);
            rocketNameText = DialogXMLParser.GetText(xml.text, rocketNameSection, ActiveLanguage.English);

            foreach (BudgetValueToText item in budgetToTextArray)
            {
                item.SetText(xml.text, ActiveLanguage.English);
            }
        }
    }

    SimpleCallback startDesignCallback;

    void StartDesigningClick(GameObject go)
    {
        if (startDesignCallback != null)
            startDesignCallback();
    }

    void MainButtonClick(GameObject go)
    {
        if (go.name == headerMainDetails.homeButton.name || go.name == headerIntroDetails.homeButton.name)
        {
            if (homeClickCallback != null)
                homeClickCallback();
        }
        else if (go.name == headerMainDetails.galleryButton.name || go.name == headerIntroDetails.galleryButton.name)
        {
            if (galleryClickCallback != null)
                galleryClickCallback();
        }
    }

    public override void Init(int round, float money, HeaderControllerBase.SimpleCallback galleryClick, HeaderControllerBase.SimpleCallback homeClick)
    {
        headerIntroDetails.tr.gameObject.SetActive(false);
        headerMainDetails.tr.gameObject.SetActive(true);
        tallHeaderDetails.tr.gameObject.SetActive(true);

        StartCoroutine(SetRound(round));

        tallHeaderDetails.missionNumber.text = round.ToString();
        tallHeaderDetails.twRatioValue.text = string.Empty;
        tallHeaderDetails.riskProgressBar.sliderValue = 0;
        tallHeaderDetails.costProgresssBar.sliderValue = 0;
        tallHeaderDetails.costProgresssBar.numberOfSteps = 0;

        tallHeaderDetails.SetArrows(thrustValue, weightValue);

        UpdateBudget(0);

        missionNumber = round;
        moneyAtStart = money;
        SetState(HeaderStates.Play, null);

        galleryClickCallback = galleryClick;
        homeClickCallback = homeClick;

        UIEventListener.Get(headerMainDetails.galleryButton.gameObject).onClick = MainButtonClick;
        UIEventListener.Get(headerMainDetails.homeButton.gameObject).onClick = MainButtonClick;
    }

    public override void ButtonInit(SimpleCallback galleryClick, SimpleCallback homeClick)
    {
        galleryClickCallback = galleryClick;
        homeClickCallback = homeClick;

		if(headerMainDetails == null) {
			return;
		}

		UIEventListener.Get(headerMainDetails.galleryButton.gameObject).onClick = MainButtonClick;
        UIEventListener.Get(headerMainDetails.homeButton.gameObject).onClick = MainButtonClick;
        UIEventListener.Get(headerIntroDetails.galleryButton.gameObject).onClick = MainButtonClick;
        UIEventListener.Get(headerIntroDetails.homeButton.gameObject).onClick = MainButtonClick;
    }

    void UpdateBudget(float newVal)
    {
        budgetValue = newVal;
        for (int i = 0; i < budgetToTextArray.Length; i++)
            if (budgetToTextArray[i].InBetween(budgetValue))
            {
                headerMainDetails.budgetValue.text = budgetToTextArray[i].text;
                break;
            }

        headerMainDetails.budgetProgressBar.sliderValue = 1 - budgetValue;
    }

    public override void SetState(HeaderControllerBase.HeaderStates newState, HeaderControllerBase.SimpleCallback callback)
    {
        HeaderState = newState;

        if (newState == HeaderStates.ShowStartButton)
        {
            startDesignCallback = callback;
            headerIntroDetails.tr.gameObject.SetActive(true);
            UIEventListener.Get(headerIntroDetails.startDesigningButton.gameObject).onClick = StartDesigningClick;
        }
        else if (newState == HeaderStates.LaunchMode)
        {
            tallHeaderDetails.tr.gameObject.SetActive(false);
            shortHeaderDetails.tr.gameObject.SetActive(true);

            shortHeaderDetails.costValue.sliderValue = costValue / maxCost;
            shortHeaderDetails.twRatioValue.text = thrustWeightRatio.ToString("0.00");
            shortHeaderDetails.riskProgressBar.sliderValue = riskValue;

            float tempBudget = budgetValue + costValue / moneyAtStart;
            UpdateBudget(tempBudget);
        }
        else if (newState == HeaderStates.Play)
        {
            tallHeaderDetails.tr.gameObject.SetActive(true);
            shortHeaderDetails.tr.gameObject.SetActive(false);
        }
    }

    public override void Hide(bool ishide)
    {
        if(ishide)
        {
            tallHeaderDetails.tr.gameObject.SetActive(false);
            shortHeaderDetails.tr.gameObject.SetActive(false);
        }
        else
        {
            SetState(HeaderState, null);
        }
        headerMainDetails.tr.gameObject.SetActive(!ishide);
    }

    public override void UpdateParams(float budget, float currentCost, float risk, float thrust, float weight)
    {
        riskValue = risk;
        thrustValue = thrust;
        weightValue = weight;

        if (thrustValue != 0 && weightValue != 0)
        {
            thrustWeightRatio = thrustValue / weightValue;

            tallHeaderDetails.twRatioValue.text = thrustWeightRatio.ToString("0.00");
        }

        tallHeaderDetails.SetArrows(thrustValue, weightValue);

        costValue = currentCost;

        budgetValue = (moneyAtStart - budget) / moneyAtStart;
        UpdateBudget(budgetValue);

        tallHeaderDetails.costProgresssBar.sliderValue = costValue / maxCost;
        tallHeaderDetails.riskProgressBar.sliderValue = risk;
    }

    public override void UpdateSuccess(float success)
    {
		if(headerMainDetails == null) {
			return;
		}

		if(headerMainDetails.successProgressBar == null) {
			return;
		}

		if(headerMainDetails.successRateValue == null) {
			return;
		}
		headerMainDetails.successProgressBar.sliderValue = success;
        headerMainDetails.successRateValue.text = (Mathf.RoundToInt((success) * 100)).ToString();
    }
}

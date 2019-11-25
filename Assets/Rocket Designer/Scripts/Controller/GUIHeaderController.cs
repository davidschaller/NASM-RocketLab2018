using UnityEngine;
using System.Collections;

public class GUIHeaderController : HeaderControllerBase
{
    const string LOGO_STYLE_NAME = "logo",
                 HEADER_BKG_STYLE_NAME = "header-bkg",
                 HEADER_BUTTON_STYLE_NAME = "sample_button",
                 HEADER_LABEL_STYLE_NAME = "label",
                 MISSION_STYLE = "mission",
                 HEADER_EXTRA_BKG = "header-extra",
                 YELLOW_SLIDER_STYLE = "yellow-slider",
                 YELLOW_SLIDER_THUMB = "yellow-slider-thumb",
                 THRUST_STYLE_NAME = "thrust",
                 WEIGHT_STYLE_NAME = "weight";

    public string missionPictureStyleName = "mission-picture";

    public GUISkin guiSkin;

    GUIStyle rocketLabLogoStyle,
             headerBackgroundStyle,
             headerButtonStyle,
             headerLabelStyle,
             headerWhiteLabelStyle,
             missionStyle,
             missionActiveStyle,
             centerHeaderStyle,
             missionPictureStyle,
             headerExtraBkgStyle,
             yellowSliderStyle,
             riskSliderThumbStyle,
             costSliderThumbStyle,
             thrustStyle,
             weightStyle;

    SimpleCallback startDesignCallback;

    public Vector2 missionsSize = new Vector2(200, 50),
                   successSize = new Vector2(100, 100),
                   galleryButtonSize = new Vector2(100, 50),
                   homeButtonSize = new Vector2(80, 40),
                   startDesigningButtonize = new Vector2(150, 30);

    public float missionMarginTop = 10,
                 missionsLabelWidth = 70,
                 successMarginLeft = 30,
                 extraMarginLeft = 40,
                 riskCostWidth = 50,
                 thrustWeightMarginLeft = -25,
                 extraColumnWidth = 100,
                 budgetMarginLeft = 100,
                 successBudgetHeight = 50,
                 launchModeColumnWidth = 100;

    public AudioClip clickSound;

    public Vector2 centerRectSize = new Vector2(300, 95),
                   extraSize = new Vector2(320, 60);

    float thrustWeightMaxWidth = 0,
          thrustWeightHeight = 0;

    public override void SetState(HeaderStates newState, SimpleCallback callback)
    {
        HeaderState = newState;

        startDesignCallback = callback;
    }

    public override void Hide(bool ishide)
    {
    }

    void OnEnable()
    {
        if (guiSkin)
        {
            rocketLabLogoStyle = guiSkin.GetStyle(LOGO_STYLE_NAME);
            headerBackgroundStyle = guiSkin.GetStyle(HEADER_BKG_STYLE_NAME);
            headerButtonStyle = guiSkin.GetStyle(HEADER_BUTTON_STYLE_NAME);
            headerLabelStyle = guiSkin.GetStyle(HEADER_LABEL_STYLE_NAME);
            headerWhiteLabelStyle = new GUIStyle(headerLabelStyle);
            headerWhiteLabelStyle.normal.textColor = Color.white;
            
            missionStyle = guiSkin.GetStyle(MISSION_STYLE);
            missionActiveStyle = new GUIStyle(missionStyle);
            missionActiveStyle.normal = missionStyle.onActive;
            missionPictureStyle = guiSkin.GetStyle(missionPictureStyleName);

            headerExtraBkgStyle = guiSkin.GetStyle(HEADER_EXTRA_BKG);

            yellowSliderStyle = guiSkin.GetStyle(YELLOW_SLIDER_STYLE);
            GUIStyle yellowSliderThumbStyle = guiSkin.GetStyle(YELLOW_SLIDER_THUMB);

            riskSliderThumbStyle = new GUIStyle(yellowSliderThumbStyle);
            costSliderThumbStyle = new GUIStyle(yellowSliderThumbStyle);
            
            thrustStyle = guiSkin.GetStyle(THRUST_STYLE_NAME);
            weightStyle = guiSkin.GetStyle(WEIGHT_STYLE_NAME);

            thrustWeightMaxWidth = thrustStyle.normal.background.width;
            thrustWeightHeight = thrustStyle.normal.background.height;
        }

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

    void OnGUI()
    {
        if (guiSkin)
        {
            Rect headerBkgRect = new Rect(0, 0, Screen.width, headerBackgroundStyle.fixedHeight);

            GUI.Box(headerBkgRect, GUIContent.none, headerBackgroundStyle);

            Rect logoRect = new Rect(0, 0, rocketLabLogoStyle.fixedWidth, rocketLabLogoStyle.fixedHeight);

            GUI.Box(logoRect, GUIContent.none, rocketLabLogoStyle);

            Rect startDesignButtonRect = new Rect(headerBkgRect.xMax / 2 - startDesigningButtonize.x / 2, headerBkgRect.yMax - startDesigningButtonize.y / 2, startDesigningButtonize.x, startDesigningButtonize.y);

            switch (HeaderState)
            {
                case HeaderStates.ShowStartButton:
                    if (HeaderState == HeaderStates.ShowStartButton)
                    {
                        if (GUI.Button(startDesignButtonRect, startDesignButtonText, headerButtonStyle))
                        {
                            if (startDesignCallback != null)
                                startDesignCallback();

                            PlayTap();
                        }
                    }
                    break;
                case HeaderStates.Play:
                case HeaderStates.LaunchMode:
                    Rect missionsRect = new Rect(logoRect.x, logoRect.yMax + missionMarginTop, missionsSize.x, missionsSize.y);

                    GUILayout.BeginArea(missionsRect);
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label(missionsText, headerLabelStyle, GUILayout.Width(missionsLabelWidth));
                            for (int i = 1; i < 5; i++)
                            {
                                GUILayout.Label(i.ToString(), i == missionNumber ? missionActiveStyle : missionStyle);
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndArea();

                    Rect successRect = new Rect(missionsRect.xMax + successMarginLeft, 0, successSize.x, successSize.y);
                    GUILayout.BeginArea(successRect);
                    {
                        GUILayout.Label(successText, headerLabelStyle);
                        GUILayout.Label(successRate.ToString() + "%", headerLabelStyle);

                        GUILayout.Label(rocketNameText, headerLabelStyle);
                    }
                    GUILayout.EndArea();

                    Rect centerRect = new Rect(Screen.width / 2 - centerRectSize.x / 2, 0, centerRectSize.x, centerRectSize.y);

                    if (HeaderState == HeaderStates.Play)
                    {
                        GUILayout.BeginArea(centerRect);
                        {
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.FlexibleSpace();
                                GUILayout.Label(GUIContent.none, missionPictureStyle);

                                GUILayout.BeginVertical();
                                {
                                    GUILayout.Label(missionsText + " " + missionNumber.ToString(), headerWhiteLabelStyle);
                                    GUILayout.Label(missionTitleText, headerWhiteLabelStyle);
                                    GUILayout.Label(missionDescriptionText, headerWhiteLabelStyle);
                                }
                                GUILayout.EndVertical();
                            }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndArea();


                        Rect extraRect = new Rect(Screen.width / 2 - extraSize.x / 2 + extraMarginLeft, centerRect.yMax, extraSize.x, extraSize.y);
                        GUI.Box(extraRect, GUIContent.none, headerExtraBkgStyle);

                        float currentThrustWidth = thrustValue / 10 * thrustWeightMaxWidth;
                        Rect thrustRect = new Rect(extraRect.x + extraRect.width / 2 - currentThrustWidth / 2 + thrustWeightMarginLeft,
                            extraRect.y, currentThrustWidth, thrustWeightHeight);
                        GUI.Box(thrustRect, GUIContent.none, thrustStyle);

                        float currentWeightWidth = weightValue / 10 * thrustWeightMaxWidth;
                        Rect weightRect = new Rect(extraRect.x + extraRect.width / 2 - currentWeightWidth / 2 + thrustWeightMarginLeft,
                            thrustRect.yMax, currentWeightWidth, thrustWeightHeight);
                        GUI.Box(weightRect, GUIContent.none, weightStyle);

                        GUILayout.BeginArea(extraRect);
                        {
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.BeginVertical(GUILayout.Width(extraColumnWidth));
                                {
                                    GUILayout.Label(riskText, headerWhiteLabelStyle);

                                    riskSliderThumbStyle.border.left = (int)(-riskCostWidth * riskValue);

                                    GUILayout.HorizontalSlider(riskValue, 0, 1, yellowSliderStyle, riskSliderThumbStyle, GUILayout.Width(riskCostWidth));
                                }
                                GUILayout.EndVertical();

                                GUILayout.BeginVertical(GUILayout.Width(extraColumnWidth));
                                {
                                    GUILayout.Label(thrustText, headerWhiteLabelStyle);
                                    GUILayout.Label(weightText, headerWhiteLabelStyle);

                                    GUILayout.BeginHorizontal();
                                    {
                                        if (thrustWeightRatio != 0)
                                        {
                                            GUILayout.Label(ratioText, headerWhiteLabelStyle);
                                            GUILayout.Label(thrustWeightRatio.ToString(), headerWhiteLabelStyle);
                                        }
                                    }
                                    GUILayout.EndHorizontal();
                                }
                                GUILayout.EndVertical();

                                GUILayout.BeginVertical(GUILayout.Width(extraColumnWidth));
                                {
                                    GUILayout.Label(costText, headerWhiteLabelStyle);

                                    costSliderThumbStyle.border.left = (int)(-riskCostWidth * costValue);

                                    GUILayout.HorizontalSlider(costValue, 0, 1, yellowSliderStyle, costSliderThumbStyle, GUILayout.Width(riskCostWidth));
                                }
                                GUILayout.EndVertical();
                            }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndArea();
                    }
                    else
                    {
                        GUILayout.BeginArea(centerRect);
                        {
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.BeginVertical(GUILayout.Width(launchModeColumnWidth));
                                {
                                    GUILayout.Label(GUIContent.none, missionPictureStyle);
                                }
                                GUILayout.EndVertical();

                                GUILayout.BeginVertical(GUILayout.Width(launchModeColumnWidth));
                                {
                                    GUILayout.Label(missionTitleText, headerWhiteLabelStyle);
                                    GUILayout.Label(missionDescriptionText, headerWhiteLabelStyle);
                                }
                                GUILayout.EndVertical();

                                GUILayout.BeginVertical(GUILayout.Width(launchModeColumnWidth));
                                {
                                    GUILayout.BeginHorizontal();
                                    {
                                        GUILayout.Label(riskText, headerWhiteLabelStyle);

                                        riskSliderThumbStyle.border.left = (int)(-riskCostWidth * riskValue);

                                        GUILayout.HorizontalSlider(riskValue, 0, 1, yellowSliderStyle, riskSliderThumbStyle, GUILayout.Width(riskCostWidth));
                                    }
                                    GUILayout.EndHorizontal();
                                    GUILayout.BeginHorizontal();
                                    {
                                        if (thrustWeightRatio != 0)
                                        {
                                            GUILayout.Label(ratioText, headerWhiteLabelStyle);
                                            GUILayout.Label(thrustWeightRatio.ToString(), headerWhiteLabelStyle);
                                        }
                                    }
                                    GUILayout.EndHorizontal();
                                    GUILayout.BeginHorizontal();
                                    {
                                        GUILayout.Label(costText, headerWhiteLabelStyle);

                                        costSliderThumbStyle.border.left = (int)(-riskCostWidth * costValue);

                                        GUILayout.HorizontalSlider(costValue, 0, 1, yellowSliderStyle, costSliderThumbStyle, GUILayout.Width(riskCostWidth));
                                    }
                                    GUILayout.EndHorizontal();
                                }
                                GUILayout.EndVertical();
                            }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndArea();
                    }

                    Rect budgetRect = new Rect(centerRect.xMax + budgetMarginLeft, 0, successSize.x, successSize.y);
                    GUILayout.BeginArea(budgetRect);
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.BeginVertical();
                            {
                                GUILayout.Label(budgetText, headerLabelStyle);

                                string budgetValueText = string.Empty;

                                for (int i = 0; i < budgetToTextArray.Length; i++)
                                    if (budgetToTextArray[i].InBetween(budgetValue))
                                        budgetValueText = budgetToTextArray[i].text;

                                GUILayout.Label(budgetValueText, headerLabelStyle);
                            }
                            GUILayout.EndVertical();

                            GUILayout.VerticalSlider(budgetValue, 1, 0, GUILayout.Height(successBudgetHeight));
                        }
                        GUILayout.EndHorizontal();

                    }
                    GUILayout.EndArea();

                    Rect galleryButtonRect = new Rect(headerBkgRect.xMax - galleryButtonSize.x, 0, galleryButtonSize.x, galleryButtonSize.y);
                    if (GUI.Button(galleryButtonRect, galleryButtonText, headerButtonStyle))
                    {
                        if (galleryClickCallback != null)
                            galleryClickCallback();

                        PlayTap();
                    }

                    Rect homeButtonRect = new Rect(headerBkgRect.xMax - homeButtonSize.x, galleryButtonRect.yMax, homeButtonSize.x, homeButtonSize.y);
                    if (GUI.Button(homeButtonRect, homeButtonText, headerButtonStyle))
                    {
                        if (homeClickCallback != null)
                            homeClickCallback();

                        PlayTap();
                    }
                    break;
            }
        }
    }

    public override void Init(int round, float money, SimpleCallback galleryClick, SimpleCallback homeClick)
    {
        missionNumber = round;
        moneyAtStart = money;
        SetState(HeaderStates.Play, null);

        galleryClickCallback = galleryClick;
        homeClickCallback = homeClick;
    }

    public override void ButtonInit(SimpleCallback galleryClick, SimpleCallback homeClick)
    {
        galleryClickCallback = galleryClick;
        homeClickCallback = homeClick;
    }

    public override void UpdateParams(float budget, float currentCost, float risk, float thrust, float weight)
    {
        riskValue = risk;
        thrustValue = thrust;
        weightValue = weight;

        if (thrustValue != 0 && weightValue != 0)
        {
            thrustWeightRatio = thrustValue / weightValue;
        }

        costValue = currentCost / 40;

        budgetValue = (moneyAtStart - budget) / moneyAtStart;
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
}

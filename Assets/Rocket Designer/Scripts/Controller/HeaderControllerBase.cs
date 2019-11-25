using UnityEngine;
using System.Collections;

public abstract class HeaderControllerBase : ControllerOfGUIBase
{
    protected SimpleCallback galleryClickCallback,
                             homeClickCallback;

    protected int missionNumber = 1;
    protected float moneyAtStart = 0,
                    riskValue = 0,
                    thrustValue = 0,
                    weightValue = 0,
                    thrustWeightRatio = 0,
                    costValue = 0,
                    budgetValue = 0,
                    successRate = 0;

    public enum HeaderStates
    {
        None,
        ShowStartButton,
        Play,
        LaunchMode
    }

    public HeaderStates HeaderState { get; protected set; }

    public string startDesignSection = "start_design_button",
                  missionsSection = "missions",
                  successSection = "success_rate",
                  missionTitleSection = "mission1_title",
                  missionDescriptionSection = "mission1_description",
                  riskSection = "risk",
                  thrustSection = "thrust",
                  weightSection = "weight",
                  rationSection = "ratio",
                  costSection = "cost",
                  budgetSection = "budget",
                  galleryButtonSection = "gallery-button",
                  homeButtonSection = "home-button",
                  rocketNameSection = "rocket-name";

    protected string startDesignButtonText,
                     missionsText,
                     successText,
                     missionTitleText,
                     missionDescriptionText,
                     riskText,
                     thrustText,
                     weightText,
                     ratioText,
                     costText,
                     budgetText,
                     galleryButtonText,
                     homeButtonText,
                     rocketNameText;

    public abstract void Init(int round, float money, SimpleCallback galleryClick, SimpleCallback homeClick);
    public abstract void ButtonInit(SimpleCallback galleryClick, SimpleCallback homeClick);

    public abstract void SetState(HeaderStates newState, SimpleCallback callback);
    public abstract void Hide(bool ishide);

    public abstract void UpdateParams(float budget, float currentCost, float risk, float thrust, float weight);

    public virtual void UpdateSuccess(float success)
    {
    }

    [System.Serializable]
    public class BudgetValueToText
    {
        public float budgetValueFrom,
                     budgetValueTo;
        public string budgetSection;

        public string text { get; private set; }

        public void SetText(string xmlText, ActiveLanguage lang)
        {
            text = DialogXMLParser.GetText(xmlText, budgetSection, lang);
        }

        public bool InBetween(float budgetValue)
        {
            return budgetValue >= budgetValueFrom && budgetValue <= budgetValueTo;
        }
    }

    public BudgetValueToText[] budgetToTextArray;

    public int maxCost = 30;
}

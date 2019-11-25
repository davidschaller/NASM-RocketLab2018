using System;
using UnityEngine;

public abstract class GeneralControllerBase : ControllerOfGUIBase
{
    public delegate void ConfirmRocketName(string rocketName);

    public delegate string GetComponentTypeText(RocketComponentBase.ComponentType ct);
    public delegate string GetComponentNameText(RocketComponentBase.ComponentName cn);

    public string enterButtonSection = "enter",
                  compareButtonSection = "compare_button",
                  compareTitleSection = "compare_title",
                  closecompareButtonSection = "close_compare_button",
                  prepareForLaunchButtonSection = "launch_button",
                  completeButtonSection = "complete_rocket_button";

    protected string enterText,
                     compareButtonText,
                     compareTitleText,
                     launchButtonText,
                     completeButtonText,
                     closecompareButtonText;

    public ConfirmRocketName confirmRocketName;

    public SimpleCallback compareRocketsCallback,
                          prepareForLaunchCallback,
                          backToDesignCallback,
                          compareCloseCallback;

    protected GetComponentTypeText getComponentTypeText;
    protected GetComponentNameText getComponentNameText;

    public abstract void ShowComparedRocket(RocketComponentsController.RocketConfig inRocket, RealRocket inRealRocket, GetComponentTypeText typeTextCallback, GetComponentNameText nameTextCallback,bool isAlert);

    public abstract void ShowRocketNamePanel(ConfirmRocketName nameCallback, SimpleCallback compareCallback, SimpleCallback launchCallback, SimpleCallback backCallback, SimpleCallback compareBackCallback);

    public abstract void HideNamePanel();
}

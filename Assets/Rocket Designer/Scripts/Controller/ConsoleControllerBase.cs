using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class ConsoleControllerBase : ControllerOfGUIBase
{
    public delegate void TabClickCallback(TabDetails tab, bool justInit);
    public delegate bool ButtonClickCallback(ButtonDetails button, bool picked);
    
    public TabClickCallback tabClickCallback;
    public ButtonClickCallback buttonClickCallback;
    public ButtonClickCallback afterButtonClickCallback;

    public SimpleCallback completeRocketDesignClick;

    public string chooseButtonSection = "choose_component_button",
              chooseButtonSectionChange = "choose_component_button_change",
              nextTabButtonSection = "to_next_tab",
              enterButtonSection = "enter",
              compareButtonSection = "compare_button",
              compareTitleSection = "compare_title",
              closecompareButtonSection = "close_compare_button",
              prepareForLaunchButtonSection = "launch_button",
              completeButtonSection = "complete_rocket_button";

    protected string chooseButtonText,
                     chooseButtonTextChange,
                     nextTabText,
                     enterText,
                     compareButtonText,
                     compareTitleText,
                     launchButtonText,
                     completeButtonText,
                     closecompareButtonText;

    public enum ConsoleStates
    {
        None,
        Play,
        CanBeComplete,
        Complete
    }

    public ConsoleStates ConsoleState { get; protected set; }

    public abstract void SetState(ConsoleControllerBase.ConsoleStates newState);

    [System.Serializable]
    public class TabDetails
    {
        public RocketComponentBase.ComponentType componentType;
        public string componentTypeText;
        public List<ButtonDetails> buttons;

        public ButtonDetails activeButton;

        public void InitButtons(Dictionary<RocketComponentBase.ComponentName, string> dictionary)
        {
            buttons = new List<ButtonDetails>();

            foreach (KeyValuePair<RocketComponentBase.ComponentName, string> item in dictionary)
            {
                buttons.Add(new ButtonDetails() { componentName = item.Key, componentNameText = item.Value, componentType = componentType });
            }

            activeButton = buttons[0];
        }

        public bool PickedButtonExists
        {
            get
            {
                if (buttons != null)
                {
                    foreach (ButtonDetails button in buttons)
                    {
                        if (button.Picked)
                            return true;
                    }
                }

                return false;
            }
        }
    }

    [System.Serializable]
    public class ButtonDetails
    {
        public RocketComponentBase.ComponentName componentName;
        public string componentNameText;

        public RocketComponentBase.ComponentType componentType;

        public ScreenDetails screen;

        public bool Picked = false;

        public void UpdateScreen(string[] properties)
        {
            screen = new ScreenDetails()
            {
                header = properties[0],
                description = properties[1],
                costName = properties[2],
                property1Name = properties[3],
                property2Name = properties[4],
                costHint = properties[5],
                property1Hint = properties[6],
                property2Hint = properties[7],
                cost = float.Parse(properties[8]),
            };

            if (!string.IsNullOrEmpty(properties[9]))
                screen.property1 = float.Parse(properties[9]);
            else
                screen.property1 = -1;

            if (!string.IsNullOrEmpty(properties[10]))
                screen.property2 = float.Parse(properties[10]);
            else
                screen.property2 = -1;

            if (!string.IsNullOrEmpty(properties[11]))
                screen.improve = float.Parse(properties[11]);
            else
                screen.improve = 0;
        }
    }

    public class ScreenDetails
    {
        public string header,
                      name,
                      description;

        public string costHint,
                      property1Hint,
                      property2Hint;

        public float cost,
                     property1,
                     property2,
                     improve;

        public string costName,
                      property1Name,
                      property2Name;
    }

    public List<TabDetails> currentTabs { get; protected set; }
    public TabDetails activeTab { get; protected set; }

    public abstract void InitTabs(Dictionary<RocketComponentBase.ComponentType, string> tabs, TabClickCallback callback, SimpleCallback completeCallback);

    public abstract void InitButtons(Dictionary<RocketComponentBase.ComponentName, string> names, TabDetails forTab, ButtonClickCallback callback, ButtonClickCallback afterCallback);

    public abstract void ConfirmSelection(RocketComponentBase.ComponentType componentType, RocketComponentBase.ComponentName componentName);

    public abstract void ToggleClickable(bool state);
    public abstract void UpdatePanelButtonState(RocketComponentsController.RocketConfig Rocket_Config);

    protected ConsoleControllerBase.TabDetails GetNextTab()
    {
        int idx = currentTabs.IndexOf(activeTab);

        idx++;
        if (idx >= currentTabs.Count)
            idx = 0;

        ConsoleControllerBase.TabDetails nextTab = currentTabs[idx];

        return nextTab;
    }

    public string GetComponentTypeText(RocketComponentBase.ComponentType ct)
    {
        foreach (TabDetails td in currentTabs)
            if (td.componentType == ct)
                return td.componentTypeText;
        return null;
    }

    public string GetComponentNameText(RocketComponentBase.ComponentName cn)
    {
        foreach (TabDetails td in currentTabs)
            foreach (ButtonDetails bt in td.buttons)
                if (bt.componentName == cn)
                    return bt.componentNameText;
        return null;
    }

    public abstract void SetCameraTo(Transform transform, bool fast);

    public void UpdateAciveButton()
    {
        buttonClickCallback(activeTab.activeButton, true);
    }
}

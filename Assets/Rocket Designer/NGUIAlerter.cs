using UnityEngine;

public class NGUIAlerter : AlerterBase
{
    Transform alertWithOneButton,
              alertWithTwoButtons;

    UILabel oneButtonText,
            twoButtonText,
            buttonText,
            button1Text,
            button2Text;

    UIImageButton button,
                  button1,
                  button2;

    OneButtonDetails currentAlert = null;
    TwoButtonDetails currentTwoButtonAlert = null;

    [System.Serializable]
    public class OneButtonDetails
    {
        public OneButtonAlerts alertType;

        public string textSection,
                      buttonSection,
                      buttonInfoSection;

        public AlternativeRenderer alternativeRenderer;
        public bool allowAlternative;

        public void SetUpAlternative(Transform anchor)
        {
            alternativeRenderer.SetUpAlternative(anchor);
        }
    }

    [System.Serializable]
    public class TwoButtonDetails
    {
        public TwoButtonAlerts alertType;

        public string textSection,
                      button1Section,
                      button2Section;
    }

    [System.Serializable]
    public class AlternativeRenderer
    {
        public Transform panel { get; private set; }
        
        public UIImageButton button { get; private set; }
        public UILabel buttonText{ get; private set; }

        public UIImageButton buttonInfo { get; private set; }
        public UILabel buttonInfoText { get; private set; }

        public UILabel text { get; private set; }

        public string panelPath,
                      buttonPath,
                      buttonTextPath,
                      buttonInfoPath,
                      buttonInfoTextPath,
                      textPath;

        public void SetUpAlternative(Transform anchor)
        {
            if (!string.IsNullOrEmpty(panelPath))
                panel = anchor.Find(panelPath);

            if (!string.IsNullOrEmpty(buttonPath) && anchor.Find(buttonPath) != null)
                button = anchor.Find(buttonPath).GetComponent<UIImageButton>();

            if (!string.IsNullOrEmpty(buttonTextPath) && anchor.Find(buttonTextPath) != null)
                buttonText = anchor.Find(buttonTextPath).GetComponent<UILabel>();

            if (!string.IsNullOrEmpty(buttonInfoPath) && anchor.Find(buttonInfoPath) != null)
                buttonInfo = anchor.Find(buttonInfoPath).GetComponent<UIImageButton>();

            if (!string.IsNullOrEmpty(buttonInfoTextPath) && anchor.Find(buttonInfoTextPath) != null)
                buttonInfoText = anchor.Find(buttonInfoTextPath).GetComponent<UILabel>();

            if (!string.IsNullOrEmpty(textPath) && anchor.Find(textPath) != null)
                text = anchor.Find(textPath).GetComponent<UILabel>();
        }
    }

    public OneButtonDetails[] oneButtonDetailsArray;
    public TwoButtonDetails[] twoButtonDetailsArray;

    void Start()
    {
        Transform anchor = ResolutionDetector.Main.Gui2D.transform;

        foreach (Transform tr in anchor.GetChild(0))
        {
            if (tr.name == "Anchor-General")
            {
                foreach (Transform panel in tr)
                {
                    switch (panel.name)
                    {
                        case "Custom Panel":
                            foreach (Transform item in panel)
                            {
                                switch (item.name)
                                {
                                    case "Alert With One Button":
                                        alertWithOneButton = item;

                                        foreach (Transform subitem in item)
                                        {
                                            switch (subitem.name)
                                            {
                                                case "Button":
                                                    button = subitem.GetComponent<UIImageButton>();

                                                    UIEventListener.Get(button.gameObject).onClick = ButtonClick;

                                                    foreach (Transform buttonItem in subitem)
                                                        if (buttonItem.GetComponent<UILabel>())
                                                            buttonText = buttonItem.GetComponent<UILabel>();
                                                    break;
                                                case "Text":
                                                    oneButtonText = subitem.GetComponent<UILabel>();
                                                    break;
                                            }
                                        }
                                        break;
                                    case "Alert With Two Buttons":
                                        alertWithTwoButtons = item;

                                        foreach (Transform subitem in item)
                                        {
                                            switch (subitem.name)
                                            {
                                                case "Text":
                                                    twoButtonText = subitem.GetComponent<UILabel>();
                                                    break;
                                                case "Button1":
                                                    button1 = subitem.GetComponent<UIImageButton>();
                                                    UIEventListener.Get(button1.gameObject).onClick = Button1Click;

                                                    foreach (Transform buttonItem in subitem)
                                                        if (buttonItem.GetComponent<UILabel>())
                                                            button1Text = buttonItem.GetComponent<UILabel>();
                                                    break;
                                                case "Button2":
                                                    button2 = subitem.GetComponent<UIImageButton>();
                                                    UIEventListener.Get(button2.gameObject).onClick = Button2Click;

                                                    foreach (Transform buttonItem in subitem)
                                                        if (buttonItem.GetComponent<UILabel>())
                                                            button2Text = buttonItem.GetComponent<UILabel>();
                                                    break;
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


        foreach (OneButtonDetails item in oneButtonDetailsArray)
        {
            item.SetUpAlternative(anchor);
        }
    }

    public override void Hide(bool isHide)
    {
        if (currentAlert != null)
        {
            if (currentAlert.alternativeRenderer.panel && currentAlert.alternativeRenderer.text && currentAlert.allowAlternative)
            {
                currentAlert.alternativeRenderer.panel.gameObject.SetActive(!isHide);
            }
            else
            {
                alertWithOneButton.gameObject.SetActive(!isHide);
            }
        }
    }

    public override bool IsActive
    {
        get
        {
            return currentAlert != null || currentTwoButtonAlert != null;
        }
    }

    public override void ShowOneButton(AlerterBase.OneButtonAlerts alertType, string[] values, AlerterBase.SimpleClickCallback callback, bool allowAlternative)
    {
        bool found = false;
        foreach (OneButtonDetails item in oneButtonDetailsArray)
        {
            if (item.alertType == alertType)
            {
                currentAlert = item;
                item.allowAlternative = allowAlternative;

                string text = DialogXMLParser.GetText(xml.text, item.textSection, ActiveLanguage.English);
                string buttonLabelText = DialogXMLParser.GetText(xml.text, item.buttonSection, ActiveLanguage.English);

                if (item.alternativeRenderer.panel && item.alternativeRenderer.button && item.alternativeRenderer.buttonText && allowAlternative)
                {
                    item.alternativeRenderer.buttonText.text = buttonLabelText;
                    UIEventListener.Get(item.alternativeRenderer.button.gameObject).onClick = AltOneButtonClick;
                }
                else
                {
                    buttonText.text = buttonLabelText;
                }

                if (values != null && values.Length > 0)
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        string template = "{" + i.ToString() + "}";

                        if (text.Contains(template))
                        {
                            text = text.Replace(template, values[i]);
                        }
                    }
                }

                if (item.alternativeRenderer.panel && item.alternativeRenderer.text && allowAlternative)
                {
                    item.alternativeRenderer.text.text = text;
                    item.alternativeRenderer.panel.gameObject.SetActive(true);
                }
                else
                {
                    oneButtonText.text = text;
                    alertWithOneButton.gameObject.SetActive(true);
                }

                found = true;
                simpleClickCallback = callback;
                break;
            }
        }

        if (!found)
            Debug.LogError("Couldn't find the alert with type '" + alertType.ToString() + "' in oneButtonDetailsArray", transform);
    }

    public override void ShowOneButton(OneButtonAlerts alertType, string[] values, SimpleClickCallback callback, SimpleClickCallback callbackInfo, bool allowAlternative)
    {
        ShowOneButton(alertType, values, callback, allowAlternative);

        bool found = false;
        foreach (OneButtonDetails item in oneButtonDetailsArray)
        {
            if (item.alertType == alertType)
            {
                currentAlert = item;

                string buttonLabelText = DialogXMLParser.GetText(xml.text, item.buttonInfoSection, ActiveLanguage.English);

                if (item.alternativeRenderer.panel && item.alternativeRenderer.buttonInfo && item.alternativeRenderer.buttonInfoText && allowAlternative)
                {
                    //item.alternativeRenderer.buttonInfoText.text = buttonLabelText;
                    UIEventListener.Get(item.alternativeRenderer.buttonInfo.gameObject).onClick = InfoOneButtonClick;
                }

                found = true;
                simpleClickCallbackInfo = callbackInfo;
                break;
            }
        }

        if (!found)
            Debug.LogError("Couldn't find the alert with type '" + alertType.ToString() + "' in oneButtonDetailsArray 2", transform);
    }

    public override void ShowTwoButtons(AlerterBase.TwoButtonAlerts alertType, string[] values, AlerterBase.TwoButtonsClickCallback callback)
    {
        bool found = false;
        foreach (TwoButtonDetails item in twoButtonDetailsArray)
        {
            if (item.alertType == alertType)
            {
                string text = DialogXMLParser.GetText(xml.text, item.textSection, ActiveLanguage.English);
                button1Text.text = DialogXMLParser.GetText(xml.text, item.button1Section, ActiveLanguage.English);
                button2Text.text = DialogXMLParser.GetText(xml.text, item.button2Section, ActiveLanguage.English);

                if (values != null && values.Length > 0)
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        string template = "{" + i.ToString() + "}";

                        if (text.Contains(template))
                        {
                            text = text.Replace(template, values[i]);
                        }
                    }
                }

                twoButtonText.text = text;
                alertWithTwoButtons.gameObject.SetActive(true);
                found = true;
                twoButtonsClickCallback = callback;
                break;
            }
        }

        if (!found)
            Debug.LogError("Couldn't find the alert with type '" + alertType.ToString() + "' in twoButtonDetailsArray", transform);
    }

    void ButtonClick(GameObject go)
    {
        currentAlert = null;

        if (simpleClickCallback != null)
            simpleClickCallback();

        if (alertWithOneButton.gameObject.activeSelf)
            alertWithOneButton.gameObject.SetActive(false);
    }

    void AltOneButtonClick(GameObject go)
    {
        currentAlert.alternativeRenderer.panel.gameObject.SetActive(false);
        currentAlert = null;

        if (simpleClickCallback != null)
            simpleClickCallback();
    }

    void InfoOneButtonClick(GameObject go)
    {
        //currentAlert.alternativeRenderer.panel.gameObject.SetActive(false);
        if (simpleClickCallbackInfo != null)
            simpleClickCallbackInfo();
    }

    void Button1Click(GameObject go)
    {
        currentTwoButtonAlert = null;

        if (twoButtonsClickCallback != null)
            twoButtonsClickCallback(true);

        alertWithTwoButtons.gameObject.SetActive(false);
    }

    void Button2Click(GameObject go)
    {
        currentTwoButtonAlert = null;

        if (twoButtonsClickCallback != null)
            twoButtonsClickCallback(false);

        alertWithTwoButtons.gameObject.SetActive(false);
    }
}

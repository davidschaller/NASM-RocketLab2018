using UnityEngine;

public class GUIAlerter : AlerterBase
{
    public GUISkin guiSkin;

    public Vector2 windowSize = new Vector2(300, 200);

    bool showOneButton = false;

    string text,
           button,
           button1,
           button2;

    [System.Serializable]
    public class OneButtonDetails
    {
        public OneButtonAlerts alertType;

        public string textSection,
                      buttonSection;

        public Vector2 windowSize;
    }

    [System.Serializable]
    public class TwoButtonDetails
    {
        public TwoButtonAlerts alertType;

        public string textSection,
                      button1Section,
                      button2Section;

        public Vector2 size;
    }

    public OneButtonDetails[] oneButtonDetailsArray;
    public TwoButtonDetails[] twoButtonDetailsArray;

    void Awake()
    {
        enabled = false;
    }

    public override void Hide(bool isHide)
    {
        enabled = false;
    }

    public void Hide()
    {
        Hide(true);
    }

    void OnGUI()
    {
        Rect windowRect = new Rect(Screen.width / 2 - windowSize.x / 2, Screen.height / 2 - windowSize.y / 2, windowSize.x, windowSize.y);

        if (showOneButton)
        {
            GUILayout.Window(1, windowRect, OneButtonWindow, string.Empty);
        }
        else
        {
            GUILayout.Window(1, windowRect, TwoButtonWindow, string.Empty);
        }
    }

    void OneButtonWindow(int id)
    {
        GUILayout.Label(text);

        if (GUILayout.Button(button))
        {
            if (simpleClickCallback != null)
            {
                simpleClickCallback();
            }
            Hide();
        }
    }

    void TwoButtonWindow(int id)
    {
        GUILayout.Label(text);

        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button(button1))
            {
                if (twoButtonsClickCallback != null)
                {
                    twoButtonsClickCallback(true);
                }
                Hide();
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(button2))
            {
                if (twoButtonsClickCallback != null)
                {
                    twoButtonsClickCallback(false);
                }
                Hide();
            }
        }
        GUILayout.EndHorizontal();
    }

    public override bool IsActive
    {
        get { return enabled; }
    }

    public override void ShowOneButton(AlerterBase.OneButtonAlerts alertType, string[] values, AlerterBase.SimpleClickCallback callback, bool allowAlternative)
    {
        bool found = false;
        foreach (OneButtonDetails item in oneButtonDetailsArray)
        {
            if (item.alertType == alertType)
            {
                windowSize = item.windowSize;

                text = DialogXMLParser.GetText(xml.text, item.textSection, ActiveLanguage.English);
                button = DialogXMLParser.GetText(xml.text, item.buttonSection, ActiveLanguage.English);

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

                showOneButton = true;
                found = true;
                enabled = true;
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
    }

    public override void ShowTwoButtons(AlerterBase.TwoButtonAlerts alertType, string[] values, AlerterBase.TwoButtonsClickCallback callback)
    {
        bool found = false;
        foreach (TwoButtonDetails item in twoButtonDetailsArray)
        {
            if (item.alertType == alertType)
            {
                windowSize = item.size;

                text = DialogXMLParser.GetText(xml.text, item.textSection, ActiveLanguage.English);
                button1 = DialogXMLParser.GetText(xml.text, item.button1Section, ActiveLanguage.English);
                button2 = DialogXMLParser.GetText(xml.text, item.button2Section, ActiveLanguage.English);

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

                showOneButton = false;
                found = true;
                enabled = true;
                twoButtonsClickCallback = callback;
                break;
            }
        }

        if (!found)
            Debug.LogError("Couldn't find the alert with type '" + alertType.ToString() + "' in twoButtonDetailsArray", transform);
    }
}

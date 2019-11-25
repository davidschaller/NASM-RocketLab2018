using UnityEngine;

public class GUILoadLevelRenderer : LoadLevelRendererBase
{
    public GUISkin guiSkin;

    GUIStyle boxGUIStyle,
             progressBkgGUIStyle,
             progressFrgGUIStyle,
             progressLabelGUIStyle;

    [System.Serializable]
    public class XMLPropertySettings
    {
        public string text = "Text",
                      progress = "Progress";

        public string boxStyle = "/root/Style/@box",
                      progressBkgStyle = "/root/Style/@progressBkg",
                      progressFrgStyle = "/root/Style/@progressFrg",
                      progressLabelStyle = "/root/Style/@progressLabel";
    }

    public XMLPropertySettings propertySettings;

    string text,
           progress;

    void Awake()
    {
        enabled = IsShowed;
    }

    public override void Show(TextAsset xml)
    {
        progress = DialogXMLParser.GetText(xml.text, propertySettings.progress, ActiveLanguage.English);
        text = DialogXMLParser.GetText(xml.text, propertySettings.text, ActiveLanguage.English);

        string boxStyle = DialogXMLParser.GetStyle(xml.text, propertySettings.boxStyle);

        string progressBkgStyle = DialogXMLParser.GetStyle(xml.text, propertySettings.progressBkgStyle);
        string progressFrgStyle = DialogXMLParser.GetStyle(xml.text, propertySettings.progressFrgStyle);
        string progressLabelStyle = DialogXMLParser.GetStyle(xml.text, propertySettings.progressLabelStyle);

        boxGUIStyle = guiSkin.GetStyle(boxStyle);
        progressBkgGUIStyle = guiSkin.GetStyle(progressBkgStyle);
        progressFrgGUIStyle = guiSkin.GetStyle(progressFrgStyle);
        progressLabelGUIStyle = guiSkin.GetStyle(progressLabelStyle);

        enabled = true;

        base.Show(xml);
    }

    void OnGUI()
    {
        if (guiSkin)
        {
            GUI.depth = -1000;

            Rect boxRect = new Rect(Screen.width / 2 - boxGUIStyle.fixedWidth / 2, Screen.height / 2 - boxGUIStyle.fixedHeight / 2, boxGUIStyle.fixedWidth, boxGUIStyle.fixedHeight);
            GUI.Box(boxRect, text, boxGUIStyle);

            GUI.BeginGroup(boxRect);
            {
                Rect bkgRect = new Rect(boxRect.width / 2 - progressBkgGUIStyle.fixedWidth / 2,
                                        boxRect.height - progressBkgGUIStyle.fixedHeight - progressBkgGUIStyle.margin.bottom,
                                        progressBkgGUIStyle.fixedWidth,
                                        progressBkgGUIStyle.fixedHeight);

                Rect frgRect = new Rect(bkgRect);
                frgRect.width *= ProgressValue;



                GUI.Box(bkgRect, GUIContent.none, progressBkgGUIStyle);
                GUI.Box(frgRect, GUIContent.none, progressFrgGUIStyle);

                GUI.Label(bkgRect, progress, progressLabelGUIStyle);
            }
            GUI.EndGroup();
        }
    }
}

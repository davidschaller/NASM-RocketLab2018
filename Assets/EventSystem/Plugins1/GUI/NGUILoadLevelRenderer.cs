using UnityEngine;

public class NGUILoadLevelRenderer : LoadLevelRendererBase
{
    public string dialogPath = "";

    Transform dialogTransform;

    private UISlider slider;
    private UILabel txtLabel;

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

    void Start()
    {
        Transform anchor = ResolutionDetector.Main.Gui2D.transform;

        if (!string.IsNullOrEmpty(dialogPath))
            dialogTransform = anchor.Find(dialogPath);

        if (dialogTransform)
        {
            Transform ob = dialogTransform.Find("Slider");
            if (ob != null)
                slider = ob.GetComponent<UISlider>();

            ob = dialogTransform.Find("Text");
            if (ob != null)
                txtLabel = ob.GetComponent<UILabel>();
        }
        enabled = IsShowed;

        if (dialogTransform)
            NGUITools.SetActive(dialogTransform.gameObject, enabled);
    }

    public override void Show(TextAsset xml)
    {
        progress = DialogXMLParser.GetText(xml.text, propertySettings.progress, ActiveLanguage.English);
        text = DialogXMLParser.GetText(xml.text, propertySettings.text, ActiveLanguage.English);

        enabled = true;

        base.Show(xml);
        if (txtLabel != null)
            txtLabel.text = text;

        if (dialogTransform)
            NGUITools.SetActive(dialogTransform.gameObject, enabled);

        ProgressChanged();
    }

    public override void Hide()
    {
        base.Hide();
        enabled = false;
        NGUITools.SetActive(dialogTransform.gameObject, enabled);
    }

    public override void ProgressChanged()
    {
        if (slider)
            slider.sliderValue = ProgressValue;
    }
}

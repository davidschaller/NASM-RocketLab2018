using UnityEngine;
using System.Collections;

public class NGUIOneButtonDialogRenderer : OneButtonDialogRendererBase
{
    public Transform dialogTranform { get; private set; }
    public Transform buttonTransform { get; private set; }
    public UILabel dialogText { get; private set; }
    public UILabel buttonText { get; private set; }

    public string dialogPath,
                  buttonPath,
                  dialogTextPath,
                  buttonTextPath;

    [System.Serializable]
    public class XMLPropertySettings
    {
        public string text = "Text",
                      buttonText = "Button";

        public string boxStyle = "/root/Style/@box",
                      buttonStyle = "/root/Style/@button";
    }

    void Init()
    {
        Transform anchor = ResolutionDetector.Main.Gui2D.transform;

        if (!string.IsNullOrEmpty(dialogPath))
            dialogTranform = anchor.Find(dialogPath);

        if (!string.IsNullOrEmpty(buttonPath))
            buttonTransform = anchor.Find(buttonPath);

        if (!string.IsNullOrEmpty(dialogTextPath))
            dialogText = anchor.Find(dialogTextPath).GetComponent<UILabel>();

        if (!string.IsNullOrEmpty(buttonTextPath))
            buttonText = anchor.Find(buttonTextPath).GetComponent<UILabel>();
    }

    public XMLPropertySettings propertySettings;

    public override void Show(TextAsset xml, SimpleVoidDelegate callback)
    {
        Init();

        clickCallback += callback;

        if (buttonTransform)
        {
            if (buttonText)
                buttonText.text = DialogXMLParser.GetText(xml.text, propertySettings.buttonText, ActiveLanguage.English);

            if (buttonText)
            {
                if (string.IsNullOrEmpty(buttonText.text))
                {
                    buttonTransform.gameObject.SetActive(false);

                    if (clickCallback != null)
                        clickCallback();
                }
                else
                    buttonTransform.gameObject.SetActive(true);
            }
        }

        if (dialogText)
            dialogText.text = DialogXMLParser.GetText(xml.text, propertySettings.text, ActiveLanguage.English);

        dialogTranform.gameObject.SetActive(true);
        UIEventListener.Get(buttonTransform.gameObject).onClick = ButtonClick;
    }

    void ButtonClick(GameObject go)
    {
        if (clickCallback != null)
            clickCallback();

        dialogTranform.gameObject.SetActive(false);
    }

    public override void Hide()
    {
        clickCallback = null;
        dialogTranform.gameObject.SetActive(false);
        base.Hide();
    }
}

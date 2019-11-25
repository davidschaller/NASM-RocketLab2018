using UnityEngine;
using System.Collections;

public class GUIOneButtonDialogRenderer : OneButtonDialogRendererBase
{
    public GUISkin guiSkin;

    GUIStyle boxGUIStyle,
             buttonGUIStyle;

    [System.Serializable]
    public class XMLPropertySettings
    {
        public string text = "Text",
                      buttonText = "Button";

        public string boxStyle = "/root/Style/@box",
                      buttonStyle = "/root/Style/@button";
    }

    public XMLPropertySettings propertySettings;

	string text,
           buttonText;

    public AudioClip clickSound;

    public Vector2 buttonSize = new Vector2(150, 40);
    public float buttonMarginButton = 30;
	
	public bool loadURL = true;
	public string URLtoLoad = "http://www.google.com";

	void OnGUI ()
	{
		if(buttonGUIStyle == null || boxGUIStyle == null)
			return;
		
        if (guiSkin)
        {
			Rect boxRect = new Rect(Screen.width / 2 - boxGUIStyle.fixedWidth / 2, Screen.height / 2 - boxGUIStyle.fixedHeight / 2, boxGUIStyle.fixedWidth, boxGUIStyle.fixedHeight);
         	GUI.Box(boxRect, text, boxGUIStyle);
			
			if(loadURL)
			{
	            GUI.BeginGroup(boxRect);
	            {
	                Rect buttonRect = new Rect(boxRect.width / 2 - buttonSize.x / 2,
	                                        boxRect.height - buttonSize.y - buttonMarginButton,
	                                        buttonSize.x,
	                                        buttonSize.y);
	
	                if (GUI.Button(buttonRect, buttonText, buttonGUIStyle))
	                {
	                    if (clickCallback != null)
	                        clickCallback();
	
	                    if (clickSound)
	                    {
	                        if (!GetComponent<AudioSource>())
	                            gameObject.AddComponent<AudioSource>();
	
	                        GetComponent<AudioSource>().PlayOneShot(clickSound);
	                    }
						
						Application.OpenURL(URLtoLoad);
	
	                    enabled = false;
	                }
	            }
	            GUI.EndGroup();
			}
        }
	}

    public override void Show(TextAsset xml, SimpleVoidDelegate callback)
    {
        buttonText = DialogXMLParser.GetText(xml.text, propertySettings.buttonText, ActiveLanguage.English);
        text = DialogXMLParser.GetText(xml.text, propertySettings.text, ActiveLanguage.English);

        string boxStyle = DialogXMLParser.GetStyle(xml.text, propertySettings.boxStyle);
        string buttonStyle = DialogXMLParser.GetStyle(xml.text, propertySettings.buttonStyle);

        boxGUIStyle = guiSkin.GetStyle(boxStyle);
        buttonGUIStyle = guiSkin.GetStyle(buttonStyle);

        enabled = true;

        clickCallback += callback;
    }
}

using UnityEngine;
using System.Collections;

public class Pictorial3GUI : MonoBehaviour
{
	public GUISkin skin;
	public string buttonStyleName = "Button";
	
	[HideInInspector]
	public Texture2D[] images;
	[HideInInspector]
	public Rect imageRect;
	[HideInInspector]
	public string[] buttonText;
	[HideInInspector]
	public string[] spanishButtonText;
	[HideInInspector]
	public Vector2 buttonTopLeft;
	[HideInInspector]
	public float intraButtonSpacing = 5;
	[HideInInspector]
	public Vector2[] buttonSizes;
	
	
	Rect buttonRect;
	void Awake ()
	{
		Pictorial3Runner.callback = Activate;
		enabled = false;
	}
	
	Pictorial3Runner runner;
	void Activate (Pictorial3Runner r)
	{
		runner = r;
		images = new Texture2D[6];
		images[0] = r.image1English;
		images[1] = r.image1Spanish;
		images[2] = r.image2English;
		images[3] = r.image2Spanish;
		images[4] = r.image3English;
		images[5] = r.image3Spanish;
		
		imageRect = r.imageRect;
		
		if (r.buttonStyle != "")
			buttonStyleName = r.buttonStyle;
		else if (buttonStyleName == "")
			buttonStyleName = "Button";
		
		buttonText = new string[3];
		buttonText[0] = r.button1Text;
		buttonText[1] = r.button2Text;
		buttonText[2] = r.button3Text;
		
		spanishButtonText = new string[3];
		spanishButtonText[0] = r.button1SpanishText;
		spanishButtonText[1] = r.button2SpanishText;
		spanishButtonText[2] = r.button3SpanishText;
		
		buttonSizes = new Vector2[3];
		buttonSizes[0] = r.button1Size;
		buttonSizes[1] = r.button2Size;
		buttonSizes[2] = r.button3Size;
		
		intraButtonSpacing = r.intraButtonSpacing;
		buttonTopLeft = r.buttonTopLeft;
		CalcButtons();
		
		enabled = true;
	}
	
	void CalcButtons ()
	{
		buttonRect = imageRect;
		
		if (buttonTopLeft.y < 0)
		{
			buttonRect.y += buttonTopLeft.y;
			buttonRect.height -= buttonTopLeft.y;
		}
		
		if (lang == ActiveLanguage.English)
			imageIndex = 0;
		else
			imageIndex = 1;
	}
	
	int imageIndex = 0;
	ActiveLanguage lang = ActiveLanguage.English;
	GUIStyle buttonStyle;
	void OnGUI ()
	{
		if (skin != null)
			GUI.skin = skin;
		
		if (buttonStyle == null)
		{
			buttonStyle = GUI.skin.GetStyle(buttonStyleName);
			
			if (buttonStyle.fixedWidth != 0 || buttonStyle.fixedHeight != 0)
			{
				for(int i=0;i<buttonSizes.Length;i++)
				{
					if (buttonStyle.fixedWidth != 0)
						buttonSizes[i].x = buttonStyle.fixedWidth;
					
					if (buttonStyle.fixedHeight != 0)
						buttonSizes[i].y = buttonStyle.fixedHeight;
				}
			}
		}
		
		if (images != null && images.Length > imageIndex)
			GUI.DrawTexture(imageRect, images[imageIndex]);
		else
			GUI.Box(imageRect, "");
		GUILayout.BeginArea(buttonRect);
		if (buttonTopLeft.y > 0)
			GUILayout.Space(buttonTopLeft.y);
		GUILayout.BeginHorizontal();
		GUILayout.Space(buttonTopLeft.x);
		
		for (int i=0;i<buttonText.Length;i++)
		{
			if (buttonText[i] != "")
			{
				GUI.SetNextControlName("Button"+i);
				string btext = lang == ActiveLanguage.English ? buttonText[i] : spanishButtonText[i];
				if (GUILayout.Button(btext, buttonStyle, GUILayout.Width(buttonSizes[i].x), GUILayout.Height(buttonSizes[i].y)))
				{
					if (lang == ActiveLanguage.English)
					{
						imageIndex = i*2;
					}
					else
					{
						imageIndex = i*2+1;
					}
					
					Debug.Log("Texture index: " + imageIndex);
				}
				GUILayout.Space(intraButtonSpacing);
			}	
		}
		GUI.FocusControl("Button"+imageIndex/2);
		GUILayout.EndHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.EndArea();
		
	}
	
	void Update ()
	{
		if (runner.EventIsFinished())
			enabled = false;
	}
}

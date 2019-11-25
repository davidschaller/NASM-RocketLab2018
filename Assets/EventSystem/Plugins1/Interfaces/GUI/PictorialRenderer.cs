using UnityEngine;

public class PictorialRenderer : MonoBehaviour
{
	public Texture2D backgroundImage;
	public Texture2D image;
	public Rect imageRect;
	public Color color;
	public string caption;
	public Rect captionRect;
	public float buttonBottomInset = 45;
	
	bool done = false;
	public bool Done
	{
		get
		{
			return done;
		}
		private set {}
	}
	
	string debugText;
	public string Debug
	{
		get
		{
			return debugText;
		}
		set
		{
			debugText = value;
		}
	}
	
	bool showButton = false;
	public bool ShowButton
	{
		get
		{
			return showButton;
		}
		set
		{
			showButton = value;
		}
	}
	
	bool showCaption = false;
	public bool ShowCaption
	{
		get
		{
			return showCaption;
		}
		set
		{
			showCaption = value;
		}
	}
	
	public string buttonText = "Continue";
	void OnGUI ()
	{
		GUI.depth = -10;
		GUI.skin = GUIManager.Skin;
		
		if (backgroundImage != null)
		{
			GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), backgroundImage);
		}

		if (Application.isEditor)
			GUI.Label(new Rect(Screen.width/2 - 100, 100, 200, 200), debugText);
		GUI.color = color;
		GUI.DrawTexture(imageRect, image);
		if (showCaption)
		{
			GUI.Box(captionRect, caption, "Pictorial Caption");
		}
		if (showButton)
		{
			Vector2 buttonSize = GUI.skin.GetStyle("Button").CalcSize(new GUIContent(buttonText));
			if (GUI.Button(new Rect(captionRect.x + captionRect.width/2 - 50, captionRect.y + captionRect.height-buttonSize.y/3, buttonSize.x, buttonSize.y), buttonText))
			{
				done = true;
				GUIManager.PlayButtonSound();
			}
			//GUI.Box(new Rect(captionRect.x + captionRect.width/2 - 50, captionRect.y + captionRect.height-buttonSize.y/3, buttonSize.x, 30), "");
		}

	}
}
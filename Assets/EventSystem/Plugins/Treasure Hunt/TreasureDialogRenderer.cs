using UnityEngine;
using System.Collections;

public class TreasureDialogRenderer : MonoBehaviour
{
	float stareHeight = 1.3f;
	float backoffDistance = 2.3f;
	
	public GUISkin skin;
	
	public string askText;
	public string text;
	public string okButton;

	public string askGUIStyle = "ask treasure button";
	public string boxGUIStyle = "Box";
	public string textGUIStyle = "Label";
	public string buttonGUIStyle = "Button";
	
	public bool forceCentered = false;
	public bool useFixedWH = false;
	public Rect dimensions;
	public AudioClip introAudio;
	public NPCController npcController;
	public EventPlayer introEventPlayer;
	public bool countsForTreasureHunt;
	
	GameObject player;
	void Start ()
	{
		if (skin == null)
			skin = TreasureHuntController.GUISkin;

		player = GameObject.FindWithTag("Player");
	}

	bool buttonClicked = false;
	public bool ButtonClicked
	{
		get
		{
			return buttonClicked;
		}
		set
		{
			buttonClicked = value;
		}
	}

	bool askClicked = false;
	bool wasLooping = false;
	void OnGUI ()
	{
		if (skin == null)
			GUI.skin = TreasureHuntController.GUISkin;
		else
			GUI.skin = skin;

		GUIStyle boxStyle = skin.GetStyle(boxGUIStyle);
		if (useFixedWH)
		{
			dimensions.width = boxStyle.fixedWidth;
			dimensions.height = boxStyle.fixedHeight;
		}
		if (forceCentered)
		{
			dimensions.x = Screen.width/2 - dimensions.width/2;
			dimensions.y = Screen.height/2 - dimensions.height/2;
		}

		GUI.Box(dimensions, "", boxStyle);
		GUILayout.BeginArea(boxStyle.margin.Remove(boxStyle.padding.Remove(dimensions)));
		
		if (!askClicked)
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(askText, askGUIStyle))
			{
				if (countsForTreasureHunt)
					TreasureMysteryScrollGUI.IncrementSlider();

				askClicked = true;
				if (introAudio)
				{
					wasLooping = npcController.GetComponent<AudioSource>().loop;
					npcController.GetComponent<AudioSource>().loop = false;
					npcController.GetComponent<AudioSource>().clip = introAudio;
					npcController.GetComponent<AudioSource>().Play();
				}
				
				if (introEventPlayer)
					introEventPlayer.PlayerTriggered();
				GUIManager.PlayButtonSound();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
		else
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(askText, "ask treasure button clicked");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			GUILayout.Label(text, textGUIStyle);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(okButton, buttonGUIStyle))
			{
				if (wasLooping)
					npcController.GetComponent<AudioSource>().loop = true;
				buttonClicked = true;
				enabled=false;
				GUIManager.PlayButtonSound();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

		}
		
		GUILayout.EndArea();
	}

	void LateUpdate ()
	{
		npcController.transform.LookAt(player.transform.position);
		float camY = Camera.main.transform.position.y;
		Camera.main.transform.position = npcController.transform.position + (Camera.main.transform.position - npcController.transform.position).normalized * backoffDistance;
		Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, camY, Camera.main.transform.position.z);
		CameraController.WatchPoint = npcController.transform.position + Vector3.up * stareHeight;
	}

    internal void Terminate()
    {
        throw new System.NotImplementedException();
    }
}

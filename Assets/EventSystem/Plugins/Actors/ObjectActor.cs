using UnityEngine;
using System.Collections;

public class ObjectActor : MonoBehaviour
{
	public string buttonText = "Click me!";
	public float buttonVerticalOffset = 0;
	public float maxButtonDistance = 1;
	public EventPlayerBase eventPlayer;
	public bool disableButtonOnClick = true;
	public EventPlayerBase enabledOnCompletionOfEvent;

	LOD lod;
	void Start ()
	{
		lod = new LOD(gameObject);
		InvokeRepeating("CheckLOD", 0, 0.15f);
		if (enabledOnCompletionOfEvent != null)
		{
			InvokeRepeating("CheckEventComplete", 0, 0.5f);
		}
		else
		{
			display = true;
		}
	}

	void CheckEventComplete ()
	{
		if (enabledOnCompletionOfEvent.Finished)
			display = true;
	}

	void ResetButton ()
	{
		clicked = false;
	}
	
	void Clicked ()
	{
		clicked = true;
		if (eventPlayer)
		{
			eventPlayer.PlayerTriggered();
		}
		else
		{
			Debug.LogWarning("No event player configured for " + name, gameObject);
		}
		
		if (!disableButtonOnClick)
		{
			Invoke("ResetButton", 0.5f);
		}
		else
		{
			enabled = false;
		}
	}

	bool clicked = false;
	bool display = false;
	void OnGUI ()
	{
		if (!Camera.main)
			return;
		
		GUI.skin = GUIManager.Skin;

		float dist = (transform.position - Camera.main.transform.position).magnitude;
		if (display && !clicked && !behindCamera && dist < maxButtonDistance)
		{
			Vector2 screenPos = GUIUtility.ScreenToGUIPoint(Camera.main.WorldToScreenPoint(transform.position + Vector3.up*buttonVerticalOffset));

			float w = GUI.skin.GetStyle(GUIManager.NPCNameButtonStyle).CalcSize(new GUIContent(buttonText)).x;
			if (GUI.Button(new Rect(screenPos.x, Screen.height-screenPos.y, w, 50), buttonText, GUIManager.NPCNameButtonStyle))
			{
				Clicked();
			}
		}
	}

	bool behindCamera = false;
	float lodMaxDistance = 30;
	float deactivateMaxDistance = 30;
	void CheckLOD ()
	{
		float cameraDistance = 1000;
		
		if (Camera.main == null)
		{
			behindCamera = true;
		}
		else
		{
			behindCamera = Vector3.Dot(Camera.main.transform.TransformDirection(Vector3.forward), transform.position - Camera.main.transform.position) < 0.40f;
			cameraDistance = (transform.position - Camera.main.transform.position).magnitude;
		}
		if (cameraDistance > lodMaxDistance || (behindCamera && cameraDistance > 10))
		{
			if (cameraDistance > deactivateMaxDistance)
				lod.UpdateLOD(LODState.Deactivate, behindCamera);
			else
				lod.UpdateLOD(LODState.Hidden, behindCamera);
		}
		else
		{
			lod.UpdateLOD(LODState.Visible, behindCamera);
		}
	}

}

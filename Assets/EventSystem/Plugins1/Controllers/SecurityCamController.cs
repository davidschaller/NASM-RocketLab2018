using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SecurityCamController : SecurityCamBase
{
    /*public string warningText = @"To be a successful spy, you must minimize your 
exposure to these surveillance cameras. 
Informants will be scared to talk to spies with high exposure. 
Try SpyWalking by holding down the Shift key while walking.";

    public string okText = "OK";
*/
    /*private float rectWidth = 380,
                  rectHeight = 180;

    protected GUIStyle boxStyle,
                       greenHeaderStyle,
                       labelStyle,
                       textFieldStyle,
                       buttonStyle,
                       introStyle;

    private float margin = 10;*/
    new void Awake()
    {
        base.Awake();
    }
    
    private void Start()
    {
    	//warningText = GetText("warningText");
    	//okText = GetText("okText");
        SetGUIStyles();
    }

    private void SetGUIStyles()
    {
        /*if (SkinManager.IsActive)
        {
            boxStyle = SkinManager.Main.GetStyle("Box", "Begin");
            greenHeaderStyle = SkinManager.Main.GetStyle("GreenHeader", "Begin");
            GUIStyle originallabelStyle = SkinManager.Main.GetStyle("Label", "Begin");
            labelStyle = new GUIStyle(originallabelStyle);
            labelStyle.alignment = TextAnchor.MiddleCenter;

            textFieldStyle = SkinManager.Main.GetStyle("textfield", "Begin");
            buttonStyle = SkinManager.Main.GetStyle("button", "Begin");
            introStyle = SkinManager.Main.GetStyle("Intro", "Begin");
        }
        else
            Debug.LogWarning("SkinManager is not active");*/
    }

    public void OnTriggerEnter(Collider coll)
    {
        if (coll.transform.tag.Equals("Player"))
        {
            GazeIntered(Input.GetKey("left shift"));
            if (!HasWarned)
            {
                MovementController.Main.LockPCControl();
                PCCamera.Main.ZoomIn(transform.position);
            }
        }
    }

    public void OnTriggerExit(Collider coll)
    {
        GazeExit();
    }

    private void ShowWarning()
    {
#if Crypto
    	ModalController.Main.AddMessage(GetText("warningText"),GetText("okText"), ModalProperties.Alignment.MiddleCenter, false, true, OnAcceptMessage);
		
#endif
    	/*
        if (boxStyle != null && greenHeaderStyle != null && labelStyle != null && textFieldStyle != null)
        {
            Rect warningRect = new Rect(Screen.width / 2 - rectWidth / 2, Screen.height - rectHeight - margin * 5, rectWidth, rectHeight);

            GUI.Box(warningRect, GUIContent.none, boxStyle);
            GUI.BeginGroup(warningRect);

            float warningHeight = labelStyle.CalcHeight(new GUIContent(warningText), warningRect.width - margin * 2);

            Rect warningTextRect = new Rect(rectWidth / 2 - (warningRect.width - margin * 2) / 2, margin, warningRect.width - margin * 2, warningHeight);
            GUI.Label(warningTextRect, warningText, labelStyle);

            Vector2 okSize = buttonStyle.CalcSize(new GUIContent(okText));
            okSize.x *= 1.5f;

            Rect registerRect = new Rect(rectWidth / 2 - okSize.x / 2, warningRect.height - okSize.y - margin * 2, okSize.x, okSize.y);

            if (GUI.Button(registerRect, okText, buttonStyle))
            {
                HasWarned = true;
                PCCamera.Main.ZoomOut(transform.position);
                MovementController.Main.UnlockPCControl(true);
            }

            GUI.EndGroup();
        }*/
    }
    
    private void OnAcceptMessage()
    {
		HasWarned = true;
		PCCamera.Main.ZoomOut(transform.position);
		MovementController.Main.UnlockPCControl(true);
    }
    
    private void OnGUI()
    {
        if (!HasWarned)
        {
            ShowWarning();
        }
    }
}

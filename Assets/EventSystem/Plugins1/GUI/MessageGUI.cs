using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// TODO: Use callbacks instead of flags

public class MessageGUI : MonoBehaviour
{
    private GUIStyle boxStyle,
                     labelStyle,
                     buttonStyle;

    private float messageRectWidth = 300,
                  messageRectHeight = 165;

    private static string messageText = string.Empty,
                          button1Text = string.Empty,
                          button2Text = string.Empty;

    private string OK = "OK";
    private float topPadding = 30,
                  bottomPadding = 20,
                  okWidth = 100,
                  buttonMargin = 30;

    // TODO: Remove this
    public static bool IsClosed
    {
        get
        {
            return string.IsNullOrEmpty(messageText);
        }
    }

    private static string pickedVariant = null;

    #region core functions

    private void Awake()
    {
    }

    private void SetGUIStyles()
    {
        if (SkinManager.IsActive)
        {
            boxStyle = SkinManager.Main.GetStyle("Box", "Begin");
            GUIStyle origLabelStyle = SkinManager.Main.GetStyle("Label", "Begin");
            labelStyle = new GUIStyle(origLabelStyle);
            labelStyle.alignment = TextAnchor.MiddleCenter;
            buttonStyle = SkinManager.Main.GetStyle("button", "Begin");
        }
        else
            Debug.LogWarning("SkinManager is not active");
    }

    private void Start()
    {
        SetGUIStyles();
    }

    private void OnGUI()
    {
        GUI.depth = 0;

        if (!string.IsNullOrEmpty(messageText))
        {
            if (!string.IsNullOrEmpty(button1Text) && !string.IsNullOrEmpty(button2Text))
                RenderDialog();
            else
                RenderMessage();
        }
    }

    #endregion

    #region rendering

    private void RenderMessage()
    {
        Vector2 textSize = labelStyle.CalcSize(new GUIContent(messageText));
        float textHeight = labelStyle.CalcHeight(new GUIContent(messageText), messageRectWidth * 0.75f);
        float textWidth = textHeight > textSize.y ? messageRectWidth * 0.75f : textSize.x;

        float gap = messageRectHeight - textHeight - topPadding - bottomPadding - buttonMargin;
        if (gap < 0)
            messageRectHeight -= gap;

        Rect messageRect = new Rect(0, Screen.height - messageRectHeight, messageRectWidth, messageRectHeight);

        GUI.Box(messageRect, GUIContent.none, boxStyle);
        GUI.BeginGroup(messageRect);

        GUI.Label(new Rect(messageRectWidth / 2 - textWidth / 2, topPadding, textWidth, textHeight), messageText, labelStyle);

        if (GUI.Button(new Rect(messageRectWidth / 2 - okWidth / 2, messageRect.height - buttonStyle.CalcSize(new GUIContent(OK)).y - bottomPadding, okWidth, buttonStyle.CalcSize(new GUIContent(OK)).y), OK, buttonStyle))
        {
            messageText = string.Empty;
        }

        GUI.EndGroup();

    }


    void RenderDialog()
    {
        Vector2 textSize = labelStyle.CalcSize(new GUIContent(messageText));
        float textHeight = labelStyle.CalcHeight(new GUIContent(messageText), messageRectWidth * 0.75f);
        float textWidth = textHeight > textSize.y ? messageRectWidth * 0.75f : textSize.x;

        float gap = messageRectHeight - textHeight - topPadding - bottomPadding - buttonMargin;
        if (gap < 0)
            messageRectHeight -= gap;

        Rect messageRect = new Rect(0, Screen.height - messageRectHeight, messageRectWidth, messageRectHeight);

        GUI.Box(messageRect, GUIContent.none, boxStyle);
        GUI.BeginGroup(messageRect);

        GUI.Label(new Rect(messageRectWidth / 2 - textWidth / 2, topPadding, textWidth, textHeight), messageText, labelStyle);

        Rect button1Rect = new Rect(messageRectWidth / 2 - okWidth,
            messageRect.height - buttonStyle.CalcSize(new GUIContent(OK)).y - bottomPadding,
                okWidth, buttonStyle.CalcSize(new GUIContent(OK)).y);

        Rect button2Rect = new Rect(messageRectWidth / 2,
            messageRect.height - buttonStyle.CalcSize(new GUIContent(OK)).y - bottomPadding,
                okWidth, buttonStyle.CalcSize(new GUIContent(OK)).y);


        if (GUI.Button(button1Rect, button1Text, buttonStyle))
        {
            pickedVariant = button1Text;
            messageText = string.Empty;
            button1Text = string.Empty;
            button2Text = string.Empty;
        }

        if (GUI.Button(button2Rect, button2Text, buttonStyle))
        {
            pickedVariant = button2Text;
            messageText = string.Empty;
            button1Text = string.Empty;
            button2Text = string.Empty;
        }

        GUI.EndGroup();      
    }

    #endregion

    #region static functions

    public static void ShowMessage(string text)
    {
        messageText = text;
        button1Text = string.Empty;
        button2Text = string.Empty;
    }

    // TODO: Add callback
    public static string ShowDialog(string text, string first, string second)
    {
        string result = null;

        if (pickedVariant != null)
        {
            result = pickedVariant;
            pickedVariant = null;

            return result;
        }
        
        messageText = text;
        button1Text = first;
        button2Text = second;

        return result;
    }

    // TODO: Remove this
    public static void CloseAll()
    {
        pickedVariant = null;
        messageText = string.Empty;
        button1Text = string.Empty;
        button2Text = string.Empty;
    }

    #endregion
}


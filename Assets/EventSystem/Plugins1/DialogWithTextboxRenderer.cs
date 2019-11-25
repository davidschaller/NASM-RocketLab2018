using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogWithTextboxRenderer : MonoBehaviour
{
    public GUISkin skin;

    public string labelText,
                  buttonText,
                  boxStyleName,
                  labelStyleName,
                  buttonStyleName,
                  textboxStyleName;

    public enum Alignment
    {
        LowerLeft,
        LowerCenter,
        LowerRight
    }

    public Rect rect;

    public DialogFinished Callback { get; set; }

    public string inputText = string.Empty;

    GUIStyle boxStyle,
             labelStyle,
             textboxStyle,
             buttonStyle;

    void OnGUI()
    {
        if (skin != null)
        {
            GUI.skin = skin;

            if (AllStylesExist())
            {
                Rect currRect = rect;
                currRect.x = boxStyle.margin.left;
                currRect.y = Screen.height - currRect.height - boxStyle.margin.bottom;

                switch (boxStyle.alignment)
                {
                    case TextAnchor.LowerCenter:
                        currRect.x = Screen.width / 2 - currRect.width / 2;
                        break;
                    case TextAnchor.LowerRight:
                        currRect.x = Screen.width - currRect.width - boxStyle.margin.right;
                        break;
                }

                GUI.BeginGroup(currRect, GUIContent.none, boxStyle);
                {
                    Rect labelRect = new Rect(labelStyle.margin.left, labelStyle.margin.top, labelStyle.fixedWidth, labelStyle.fixedHeight);
                    GUI.Label(labelRect, labelText, labelStyle);

                    Rect inputTextRect = new Rect(labelRect.xMax - textboxStyle.fixedWidth - textboxStyle.margin.right,
                        labelRect.yMax - textboxStyle.fixedHeight - textboxStyle.margin.bottom, textboxStyle.fixedWidth, textboxStyle.fixedHeight);

                    inputText = GUI.TextField(inputTextRect, inputText, textboxStyle);
                }
                GUI.EndGroup();

                Rect buttonRect = new Rect(currRect.x + currRect.width / 2 - buttonStyle.fixedWidth / 2, 
                    currRect.yMax - buttonStyle.fixedHeight / 2, buttonStyle.fixedWidth, buttonStyle.fixedHeight);

                if (GUI.Button(buttonRect, buttonText, buttonStyle))
                {
                    if (IsValid(inputText))
                    {
                        PrepareManager.PrepareProperties.commanderName = inputText;

                        if (Callback != null)
                            Callback();
                    }
                }
            }
        }
    }

    bool AllStylesExist()
    {
        if (boxStyle == null)
        {
            boxStyle = skin.GetStyle(boxStyleName);
        }
        else
        {
            rect = new Rect(boxStyle.margin.left, 0, boxStyle.fixedWidth, boxStyle.fixedHeight);
        }

        if (labelStyle == null)
            labelStyle = skin.GetStyle(labelStyleName);

        if (textboxStyle == null)
            textboxStyle = skin.GetStyle(textboxStyleName);

        if (buttonStyle == null)
            buttonStyle = skin.GetStyle(buttonStyleName);

        return boxStyle != null && labelStyle != null && textboxStyle != null && buttonStyle != null;
    }

    bool IsValid(string inputText)
    {
        return !string.IsNullOrEmpty(inputText);
    }

    public void SetStyleNames(string boxStyleName, string labelStyleName, string buttonStyleName, string textboxStyleName)
    {
        this.boxStyleName = boxStyleName;
        this.labelStyleName = labelStyleName;
        this.buttonStyleName = buttonStyleName;
        this.textboxStyleName = textboxStyleName;
    }

    public void Terminate()
    {
        enabled = false;

        Destroy(gameObject);
    }
}
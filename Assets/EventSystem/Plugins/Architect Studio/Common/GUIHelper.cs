using UnityEngine;
using System;

public static class GUIHelpers
{
    public static string ShowTextBoxWithHint(Rect p_rect, string p_text, string p_hintText, Event p_event, GUIStyle p_style, GUIStyle p_hintStyle, ref bool ref_isFocused)
    {
        if (p_event.type == EventType.MouseDown && p_event.button == 0 && p_rect.Contains(p_event.mousePosition))
        {
            ref_isFocused = true;
        }
        else if (p_event.type == EventType.MouseDown && p_event.button == 0 && !p_rect.Contains(p_event.mousePosition))
        {
            ref_isFocused = false;
        }

        p_text = GUI.TextArea(p_rect, string.IsNullOrEmpty(p_text) && !ref_isFocused ? p_hintText : p_text, 
            string.IsNullOrEmpty(p_text) && !ref_isFocused ? p_hintStyle : p_style);

        if (p_text.Equals(p_hintText))
            p_text = string.Empty;
        
        return p_text;
    }

    public static int DrawSubTabSwitcher(int p_input, int p_min, int p_max, GUIStyle p_arrowStyle, GUIStyle p_arrowLabelStyle, GUIStyle p_pickedStyle, Rect p_panel)
    {
        int result = p_input;

        float buttonWidth = p_arrowStyle.normal.background.width;
        float buttonHeight = p_arrowStyle.normal.background.height;

        Vector2 leftArrowCenter = new Vector2(p_panel.x - buttonWidth / 2, p_panel.y + buttonHeight / 2);
        GUIUtility.RotateAroundPivot(180, leftArrowCenter);

        Rect leftButtonRect = new Rect(p_panel.x - buttonWidth * 1.5f, p_panel.y - p_panel.height + buttonHeight * 4, buttonWidth, buttonHeight);

        if (GUI.Button(leftButtonRect, string.Empty, p_arrowStyle))
        {
            result--;
            if (result < p_min)
                result = p_max;
        }

        GUI.matrix = Matrix4x4.identity;

        Rect roghtButtonRect = new Rect(p_panel.x + p_panel.width - buttonWidth / 2 - 5, p_panel.y + p_panel.height - buttonHeight * 4, buttonWidth, buttonHeight);

        if (GUI.Button(roghtButtonRect, string.Empty, p_arrowStyle))
        {
            result++;
            if (result > p_max)
                result = p_min;
        }

        string leftText = result - 1 < p_min ? GetSubTabName(p_max) : GetSubTabName(result - 1);
        Vector2 buttonSize = p_arrowStyle.CalcSize(new GUIContent(leftText));

        Rect leftButtonTextRect = new Rect(p_panel.x - buttonSize.x / 2 + 13, p_panel.y + p_panel.height - buttonSize.y * 9.6f, buttonSize.x, buttonSize.y);

        GUI.Label(leftButtonTextRect, leftText, p_arrowLabelStyle);

        string rightText = GetSubTabName(result);
        buttonSize = p_arrowStyle.CalcSize(new GUIContent(rightText));

        Rect rightButtonTextRect = new Rect(p_panel.x + p_panel.width / 2 - buttonSize.x / 2, p_panel.y + p_panel.height - buttonSize.y * 9.6f, buttonSize.x, buttonSize.y);

        GUI.Label(rightButtonTextRect, rightText.ToUpper(), p_pickedStyle);

        string centerText = (result + 1) > p_max ? GetSubTabName(p_min) : GetSubTabName(result + 1);

        buttonSize = p_arrowStyle.CalcSize(new GUIContent(centerText));

        Rect centerTextRect = new Rect(p_panel.x + p_panel.width - buttonSize.x / 2 - 5, p_panel.y + p_panel.height - buttonSize.y * 9.6f, buttonSize.x, buttonSize.y);

        GUI.Label(centerTextRect, centerText, p_arrowLabelStyle);

        return result;
    }


    private static string GetSubTabName(int p_index)
    {
        return Enum.GetNames(typeof(InteriorDesignGUI.SubTabs))[p_index].ToUpper();
    }
}


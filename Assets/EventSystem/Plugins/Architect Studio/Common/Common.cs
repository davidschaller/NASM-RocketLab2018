using UnityEngine;
using System.Globalization;
using System.Text;
using System;
using System.Text.RegularExpressions;

public static class Common
{
    public static int SetCullingMask(params string[] p_layers)
    {
        int result = 0;

        foreach (string layer in p_layers)
        {
            result |= 1 << LayerMask.NameToLayer(layer);
        }

        return result;
    }

    public static Vector2 GetContentSize(GUIStyle p_style, GUIContent p_content)
    {
        return p_style.CalcSize(p_content);
    }

    public static string Vector3ToString(Vector3 input)
    {
        string output = string.Empty;

        output += string.Format("({0}, {1}, {2})", 
            input.x.ToString().Replace(',', '.'),
                input.y.ToString().Replace(',', '.'),
                    input.z.ToString().Replace(',', '.'));

        return output;
    }

    public static Vector3 StringToVector3(string input)
    {
        Vector3 output = Vector3.zero;

        input = input.Replace("(", "");
        input = input.Replace(")", "");

        output.x = float.Parse(input.Split(',')[0], CultureInfo.InvariantCulture.NumberFormat);
        output.y = float.Parse(input.Split(',')[1], CultureInfo.InvariantCulture.NumberFormat);
        output.z = float.Parse(input.Split(',')[2], CultureInfo.InvariantCulture.NumberFormat);

        return output;
    }

    public static string RectToString(Rect rect)
    {
        string output = string.Empty;

        output += string.Format("({0}, {1}, {2}, {3})",
            rect.x.ToString().Replace(',', '.'),
                rect.y.ToString().Replace(',', '.'),
                    rect.width.ToString().Replace(',', '.'),
                        rect.height.ToString().Replace(',', '.'));

        return output;
    }

    public static Rect StringToRect(string input)
    {
        input = input.Replace("(", "");
        input = input.Replace(")", "");

        Rect result = new Rect();
        result.x = float.Parse(input.Split(',')[0], CultureInfo.InvariantCulture.NumberFormat);
        result.y = float.Parse(input.Split(',')[1], CultureInfo.InvariantCulture.NumberFormat);
        result.width = float.Parse(input.Split(',')[2], CultureInfo.InvariantCulture.NumberFormat);
        result.height = float.Parse(input.Split(',')[3], CultureInfo.InvariantCulture.NumberFormat);

        return result;
    }

    internal static float[] Vector3ToFloatArray(Vector3 vector3)
    {
        return new float[3] { vector3.x, vector3.y, vector3.z };
    }

    internal static Vector3 FloatArrayToVector3(float[] array)
    {
        return new Vector3(array[0], array[1], array[2]);
    }

    public static GUIStyle ApplyStyle(GUIStyle p_itemStyle, InteriorItemDefinition p_item, InteriorItemDefinition p_selectedItem)
    {
        GUIStyle result = GUIStyle.none;

        GUIStyle selectedItemStyle = new GUIStyle(p_itemStyle);
        selectedItemStyle.normal = selectedItemStyle.active;

        if (p_selectedItem != null)
        {
            if ((p_item.asNavigation && p_item.subtab == p_selectedItem.subtab) || (p_item == p_selectedItem))
            {
                result = selectedItemStyle;
            }
            else
            {
                result = p_itemStyle;
            }
        }
        else
            result = p_itemStyle;

        return result;
    }

    internal static Vector3 CursorTo3D(Vector2 p_origCursor)
    {
        Vector3 result = p_origCursor;
        result.y = Screen.height - result.y;

        return result;
    }

    public static Texture2D UpdateTexture(float p_selIAlpha)
    {
        Texture2D result = new Texture2D(2, 2);

        for (int i = 0; i <= result.height; i++)
            for (int j = 0; j <= result.width; j++)
            {
                result.SetPixel(i, j, new Color(1f, 1f, 1f, p_selIAlpha));
            }

        result.Apply();

        return result;
    }

    public static Texture2D UpdateTexture(Texture2D p_input, float p_selIAlpha)
    {
        Texture2D result = new Texture2D(p_input.width, p_input.height);

        for (int i = 0; i <= p_input.height; i++)
            for (int j = 0; j <= p_input.width; j++)
            {
                Color current = p_input.GetPixel(i, j);
                if (current.a != 0)
                {
                    current.a = p_selIAlpha;
                }

                result.SetPixel(i, j, current);
            }

        result.Apply();

        return result;
    }

    public static int SelectionGrid(Rect p_switcherBtnRect, int p_switcherIndex, GUIContent[] p_switcherContent,
        GUIStyle p_switcherLeft, GUIStyle p_switcherLeftActive, GUIStyle p_switcherRight, GUIStyle p_switcherRightActive)
    {
        Rect left = new Rect(p_switcherBtnRect.x, p_switcherBtnRect.y, 87, 22);
        Rect right = new Rect(p_switcherBtnRect.x + 82, p_switcherBtnRect.y, 74, 22);


        if (GUI.Button(left, p_switcherContent[0], p_switcherIndex == 0 ? p_switcherLeftActive : p_switcherLeft))
        {
            return 0;
        }
        if (GUI.Button(right, p_switcherContent[1], p_switcherIndex == 0 ? p_switcherRight : p_switcherRightActive))
        {
            return 1;
        }
        return p_switcherIndex;
    }

    public static Material GetMaterialByName(string p_materialName)
    {
        InteriorItemDefinition item =
            InteriorItemManager.InteriorItemsList.Find(delegate(InteriorItemDefinition p)
            {
                return p.material && p.material.name.Equals(p_materialName);
            });

        if (item)
            return item.material;
        else
        {
            return null;
        }
    }

    public static Vector2 ToScreen2D(Vector3 p_pos, Camera p_camera)
    {
        Vector2 result = p_camera.WorldToScreenPoint(p_pos);
        result.y = Screen.height - result.y;

        return result;
    }
    
    public static Texture2D ResizeTexture(Texture2D p_original, int p_width, int p_height, int algo)
    {
        Texture2D resizedImage = new Texture2D(p_width, p_height);
        Texture2D bTemp = (Texture2D)GameObject.Instantiate(p_original, Vector3.zero, Quaternion.identity);
        switch (algo)
        {
            case (1): //bilinear
                float fraction_x, fraction_y, one_minus_x, one_minus_y;
                int ceil_x, ceil_y, floor_x, floor_y;
                Color c1 = new Color();
                Color c2 = new Color();
                Color c3 = new Color();
                Color c4 = new Color();
                float red, green, blue;

                float b1, b2;

                float nXFactor = (float)bTemp.width / (float)p_width;
                float nYFactor = (float)bTemp.height / (float)p_height;

                for (int x = 0; x < p_width; ++x)
                    for (int y = 0; y < p_height; ++y)
                    {
                        // Setup
                        floor_x = (int)Mathf.Floor((x * nXFactor));
                        floor_y = (int)Mathf.Floor((y * nYFactor));
                        ceil_x = floor_x + 1;
                        if (ceil_x >= bTemp.width) ceil_x = floor_x;

                        ceil_y = floor_y + 1;
                        if (ceil_y >= bTemp.height) ceil_y = floor_y;

                        fraction_x = x * nXFactor - floor_x;
                        fraction_y = y * nYFactor - floor_y;
                        one_minus_x = 1.0f - fraction_x;
                        one_minus_y = 1.0f - fraction_y;

                        c1 = bTemp.GetPixel(floor_x, floor_y);
                        c2 = bTemp.GetPixel(ceil_x, floor_y);
                        c3 = bTemp.GetPixel(floor_x, ceil_y);
                        c4 = bTemp.GetPixel(ceil_x, ceil_y);

                        // Blue

                        b1 = (one_minus_x * c1.b + fraction_x * c2.b);

                        b2 = (one_minus_x * c3.b + fraction_x * c4.b);

                        blue = (one_minus_y * (float)(b1) + fraction_y * (float)(b2));

                        // Green

                        b1 = (one_minus_x * c1.g + fraction_x * c2.g);

                        b2 = (one_minus_x * c3.g + fraction_x * c4.g);

                        green = (one_minus_y * (float)(b1) + fraction_y * (float)(b2));

                        // Red

                        b1 = (one_minus_x * c1.r + fraction_x * c2.r);

                        b2 = (one_minus_x * c3.r + fraction_x * c4.r);

                        red = (one_minus_y * (float)(b1) + fraction_y * (float)(b2));
                        resizedImage.SetPixel(x, y, new Color(red, green, blue));

                    }
                resizedImage.Apply();
                break;
            default:
                Debug.Log("not implemented");
                break;
        }

        return resizedImage;
    }

    public static string NormalizeToNumeric(string p_input)
    {
        StringBuilder result = new StringBuilder();

        if (!string.IsNullOrEmpty(p_input))
        {
            foreach (Char chr in p_input)
            {
                if (Char.IsDigit(chr))
                {
                    result.Append(chr);
                }
            }
        }
        return result.ToString();
    }


    internal static string NormalizeToName(string p_input)
    {
        StringBuilder result = new StringBuilder();

        if (!string.IsNullOrEmpty(p_input))
        {
            string[] split = p_input.Split(' ');

            foreach (string part in split)
            {
                bool isFirst = true;
                foreach (Char chr in part)
                {
                    if (Char.IsLetter(chr))
                    {                        
                        if (isFirst)
                        {
                            result.Append(chr.ToString().ToUpper());
                            isFirst = false;
                        }
                        else
                            result.Append(chr.ToString());
                    }
                }

                result.Append(' ');
            }
        }

        return result.ToString().TrimStart();
    }

    internal static float FeetToMeter(int p_input)
    {
        return p_input * 0.3048f;
    }

    public static bool IsValidEmail(string email)
    {
        const string emailFilter = "^([a-zA-Z0-9_.-])+@(([a-zA-Z0-9-])+\\.)+([a-zA-Z0-9]{2,4})$";
        return Regex.IsMatch(email, emailFilter, RegexOptions.None);
    }

    public static void DrawTextureAt(Vector2 p_loc, Texture2D p_tex)
    {
        GUI.DrawTexture(new Rect(p_loc.x, p_loc.y, p_tex.width, p_tex.height), p_tex);
    }

    public static AudioSource AddAudioTo(GameObject go)
    {
        if (!go.GetComponent<AudioSource>())
        {
            go.AddComponent<AudioSource>();
        }

        go.GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Linear;
        go.GetComponent<AudioSource>().minDistance = 0;
        go.GetComponent<AudioSource>().maxDistance = 50;

        return go.GetComponent<AudioSource>();
    }
}

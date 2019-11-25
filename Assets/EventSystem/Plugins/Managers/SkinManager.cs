using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public List<GUISkin> skins;

    private List<Style> styles;
    public static SkinManager Main { get; private set; }

    public static bool IsActive
    {
        get
        {
            return Main != null;
        }
    }
    
    private void Awake()
    {
        if (Main == null)
        {
            Main = this;
            styles = new List<Style>();

            if (skins != null)
            {
                foreach (GUISkin skin in skins)
                {
                    styles.Add(new Style(skin.box.name, skin.name, skin.box));
                    styles.Add(new Style(skin.button.name, skin.name, skin.button));
                    styles.Add(new Style(skin.label.name, skin.name, skin.label));
                    styles.Add(new Style(skin.textField.name, skin.name, skin.textField));
                    styles.Add(new Style(skin.toggle.name, skin.name, skin.toggle));

                    if (skin.customStyles != null)
                    {
                        foreach (GUIStyle style in skin.customStyles)
                            styles.Add(new Style(style.name, skin.name, style));
                    }
                }
            }
        }
    }

    public GUIStyle GetStyle(string styleName, string skinName)
    {
        if (styles == null)
        {
            Awake();
        }

        if (styles != null)
        {
            Style style = styles.Find(s => s.Name.Equals(styleName, System.StringComparison.InvariantCultureIgnoreCase)
                && s.Skin.Equals(skinName, System.StringComparison.InvariantCultureIgnoreCase));

            if (style != null)
                return style.Value;
        }
        
        return null;
    }

    private class Style
    {
        public string Name { get; private set; }
        public string Skin { get; private set; }
        public GUIStyle Value { get; private set; }

        public Style(string name, string skin, GUIStyle value)
        {
            Name = name;
            Skin = skin;
            Value = value;
        }
    }

    public GUISkin GetSkinByName(string name)
    {
        return skins.Find(p => p.name.Equals(name));
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Localizer : MonoBehaviour
{
    static string DEFAULT_LANG = "en";
    static string PATH_TEMPLATE = "{0}/{1}.xml";

    static Dictionary<string, WWW> wwwDict = new Dictionary<string, WWW>();

    static Dictionary<string, Items> itemsDict = new Dictionary<string, Items>();

    static string BaseURL
    {
        get
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
                return Application.dataPath + "/xml/";
            else
                return "file://" + Application.dataPath + "/../xml/";
        }
    }

    public static void RegisterPage(string page)
    {
        if (!wwwDict.ContainsKey(page))
        {
            wwwDict.Add(page, new WWW(string.Format(PATH_TEMPLATE, BaseURL, page)));
        }
    }

    public static string GetText(string page, string id, string lang)
    {
        if (itemsDict.ContainsKey(page))
        {
            return itemsDict[page].GetText(id, lang);
        }
        else if (wwwDict.ContainsKey(page))
        {
            if (wwwDict[page].isDone)
            {
                if (!itemsDict.ContainsKey(page))
                    itemsDict.Add(page, Desrializator.Deserialize(wwwDict[page].text));
            }
        }

        return string.Empty;
    }

    public static string GetText(string page, string id)
    {
        return GetText(page, id, DEFAULT_LANG);
    }

    
}


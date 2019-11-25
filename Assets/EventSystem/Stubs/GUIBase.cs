using UnityEngine;

public class GUIBase : MonoBehaviour
{
    public string pageName = "page_name";

    protected virtual void Awake()
    {
        Debug.Log("Registering " + pageName + " xml");
        Localizer.RegisterPage(pageName);
    }

    protected string GetText(string id, string lang)
    {
        return Localizer.GetText(pageName, id, lang);
    }

    protected string GetText(string id)
    {
        return Localizer.GetText(pageName, id);
    }
}


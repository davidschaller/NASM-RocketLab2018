using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FullscreenButton : MonoBehaviour
{
    private Resolution myDefaultResolution;
    private Text myText;

    void Awake()
    {
        myText = GetComponentInChildren<Text>();
        myDefaultResolution = Screen.currentResolution;
    }

    public void SwitchFullScreen()
    {
        bool fullscreen = !Screen.fullScreen;

        if (fullscreen)
        {
            Resolution[] resolutions = Screen.resolutions;
            Resolution highestRes = resolutions[resolutions.Length - 1];
            Screen.SetResolution(highestRes.width, highestRes.height, true);

            myText.text = "Windowed";
        }
        else
        {
            Screen.SetResolution(myDefaultResolution.width, myDefaultResolution.height, false);
            
            myText.text = "Fullscreen";
        }
    }
}

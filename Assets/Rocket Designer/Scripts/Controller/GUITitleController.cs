using UnityEngine;
using System.Collections;

public class GUITitleController : MonoBehaviour
{
    const string TITLE_BKG = "title_background",
                 START_DESIGNING = "start_designing_rockets_button";

    public GUISkin devTempSkin;

    GUIStyle titleBkgStyle,
             startDEsigningRocketsButtonStyle;

    public Rect startDesigningRocketsButtonRect;

    public EventPlayer levelLoaderEventPlayer;

    bool loading = false;

    void OnEnable()
    {
        titleBkgStyle = devTempSkin.GetStyle(TITLE_BKG);
        startDEsigningRocketsButtonStyle = devTempSkin.GetStyle(START_DESIGNING);
    }

    void OnGUI()
    {
        GUI.skin = devTempSkin;

        GUI.DrawTexture(new Rect(Screen.width / 2 - titleBkgStyle.fixedWidth / 2,
            Screen.height / 2 - titleBkgStyle.fixedHeight / 2, titleBkgStyle.fixedWidth, titleBkgStyle.fixedHeight), titleBkgStyle.normal.background);

        GUI.enabled = !loading;

        if (GUI.Button(startDesigningRocketsButtonRect, GUIContent.none, startDEsigningRocketsButtonStyle))
        {
            if (levelLoaderEventPlayer)
                levelLoaderEventPlayer.PlayerTriggered();

            loading = !loading;
        }
    }
}

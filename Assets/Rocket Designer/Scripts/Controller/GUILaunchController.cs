using UnityEngine;
using System.Collections;

public class GUILaunchController : LaunchControllerBase
{
    const string LABEL_STYLE = "countdown-label",
                 BUTTON_STYLE_NAME = "sample_button";

    public GUISkin guiSkin;

    GUIStyle buttonStyle,
             labelStyle;

    float time = 0,
          timerLeft = 0;

    public Vector2 launchButtonSize = new Vector2(200, 30),
                   virtualSpaceButtonSize = new Vector2(150, 50),
                   pauseCountdownButtonSize = new Vector2(200, 30),
                   takePictureButtonSize = new Vector2(100, 50),
                   countDownLabelSize = new Vector2(200, 50);

    public AudioClip pauseAudioClip,
                     tapAudioClip,
                     takePictureAudioClip,
                     launchAudioClip;

    public float margin = 10,
                 marginPause = 100,
                 marginPauseRight = 50;

    bool countDown = false,
         paused = false,
         launched = false;

    void Awake()
    {
        enabled = false;
    }

    void OnEnable()
    {
        if (guiSkin)
        {
            buttonStyle = guiSkin.GetStyle(BUTTON_STYLE_NAME);
            labelStyle = guiSkin.GetStyle(LABEL_STYLE);
        }

        if (xml)
        {
            launchButtonText = DialogXMLParser.GetText(xml.text, launchButtonSection, ActiveLanguage.English);
            virtualSpaceButtonText = DialogXMLParser.GetText(xml.text, virtualSpaceButtonSection, ActiveLanguage.English);
            pauseCountdownButtonText = DialogXMLParser.GetText(xml.text, pauseCountdownButtonSection, ActiveLanguage.English);
            takePictureText = DialogXMLParser.GetText(xml.text, takePictureSection, ActiveLanguage.English);
        }
    }

    void FixedUpdate()
    {
        if (countDown && !paused && !launched)
        {
            timerLeft -= Time.deltaTime;

            if (timerLeft <= 0)
            {
                timerLeft = 0;
                launched = true;
                launchClickCallback(true);
            }
        }
    }

    void OnGUI()
    {
        if (guiSkin)
        {
            Rect virtualRealButtonRect = new Rect(margin, Screen.height - virtualSpaceButtonSize.y - margin, virtualSpaceButtonSize.x, virtualSpaceButtonSize.y);
            if (GUI.Button(virtualRealButtonRect, virtualSpaceButtonText, buttonStyle))
            {
                PlayTap(tapAudioClip);
            }

            if (!countDown)
            {
                Rect launchButtonRect = new Rect(Screen.width / 2 - launchButtonSize.x / 2, Screen.height - launchButtonSize.y - margin, launchButtonSize.x, launchButtonSize.y);
                if (GUI.Button(launchButtonRect, launchButtonText, buttonStyle))
                {
                    if (launchClickCallback != null)
                    {
                        launchClickCallback(false);
                    }

                    countDown = true;

                    PlayTap(launchAudioClip);
                }
            }

            GUI.enabled = countDown;

            Rect pauseButtonRect = new Rect(Screen.width - pauseCountdownButtonSize.x - marginPauseRight, Screen.height - pauseCountdownButtonSize.y - marginPause, pauseCountdownButtonSize.x, pauseCountdownButtonSize.y);

            if (!launched)
            {
                if (GUI.Button(pauseButtonRect, pauseCountdownButtonText, buttonStyle))
                {
                    paused = !paused;
                    PlayTap(pauseAudioClip);
                }
            }

            GUI.enabled = true;

            Rect countDownRect = new Rect(pauseButtonRect.x, pauseButtonRect.y - countDownLabelSize.y, countDownLabelSize.x, countDownLabelSize.y);
            GUI.Label(countDownRect, timerLeft.ToString(), labelStyle);
            
            Rect takePictureButtonRect = new Rect(Screen.width - takePictureButtonSize.x - margin, Screen.height - takePictureButtonSize.y - margin, takePictureButtonSize.x, takePictureButtonSize.y);
            if (GUI.Button(takePictureButtonRect, takePictureText, buttonStyle))
            {
                PlayTap(takePictureAudioClip);
                if (takePictureClickCallback != null)
                {
                    takePictureClickCallback();
                }
            }
        }
    }

    LaunchCallback launchClickCallback;

    SimpleCallback virtualRealClickCallback,
                   takePictureClickCallback;

    public override void Toggle(bool on)
    {
        enabled = on;
    }

    void PlayTap(AudioClip clip)
    {
        if (clip)
        {
            if (!GetComponent<AudioSource>())
                gameObject.AddComponent<AudioSource>();

            GetComponent<AudioSource>().PlayOneShot(clip);
        }
    }

    public override void Init(float countDownTime, LaunchControllerBase.LaunchCallback launchClick, int launchAttempts, string flightSummary, bool isSuccess)
    {
        throw new System.NotImplementedException();
    }
}

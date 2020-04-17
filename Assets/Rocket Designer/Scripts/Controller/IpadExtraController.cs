using UnityEngine;
using System.Collections;
#if !UNITY_WEBGL
using System.IO;
#endif

public class IpadExtraController : MonoBehaviour
{
    public AudioClip takePictureAudioClip,
                     realSpaceportAudioClip,
                     clickAudioClip;

    public float screenshotEffectTime = 1;

    Rect targetScreenshotRect;

    public Vector2 targetScreenshotCorrection = new Vector2(-5, 10);

    WebCamTexture webcamTexture;

    UIImageButton takePictureButton,
                  realSpaceportButton;

    public Camera webCameraCam;
    Camera guiCam2D;

    public Camera newRealWebCam;

    public Camera[] otherCameras;
    int[] cachedCameraMasks;
    CameraClearFlags[] cameraClearFlags;

    public float screenAngleRot = 2;
    public string emailDefaultRecipient = "";
    public string emailDefaultSubject = "My Rocket!";
    public string emailDefaultBody = "";

    public string facebookMessage = "Check out my Rocket";
    public string twitterMessage = "Check out my Rocket";

    public string shareButtonText = "Share",
                  hideShareButtonText = "Hide";

	UIImageButton shareButton;
	UILabel shareLabel;
	bool shareIsOpen = false;

	Transform shareGroup;

	public string webiteForPictures = "http://somewebite.com";

#if !UNITY_WEBGL

	void Start()
    {
        webCameraCam.enabled = false;
        newRealWebCam.gameObject.SetActive(false);

        Transform anchor = ResolutionDetector.Main.Gui2D.transform;

        guiCam2D = anchor.GetChild(0).GetComponent<Camera>();

        foreach (Transform tr in anchor.GetChild(0))
        {
            if (tr.name == "Anchor-General")
            {
                foreach (Transform panel in tr)
                {
                    switch (panel.name)
                    {
                        case "Flight Panel":

                            foreach (Transform child in panel)
                            {
                                switch (child.name)
                                {
                                    case "Button: Take Picture (ipad only)":
                                        takePictureButton = RocketDesignerCommon.MakeButton(child, takePictureAudioClip);
                                        takePictureButton.gameObject.SetActive(false);
                                        break;
                                    case "Button: Virtual/Real Spaceport (ipad only)":
                                        realSpaceportButton = RocketDesignerCommon.MakeButton(child, realSpaceportAudioClip);
                                        realSpaceportButton.gameObject.SetActive(false);
                                        break;
                                    case "Share Group":
                                        shareGroup = child;
                                        shareGroup.gameObject.SetActive(false);

                                        foreach (Transform insideTr in child)
                                        {
                                            UIImageButton btn = null;

                                            switch (insideTr.name)
                                            {
                                                case "Button: Send by email":
                                                    btn = RocketDesignerCommon.MakeButton(insideTr, clickAudioClip);
                                                    break;
                                                case "Button: Share on Facebook":
                                                    btn = RocketDesignerCommon.MakeButton(insideTr, clickAudioClip);
                                                    break;
                                                case "Button: Share on Twitter":
                                                    btn = RocketDesignerCommon.MakeButton(insideTr, clickAudioClip);
                                                    break;
                                            }

                                            UIEventListener.Get(btn.gameObject).onClick = ShareButtons;
                                        }
                                        break;
                                    case "Button: Share":
                                        shareButton = RocketDesignerCommon.MakeButton(child, clickAudioClip);
                                        shareButton.gameObject.SetActive(false);

                                        UIEventListener.Get(shareButton.gameObject).onClick = ShareButtons;

                                        foreach (Transform shareButtonItem in shareButton.transform)
                                        {
                                            if (shareButtonItem.name.ToLower() == "idle")
                                            {
                                                foreach (Transform shareButtonItemItem in shareButtonItem.transform)
                                                {
                                                    if (shareButtonItemItem.GetComponent<UILabel>())
                                                    {
                                                        shareLabel = shareButtonItemItem.GetComponent<UILabel>();
                                                        shareLabel.text = shareIsOpen ? hideShareButtonText : shareButtonText;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                }
                            }
                            break;
                    }
                }
            }
            break;
        }

        cameraClearFlags = new CameraClearFlags[1];
    }

    Rect webCamRect;

    Texture2D lastScreenshot,
              flashTex;
    
    float screenshotEffectTimer = 0,
          screenhotAngle = 0;
    Rect screenshotRect;

    const string ROCKET_PICRURE_NAME = "RocketPicture.png";

    void TakePictureClick(GameObject go)
    {
        StartCoroutine(TakePictureAndSend());
    }

    IEnumerator TakePictureAndSend()
    {
        if (lastScreenshot || flashTex)
            yield break;

        shareButton.gameObject.SetActive(false);
        shareGroup.gameObject.SetActive(false);

        lastScreenshot = null;

        yield return new WaitForEndOfFrame();

        if (flashTex == null)
        {
            flashTex = new Texture2D(1, 1);
            flashTex.SetPixel(0, 0, Color.white);
            flashTex.Apply();
        }

        screenshotEffectTimer = 0;
        screenhotAngle = 0;

        // Create a texture the size of the screen, RGB24 format
        int width = Screen.width;
        int height = Screen.height;
        lastScreenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        // Read screen contents into the texture
        lastScreenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        lastScreenshot.Apply();

        screenshotRect = new Rect(0, 0, lastScreenshot.width, lastScreenshot.height);

        if (guiCam2D)
        {
            Transform trIdle = takePictureButton.transform.GetChild(0);

            foreach (Transform tr in trIdle)
            {
                if (tr.GetComponent<UISprite>())
                {
                    Vector3 screenPos = guiCam2D.WorldToScreenPoint(tr.position);

                    targetScreenshotRect = new Rect(screenPos.x + targetScreenshotCorrection.x, Screen.height - screenPos.y + targetScreenshotCorrection.y, 0, 0);
                    break;
                }
            }
        }

        // Encode texture into PNG
        byte[] bytes = lastScreenshot.EncodeToPNG();
#if UNITY_IOS
		File.WriteAllBytes(Application.persistentDataPath + "/" + ROCKET_PICRURE_NAME, bytes);
		EtceteraBinding.saveImageToPhotoAlbum(Application.persistentDataPath + "/" + ROCKET_PICRURE_NAME);

        shareButton.gameObject.SetActive(true);
#endif
        /*
        StartCoroutine(EmailUtility.SendEmailWithAttachment(emailData.recipient, emailData.sender, emailData.subject, emailData.body, bytes));
         */
    }

    bool realWorld = false;
    void SpaceportClick(GameObject go)
    {
        Debug.Log("SpaceportClick");

        realWorld = !realWorld;

        if (realWorld)
        {
            StartCoroutine(CamAuthorization());
        }
        else
        {
            if (webcamTexture != null)
                webcamTexture.Stop();

            webCameraCam.enabled = false;
            newRealWebCam.gameObject.SetActive(false);

            for (int i = 0; i < otherCameras.Length; i++)
            {
                otherCameras[i].enabled = true;
            }
        }
    }

    Transform webCamPlane;
    IEnumerator CamAuthorization()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam | UserAuthorization.Microphone);

        if (Application.HasUserAuthorization(UserAuthorization.WebCam | UserAuthorization.Microphone))
        {
            if (webcamTexture == null)
            {
                if (iOSInfo.isIPadMini)
                {
                    webcamTexture = new WebCamTexture(Screen.width / 2, Screen.height / 2);
                }
                else
                {
                    webcamTexture = new WebCamTexture(Screen.width * 2, Screen.height * 2);
                }

                webCamPlane = webCameraCam.transform.GetChild(0);

                if (!Application.isEditor)
                {
                    switch (Screen.orientation)
                    {
                        case ScreenOrientation.LandscapeLeft:
                        case ScreenOrientation.LandscapeRight:
                            webCamPlane.localScale = new Vector3(0.32f, 1, 0.2f);
                            break;
                        case ScreenOrientation.Portrait:
                        case ScreenOrientation.PortraitUpsideDown:
                            webCamPlane.localScale = new Vector3(0.42f, 1, 0.32f);   
                            break;
                    }
                }

                webCamPlane.GetComponent<Renderer>().material.mainTexture = webcamTexture;

            }
            webcamTexture.Play();

            webCameraCam.enabled = true;
            newRealWebCam.gameObject.SetActive(true);

            for (int i = 0; i < otherCameras.Length; i++)
            {
                otherCameras[i].enabled = false;
            }
        }
    }

    void ShareButtons(GameObject go)
    {
        if (go.name == "Button: Share")
        {
            shareIsOpen = !shareIsOpen;

            shareGroup.gameObject.SetActive(shareIsOpen);
            shareLabel.text = shareIsOpen ? hideShareButtonText : shareButtonText;
        }
        else if (go.name == "Button: Send by email")
        {
#if UNITY_IOS
            EtceteraBinding.showMailComposerWithAttachment(Application.persistentDataPath + "/" + ROCKET_PICRURE_NAME,
                "image/png",
                ROCKET_PICRURE_NAME,
                emailDefaultRecipient,
                emailDefaultSubject,
                emailDefaultBody,
                true);
#endif
        }
        else if (go.name == "Button: Share on Facebook")
        {
#if UNITY_IOS
            string pathToImage = Application.persistentDataPath + "/" + ROCKET_PICRURE_NAME;
            if (!System.IO.File.Exists(pathToImage))
                pathToImage = null;
			FacebookBinding.showFacebookComposer(facebookMessage, pathToImage, webiteForPictures);
#endif
        }
        else if (go.name == "Button: Share on Twitter")
        {
#if UNITY_IOS
			if (TwitterBinding.isTweetSheetSupported() && TwitterBinding.canUserTweet())
			{
				string pathToImage = Application.persistentDataPath + "/" + ROCKET_PICRURE_NAME;
				if( !System.IO.File.Exists( pathToImage ) )
					pathToImage = null;
				TwitterBinding.showTweetComposer( twitterMessage, pathToImage );
			}
#endif
        }
    }

    void Update()
    {
#if UNITY_IOS

        if (!Application.isEditor)
        {
            if (webcamTexture != null && webcamTexture.isPlaying && webCamPlane)
            {
                switch (Screen.orientation)
                {
                    case ScreenOrientation.LandscapeLeft:
                        webCamPlane.localEulerAngles = new Vector3(90, 180, 0);
                        break;
                    case ScreenOrientation.LandscapeRight:
                        webCamPlane.localEulerAngles = new Vector3(270, 0, 0);
                        break;
                    case ScreenOrientation.Portrait:
                        webCamPlane.localEulerAngles = new Vector3(0, 270, 90);
                        break;
                    case ScreenOrientation.PortraitUpsideDown:
                        webCamPlane.localEulerAngles = new Vector3(0, 90, 270);
                        break;
                }
            }
        }
#endif
    }

#endif

	public void CheckForIpadExtraButtons(bool on)
    {
#if UNITY_IOS
        if (!Application.isEditor)
        {
            if (on)
            {
                UIEventListener.Get(realSpaceportButton.gameObject).onClick = SpaceportClick;
                realSpaceportButton.gameObject.SetActive(true);

                UIEventListener.Get(takePictureButton.gameObject).onClick = TakePictureClick;
                takePictureButton.gameObject.SetActive(true);
            }
            else
            {
                realSpaceportButton.gameObject.SetActive(false);
                takePictureButton.gameObject.SetActive(false);
            }
        }
#endif
    }

    public void HideShareButtons()
    {
        if (shareGroup)
            shareGroup.gameObject.SetActive(false);

        if (shareButton)
            shareButton.gameObject.SetActive(false);

        if (realSpaceportButton)
            realSpaceportButton.gameObject.SetActive(false);

        if (takePictureButton)
            takePictureButton.gameObject.SetActive(false);
    }
}

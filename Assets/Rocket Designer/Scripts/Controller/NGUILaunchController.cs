using UnityEngine;
using System.Collections;

public class NGUILaunchController : LaunchControllerBase
{
    float time = 0,
          timerLeft = 0;

    public AudioClip pauseAudioClip,
                     buttonAudioClip,
                     takePictureAudioClip,
                     launchAudioClip;

    bool countDown = false,
         paused = false,
         launched = false;


    public class FlightPanelDetails
    {
        public Transform tr,
                         summaryPopup, 
                         inflightBox;

        public UIImageButton destroyButton,
                             launchButton,
                             redesignButton;

        public UILabel altitude,
                       flightSummary,
                       failureSummary,
                       inflightHeader,
                       summaryHeader;
    }

    FlightPanelDetails flightPanelDetails;

    void Start()
    {
        if (xml)
        {
            launchButtonText = DialogXMLParser.GetText(xml.text, launchButtonSection, ActiveLanguage.English);
            virtualSpaceButtonText = DialogXMLParser.GetText(xml.text, virtualSpaceButtonSection, ActiveLanguage.English);
            pauseCountdownButtonText = DialogXMLParser.GetText(xml.text, pauseCountdownButtonSection, ActiveLanguage.English);
            takePictureText = DialogXMLParser.GetText(xml.text, takePictureSection, ActiveLanguage.English);
            inflightHeaderText = DialogXMLParser.GetText(xml.text, inflightHeaderSection, ActiveLanguage.English);

            summaryHeaderSuccessText = DialogXMLParser.GetText(xml.text, summaryHeaderSuccessSection, ActiveLanguage.English);
            summaryHeaderFailureText = DialogXMLParser.GetText(xml.text, summaryHeaderFailureSection, ActiveLanguage.English);
        }

        Transform anchor = ResolutionDetector.Main.Gui2D.transform;

        foreach (Transform tr in anchor.GetChild(0))
        {
            if (tr.name == "Anchor-General")
            {
                foreach (Transform panel in tr)
                {
                    switch (panel.name)
                    {
                        case "Flight Panel":
                            flightPanelDetails = new FlightPanelDetails();
                            flightPanelDetails.tr = panel;

                            foreach (Transform child in panel)
                            {
                                switch (child.name)
                                {
                                    case "Button: Launch Rocket":
                                        flightPanelDetails.launchButton = RocketDesignerCommon.MakeButton(child, buttonAudioClip);
                                        UIEventListener.Get(child.gameObject).onClick = LaunchRocketClick;
                                        break;
                                    case "Flight Summary Popup":
                                        flightPanelDetails.summaryPopup = child;

                                        foreach (Transform flightSummaryItem in child)
                                        {
                                            switch (flightSummaryItem.name)
                                            {
                                                    /*
                                                case "Altitude Label":
                                                    flightPanelDetails.altitude = flightSummaryItem.GetComponent<UILabel>();
                                                    break;
                                                     */
                                                case "Flight Summary Text":
                                                    flightPanelDetails.flightSummary = flightSummaryItem.GetComponent<UILabel>();
                                                    break;
                                                    /*
                                                case "Failure Summary Text":
                                                    flightPanelDetails.failureSummary = flightSummaryItem.GetComponent<UILabel>();
                                                    break;
                                                     */
                                                case "Flight Header":
                                                    flightPanelDetails.summaryHeader = flightSummaryItem.GetComponent<UILabel>();
                                                    break;
                                            }
                                        }
                                        break;
                                    case "In-Flight Status Box":
                                        flightPanelDetails.inflightBox = child;

                                        foreach (Transform inflightItem in child)
                                        {
                                            switch (inflightItem.name)
                                            {
                                                case "Flight Header":
                                                    flightPanelDetails.inflightHeader = inflightItem.GetComponent<UILabel>();
                                                    break;
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

    LaunchCallback launchClickCallback;

    public override void Init(float countDownTime, LaunchCallback launchClick, int launchAttempts, string flightSummarySection, bool isSuccess)
    {
        time = countDownTime;
        timerLeft = countDownTime;
        countDown = false;
        paused = false;
        launched = false;

        launchClickCallback = launchClick;

        flightPanelDetails.inflightHeader.text = string.Format(inflightHeaderText, launchAttempts);

        if (isSuccess)
        {
            flightPanelDetails.summaryHeader.text = string.Format(summaryHeaderSuccessText, launchAttempts);
        }
        else
            flightPanelDetails.summaryHeader.text = string.Format(summaryHeaderFailureText, launchAttempts);

        string summaryText = DialogXMLParser.GetText(xml.text, flightSummarySection, ActiveLanguage.English);

        flightPanelDetails.flightSummary.text = summaryText;
    }

    public override void Toggle(bool on)
    {
        if (on)
        {
            flightPanelDetails.summaryPopup.gameObject.SetActive(false);
            flightPanelDetails.inflightBox.gameObject.SetActive(false);

            flightPanelDetails.launchButton.gameObject.SetActive(true);
        }
        else
        {
            flightPanelDetails.inflightBox.gameObject.SetActive(false);
        }
    }

    void LaunchRocketClick(GameObject go)
    {
        if (launchClickCallback != null)
            launchClickCallback(false);

        countDown = true;

        flightPanelDetails.launchButton.gameObject.SetActive(false);
    }
}

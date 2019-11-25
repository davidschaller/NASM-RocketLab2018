using UnityEngine;
using System.Collections;

public abstract class LaunchControllerBase : ControllerOfGUIBase
{
    public delegate void LaunchCallback(bool launched);

    public string launchButtonSection = "start-launch-button",
                  virtualSpaceButtonSection = "virtual-space-button",
                  pauseCountdownButtonSection = "pause-countdown-button",
                  takePictureSection = "take-picture-button",
                  inflightHeaderSection = "inflight-header",
                  summaryHeaderSuccessSection = "summary-header-success",
                  summaryHeaderFailureSection = "summary-header-failure";


    protected string launchButtonText,
                     virtualSpaceButtonText,
                     pauseCountdownButtonText,
                     takePictureText,
                     inflightHeaderText,
                     summaryHeaderSuccessText,
                     summaryHeaderFailureText;

    public abstract void Init(float countDownTime, LaunchCallback launchClick, int launchAttempts, string flightSummary, bool isSuccess);

    public abstract void Toggle(bool on);
}

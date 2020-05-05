using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissionController : MonoBehaviour
{
	public AudioSource workshopSfx;
    public int missionNumber;
	const int LAST_MISSION_NUMBER = 3;

    public GameObject rocketParentPrefab;

    public Transform rocketPlaceholder,
                     rocketLaunchPlaceholder;

    public string rocketCopyToLaunchName = "All Rockets Prefab";

    public Vector3 rocketEulerCorrection = new Vector3(0, 90, 0),
                   rocketPosCorrection = new Vector3(0, 0, 25);

    public Vector3 rocketEulerLaunchCorrection = new Vector3(0, 0, 270),
                   rocketPosLaunchCorrection = new Vector3(0, 25, 0);

    public JustEventPlayerPair introPair;
    
    public RocketComponentBase.ComponentType[] CompareComponentType;

    HeaderControllerBase headerControllerBase;
    ConsoleControllerBase consoleControllerBase;
    LaunchControllerBase launchControllerBase;
    GeneralControllerBase generalControllerBase;
    IpadExtraController ipadExtraController;

    RocketComponentsController rocketComponentsController;

    AlerterBase alerterBase;

    Transform trInstantiatedRocket;

    public bool resetBudgetAfterLaunch = false;
    public float moneyAtStart = 50,
                 thrustConversionFactor = 100,
                 weightConverionFactor = 100,
                 aerodynamicsEffectOnThrust = 10f,
                 thrustWeightTooLowRatioThreshold = .5f,
                 increaseReliabilityForAll = 1,
                 increaseReliabilityOfOne = 2,
				 increaseReliabilityForMultiFail = 1,
                 countDownTime = 10;

    int launchAttempts = 0;
    int failedLaunchAttempts = 0;
	
	public string spaceportGalleryURL = "http://www.google.com/";
	public string nextMissionButtonText = "Next Mission";

    public EventPlayer epNextMissionLoader,
                       epHomeLoader;

    [System.Serializable]
    public class MissionRequirements
    {
        public float thrustWeightRatio = .9f,
                     controlRating = 2;
    }

    public MissionRequirements missionRequirements;

    float moneyLeft = 0;

	GameObject componentPrewiev;

    public JustEventPlayerPair prepareRocketEventsPair;

    public EventPlayer epLaunchStart,
                       epReturnToDesigning;

    public enum Stages
    {
        Any,
        Stage1,
        Stage2,
        Stage3
    }

    public enum Shapes
    {
        Any,
        Tapered,
        Cylinder,
        Flared
    }

    [System.Serializable]
    public class EventPlayerPair
    {
        public EventPlayer epFirst,
                           epLast;

        public enum Controls
        {
            Any,
            Fins,
            Gimbaled,
            Exhaust
        }

        public enum Propellants
        {
            Any,
            Kerosene,
            Liquid,
            Solid
        }

        [System.Serializable]
        public class ExtraEvent
        {
            public Stages stage = Stages.Any;
            public Controls control = Controls.Any;
            public Shapes shape = Shapes.Any;
            public Propellants propellant = Propellants.Any;

            public EventPlayer eventPlayer;

            public bool ControlMatch(RocketComponentBase.ComponentName componentName)
            {
                if (control == Controls.Any)
                {
                    return true;
                }
                else
                {
                    switch (componentName)
                    {
                        case RocketComponentBase.ComponentName.MoveableFins:
                            return control == Controls.Fins;
                        case RocketComponentBase.ComponentName.GimbaledEngines:
                            return control == Controls.Gimbaled;
                        case RocketComponentBase.ComponentName.ExhaustVanes:
                            return control == Controls.Exhaust;
                    }
                }

                return false;
            }

            public bool PropellantMatch(RocketComponentBase.ComponentName componentName)
            {
                if (propellant == Propellants.Any)
                {
                    return true;
                }
                else
                {
                    switch (componentName)
                    {
                        case RocketComponentBase.ComponentName.SolidFuel:
                            return propellant == Propellants.Solid;
                        case RocketComponentBase.ComponentName.LiquidHydrogen:
                            return propellant == Propellants.Liquid;
                        case RocketComponentBase.ComponentName.Kerosene:
                            return propellant == Propellants.Kerosene;
                    }
                }

                return false;
            }

            public bool ShapeMatch(RocketComponentBase.ComponentName componentName)
            {
                if (shape == Shapes.Any)
                {
                    return true;
                }
                else
                {
                    switch (componentName)
                    {
                        case RocketComponentBase.ComponentName.Cylindrical:
                            return shape == Shapes.Cylinder;
                        case RocketComponentBase.ComponentName.Flared:
                            return shape == Shapes.Flared;
                        case RocketComponentBase.ComponentName.Tapered:
                            return shape == Shapes.Tapered;
                    }
                }

                return false;
            }

            public bool StageMatch(RocketComponentBase.ComponentName componentName)
            {
                if (stage == Stages.Any)
                {
                    return true;
                }
                else
                {
                    switch (componentName)
                    {
                        case RocketComponentBase.ComponentName.OneStage:
                            return stage == Stages.Stage1;
                        case RocketComponentBase.ComponentName.TwoStage:
                            return stage == Stages.Stage2;
                        case RocketComponentBase.ComponentName.ThreeStage:
                            return stage == Stages.Stage3;
                    }
                }

                return false;
            }
        }

        public ExtraEvent[] extraEvents;

        public string summary_section = "";

        public bool isSuccess = false;

        public bool TheSameAs(EventPlayerPair pair)
        {
            if (pair != null)
            {
                return epFirst.name == pair.epFirst.name && epLast.name == pair.epLast.name;
            }
                
            return false;
        }
    }

    [System.Serializable]
    public class JustEventPlayerPair
    {
        public EventPlayer epFirst,
                           epLast;
    }

    [System.Serializable]
    public class ReliabilityFailure
    {
        public RocketComponentBase.ComponentType componentType;
        public EventPlayerPair[] epFailures;
    }
    public ReliabilityFailure[] reliabilityFailures;

    public EventPlayerPair epThrustWeightIsHigher,
                           epThrustWeightIsMarginal,
                           epThrustWeightIsClearlyTooLow;

    EventPlayerPair currentEventPair;

    DragMouseOrbit dragMouseOrbit;

    [System.Serializable]
    public class ControlFailure
    {
        public EventPlayerPair epLossOfControl1,
                               epLossOfControl2;
    }

    public ControlFailure controlFailure;
    RocketComponentBase[] failuredComponents;

    [System.Serializable]
    public class EmailData
    {
        public string recipient = "martblnec@gmail.com";
        public string sender = "sasa_@inbox.ru";
        public string subject = "Screenshot";
        public string body = "";
    }

    public EmailData emailData;

    public Transform[] objectsToReset;

    public EventPlayerPair debugPair;

    [System.Serializable]
    public class RocketNameViewer
    {
        public Stages stage;
        public Shapes shape;
        public Transform cameraPlaceholder;

        public bool ViewerMatch(RocketComponentBase.ComponentName pickedShape, RocketComponentBase.ComponentName pickedStage)
        {
            return ShapeMatch(pickedShape) && StageMatch(pickedStage) && cameraPlaceholder;
        }

        bool ShapeMatch(RocketComponentBase.ComponentName componentName)
        {
            if (shape == Shapes.Any)
            {
                return true;
            }
            else
            {
                switch (componentName)
                {
                    case RocketComponentBase.ComponentName.Cylindrical:
                        return shape == Shapes.Cylinder;
                    case RocketComponentBase.ComponentName.Flared:
                        return shape == Shapes.Flared;
                    case RocketComponentBase.ComponentName.Tapered:
                        return shape == Shapes.Tapered;
                }
            }

            return false;
        }

        bool StageMatch(RocketComponentBase.ComponentName componentName)
        {
            if (stage == Stages.Any)
            {
                return true;
            }
            else
            {
                switch (componentName)
                {
                    case RocketComponentBase.ComponentName.OneStage:
                        return stage == Stages.Stage1;
                    case RocketComponentBase.ComponentName.TwoStage:
                        return stage == Stages.Stage2;
                    case RocketComponentBase.ComponentName.ThreeStage:
                        return stage == Stages.Stage3;
                }
            }

            return false;
        }
    }

    public RocketNameViewer[] rocketNameViewers;
    

    Vector3[] cameraPositions;
    Quaternion[] cameraRotations;

    void Awake()
    {
        consoleControllerBase = (ConsoleControllerBase)GameObject.FindObjectOfType(typeof(ConsoleControllerBase));
        headerControllerBase = (HeaderControllerBase)GameObject.FindObjectOfType(typeof(HeaderControllerBase));
        launchControllerBase = (LaunchControllerBase)GameObject.FindObjectOfType(typeof(LaunchControllerBase));
        generalControllerBase = (GeneralControllerBase)GameObject.FindObjectOfType(typeof(GeneralControllerBase));
        ipadExtraController = (IpadExtraController)GameObject.FindObjectOfType(typeof(IpadExtraController));

        rocketComponentsController = (RocketComponentsController)GameObject.FindObjectOfType(typeof(RocketComponentsController));
        alerterBase = (AlerterBase)GameObject.FindObjectOfType(typeof(AlerterBase));

        moneyLeft = moneyAtStart;

        if (Camera.main)
            dragMouseOrbit = Camera.main.GetComponent<DragMouseOrbit>();

        cameraPositions = new Vector3[objectsToReset.Length];
        cameraRotations = new Quaternion[objectsToReset.Length];

        for (int i = 0; i < objectsToReset.Length; i++)
        {
            cameraPositions[i] = objectsToReset[i].transform.position;
            cameraRotations[i] = objectsToReset[i].transform.rotation;
        }
    }

    void Start()
    {
        if (introPair.epFirst)
        {
            introPair.epLast.SubscribeForFinish(OnIntroFinish);
            introPair.epFirst.PlayerTriggered();
        }
        else
        {
            OnIntroFinish(null);
        }

        launchAttempts = MissionModel.launchAttempts;
        failedLaunchAttempts = MissionModel.failedLaunchAttempts;

        if (launchAttempts > 0)
            headerControllerBase.UpdateSuccess((float)(missionNumber - 1) / (float)launchAttempts);
        else
            headerControllerBase.UpdateSuccess(0);

        if (MissionModel.Improvements != null)
        {
            rocketComponentsController.SetImprovementsFromArray(MissionModel.Improvements);
        }

        headerControllerBase.ButtonInit(GalleryClick, HomeClick);
    }

    void OnIntroFinish(EventPlayer ep)
    {
        if (rocketParentPrefab && rocketPlaceholder)
        {
            GameObject go = Instantiate((GameObject)rocketParentPrefab, rocketPlaceholder.position, rocketPlaceholder.rotation) as GameObject;

            if (go)
            {
                trInstantiatedRocket = go.transform;
                trInstantiatedRocket.transform.parent = rocketPlaceholder;

                trInstantiatedRocket.localEulerAngles = rocketEulerCorrection;
                trInstantiatedRocket.localPosition = rocketPosCorrection;

                rocketComponentsController.CollectComponents(go.transform);
            }
        }

        //headerControllerBase.SetState(HeaderControllerBase.HeaderStates.ShowStartButton, StartDesigningClick);
        StartDesigningClick();
    }

    void StartDesigningClick()
    {
        headerControllerBase.Init(missionNumber, moneyAtStart, GalleryClick, HomeClick);

        consoleControllerBase.InitTabs(rocketComponentsController.GetTabs(), TabClickCallback, CompleteClick);

        consoleControllerBase.SetState(ConsoleControllerBase.ConsoleStates.Play);
    }

    void GalleryClick()
    {
        Application.OpenURL(spaceportGalleryURL);
    }

    bool isLoadingTitle = false,
         isHidingOuterPanel = false;
    public void HomeClick()
    {
		// When commented in, these lines prevent the button from working
		// after the rocket has finished launching.
		//
        //if (alerterBase.IsActive)
        //    return;

        MissionModel.ResetAll();
        epHomeLoader.PlayerTriggered();
        headerControllerBase.SetState(HeaderControllerBase.HeaderStates.None, null);
        consoleControllerBase.SetState(ConsoleControllerBase.ConsoleStates.None);
        launchControllerBase.Toggle(false);
        isLoadingTitle = true;
    }

    void TabClickCallback(ConsoleControllerBase.TabDetails tab, bool justInit)
    {
        if (tab.buttons == null)
        {
            consoleControllerBase.InitButtons(rocketComponentsController.GetButtons(tab.componentType), tab, ButtonClick, AfterButtonClick);
        }

        if (alerterBase.IsActive)
            return;

        if (!justInit)
        {
            isHidingOuterPanel = tab.componentType == RocketComponentBase.ComponentType.Propellant;
            rocketComponentsController.ToggleOuterPanel(isHidingOuterPanel);
        }


        consoleControllerBase.UpdatePanelButtonState(rocketComponentsController.Rocket_Config);
    }

    bool AfterButtonClick(ConsoleControllerBase.ButtonDetails button, bool picked)
    {
        if (alerterBase.IsActive)
            return false;

        float componentCost = rocketComponentsController.GetCost(button.componentName);
        float refund = rocketComponentsController.GetRefund(button.componentType);
        float new_cost = rocketComponentsController.GetTotalCost() - refund + componentCost;

        if (moneyLeft < new_cost)
            alerterBase.ShowOneButton(AlerterBase.OneButtonAlerts.NotEnoughMoney, new string[1] { button.componentNameText }, null, false);

        return true;
    }

    bool ButtonClick(ConsoleControllerBase.ButtonDetails button, bool picked)
    {
        if (alerterBase.IsActive)
            return false;

        float componentCost = rocketComponentsController.GetCost(button.componentName);
        float refund = rocketComponentsController.GetRefund(button.componentType);
        float new_cost = rocketComponentsController.GetTotalCost() - refund + componentCost;

        Debug.Log("moneyLeft=" + moneyLeft + ", componentCost=" + componentCost + ", refund " + refund + ", new_cost " + new_cost);

        //if (moneyLeft >= rocketComponentsController.GetTotalCost())
        {
            button.UpdateScreen(rocketComponentsController.GetScreenDetails(button.componentName));
            if (componentPrewiev != null)
                Destroy(componentPrewiev);

            GameObject cPrefab = rocketComponentsController.GetPreview(button.componentName);
            if (cPrefab != null)
            {
                componentPrewiev = Instantiate((GameObject)cPrefab, new Vector3(0, 0, 0), cPrefab.transform.rotation) as GameObject;
                NGUITools.SetLayer(componentPrewiev, LayerMask.NameToLayer("Component"));
            }

            rocketComponentsController.UpdateRocket(button.componentType, button.componentName);

            float risk = rocketComponentsController.GetRisk();
            float thrust = 0,
                  weight = 0;

            rocketComponentsController.GetThrustWeightRatio(out thrust, out weight, aerodynamicsEffectOnThrust, thrustConversionFactor, weightConverionFactor);
            headerControllerBase.UpdateParams(moneyLeft, rocketComponentsController.GetTotalCost(), risk, thrust, weight);
            consoleControllerBase.ConfirmSelection(button.componentType, button.componentName);

            if (rocketComponentsController.AreAllComponentsSet())
            {
                if (consoleControllerBase.ConsoleState != ConsoleControllerBase.ConsoleStates.CanBeComplete)
                {
                    consoleControllerBase.SetState(ConsoleControllerBase.ConsoleStates.CanBeComplete);
                    consoleControllerBase.ToggleClickable(true);
                }
            }

            return true;
        }

    }

    void AlertCallBack()
    {
        consoleControllerBase.ToggleClickable(true);
    }

    void CompleteClick()
    {
        if (alerterBase.IsActive)
            return;

        if (moneyLeft < rocketComponentsController.GetTotalCost())
        {
            alerterBase.ShowOneButton(AlerterBase.OneButtonAlerts.NotEnoughMoney, new string[1] { "this rocket" }, AlertCallBack, false);
            return;
        }

        rocketComponentsController.ToggleOuterPanel(false);

        float thrust = 0,
              weight = 0;
        rocketComponentsController.GetThrustWeightRatio(out thrust, out weight, aerodynamicsEffectOnThrust, thrustConversionFactor, weightConverionFactor);

        float thrustWeightRatio = thrust / weight;

        if (thrustWeightRatio >= missionRequirements.thrustWeightRatio)
        {
            CompleteDesigning();
        }
        else
        {
            consoleControllerBase.ToggleClickable(false);
            alerterBase.ShowTwoButtons(AlerterBase.TwoButtonAlerts.ThrustWeightIsTooLow, new string[1] { missionRequirements.thrustWeightRatio.ToString() } , ThrustWeightIsTooLowCallback);
        }
    }

    void ThrustWeightIsTooLowCallback(bool whatever)
    {
        if (whatever)
        {
            CompleteDesigning();
        }

        consoleControllerBase.ToggleClickable(true);
    }

    void CompleteDesigning()
    {
        consoleControllerBase.SetState(ConsoleControllerBase.ConsoleStates.Complete);
        generalControllerBase.ShowRocketNamePanel(SetRocketName, CompareRockets, PrepareForLaunch, BackToDesign, BackFromComparison);
    }

    void SetRocketName(string name)
    {
        rocketComponentsController.SetName(name);

        RocketNameViewer viewer = null;
        for (int i = 0; i < rocketNameViewers.Length; i++)
        {
            if (rocketNameViewers[i].ViewerMatch(rocketComponentsController.Rocket_Config.pickedShape, rocketComponentsController.Rocket_Config.pickedStage))
            {
                viewer = rocketNameViewers[i];
                break;
            }
        }

        if (viewer != null)
        {
            consoleControllerBase.SetCameraTo(viewer.cameraPlaceholder, false);
        }
    }

    void CompareRockets()
    {
        headerControllerBase.SetState(HeaderControllerBase.HeaderStates.LaunchMode, null);
        generalControllerBase.ShowComparedRocket(rocketComponentsController.Rocket_Config, rocketComponentsController.FindRealRocket(GetMissionCCT()), consoleControllerBase.GetComponentTypeText, consoleControllerBase.GetComponentNameText, false);
    }

    void CompareRockets2()
    {
        alerterBase.Hide(true);
        headerControllerBase.Hide(true);
        generalControllerBase.ShowComparedRocket(rocketComponentsController.Rocket_Config, rocketComponentsController.FindRealRocket(GetMissionCCT()), consoleControllerBase.GetComponentTypeText, consoleControllerBase.GetComponentNameText, true);
    }

    private RocketComponentBase.ComponentType GetMissionCCT()
    {
        RocketComponentBase.ComponentType res = RocketComponentBase.ComponentType.None;
        if (CompareComponentType.Length >= missionNumber)
            res = CompareComponentType[missionNumber - 1];
        return res;
    }

    void BackFromComparison()
    {
        alerterBase.Hide(false);
        headerControllerBase.Hide(false);
        headerControllerBase.SetState(HeaderControllerBase.HeaderStates.Play, null);
    }

    void BackToDesign()
    {
        consoleControllerBase.SetState(ConsoleControllerBase.ConsoleStates.CanBeComplete);
        consoleControllerBase.ToggleClickable(true);

        rocketComponentsController.ToggleOuterPanel(isHidingOuterPanel);

        generalControllerBase.HideNamePanel();

		if(workshopSfx != null) {
			if (!workshopSfx.isPlaying) {
				workshopSfx.Play();
			}
		}
	}

    Transform rocketCopyToLaunch;

    CameraRotationWithGyro gyroScript;

    void PrepareForLaunch()
    {
        GameObject goCameras = GameObject.Find("Cameras");

        if (goCameras)
        {
            foreach (Transform tr in goCameras.transform)
            {
                if (tr.GetComponent<CameraRotationWithGyro>())
                {
                    gyroScript = tr.GetComponent<CameraRotationWithGyro>();

                    gyroScript.Toggle(true);
                    break;
                }
            }
        }

        consoleControllerBase.SetCameraTo(null, true);

        launchAttempts++;
        moneyLeft -= rocketComponentsController.GetTotalCost();

        headerControllerBase.SetState(HeaderControllerBase.HeaderStates.LaunchMode, null);

        float thrust = 0,
              weight = 0;
        rocketComponentsController.GetThrustWeightRatio(out thrust, out weight, aerodynamicsEffectOnThrust, thrustConversionFactor, weightConverionFactor);

        float thrustWeightRatio = thrust / weight;

        if (thrustWeightRatio >= missionRequirements.thrustWeightRatio)
        {
            float control = rocketComponentsController.GetComponentValue(RocketComponentBase.ComponentType.Control);
            if (control >= missionRequirements.controlRating)
            {
                failuredComponents = rocketComponentsController.GetFailureComponents();

                if (failuredComponents.Length == 0)
                {
                    currentEventPair = epThrustWeightIsHigher;
                }
                else
                {
                    if (failuredComponents.Length == 1)
                    {
                        foreach (ReliabilityFailure item in reliabilityFailures)
                        {
                            if (item.componentType == failuredComponents[0].componentType)
                            {
                                int max_random = 3;
                                if (item.componentType == RocketComponentBase.ComponentType.Stages)
                                {
                                    switch (rocketComponentsController.Rocket_Config.pickedStage)
                                    {
                                        case RocketComponentBase.ComponentName.OneStage:
                                            max_random = 1;
                                            break;
                                        case RocketComponentBase.ComponentName.TwoStage:
                                            max_random = 2;
                                            break;
                                        case RocketComponentBase.ComponentName.ThreeStage:
                                            max_random = 3;
                                            break;
                                    }
                                }

                                int random = Random.Range(0, max_random);
                                currentEventPair = item.epFailures[random];
                                break;
                            }
                        }
                    }
                    else
                    {
                        int randomIndex = Random.Range(0, reliabilityFailures.Length);

                        foreach (ReliabilityFailure item in reliabilityFailures)
                        {
                            if (item.componentType == reliabilityFailures[randomIndex].componentType)
                            {
                                int max_random = 3;
                                if (item.componentType == RocketComponentBase.ComponentType.Stages)
                                {
                                    switch (rocketComponentsController.Rocket_Config.pickedStage)
                                    {
                                        case RocketComponentBase.ComponentName.OneStage:
                                            max_random = 1;
                                            break;
                                        case RocketComponentBase.ComponentName.TwoStage:
                                            max_random = 2;
                                            break;
                                        case RocketComponentBase.ComponentName.ThreeStage:
                                            max_random = 3;
                                            break;
                                    }
                                }

                                int random = Random.Range(0, max_random);
                                currentEventPair = item.epFailures[random];
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                int random = Random.Range(0, 2);

                if (random == 0)
                {
                    currentEventPair = controlFailure.epLossOfControl1;
                }
                else if (random == 1)
                {
                    currentEventPair = controlFailure.epLossOfControl2;
                }
            }
        }
        else
        {
            if ((missionRequirements.thrustWeightRatio - thrustWeightRatio) >= thrustWeightTooLowRatioThreshold)
            {
                currentEventPair = epThrustWeightIsMarginal;
            }
            else
            {
                currentEventPair = epThrustWeightIsClearlyTooLow;
            }
        }

        Debug.Log("Prepare for launch, instantiating the rocket");

        rocketCopyToLaunch = (Transform)Instantiate((Transform)trInstantiatedRocket, rocketLaunchPlaceholder.position, Quaternion.identity);

        if (!string.IsNullOrEmpty(rocketCopyToLaunchName))
            rocketCopyToLaunch.name = rocketCopyToLaunchName;

        Debug.Log(rocketCopyToLaunch.name, rocketCopyToLaunch);

        rocketCopyToLaunch.transform.parent = rocketLaunchPlaceholder;
        rocketCopyToLaunch.localEulerAngles = rocketEulerLaunchCorrection;
        rocketCopyToLaunch.localPosition = rocketPosLaunchCorrection;
        float offset  = rocketComponentsController.GetRocketTransformOffsetOnStart();
        rocketCopyToLaunch.localPosition += Vector3.up * offset;

        NGUITools.SetActive(rocketCopyToLaunch.gameObject, false);

        if (dragMouseOrbit)
            dragMouseOrbit.Toggle(false, true);

        if (prepareRocketEventsPair.epFirst)
        {
            Debug.Log("MissionController: Triggering 'Prepare Rocket Events Pair, epFirst", prepareRocketEventsPair.epFirst.transform);

            prepareRocketEventsPair.epLast.SubscribeForFinish(OnPrepareFinish);
            prepareRocketEventsPair.epFirst.PlayerTriggered();
        }

        if (debugPair.epFirst && debugPair.epLast)
        {
            currentEventPair = debugPair;
        }
    }
    
    /// <summary>
    ///  Show launch control panel when transition from designing to launching has finished.
    /// </summary>
    void OnPrepareFinish(EventPlayer ep)
    {
        NGUITools.SetActive(rocketCopyToLaunch.gameObject, true);

        ipadExtraController.CheckForIpadExtraButtons(true);

        Debug.Log("MissionController: 'Prepare Rocket Events Pair', epLast has finished", prepareRocketEventsPair.epLast.transform);

        if (!isLoadingTitle)
        {
			launchControllerBase.Init(countDownTime, LaunchRocketClick, launchAttempts, currentEventPair.summary_section, currentEventPair.isSuccess);
            launchControllerBase.Toggle(true);

            if (currentEventPair != epThrustWeightIsHigher)
                failedLaunchAttempts++;
        }
    }

    void LaunchRocketClick(bool launched)
    {
        if (launched)
        {
            if (currentEventPair != null)
            {
                Debug.Log("MissionController: Triggering '" + currentEventPair.epFirst.name + "'", currentEventPair.epFirst.transform);
                currentEventPair.epLast.SubscribeForFinish(RocketLaunchFinalEvent);
                currentEventPair.epFirst.PlayerTriggered();

                List<EventPlayerPair.ExtraEvent> extraEventsToTrigger = new List<EventPlayerPair.ExtraEvent>();
                foreach (EventPlayerPair.ExtraEvent extraEvent in currentEventPair.extraEvents)
                {
                    if (extraEvent.ControlMatch(rocketComponentsController.Rocket_Config.pickedControl) && 
                        extraEvent.PropellantMatch(rocketComponentsController.Rocket_Config.pickedPropellant) &&
                        extraEvent.ShapeMatch(rocketComponentsController.Rocket_Config.pickedShape) &&
                        extraEvent.StageMatch(rocketComponentsController.Rocket_Config.pickedStage))
                    {
                        extraEventsToTrigger.Add(extraEvent);
                    }
                }

                foreach (EventPlayerPair.ExtraEvent item in extraEventsToTrigger)
                {
if (item.eventPlayer)
                    {
                        Debug.Log("MissionController: Triggering extra event'" + item.eventPlayer.name + "'", item.eventPlayer.transform);
                        item.eventPlayer.PlayerTriggered();
                    }
                }

                Debug.Log("MissionController: IMPORTANT! Now I'm waiting for '" + currentEventPair.epLast.name + "' to be finished", currentEventPair.epLast.transform);
            }
        }
        else
        {
            Debug.Log("MissionController: Triggering 'ep Launch Start'", epLaunchStart.transform);

			if (workshopSfx != null) {
				workshopSfx.Stop();
			}

			if (epLaunchStart)
                epLaunchStart.PlayerTriggered();
        }
    }

    void RocketLaunchFinalEvent(EventPlayer ep)
    {
        Debug.Log("MissionController: Rocket Launch Final Event '" + ep.name + "' has finished", ep.transform);

        ipadExtraController.HideShareButtons();

        if (!isLoadingTitle)
        {
            string headerText = string.Empty,
                   statisticsText = string.Empty,
                   resultText = string.Empty,
                   statusText = string.Empty,
                   resumeText = string.Empty;

            launchControllerBase.Toggle(false);

			//uncomment for cheat to force success panel to show
//			currentEventPair.isSuccess = true;

			if (currentEventPair.isSuccess)
            {
                string multipleSForAttempts = launchAttempts > 1 ? "s" : "";
                alerterBase.ShowOneButton(AlerterBase.OneButtonAlerts.Success, new string[2] { launchAttempts.ToString(), multipleSForAttempts }, ToTheNextMission, CompareRockets2, true);

                // Successful flight: Increase reliability atings on all components by 1 point – editable in Inspector
                rocketComponentsController.ImproveComponents(rocketComponentsController.GetActiveComponents(), increaseReliabilityForAll);
				
				// This is for the Button Text to replace the Next Mission text with. It's kinda wonky, but it works.
				if(missionNumber == LAST_MISSION_NUMBER)
				{
					GameObject obj = GameObject.Find("Flight Panel");
					Transform t = obj.transform.Find("Flight Summary Popup/Button/ButtonText/Button Text");
					UILabel label = t.gameObject.GetComponent<UILabel>();
					label.text = nextMissionButtonText;
				}
            }
            else
            {
                // Thrust/Weight failure or loss of Control failure.
                if (failuredComponents == null)
                {
                    if (currentEventPair.TheSameAs(epThrustWeightIsMarginal) || currentEventPair.TheSameAs(epThrustWeightIsClearlyTooLow))
                    {
                        alerterBase.ShowOneButton(AlerterBase.OneButtonAlerts.ThristWeightIsTooLow, new string[1] { missionRequirements.thrustWeightRatio.ToString() }, RedesignRocket, CompareRockets2, true);
                    }
                    else if (currentEventPair.TheSameAs(controlFailure.epLossOfControl1) || currentEventPair.TheSameAs(controlFailure.epLossOfControl2))
                    {
                        alerterBase.ShowOneButton(AlerterBase.OneButtonAlerts.ControlIsTooLow, new string[1] { missionRequirements.controlRating.ToString() }, RedesignRocket, CompareRockets2, true);
                    }
                    else
                        Debug.LogError("failuredComponents is NULL, but it is not one of the ThrustWeight problems for some reason. This should never happen.");
                }
                else
                {
                    if (failuredComponents.Length == 0)
                    {
                        if (debugPair.epFirst && debugPair.epLast)
                        {
                            Debug.Log("It looks like you're debugging");
                            if (currentEventPair.TheSameAs(epThrustWeightIsMarginal) || currentEventPair.TheSameAs(epThrustWeightIsClearlyTooLow))
                            {
                                alerterBase.ShowOneButton(AlerterBase.OneButtonAlerts.ThristWeightIsTooLow, new string[1] { missionRequirements.thrustWeightRatio.ToString() }, RedesignRocket, CompareRockets2, true);
                            }
                            else if (currentEventPair.TheSameAs(controlFailure.epLossOfControl1) || currentEventPair.TheSameAs(controlFailure.epLossOfControl2))
                            {
                                alerterBase.ShowOneButton(AlerterBase.OneButtonAlerts.ControlIsTooLow, new string[1] { missionRequirements.controlRating.ToString() }, RedesignRocket, CompareRockets2, true);
                            }
                        }
                        else
                            Debug.LogError("There is no failuredComponents, but the selected pair is not marked as 'Success', so I don't know what to do, this should never happen.");
                    }
                    else if (failuredComponents.Length == 1)
                    {
                        alerterBase.ShowOneButton(AlerterBase.OneButtonAlerts.FailedComponentIsFixed, new string[1] { failuredComponents[0].GetComponentName() }, RedesignRocket, CompareRockets2, true);

                        // If only one component is calculated to fail, then increase the reliability of that component only by 2 points, editable in inspector
                        rocketComponentsController.ImproveComponents(failuredComponents, increaseReliabilityOfOne);
                    }
                    else
                    {
                        alerterBase.ShowOneButton(AlerterBase.OneButtonAlerts.TooManyComponentsFailed, null, RedesignRocket, CompareRockets2, true);
                        // If multiple components are calculated to fail, then do not increase reliability on any components. 
                        //(Engineers cannot deduce the cause of the failure, nor improve that component.)

						// the above comment is no longer the case, we will now improve the failed components
						// if more than 1 component fails, allow an increase in reliability for those components, but different than normal increase
						rocketComponentsController.ImproveComponents(failuredComponents,increaseReliabilityForMultiFail);
                    }
                }
            }
        }

        failuredComponents = null;

        if (currentEventPair.isSuccess)
        {
            headerControllerBase.UpdateSuccess(((float) missionNumber) / (float)launchAttempts);
        }
        else
        {
            headerControllerBase.UpdateSuccess(((float)missionNumber - 1) / (float)launchAttempts);
        }
    }

    void ToTheNextMission()
    {
//        MissionModel.SaveAttempts(launchAttempts, failedLaunchAttempts);
        launchControllerBase.Toggle(false);

        float[] improvements = rocketComponentsController.GetImprovementsAsArray();
        MissionModel.SaveImprovements(improvements);

        consoleControllerBase.SetState(ConsoleControllerBase.ConsoleStates.None);
        headerControllerBase.SetState(HeaderControllerBase.HeaderStates.None, null);

        epNextMissionLoader.PlayerTriggered();
    }

    void RedesignRocket()
    {
        if (resetBudgetAfterLaunch)
            moneyLeft = moneyAtStart;

        consoleControllerBase.UpdateAciveButton();

        rocketComponentsController.ToggleOuterPanel(isHidingOuterPanel);

        if (gyroScript)
        {
            gyroScript.Toggle(false);
        }

        EventPlayer[] activeEventPlayers = (EventPlayer[])GameObject.FindObjectsOfType(typeof(EventPlayer));

        if (activeEventPlayers != null)
        {
            foreach (EventPlayer ep in activeEventPlayers)
            {
                if (ep.HasTriggered)
                {
                    ep.ResetRecursively();
                }
            }
        }

		if(workshopSfx != null) {
			if (!workshopSfx.isPlaying) {
				workshopSfx.Play();
			}
		}

		epReturnToDesigning.SubscribeForFinish(ReturnToRedesigning);
        epReturnToDesigning.PlayerTriggered();
    }

    void ReturnToRedesigning(EventPlayer ep)
    {
        ipadExtraController.CheckForIpadExtraButtons(false);

        Debug.Log("ReturnToRedesigning");

        if (rocketCopyToLaunch)
            Destroy(rocketCopyToLaunch.gameObject);

        if (!isLoadingTitle)
        {
            if (moneyLeft < rocketComponentsController.GetTotalCost())
            {
                consoleControllerBase.ToggleClickable(false);
                alerterBase.ShowTwoButtons(AlerterBase.TwoButtonAlerts.HasSpentAllTheirMoney, null, RestartGameOrRound);
            }
            else
            {
                headerControllerBase.SetState(HeaderControllerBase.HeaderStates.Play, null);

                if (dragMouseOrbit)
                    dragMouseOrbit.Toggle(true, false);

                float risk = rocketComponentsController.GetRisk();
                float thrust = 0,
                      weight = 0;
                rocketComponentsController.GetThrustWeightRatio(out thrust, out weight, aerodynamicsEffectOnThrust, thrustConversionFactor, weightConverionFactor);
                headerControllerBase.UpdateParams(moneyLeft, rocketComponentsController.GetTotalCost(), risk, thrust, weight);

                consoleControllerBase.SetState(ConsoleControllerBase.ConsoleStates.CanBeComplete);
                consoleControllerBase.ToggleClickable(true);

                ResetCameras();
            }
        }
    }

    void ResetCameras()
    {
        for (int i = 0; i < objectsToReset.Length; i++)
        {
            objectsToReset[i].transform.position = cameraPositions[i];
            objectsToReset[i].transform.rotation = cameraRotations[i];
        }
    }

    void RestartGameOrRound(bool restartGame)
    {
        if (restartGame)
        {
            HomeClick();
        }
        else
        {
            Application.LoadLevelAsync(Application.loadedLevelName);
        }
    }

    IEnumerator TakePictureAndSend()
    {
        yield return new WaitForEndOfFrame();

        // Create a texture the size of the screen, RGB24 format
        int width = Screen.width;
        int height = Screen.height;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        // Read screen contents into the texture
        tex.ReadPixels (new Rect(0, 0, width, height), 0, 0);
        tex.Apply ();

        // Encode texture into PNG
        byte[] bytes = tex.EncodeToPNG();
        Destroy (tex);
        StartCoroutine(EmailUtility.SendEmailWithAttachment(emailData.recipient, emailData.sender, emailData.subject, emailData.body, bytes));
    }
}

using UnityEngine;
using System.Collections;
using UnityEditor;

public class EventUtility
{
	public static string[] eventTypes = new string[]
	{
		"Test Event", 
		"Instantiate Prefab", 
	    "Activate Object",
	    "Destroy Object",
		"Camera Waypoint", 
		"Actor Waypoint", 
		"Animate", 
		"Animate NPC", 
		"Null Event", 
		"Suspend Player Control Event", 
		"Resume Player Control Event", 
		"Dialog Event",
        "Interview Event",
		"Activate Goal Event",
		"Start Flashback Event",
		"Pictorial Event",
	    "Pictorial 3 Event",
		"Move Object Linear Event",
		"Load Scene Event",
		"Start Cannon Game Event",
		"Start Treasure Hunt Event",
		"One Button Dialog Event",
		"Treasure Dialog Event",
		"Audio Event",
		"Open URL Event",
		"Assign EP To NPC Talk Event",
		"Letter Writing",
        "Enable NPC Talk Event",
        "Dismount Event",
        "Mount Event",
        "Spy Glass",
	    "Wait Time Event",
        "Disable Object Event",
	    "Progress Event",
	    "New Requirement Event",
	    "Multiple Choice Question Event",
	    "Multiple Choice Selector Event",
	    "Simple Dialog Event",
	    "Trigger Event Scenario Event",
	    "Checkbox Quiz Question Event",
	    "Change Camera Layer Mask Event",
        "Face Init Event",
        "Face Animate Event",
        "Change Flag Event",
        "Toggle Music Event",
        "Camera Dissolve Transition Event",
        "Dialog With Textbox Event",
		"Update SkyBox Event",
        "Terminate EventPlayers Event"
	};
	
	static int GenerateUniqueInstantiateAutoName ()
	{
		SimpleSemaphore sem = SimpleSemaphore.GetSemaphoreObject("Assets/999 Support Data/InstantiateIndex.asset");
		return sem.Value;
	}
	
	public static void AddEvent (int addEventType, EventScenario evs)
	{
		EventBase newEvent = null;
		switch(eventTypes[addEventType])
		{
			case "Test Event":
				newEvent = new TestEvent();
				break;
			case "Instantiate Prefab":
				newEvent = new InstantiatePrefabEvent();
                /*
			    InstantiatePrefabEvent ipe = (InstantiatePrefabEvent)newEvent;
			    int ind = GenerateUniqueInstantiateAutoName();
			    string autoName = "Auto " + ind;
			    ipe.SetAutoName(autoName);
			    EventEditor.AddNewVar(autoName+" go", typeof(UnityEngine.GameObject));
			    EventEditor.AddNewVar(autoName+" tr", typeof(UnityEngine.Transform));
                 */
				break;
			case "Camera Waypoint":
				newEvent = new CameraWaypointEvent();
				break;
		    case "Activate Object":
			    newEvent = new ActivateObjectEvent();
			    break;
		    case "Destroy Object":
			    newEvent = new DestroyObjectEvent();
			    break;
			case "Actor Waypoint":
				newEvent = new ActorWaypointEvent();
				break;
			case "Animate":
				newEvent = new AnimateEvent();
				break;
			case "Animate NPC":
				newEvent = new AnimateNPCEvent();
				break;
			case "Null Event":
				newEvent = new NullEvent();
				break;
			case "Suspend Player Control Event":
				newEvent = new SuspendPlayerControlEvent();
				break;
			case "Resume Player Control Event":
				newEvent = new ResumePlayerControlEvent();
				break;
			case "Dialog Event":
				newEvent = new DialogEvent();
				break;
#if Texas
            case "Interview Event":
                newEvent = new InterviewEvent();
                break;
#endif
			case "Activate Goal Event":
				newEvent = new ActivateGoalEvent();
				break;
			case "Start Flashback Event":
				newEvent = new StartFlashbackEvent();
				break;
			case "Pictorial Event":
				newEvent = new PictorialEvent();
				break;
		    case "Pictorial 3 Event":
			    newEvent = new Pictorial3Event();
			    break;
			case "Move Object Linear Event":
				newEvent = new MoveObjectLinearEvent();
				break;
			case "Load Scene Event":
				newEvent = new LoadLevelEvent();
				break;
			case "Start Cannon Game Event":
				newEvent = new StartCannonGameEvent();
				break;
			case "Start Treasure Hunt Event":
				newEvent = new StartTreasureHuntEvent();
				break;
			case "One Button Dialog Event":
				newEvent = new OneButtonDialogEvent();
				break;
			case "Treasure Dialog Event":
				newEvent = new TreasureDialogEvent();
				break;
			case "Audio Event":
				newEvent = new AudioEvent();
				break;
			case "Open URL Event":
				newEvent = new OpenURLEvent();
				break;
			case "Assign EP To NPC Talk Event":
				newEvent = new AssignEPToNPCTalkEvent();
				break;
			case "Letter Writing":
				newEvent = new LetterWritingEvent();
				break;
            case "Enable NPC Talk Event":
                newEvent = new EnableNPCTalkEvent();
                break;
            case "Dismount Event":
                newEvent = new DismountEvent();
                break;
            case "Mount Event":
                newEvent = new MountEvent();
                break;
            case "Disable Object Event":
                newEvent = new DisableObjectEvent();
                break;
            case "Wait Time Event":
                newEvent = new WaitTimeEvent();
                break;
            case "Spy Glass":
                newEvent = new SpyGlassEvent();
                break;
		    case "Progress Event":
			    newEvent = new ProgressEvent();
			    break;
		    case "New Requirement Event":
			    newEvent = new RequirementEvent();
			    break;
		    case "Multiple Choice Question Event":
			    newEvent = new MultipleChoiceQuestionEvent();
			    break;
		    case "Multiple Choice Selector Event":
			    newEvent = new MultipleChoiceSelectorEvent();
			    break;
		    case "Simple Dialog Event":
			    newEvent = new SimpleDialogEvent();
			    break;
		    case "Trigger Event Scenario Event":
			    newEvent = new TriggerEventScenarioEvent();
			    break;
		    case "Checkbox Quiz Question Event":
			    newEvent = new CheckboxQuizQuestionEvent();
			    break;
		    case "Change Camera Layer Mask Event":
			    newEvent = new ChangeCameraLayerMaskEvent();
			    break;
            case "Face Init Event":
                newEvent = new FaceInitEvent();
                break;
            case "Face Animate Event":
                newEvent = new FaceAnimateEvent();
                break;
            case "Change Flag Event":
                newEvent = new ChangeFlagEvent();
                break;
            case "Toggle Music Event":
                newEvent = new ToggleMusicEvent();
                break;
            case "Camera Dissolve Transition Event":
                newEvent = new CameraDissolveTransitionEvent();
                break;
            case "Dialog With Textbox Event":
                newEvent = new DialogWithTextboxEvent();
                break;
            case "Update SkyBox Event":
                newEvent = new UpdateSkyBoxEvent();
                break;
            case "Terminate EventPlayers Event":
                newEvent = new TerminateEventPlayersEvent();
                break;
		}

		RegisterEvent(newEvent, evs);
		evs.AddEvent(newEvent);
		EditorUtility.SetDirty(evs);
	}
	
	public static void RegisterEvent(EventBase ev, EventScenario es)
	{
        System.IO.Directory.CreateDirectory(Application.dataPath + "/08 Resources/Events/");
        System.IO.Directory.CreateDirectory(Application.dataPath + "/08 Resources/Events/" + es.name + "/");
        AssetDatabase.CreateAsset(ev, AssetDatabase.GenerateUniqueAssetPath("Assets/08 Resources/Events/" + es.name + "/Event.asset"));
	}
}

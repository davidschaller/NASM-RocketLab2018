using UnityEngine;
using System.Collections;

public class EventSubstitute : MonoBehaviour
{
	void Awake ()
	{
		MTDebug.Construct();
		MTDebug.WatchVariable("Event Model: {0}", "placeholder");
		
	}
	void Start ()
	{
		MovementController.GetMovementController().SetPCControl();
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public delegate void EventNotification(EventBase ev);

[System.Serializable]
public class EventException : System.Exception
{
	string err;
	public string EventError
	{
		get
		{
			return err;
		}
		private set {}
	}
	
    public EventException() 
	{
	}
    public EventException(string message) 
	{
		err = message;
	}
    public EventException(string message, System.Exception inner) 
	{
		err = message;
	}

    // Constructor needed for serialization 
    // when exception propagates from a remoting server to the client.
    protected EventException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) 
	{
		
	}
}

[System.Serializable]
public abstract class EventBase : ScriptableObject
{
	[HideInInspector]
	public bool startEvent = false;
	public EventNotification notifyOnExit;
	
	public abstract string GetEventName();
	public abstract IEvent CreateRunner();
	
	[HideInInspector]
	public string nickname = "event";
	
	public string GetNickname()
	{
		return nickname;
	}
	
	public void SetNickname(string n)
	{
		nickname = n;
	}
	
	public void Exit ()
	{
		if (notifyOnExit != null)
		{
			foreach(EventBase ev in exitEvents)
			{
				notifyOnExit(ev);
			}
		}
		ClearNotifyOnExits();
	}
	
	public void ClearNotifyOnExits ()
	{
		notifyOnExit = null;
	}

	public EventPlayer Player
	{
		get
		{
			return calledBy;
		}
		private set
		{
		}
	}

   // bool hasEntered = false;
	void EnterEvent (IEvent runner)
	{
        //hasEntered = true;

		abort = false;
		finishedEarly = false;
		finished = false;

		if (calledBy.ThreadCount == 0)
			calledBy.ThreadCount = 1;
		
		/*try*/
		{
			//Debug.Log("ExecuteEvent " + GetNickname() + " via " + sender.name);
			runner.OnEnter();
		}
		/*catch(EventException ex)
		{
			Debug.LogWarning("Event setup exception: " + ex.EventError, calledBy.gameObject);
			abort = true;
			}*/
	}

	void ExitEvent (EventNotification not, IEvent runner)
	{
		if (runner == null)
			abort = true;
		
		if (!abort)
		{
			runner.OnExit();
			
			// TODO move the notify on exit to be specific to a runner
			// instance (otherwise only the first runner gets his
			// notifications done)

            NormalizeExits();

			int exitEventCount = exitEvents == null ? 0 : exitEvents.Length;

            if (exitEventCount > 1)
            {
                if (calledBy.debugPlayer) Debug.Log("Incrementing thread count by " + (exitEventCount - 1) + " for player " + calledBy.gameObject.name + " and event " + GetEventName());
                calledBy.ThreadCount += exitEventCount - 1;
            }

            foreach (EventBase ev in exitEvents)
            {
                not(ev);
                if (notifyOnExit != null)
                    notifyOnExit(ev);
            }

            if ((exitEvents == null || exitEvents.Length == 0) && !calledBy.Finished)
            {
                if (calledBy.debugPlayer) Debug.Log("Calling ScenarioFinished on " + calledBy.name, calledBy.gameObject);
                calledBy.ScenarioFinished();
            }

			//ClearNotifyOnExits();
		}
	}

	bool finishedEarly = false;
	public void ExitEventEarly (EventPlayer player)
	{
		finishedEarly = true;
		
		// should we init and exit, or skip exit if not initialized?
		/*
		if (runner == null)
		{
			runner = CreateRunner();
			runner.OnEnter();
		}
		*/

        if (runnerMap.ContainsKey(player.transform))
        {
            foreach (IEvent runner in runnerMap[player.transform])
            {
                if (runner == null)
                    abort = true;

                if (!abort && !finished)
                {
                    runner.OnTerminate();
                }
            }
        }
	}
	
	protected EventPlayer calledBy;
	public EventPlayer EventPlayer
	{
		get
		{
			return calledBy;
		}
	}
	//protected IEvent runner;
	protected bool abort = false;
	bool debugEvent = false;
	Dictionary<Transform, List<IEvent>> runnerMap = new Dictionary<Transform, List<IEvent>>();
	Dictionary<Transform, float> playerFrameTracker = new Dictionary<Transform, float>();
	//float lastExecutedFrame=0;
	public IEnumerator ExecuteEvent (EventPlayer sender, EventNotification not, bool debugEvent)
	{

		if (playerFrameTracker.ContainsKey(sender.transform) && playerFrameTracker[sender.transform] == Time.frameCount)
			yield return 0;

		playerFrameTracker[sender.transform] = Time.frameCount;
		
		//if (lastExecutedFrame == Time.frameCount && Time.frameCount > 100)
		//yield return 0;
		//lastExecutedFrame = Time.frameCount;
		
		calledBy = sender;
		this.debugEvent = debugEvent;

        IEvent runner = CreateRunner();

        if (!runnerMap.ContainsKey(sender.transform))
        {
            runnerMap.Add(sender.transform, new List<IEvent>());
        }
        runnerMap[sender.transform].Add(runner);

        EnterEvent(runner);

		while (!abort && !runner.EventIsFinished())
		{
            SV_NPCController oSV_NPCController = (SV_NPCController)sender.GetComponent(typeof(SV_NPCController));

            if (!sender.respectLOD || !oSV_NPCController || (sender.respectLOD && oSV_NPCController && CanExecute(oSV_NPCController.substitutionNPC.transform, sender.lodMaxDistance, sender.behindLodMaxDistance)))
            {
                if (debugEvent)
                    Debug.Log(Time.time.ToString("000.000") +
                              " executing event player " + sender.name +
                              " event type: " + this.GetType() +
                              " runner string: " + runner.ToString() +
                              ", event string: " + this.ToString() +
                              " runner null? " + (runner == null) +
                              " runner finished: " + runner.EventIsFinished());

                if (!Paused)
                    runner.OnExecute();
            }

			yield return 0;
		}

		if (debugEvent)
			Debug.Log("Event has terminated " + GetEventName(), sender);

        if (!finishedEarly)
        {
            ExitEvent(not, runner);
        }
		
		finished = true;
	}

    private bool behindCamera = false;
    public bool CanExecute(Transform eventTransform, float lodMaxDistance, float behindLodMaxDistance)
    {
        float cameraDistance = 1000;

        if (Camera.main == null)
        {
            behindCamera = true;
        }
        else
        {
            behindCamera = Vector3.Dot(Camera.main.transform.TransformDirection(Vector3.forward), eventTransform.transform.position - Camera.main.transform.position) < 0.40f;
            cameraDistance = (eventTransform.transform.position - Camera.main.transform.position).magnitude;
        }

        if (cameraDistance > lodMaxDistance ||
            (behindCamera && cameraDistance > behindLodMaxDistance))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
	
	public void StartEvent ()
	{
		//StartCoroutine(ExecuteEvent());
	}

    /// <summary>
    /// We've got bugs if the exit events repeat
    /// </summary>
    void NormalizeExits()
    {
        List<EventBase> newEvents = new List<EventBase>();

        for (int i = 0; i < exitEvents.Length; i++)
        {
            if (!newEvents.Contains(exitEvents[i]))
            {
                newEvents.Add(exitEvents[i]);
            }
        }

        exitEvents = newEvents.ToArray();
    }

	public void AddExitNotification(EventNotification not)
	{
		notifyOnExit += not;
	}
	
	public void DeleteExitEvents ()
	{
		exitEvents = null;
		exitEvents = new EventBase[0];
	}
	
	[HideInInspector]
	public EventBase[] exitEvents;

	public void AddExitEvent(EventBase ev)
	{
		if (exitEvents == null)
			exitEvents = new EventBase[0];

        bool gotAlready = false;
        for (int i = 0; i < exitEvents.Length; i++)
        {
            if (exitEvents[i] == ev)
                gotAlready = true;
        }

        if (!gotAlready)
        {
            EventBase[] newEvents = new EventBase[exitEvents.Length + 1];
            for (int i = 0; i < exitEvents.Length; i++)
            {
                newEvents[i] = exitEvents[i];
            }
            newEvents[exitEvents.Length] = ev;
            exitEvents = newEvents;
        }
	}

	bool finished = false;
	protected void EventFinished()
	{
		finished = true;
	}
	
	public bool IsFinished()
	{
		return finished;
	}


	public Rect EditorRect ()
	{
		editorPosition.width = editorWH.x;
		editorPosition.height = editorWH.y;
		
		return editorPosition;
	}

	[HideInInspector]
	public Rect editorPosition = new Rect(10, 150, 50, 50);
	[HideInInspector]
	public Rect selectedEventRect = new Rect(10,150,50,50);
	[HideInInspector]
	public Rect connectorRect = new Rect(10 + 50-10, 10 + 50-10, 10, 10);
	Vector2 editorWH = new Vector3(50, 50);
	[HideInInspector]
	public bool dragging = false;
	[HideInInspector]
	public bool draggingConnector = false;
	public static EventBase selected;
	public static bool repaint = false;
	public static Vector2 dropConnection;
	public static EventBase dropOriginator;
	[HideInInspector]
	public bool isDirty = false;
	public void EditorGUI (int ind)
	{
		editorPosition.width = editorWH.x;
		editorPosition.height = editorWH.y;
		
		connectorRect = new Rect(editorPosition.x + editorPosition.width-10, editorPosition.y + editorPosition.height-10, 10, 10);
		if (selected == this)
		{
			selectedEventRect = editorPosition;
			selectedEventRect.x -= 5;
			selectedEventRect.y -= 5;
			selectedEventRect.width += 10;
			selectedEventRect.height += 10;
			GUI.color = Color.red;
			GUI.Box(selectedEventRect, "");
		}
		if (ind == 0)
			GUI.color = Color.blue;
		else
			GUI.color = Color.green;
		GUI.Box(editorPosition, "");
		GUI.Label(editorPosition, GetNickname());
		
		GUI.color = Color.black;
		GUI.Box(connectorRect, "");
		GUI.color = Color.green;
		
		if(Event.current.type == EventType.MouseDown && editorPosition.Contains(Event.current.mousePosition) && !connectorRect.Contains(Event.current.mousePosition))
		{
		    if(Event.current.clickCount == 1)
			{
			    if(Event.current.button == 0)
				{
					dragging = true;
				}
			}
			else if(Event.current.clickCount == 2)
			{
			}
			Event.current.Use();
		}
		else if (Event.current.type == EventType.MouseDown && connectorRect.Contains(Event.current.mousePosition))
		{
			if(Event.current.clickCount == 1)
			{
			    if(Event.current.button == 0)
				{
					draggingConnector = true;
				}
			}
			Event.current.Use();
		}
		else if(Event.current.type == EventType.MouseDrag && dragging)
		{ 
			// dragging
			editorPosition.x += Event.current.delta.x;
			editorPosition.y += Event.current.delta.y;
			Event.current.Use();
		}
		else if(Event.current.type == EventType.MouseUp && dragging)
		{ 
			// done dragging
			dragging = false;
			Event.current.Use();
			selected = this;
			isDirty = true;
		}
		else if (Event.current.type == EventType.MouseDrag && draggingConnector)
		{
			Event.current.Use();
		}
		else if (Event.current.type == EventType.MouseUp && draggingConnector)
		{
			draggingConnector = false;
			dropConnection = Event.current.mousePosition;
			dropOriginator = this;
			Event.current.Use();
			isDirty = true;
		}
		else if (Event.current.type == EventType.MouseDown && !editorPosition.Contains(Event.current.mousePosition) && !draggingConnector)
		{
			/*selected = null;
						repaint = true;
						Debug.Log("Unselect");*/
			//Event.current.Use();
		}
	}
	
	string FormatMessage(object sender, string msg)
	{
		return String.Format("event type: {0} nickname: ({1}) {2}", sender.GetType(), GetNickname(), msg);
	}
	
	protected void EventLog(object sender, string msg)
	{
		Debug.Log(FormatMessage(sender, msg));
	}

	protected void EventLogError(object sender, string msg)
	{
		Debug.LogError(FormatMessage(sender, msg));
	}

	protected void EventException(object sender, string msg)
	{
		EventLogError(sender, msg);
		//throw new EventException(msg);
	}

    public bool Paused { get; private set; }

    public void Pause()
    {
        Paused = true;
    }

    public void Resume()
    {
        Paused = false;
    }

    public void Stop()
    {
        finished = true;
    }

    public void Reset(EventPlayer ep)
    {
        if (runnerMap != null)
        {
            if (runnerMap.ContainsKey(ep.transform))
            {
                if (runnerMap[ep.transform] != null)
                {
                    foreach (IEvent item in runnerMap[ep.transform])
                    {
                        if (item != null)
                        {
                            item.OnReset();
                        }
                    }
                }
            }
        }
    }
}

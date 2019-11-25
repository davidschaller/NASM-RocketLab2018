using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

class MoveObjectLinearRunner : IEvent
{
	//TODO [HideInInspector]
	public Transform fromPoint;
	public Transform toPoint;
	public Transform objectToMove;
	public float moveSpeed;
	public bool easeIn;
	public float finishDistance;

	string NullVarMessage (string var)
	{
		return "Move Object Linear exiting (" + var + " is NULL) - " +
			"check to make sure the object has not been deactivated.  " +
			"If it has intentionally, deactivate this player too!";
	}

    Vector3 startpos;
	
	
	bool eventFinished = false;
	public void OnEnter ()
	{
		if (fromPoint == null)
		{
			eventFinished = true;
			throw new EventException(NullVarMessage("From Point"));
		}
		if (toPoint == null)
		{
			eventFinished = true;
			throw new EventException(NullVarMessage("To Point"));
		}
		if (objectToMove == null)
		{
			eventFinished = true;
			throw new EventException(NullVarMessage("Object To Move"));
		}
		Log("Move Object Linear moving " + objectToMove.name + " from " + fromPoint.name + " to " + toPoint.name);

        startpos = objectToMove.transform.position;
	}

	void Log(string msg)
	{
		Debug.Log("======== " + msg);
	}
	
	public void OnExecute ()
	{
		float currMoveSpeed = moveSpeed;
		float distFromStart = (objectToMove.position - fromPoint.position).magnitude;
		float distToFinish = (objectToMove.position - toPoint.position).magnitude;

		if (easeIn)
		{
			if (distFromStart < 2)
			{
				currMoveSpeed = Mathf.Lerp(0.1f, moveSpeed, distFromStart * 0.5f);
			}
			else if (distToFinish < 2)
			{
				currMoveSpeed = Mathf.Lerp(0, moveSpeed, distToFinish * 0.5f);
			}
		}
		
        float passedDist = (objectToMove.position - startpos).magnitude,
              maxDist = (toPoint.position - startpos).magnitude;
		
		objectToMove.position += (toPoint.position - objectToMove.position).normalized * Time.deltaTime * currMoveSpeed;
        if (distToFinish < finishDistance || (maxDist <= passedDist))
		{
			eventFinished = true;
		}
	}
	
	public void OnExit ()
	{
		Log("Move Object Linear finished moving " + objectToMove.name + " to " + toPoint.name);
	}


	public bool EventIsFinished ()
	{
		return eventFinished;
	}
	
    public void OnReset()
    {
    }

    public void OnTerminate()
    {
    }
}

[System.Serializable]
public class MoveObjectLinearEvent : EventBase
{
	//TODO [HideInInspector]
	public Transform fromPoint;
	public Transform toPoint;
	public Transform objectToMove;
	public float moveSpeed;
	public bool easeIn;
	public float finishDistance;
	
	public override IEvent CreateRunner ()
	{
		MoveObjectLinearRunner runner = new MoveObjectLinearRunner();
		runner.fromPoint = fromPoint;
		runner.toPoint = toPoint;
		runner.objectToMove = objectToMove;
		runner.moveSpeed = moveSpeed;
		runner.easeIn = easeIn;
		runner.finishDistance = finishDistance;
		return runner;
	}

	string eventName = "Move Object Linear";
	public override string GetEventName ()
	{
		return eventName;
	}
}
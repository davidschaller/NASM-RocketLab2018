using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

class InstantiateRunner : IEvent
{
	public GameObject prefab;
    public Transform parent;
    public Vector3 localPosition = Vector3.zero,
                   localEuler = Vector3.zero;
    
    public void OnEnter()
	{
        GameObject go = null;

        go = (GameObject)GameObject.Instantiate(prefab);

        if (parent)
            go.transform.parent = parent;

        go.transform.localPosition = localPosition;
        go.transform.localEulerAngles = localEuler;

		eventFinished = true;
	}
	public void OnExecute ()
	{
	}
	public void OnExit ()
	{
	}

	bool eventFinished = false;
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
public class InstantiatePrefabEvent : EventBase
{
	public GameObject prefab;
    public Transform parent;

    public Vector3 localPosition = Vector3.zero,
                   localEuler = Vector3.zero;
	
	public override IEvent CreateRunner ()
	{
		InstantiateRunner runner = new InstantiateRunner();
		runner.prefab = prefab;
        runner.parent = parent;
        runner.localPosition = localPosition;
        runner.localEuler = localEuler;

        return runner;
	}
	
	string eventName = "Instantiate Prefab";
	public override string GetEventName ()
	{
		return eventName;
	}
}
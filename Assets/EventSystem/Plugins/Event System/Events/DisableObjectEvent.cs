using UnityEngine;
using System.Collections;
using System;

class DisableObjectRunner : IEvent
{
    public GameObject targetObject;

    public Transform parentToDeactivate;
    public string pathToDeactivate;

    public void OnEnter()
    {
        if (targetObject)
        {
            targetObject.SetActive(false);
        }
        else
        {
            if (parentToDeactivate && !string.IsNullOrEmpty(pathToDeactivate))
            {
                Transform child = parentToDeactivate.Find(pathToDeactivate);

                if (child)
                {
                    child.gameObject.SetActive(false);
                }
                else
                {
                    Debug.LogWarning("I couldn't fin a child with path '" + pathToDeactivate + "' inside of '" + parentToDeactivate.name + "' tranform", parentToDeactivate.transform);
                }
            }
            else
            {
                Debug.LogWarning("target Object to deactivate OR parent To Deactivate with the path are not set");
            }
        }        


        eventFinished = true;
    }
    public void OnExecute()
    {
    }
    public void OnExit()
    {
    }

    bool eventFinished = false;
    public bool EventIsFinished()
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
public class DisableObjectEvent : EventBase
{
    //[HideInInspector]
    public GameObject targetObject;
    public Transform parentToDeactivate;
    public string pathToDeactivate;

    public override IEvent CreateRunner()
    {
        DisableObjectRunner runner = new DisableObjectRunner();

        runner.targetObject = targetObject;
        runner.parentToDeactivate = parentToDeactivate;
        runner.pathToDeactivate = pathToDeactivate;

        return runner;
    }

    string eventName = "Disable Object Event";
    public override string GetEventName()
    {
        return eventName;
    }
}
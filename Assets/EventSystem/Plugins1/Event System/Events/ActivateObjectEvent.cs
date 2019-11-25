using UnityEngine;
using System.Collections;
using System;

class ActivateObjectRunner : IEvent
{
    public GameObject targetObject;
    public string oneButtonDialogToPause;
    public int partActivateion = 0;

    public Transform parentToActivate;
    public string pathToActivate;

    GameObjectActivator obj;

    public void OnEnter()
    {
        if (targetObject)
        {
            if (partActivateion > 0)
            {
                obj = GameObjectActivator.DoActivation(targetObject, partActivateion, oneButtonDialogToPause);
            }
            else
                targetObject.SetActiveRecursively(true);
        }
        else if (parentToActivate && !string.IsNullOrEmpty(pathToActivate))
        {
            Transform child = parentToActivate.Find(pathToActivate);

            if (child)
            {
                if (partActivateion > 0)
                {
                    obj = GameObjectActivator.DoActivation(child.gameObject, partActivateion, oneButtonDialogToPause);
                }
                else
                    child.gameObject.SetActiveRecursively(true);
            }
            else
            {
                Debug.Log("No find child Object to set");
            }
        }

        eventFinished = true;
    }

    public void OnExecute()
    {
    }

    public void OnExit()
    {
        partActivateion = 0;
        oneButtonDialogToPause = string.Empty;
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
public class ActivateObjectEvent : EventBase
{
    //[HideInInspector]
    public GameObject targetObject;
    public string oneButtonDialogToPause;
    public int partActivateion = 0;

    public Transform parentToActivate;
    public string pathToActivate;

    public override IEvent CreateRunner()
    {
        ActivateObjectRunner runner = new ActivateObjectRunner();

        runner.targetObject = targetObject;
        runner.oneButtonDialogToPause = oneButtonDialogToPause;
        runner.partActivateion = partActivateion;
        runner.parentToActivate = parentToActivate;
        runner.pathToActivate = pathToActivate;

        oneButtonDialogToPause = string.Empty;
        partActivateion = 0;

        return runner;
    }

    string eventName = "Activate Object Event";
    public override string GetEventName()
    {
        return eventName;
    }
}
using UnityEngine;
using System.Collections;
using System;

class DestroyObjectRunner : IEvent
{
    public GameObject targetObject;
    public bool destroyOnlyChildren;

    public Transform parentToInstantiate;
    public string pathToInstantiate;

    public void OnEnter()
    {
        if (targetObject)
        {
            if (destroyOnlyChildren)
            {
                foreach (Transform child in targetObject.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
            else
			    GameObject.Destroy(targetObject);
        }
        else if (parentToInstantiate && !string.IsNullOrEmpty(pathToInstantiate))
        {
            Transform child = parentToInstantiate.Find(pathToInstantiate);

            if (child)
            {
                if (destroyOnlyChildren)
                {
                    foreach (Transform child2 in child)
                    {
                        GameObject.Destroy(child2.gameObject);
                    }
                }
                else
                    GameObject.Destroy(child.gameObject);
            }
            else
            {
                Debug.Log("No child Object to set");
            }

        }
        else
        {
            Debug.Log("No Target Object to set");
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
public class DestroyObjectEvent : EventBase
{
    //[HideInInspector]
    public GameObject targetObject;
    public bool destroyOnlyChildren;

    public Transform parentToInstantiate;
    public string pathToInstantiate;

    public override IEvent CreateRunner()
    {
        DestroyObjectRunner runner = new DestroyObjectRunner();

        runner.targetObject = targetObject;
        runner.destroyOnlyChildren = destroyOnlyChildren;
        runner.parentToInstantiate = parentToInstantiate;
        runner.pathToInstantiate = pathToInstantiate;

        return runner;
    }

    string eventName = "Destroy Object Event";
    public override string GetEventName()
    {
        return eventName;
    }
}
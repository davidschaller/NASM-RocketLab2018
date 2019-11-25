using UnityEngine;
using System.Collections;
using System;

class FaceInitRunner : IEvent
{
    public NPCController target;
    public TextAsset xml;
    public bool instantiateNPC;

    bool finished = false;

    public void OnEnter()
    {
        if (target.transform.childCount > 0)
        {
            Process();
        }
    }

    void Process()
    {
        foreach (Transform tr in target.transform)
        {
            if (tr.GetComponent<LODController>())
            {
                foreach (Transform child in tr)
                {
                    if (child.GetComponent<Animation>())
                    {
                        Common.AddAudioTo(child.gameObject);

                        if (!child.gameObject.GetComponent<FaceFXController>())
                        {
                            FaceFXController faceFX = child.gameObject.AddComponent<FaceFXController>();
                            faceFX.ImportXML(xml.text);
                            finished = true;
                        }
                        else
                            Debug.LogWarning(target.name + " already has FaceFXController");

                        break;
                    }
                }
                break;
            }
            else if (tr.GetComponent<Animation>())
            {
                Common.AddAudioTo(tr.gameObject);

                if (!tr.gameObject.GetComponent<FaceFXController>())
                {
                    FaceFXController faceFX = tr.gameObject.AddComponent<FaceFXController>();
                    faceFX.ImportXML(xml.text);
                    finished = true;
                }
                else
                    Debug.LogWarning(target.name + " already has FaceFXController");
            }
        }
    }

    float timer = 0;
    bool hasActivated = false;

    public void OnExecute()
    {
        if (finished)
        {
            if (timer > 3)
            {
                eventFinished = true;
            }

            timer += Time.deltaTime;
        }
        else if (target.transform.childCount > 0)
        {
            Process();
        }
        else if (target.transform.childCount == 0 && instantiateNPC && !hasActivated)
        {
            target.ActivateNPC();
            hasActivated = true;
        }
    }

    public void OnExit()
    {
    }

    string GetNickname()
    {
        return "FaceInitRunner";
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
public class FaceInitEvent : EventBase
{
    public NPCController target;
    public TextAsset xml;
    public bool instantiateNPC;

    public override IEvent CreateRunner()
    {
        FaceInitRunner runner = new FaceInitRunner();
        runner.target = target;
        runner.xml = xml;
        runner.instantiateNPC = instantiateNPC;

        return runner;
    }

    string eventName = "Face Init Event";
    public override string GetEventName()
    {
        return eventName;
    }
}
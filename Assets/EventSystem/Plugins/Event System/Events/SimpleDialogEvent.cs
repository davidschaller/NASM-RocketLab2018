using UnityEngine;
using System;
using System.Xml;
using System.Xml.XPath;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
class SimpleDialogRunner : IEvent
{
	public TextAsset xmlFile;
	public Transform optionalPositionReference;
	public float heightAboveReference;
	public Rect dimensions;
	public Camera camera;
    public GameObject ezGUIdialogPrefab;
    public AudioClip audioClip;
    public string lipSynchAnimationClipName;
    public TextAsset xmlLipSynch;
    public float distanceToCamera;
    public NPCController forceNPC;
    public AnimationClip animationClip;
	public FaceFXController faceFX;
	public EventBase eBase;
    public WrapMode wrapMode = WrapMode.Once;
	
	public delegate void EnableCallback(SimpleDialogRunner r);
	public static EnableCallback enableCallback;

    public void OnEnter()
    {
        if (enableCallback != null)
            enableCallback(this);
    }

    public void OnExecute()
    {
    }

    public void OnExit()
    {
        PictorialRunner.exitFromAnotherEvent = true;
        Pictorial3Runner.exitFromAnotherEvent = true;
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
public class SimpleDialogEvent : EventBase
{
	public TextAsset xmlFile;
	public Transform optionalPositionReference;
	public float heightAboveReference;
	public Rect dimensions;
	public Camera camera;
    public GameObject ezGUIdialogPrefab;
    public AudioClip audioClip;
    public string lipSynchAnimationClipName;
    public TextAsset xmlLipSynch;
    public float distanceToCamera;
    public NPCController forceNPC;
    public AnimationClip animationClip;
    public WrapMode wrapMode = WrapMode.Once;

	//[HideInInspector]
    public override IEvent CreateRunner()
    {
        SimpleDialogRunner r = new SimpleDialogRunner();
        r.eBase = this;
        r.xmlFile = xmlFile;
        r.optionalPositionReference = optionalPositionReference;
        r.heightAboveReference = heightAboveReference;
        r.dimensions = dimensions;
        r.camera = camera;
        r.ezGUIdialogPrefab = ezGUIdialogPrefab;
        r.audioClip = audioClip;
        r.lipSynchAnimationClipName = lipSynchAnimationClipName;
        r.xmlLipSynch = xmlLipSynch;
        r.distanceToCamera = distanceToCamera;
        r.forceNPC = forceNPC;
        r.animationClip = animationClip;
        r.wrapMode = wrapMode;

        return r;
    }
	
	string eventName = "Simple Dialog Event";
	public override string GetEventName ()
	{
		return eventName;
	}
}
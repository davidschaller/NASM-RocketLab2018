using UnityEngine;
using System.Collections;
using System;

class PictorialRunner : IEvent
{
	public TextAsset xmlFile;
	
	public Texture2D backgroundImage;
	public Texture2D image;
	public Vector2 displaySize;
	public Vector2 displayLocation;
	public Vector2 startSize;
	public Vector2 startLocation;
	public Vector2 finishSize;
	public Vector2 finishLocation;
	public float fadeTime;
	public float moveTime;
	public Vector2 captionLocation;
	public Vector2 captionSize;
	public float buttonBottomInset = 45;
	
	enum PictorialState
	{
		Init,
		MoveIn,
		Display,
		MoveOut,
		Finished,
	}
	PictorialState state = PictorialState.Init;
	
	PictorialRenderer renderer;
	float fade = 0;
	
	// hack to allow Pictorial event to have no button and be exited via another event's button
	public static bool exitFromAnotherEvent = false;
	public void OnEnter ()
	{
		
		// hack to allow Pictorial event to have no button and be exited via another event's button
		exitFromAnotherEvent = false;
		Debug.Log("Enter PictorialEvent: fade time: "+fadeTime);

		if (MovementController.Main)
			MovementController.Main.LockPCControl();

		GameObject go = new GameObject();
		renderer = (PictorialRenderer)go.AddComponent(typeof(PictorialRenderer));
		renderer.backgroundImage = backgroundImage;
		renderer.image = image;
		renderer.imageRect = new Rect(startLocation.x, startLocation.y, startSize.x, startSize.y);
		renderer.captionRect = new Rect(captionLocation.x, captionLocation.y, captionSize.x, captionSize.y);
		renderer.caption = DialogXMLParser.GetText(xmlFile.text, "DialogText", ActiveLanguage.English);
		renderer.buttonText = DialogXMLParser.GetText(xmlFile.text, "ButtonText", ActiveLanguage.English);
		
		if (fadeTime != 0)
		{
			renderer.color = Color.white;
		}
		state = PictorialState.MoveIn;
		renderer.ShowCaption = true;
	}
	
	bool fadeInFinished = false;
	void FadeIn ()
	{
		if (fadeTime != 0 && renderer.color.a <= 1)
		{
			fade += Time.deltaTime/fadeTime;
			renderer.color.a = fade;
		}
		else
		{
			renderer.color = Color.white;
			fadeInFinished = true;
		}
	}
	
	float moveCounter = 0;
	
	bool MoveFromTo(Vector2 from, Vector2 fSize, Vector2 to, Vector2 tSize, float timeToReach)
	{
		Vector2 newXY = (Vector2)Vector3.Lerp((Vector3)from, (Vector3)to, moveCounter);
		Vector2 newSize = (Vector2)Vector3.Lerp((Vector3)fSize, (Vector3)tSize, moveCounter);
		
		renderer.imageRect = new Rect(newXY.x, newXY.y, newSize.x, newSize.y);
		moveCounter += Time.deltaTime/timeToReach;

		if (newXY.x == to.x && newXY.y == to.y)
		{
			return true;
		}
		
		return false;
	}
	
	Color imageColor;
	public void OnExecute ()
	{
		renderer.Debug = "State: " + state + " movetime: " + moveTime;
		
		if (state == PictorialState.Finished 
		    
		    // hack to allow Pictorial event to have no button and be exited via another event's button
		    || renderer.buttonText == "" && exitFromAnotherEvent)
			
		{
			eventFinished = true;
		}
		else
		{
			if (state == PictorialState.MoveIn || (state == PictorialState.Display && !fadeInFinished))
			{
				FadeIn();
			}
			
			if (state == PictorialState.MoveIn)
			{
				if (MoveFromTo(startLocation, startSize, displayLocation, displaySize, moveTime))
				{
					state = PictorialState.Display;
					if (renderer.buttonText != "")
						renderer.ShowButton = true;
					//renderer.ShowCaption = true;
				}
			}
			else if (state == PictorialState.MoveOut)
			{
				if (MoveFromTo(displayLocation, displaySize, finishLocation, finishSize, moveTime))
				{
					state = PictorialState.Finished;
				}
			}
			else if (state == PictorialState.Display && renderer.Done)
			{
				state = PictorialState.MoveOut;
				moveCounter = 0;
				renderer.ShowButton = false;
				//renderer.ShowCaption = false;
			}
			
		}
	}
	public void OnExit ()
	{
		Pictorial3Runner.exitFromAnotherEvent = true;
		
		GameObject.Destroy(renderer.gameObject);
		if (MovementController.Main)
			MovementController.Main.UnlockPCControl(false);

		Debug.Log("Exit PictorialEvent");
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
public class PictorialEvent : EventBase
{
	public TextAsset xmlFile;
	public Texture2D backgroundImage;
	public Texture2D image;
	public Vector2 displaySize;
	public Vector2 displayLocation;
	public Vector2 startSize;
	public Vector2 startLocation;
	public Vector2 finishSize;
	public Vector2 finishLocation;
	public float fadeTime;
	public float moveTime = 2;
	public Vector2 captionLocation;
	public Vector2 captionSize;
	public float buttonBottomInset = 45;
	
	//[HideInInspector]
	public override IEvent CreateRunner ()
	{
		PictorialRunner runner = new PictorialRunner();
		runner.xmlFile = xmlFile;
		runner.backgroundImage = backgroundImage;
		runner.image = image;
		runner.displaySize = displaySize;
		runner.displayLocation = displayLocation;
		runner.startSize = startSize;
		runner.startLocation = startLocation;
		runner.fadeTime = fadeTime;
		runner.finishSize = finishSize;
		runner.finishLocation = finishLocation;
		runner.moveTime = moveTime;
		runner.captionLocation = captionLocation;
		runner.captionSize = captionSize;
		runner.buttonBottomInset = buttonBottomInset;
		return runner;
	}
	
	string eventName = "Pictorial Event";
	public override string GetEventName ()
	{
		return eventName;
	}
}
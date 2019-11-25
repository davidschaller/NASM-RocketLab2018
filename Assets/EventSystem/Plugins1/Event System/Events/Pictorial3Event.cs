using UnityEngine;
using System.Collections;
using System;

class Pictorial3Runner : IEvent
{
	public Texture2D image1English;
	public Texture2D image1Spanish;
	public Texture2D image2English;
	public Texture2D image2Spanish;
	public Texture2D image3English;
	public Texture2D image3Spanish;
	public string button1Text;
	public string button1SpanishText;
	public string button2Text;
	public string button2SpanishText;
	public string button3Text;
	public string button3SpanishText;
	public Rect imageRect;
	public string buttonStyle;
	public Vector2 buttonTopLeft;
	public float intraButtonSpacing;
	public Vector2 button1Size;
	public Vector2 button2Size;
	public Vector2 button3Size;
	
	public delegate void Activate (Pictorial3Runner r);
	public static Activate callback;
	public static bool exitFromAnotherEvent = false;
	public void OnEnter ()
	{
		callback(this);
		exitFromAnotherEvent = false;
	}
	public void OnExecute ()
	{
		if (exitFromAnotherEvent)
			eventFinished = true;
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
public class Pictorial3Event : EventBase
{
	public Texture2D image1English;
	public Texture2D image1Spanish;
	public Texture2D image2English;
	public Texture2D image2Spanish;
	public Texture2D image3English;
	public Texture2D image3Spanish;
	public string button1Text;
	public string button1SpanishText;
	public string button2Text;
	public string button2SpanishText;
	public string button3Text;
	public string button3SpanishText;
	public Rect imageRect;
	public string buttonStyle;
	public Vector2 buttonTopLeft;
	public float intraButtonSpacing;
	public Vector2 button1Size;
	public Vector2 button2Size;
	public Vector2 button3Size;
				
	//[HideInInspector]
	public override IEvent CreateRunner ()
	{
		Pictorial3Runner r = new Pictorial3Runner();
		r.image1English = image1English;
		r.image1Spanish = image1Spanish;
		r.image2English = image2English;
		r.image2Spanish = image2Spanish;
		r.image3English = image3English;
		r.image3Spanish = image3Spanish;
		r.button1Text = button1Text;
		r.button1SpanishText = button1SpanishText;
		r.button2Text = button2Text;
		r.button2SpanishText = button2SpanishText;
		r.button3Text = button3Text;
		r.button3SpanishText = button3SpanishText;
		r.imageRect = imageRect;
		r.buttonStyle = buttonStyle;
		r.buttonTopLeft = buttonTopLeft;
		r.intraButtonSpacing = intraButtonSpacing;
		r.button1Size = button1Size;
		r.button2Size = button2Size;
		r.button3Size = button3Size;
		return r;
	}
	
	string eventName = "Pictorial 3 Event";
	public override string GetEventName ()
	{
		return eventName;
	}
}
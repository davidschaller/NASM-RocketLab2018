using UnityEngine;
using System.Collections;
using System;

class AudioRunner : IEvent
{
	//[HideInInspector]
	public AudioClip audioClip;
	public bool isSoundtrack;
	public GameObject soundSource;
	public int timesToPlay;
	public float volume;
	public AudioEvent audioEvent;
    public TextAsset xmlSubtitles;
	
	static AudioRunner soundTrack;
	public static AudioRunner SoundTrack
	{
		get
		{
			return soundTrack;
		}
		set
		{
			if (value == null && soundTrack != null)
			{
				soundTrack.eventFinished = true;
			}
			soundTrack = value;
		}
	}

    public static void TerminateSoundTrack()
    {
        if (soundTrack != null)
        {
            soundTrack.eventFinished = true;
        }
        if (GameObject.Find("Sound") != null)
        {
            GameObject.Destroy(GameObject.Find("Sound"));
        }
    }

	int playCount = 0;
	AudioSource audioSource;
	public void OnEnter ()
	{
        if (audioClip == null)
        {
            throw new EventException("Audio clip is NULL");
        }

		if (isSoundtrack)
		{
            if (soundTrack != null && soundTrack != this)
			{
				soundTrack.eventFinished = true;

                soundTrack.audioSource.enabled = false;
                GameObject goToDestroy = soundTrack.audioSource.gameObject;
				GameObject.Destroy(soundTrack.audioSource);
                GameObject.DestroyImmediate(goToDestroy);
			}

			soundTrack = this;
		}

        //Debug.Log("Enter audio event with clip " + audioClip.name + (isSoundtrack ? " as SoundTrack" : " as a sound"));

        GameObject go = new GameObject((isSoundtrack ? "Soundtrack: " : "Sound: ") + audioClip.name);
        audioSource = Common.AddAudioTo(go);

        audioSource.playOnAwake = false;
		audioSource.volume = volume;
		if (Application.isEditor && GUIManager.musicMuted)
		{
			storedVolume = volume;
			audioSource.volume = 0;
		}

        audioTimer = 0;
		audioSource.clip = audioClip;

        if (timesToPlay > 0)
        {
            audioLength = audioClip.length * timesToPlay;
            audioSource.loop = true;
        }
        else
        {
            audioSource.loop = false;
            audioLength = audioClip.length;
        }
		
		if (soundSource)
		{
			go.transform.parent = soundSource.transform;
			go.transform.localPosition = Vector3.zero;
		}
		else
		{
			if (Camera.main)
			{
				go.transform.parent = Camera.main.transform;
				go.transform.localPosition = Vector3.zero;
			}
		}

        audioSource.gameObject.SetActiveRecursively(true);

		if (isSoundtrack)
			eventFinished = true;

        if (SubtitlesManager.Main && xmlSubtitles)
        {
            SubtitlesManager.Main.Show(xmlSubtitles);
            xmlSubtitles = null;
        }

        audioSource.Play();
	}

	public static void MuteSoundtrack (bool tog)
	{
		if (SoundTrack == null)
			return;
		
		if (tog && SoundTrack.audioSource.volume > 0)
		{
			SoundTrack.storedVolume = SoundTrack.audioSource.volume;
			SoundTrack.audioSource.volume = 0;
		}
		else if (!tog && SoundTrack.audioSource.volume == 0)
		{
			SoundTrack.audioSource.volume = SoundTrack.storedVolume;
		}
	}

    float audioTimer = 0,
          audioLength = 0;

	float storedVolume = -1;

	/* isSoundTrack audio events should never reach OnExecute... */
	public void OnExecute ()
	{
        if (isSoundtrack)
        {
            eventFinished = true;
            return;
        }

        audioTimer += Time.deltaTime;
        if (audioSource && audioTimer > audioLength)
        {
            eventFinished = true;
        }
	}
	public void OnExit ()
	{
		//Debug.Log("Exiting audio event with clip " + audioClip.name);

		if (isSoundtrack)
		{
			if (soundTrack == this)
			{
				Debug.Log("Leaving soundtrack to play, but exiting event");
			}
		}
		else
		{
            if (audioSource)
            {
                audioSource.GetComponent<AudioSource>().Stop();
                GameObject.Destroy(audioSource.gameObject);
            }
		}		
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
public class AudioEvent : EventBase
{
	//[HideInInspector]
	public AudioClip audioClip;
	public bool isSoundtrack;
	public GameObject soundSource;
	public int timesToPlay;
	public float volume;
    public TextAsset xmlSubtitles;
	
	public override IEvent CreateRunner ()
	{
		AudioRunner runner = new AudioRunner();
		runner.audioClip = audioClip;
		runner.isSoundtrack = isSoundtrack;
		runner.soundSource = soundSource;
		runner.timesToPlay = timesToPlay;
		runner.volume = volume;
		runner.audioEvent = this;
        runner.xmlSubtitles = xmlSubtitles;
        xmlSubtitles = null;

		return runner;
	}
	
	
	string eventName = "Audio Event";
	public override string GetEventName ()
	{
		return eventName;
	}
}
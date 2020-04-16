using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioToggle : MonoBehaviour
{
	public Toggle audioToggle;
	bool audioOn;

	AudioSource audioSrc;

	OptionsData optionsData;

	IEnumerator Start()
	{
		yield return null;
		audioOn = true;
		audioSrc = gameObject.GetComponent<AudioSource>() as AudioSource;
		if(audioSrc == null) {
			Debug.Log("Audio Toggle has no audio source. Add one to play a button click sound.");
		}

		if(audioToggle == null) {
			audioToggle = gameObject.GetComponent<Toggle>() as Toggle;
		}

		optionsData = FindObjectOfType<OptionsData>();
		if(optionsData != null) {
			if (!optionsData.AudioOn) {
				audioToggle.isOn = false;
			}
		}
	}

	public void ToggleAudio()
	{
		audioOn = !audioOn;
		if (audioOn) {
			AudioListener.volume = 1.0f;
		} else {
			AudioListener.volume = 0.0f;
		}

		if(optionsData != null) {
			optionsData.AudioOnOff(audioOn);
		}

		if (audioSrc != null) {
			audioSrc.Play();
		}
	}
}

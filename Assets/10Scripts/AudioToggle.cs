using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioToggle : MonoBehaviour
{
	public UICheckbox audioToggle;
	bool audioOn;

	OptionsData optionsData;

	IEnumerator Start()
	{
		AudioListener.volume = 0.0f;

		yield return null;
		audioOn = true;

		if(audioToggle == null) {
			audioToggle = gameObject.GetComponent<UICheckbox>() as UICheckbox;
		}

		optionsData = FindObjectOfType<OptionsData>();
		if(optionsData != null) {
			if (!optionsData.AudioOn) {
				audioToggle.isChecked = false;
			} else {
				AudioListener.volume = 1.0f;
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
	}
}

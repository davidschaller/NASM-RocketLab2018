using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioToggle : MonoBehaviour
{
	public UICheckbox audioToggle;
	bool audioOn;

	OptionsData optionsData;

	bool blockInput;

	IEnumerator Start()
	{
		AudioListener.volume = 0.0f;

		yield return null;
		audioOn = true;

		if(audioToggle == null) {
			audioToggle = gameObject.GetComponent<UICheckbox>() as UICheckbox;
		}

		blockInput = false;

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
		if (blockInput) {
			return;
		}

		blockInput = true;

		audioOn = !audioOn;
		if (audioOn) {
			AudioListener.volume = 1.0f;
		} else {
			AudioListener.volume = 0.0f;
		}

		if(optionsData != null) {
			optionsData.AudioOnOff(audioOn);
		}

		StartCoroutine(Unblock());
	}

	IEnumerator Unblock() {
		yield return null;
		blockInput = false;
	}
}

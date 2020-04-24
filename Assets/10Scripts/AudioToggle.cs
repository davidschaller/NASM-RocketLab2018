using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioToggle : MonoBehaviour
{
	public UIToggle audioToggle;
	bool audioOn;

	OptionsData optionsData;

	bool blockInput;

	IEnumerator Start()
	{
		AudioListener.volume = 0.0f;

		yield return null;
		audioOn = true;

		if(audioToggle == null) {
			audioToggle = gameObject.GetComponent<UIToggle>() as UIToggle;
		}

		blockInput = false;

		optionsData = FindObjectOfType<OptionsData>();
		if(optionsData != null) {
			if (!optionsData.AudioOn) {
				audioToggle.value = false;
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

		bool audioCheck = false;
		if (audioOn) {
			if(!audioToggle.value) {
				audioToggle.value = true;
				audioCheck = true;
			}
		} else {
			if (audioToggle.value) {
				audioToggle.value = false;
				audioCheck = true;
			}
		}

		if (audioCheck) {
			yield return null;
		}

		blockInput = false;
	}
}

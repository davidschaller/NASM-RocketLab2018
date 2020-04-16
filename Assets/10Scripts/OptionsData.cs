using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsData : MonoBehaviour
{
	bool audioOn;
	public bool AudioOn {
		get { return audioOn; }
	}

	bool isInstance;
    void Start()
    {
		OptionsData[] od = FindObjectsOfType<OptionsData>();
		if(od.Length > 1) {
			if (!isInstance) {
				Destroy(gameObject);
				return;
			}
		}

		isInstance = true;
		audioOn = true;
		DontDestroyOnLoad(gameObject);
    }

	public void AudioOnOff(bool isOn)
	{
		audioOn = isOn;
	}
}

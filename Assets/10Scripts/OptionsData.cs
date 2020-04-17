using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class OptionsData : MonoBehaviour
{
#if UNITY_WEBGL
	[DllImport("__Internal")]
	private static extern void Init();
#endif

	protected bool audioOn;
	public bool AudioOn {
		get { return audioOn; }
	}

	protected bool treatAsMobile;
	public bool TreatAsMobile {
		get { return treatAsMobile; }
	}

	protected bool isInstance;
    void Start()
    {
		OptionsData[] od = FindObjectsOfType<OptionsData>();
		if(od.Length > 1) {
			if (!isInstance) {
				Destroy(gameObject);
				return;
			}
		}

		treatAsMobile = false;
		isInstance = true;
		audioOn = true;
		DontDestroyOnLoad(gameObject);

#if UNITY_WEBGL
		if (!Application.isEditor) {
			StartCoroutine("SendInit");
		}
#endif
	}

	IEnumerator SendInit()
	{
		yield return null;
#if UNITY_WEBGL
		Init();
#endif
	}

	public void AudioOnOff(bool isOn)
	{
		audioOn = isOn;
	}

	public void SetAsTouchDevice()
	{
#if UNITY_WEBGL
		treatAsMobile = true;
#endif
	}
}

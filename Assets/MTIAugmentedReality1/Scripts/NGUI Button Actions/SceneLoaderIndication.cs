using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneLoaderIndication : MTIUIBase
{
	public static List<SceneLoaderIndication> candidates;
	
	void Start ()
	{
		if (candidates == null)
			candidates = new List<SceneLoaderIndication>();
		
		candidates.Add(this);
		
		Debug.Log("Scene loader candidates: " + candidates.Count);
		
//		active = false;
	}
	
	IEnumerator StartLoadLevel (string lvl)
	{
		UISlider slider = GetComponent<UISlider>();
		if (slider == null)
		{
			yield return 0;
			Application.LoadLevel(lvl);
		}
		else
		{
			AsyncOperation async = Application.LoadLevelAsync(lvl);
		
			while (!async.isDone)
			{
				slider.sliderValue = async.progress;
				yield return async;
			}			
		}
	}
	
	public static void LoadLevel (string lvl)
	{
		foreach(SceneLoaderIndication s in candidates)
		{
			if (s != null && s.gameObject.transform.root.gameObject.activeInHierarchy)
			{
//				s.active = true;
				s.StartCoroutine(s.StartLoadLevel(lvl));
			}
		}
	}
}

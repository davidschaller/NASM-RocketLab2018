using UnityEngine;
using System.Collections;

public class LoadMainSceneButton : MonoBehaviour
{
	public string mainSceneName = "WotB town";
	public GameObject[] activateBeforeSceneLoad;
	UISlider loadingBar;

	public static LoadMainSceneButton main;
	IEnumerator Start ()
	{
		yield return 0;
		
		main = this;
		foreach(GameObject g in activateBeforeSceneLoad)
		{
			g.SetActiveRecursively(false);
		}
	}

	public void DoLoad ()
	{
		foreach(GameObject go in activateBeforeSceneLoad)
		{
			if (go != null)
			{
				go.SetActiveRecursively(true);
				if (loadingBar == null)
				{
					loadingBar = go.GetComponent<UISlider>();
				}				
			}
		}
		
		
		StartCoroutine(LoadLevelProgress());		
	}

	void OnClick ()
	{
		PersistentData.Main.virtualWalkthrough = false;
		
		MovementButton.RemoveAllButtons();
		
		DoLoad();
	}
	
	IEnumerator LoadLevelProgress ()
	{
		AsyncOperation async = Application.LoadLevelAsync(mainSceneName);
		
		while (!async.isDone)
		{
			if (loadingBar)
				loadingBar.sliderValue = async.progress;
			yield return async;
		}
	}
}

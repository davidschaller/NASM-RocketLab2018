using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour 
{
	public void OpenMainScene()
	{
        SceneManager.LoadScene("Main");
	}

	public void OpenBackgroundStarfieldScene()
	{
		SceneManager.LoadScene("BackgroundStarfieldExample");
	}

	public void OpenInfiniteStarfieldScene()
	{
        SceneManager.LoadScene("InfiniteStarfieldExample");
	}

	public void OpenStarmapScene()
	{
        SceneManager.LoadScene("StarmapExample");
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) 
		{
			OpenMainScene();
		}
	}
}

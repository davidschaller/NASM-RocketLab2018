using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
    static LevelLoader main;

    const string GO_NAME = "Level Loader";

    LoadLevelRendererBase loadLevelRenderer;

    float progress = 0;

	string sceneName;

    AudioSource mainTheme;

    public static void LoadLevelProgress(string sceneName, Transform trloadLevelDialog, TextAsset xml)
    {
        GameObject go = new GameObject(GO_NAME);

        main = go.AddComponent<LevelLoader>();
        main.enabled = false;

        main.StartLoading(sceneName, trloadLevelDialog, xml);

        Application.backgroundLoadingPriority = ThreadPriority.High;

        AudioSource[] aSources = (AudioSource[])GameObject.FindObjectsOfType(typeof(AudioSource));
        if (aSources != null && aSources.Length > 0)
        {
            foreach (AudioSource item in aSources)
            {
                if (item.isPlaying && item.transform.name.StartsWith("Sound: "))
                {
                    main.mainTheme = item;
                    break;
                }
            }
        }
    }

    void StartLoading(string scene, Transform trloadLevel, TextAsset xml)
    {
        if (trloadLevel)
        {
            loadLevelRenderer = trloadLevel.GetComponent<LoadLevelRendererBase>();

            if (loadLevelRenderer)
            {
                loadLevelRenderer.Show(xml);
            }
        }

        sceneName = scene;

        enabled = true;
    }

    float TestLoading()
    {
        progress += Time.deltaTime * 0.3f;

        return Mathf.Clamp(progress, 0, 1);
    }
	
	void Update ()
	{
        if (Application.isEditor)
        {
            progress = TestLoading();
        }
        else
            progress = Application.GetStreamProgressForLevel(sceneName);

        if (loadLevelRenderer)
        {
            loadLevelRenderer.UpdateProgress(progress);
        }

        if (progress == 1)
        {
            Application.LoadLevelAsync(sceneName);
            enabled = false;
            Invoke("DestroyMe", 0.5f);
        }
	}

    void DestroyMe()
    {
        Destroy(gameObject);
    }

    public static void Terminate()
    {
        if (main)
        {
            main.enabled = false;
            main.DestroyMe();
            if (main.loadLevelRenderer)
            {
                main.loadLevelRenderer.Hide();
            }
        }
    }
}

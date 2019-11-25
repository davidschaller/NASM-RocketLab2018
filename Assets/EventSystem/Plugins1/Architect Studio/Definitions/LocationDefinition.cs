using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

public class LocationDefinition : MonoBehaviour
{
    public string sceneName;
    public Vector3 importedScenePosition;

    public Texture2D picture,
                     thumbnailPicture;
    public string url;
    public List<string> features;
    public int id = 0;

    private WWW stream;
    public WWW Stream
    {
        get
        {
            return stream;
        }
        set 
        {
            stream = value;
        }
    }

    public void Download()
    {
        stream = new WWW(url);
    }

    public bool IsDownloading()
    {
        return stream != null && !stream.isDone;
    }

    public bool IsDownloaded()
    {
        return stream != null && stream.isDone;
    }

    public float GetDownloadProgress()
    {
        if (stream != null)
        {
            return stream.progress;
        }
        else
            return 0;
    }

    private AssetBundle ab = null;
    public bool IsInstantiated()
    {   
        return ab != null;
    }

    public void Instantiate()
    {
        if (string.IsNullOrEmpty(stream.error))
        {
            ab = stream.assetBundle;
            if (!string.IsNullOrEmpty(sceneName))
                Application.LoadLevelAdditive(sceneName);

            this.gameObject.SetActiveRecursively(true);
            StartCoroutine(SetImportedPosition("Imported Scene", importedScenePosition));
        }
        else
        {
            Debug.LogError(stream.error);
            stream = null;
        }
    }

    private IEnumerator SetImportedPosition(string p_Name, Vector3 p_Position)
    {
        while (!GameObject.Find(p_Name))
            yield return 0;

        GameObject.Find(p_Name).transform.position = p_Position;

        SoundManager.SoundSwitcher(ArchitectStudioGUI.CurrentState);

        yield return 0;
    }

    public void Destroy()
    {
        if (ab != null)
        {
            ab.Unload(true);
            ab = null;
        }
    }

    public void Deactivate()
    {
        gameObject.SetActiveRecursively(false);
    }

    public void Activate()
    {
        Debug.Log("Activating " + gameObject.name);
        gameObject.SetActiveRecursively(true);
    }
}

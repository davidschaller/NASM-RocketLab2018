using UnityEngine;
using System;

public class GalleryCleaner : MonoBehaviour
{
    public int maxDaysInGallery = 30;
    public bool removeALLOnStart = false;

    private SaveAS3DManager oSaveAS3DManager;

    private void Awake()
    {
        oSaveAS3DManager = (SaveAS3DManager)GameObject.FindObjectOfType(typeof(SaveAS3DManager));

        if (oSaveAS3DManager == null)
            Debug.Log("LoadAS3DManager is NULL");
    }

    private void Start()
    {
        if (removeALLOnStart)
        {
            oSaveAS3DManager.RemoveAllFromGallery();
        }
    }

    private void OnGUI()
    {
        if (oSaveAS3DManager == null)
            this.enabled = false;

        if (LoadAS3DManager.GameList != null && LoadAS3DManager.GameList.Count > 0)
        {
            foreach (GameData game in LoadAS3DManager.GameList)
            {
                if (DateTime.Now.AddDays(-maxDaysInGallery) > game.oInfo.dateOfSubmition)
                {
                    oSaveAS3DManager.Prune(game.GameId);
                }
            }
            this.enabled = false;
        }
    }
}


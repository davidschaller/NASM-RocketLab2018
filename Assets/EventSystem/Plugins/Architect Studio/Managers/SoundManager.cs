using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class SoundManager
{
    private static List<AudioSource> audioList;
    public static List<AudioSource> AudioList
    {
        get
        {
            audioList = new List<AudioSource>();

            GameObject goImportedScene = GameObject.Find("Imported Scene");

            if (goImportedScene)
            {
                foreach (AudioSource audioTr in goImportedScene.GetComponentsInChildren<AudioSource>())
                {
                    audioList.Add(audioTr);
                }
            }

            return audioList;
        }
        set
        {
            audioList = value;
        }
    }

    private static void EnableAllSounds()
    {
        foreach (AudioSource audio in AudioList)
        {
            audio.enabled = true;
        }
    }

    private static void DisableAllSounds()
    {
        foreach (AudioSource audio in AudioList)
        {
            audio.enabled = false;
        }
    }

    public static void SoundSwitcher(ArchitectStudioGUI.States p_state)
    {
        if (p_state == ArchitectStudioGUI.States.TourHouse || p_state == ArchitectStudioGUI.States.GalleryTour)
            SoundManager.EnableAllSounds();
        else
            SoundManager.DisableAllSounds();
    }
}


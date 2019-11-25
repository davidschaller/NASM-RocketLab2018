using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

public class ScreenShotMaker
{
    public static Texture2D GetImageFromCamera(int p_width, int p_height, Camera p_camera)
    {
        Rect savedCamerRect = p_camera.rect;

        p_camera.rect = new Rect(0, 0, 1, 1);

        Texture2D screenShot = ReadImage(p_width, p_height, p_camera);

        p_camera.rect = savedCamerRect;

        return screenShot;
    }

    public static Texture2D GetImageFromCamera(int p_width, int p_height, Camera p_camera, Rect p_camRect, Vector3 p_camPosition, Vector3 p_lookAt)
    {
        Rect savedCamerRect = p_camera.rect;
        Vector3 savedCamPosition = p_camera.transform.position;
        Quaternion savedCamRotation = p_camera.transform.rotation;

        p_camera.rect = new Rect(0, 0, 1, 1);
        p_camera.transform.position = p_camPosition;
        p_camera.transform.LookAt(p_lookAt);

        Texture2D screenShot = ReadImage(p_width, p_height, p_camera);

        p_camera.rect = savedCamerRect;
        p_camera.transform.position = savedCamPosition;
        p_camera.transform.rotation = savedCamRotation;

        return screenShot;
    }

    private static string ScreenShotName(int width, int height)
    {
        return string.Format("{0}/screenshots/screen_{1}x{2}_{3}.png", Application.dataPath, width, height, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    private static Texture2D ReadImage(int p_width, int p_height, Camera p_camera)
    {
        RenderTexture rt = new RenderTexture(p_width, p_height, 24);
        p_camera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(p_width, p_height, TextureFormat.RGB24, false);
        p_camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, p_width, p_height), 0, 0);
        p_camera.targetTexture = null;
        RenderTexture.active = null;
        GameObject.Destroy(rt);

        return screenShot;
    }

    internal static void SaveLocalCopy(byte[] p_bytes)
    {
        string filename = ScreenShotName(1, 1);

        Debug.Log("Saving to " + filename);
        //File.WriteAllBytes(filename, p_bytes);
    }

}


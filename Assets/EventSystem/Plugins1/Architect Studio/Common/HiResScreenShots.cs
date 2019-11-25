using UnityEngine;
using System.Collections;

public class HiResScreenShots : MonoBehaviour
{
    private int resWidth = 210;
    private int resHeight = 150;

    private bool takeHiResShot = false;

    public static string ScreenShotName(int width, int height)
    {
        return string.Format("{0}/screenshots/screen_{1}x{2}_{3}.png", Application.dataPath, width, height, System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    public void TakeHiResShot()
    {
        takeHiResShot = true;
    }

    Rect oldRect;
    Vector3 oldPos = Vector3.zero;
    Quaternion oldRotation;

    void LateUpdate()
    {
        takeHiResShot |= Input.GetKeyDown("k");
        if (takeHiResShot)
        {
            if (oldPos == Vector3.zero)
            {
                oldRect = GetComponent<Camera>().rect;
                oldPos = GetComponent<Camera>().transform.position;
                oldRotation = GetComponent<Camera>().transform.rotation;
            }

            GetComponent<Camera>().rect = new Rect(0, 0, 1, 1);
            // Get Look At center
            Vector3 buildingCenter = BuildingManager.Picked.transform.position +
                BuildingManager.Picked.transform.TransformDirection(Vector3.forward) * BuildingManager.Picked.lookAtOffset.x +
                    BuildingManager.Picked.transform.TransformDirection(Vector3.right) * BuildingManager.Picked.lookAtOffset.y;

            GetComponent<Camera>().transform.position = buildingCenter + Vector3.up * 30 + BuildingManager.Picked.transform.TransformDirection(Vector3.forward) * 100;
            GetComponent<Camera>().transform.LookAt(buildingCenter);

            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            GetComponent<Camera>().targetTexture = rt;
            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            GetComponent<Camera>().Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            GetComponent<Camera>().targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            Destroy(rt);
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = ScreenShotName(resWidth, resHeight);
            //System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("Took screenshot to: {0}", filename));
            takeHiResShot = false;

            GetComponent<Camera>().rect = oldRect;
            GetComponent<Camera>().transform.position = oldPos;
            GetComponent<Camera>().transform.rotation = oldRotation;
        }
    }
}

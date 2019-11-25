using UnityEngine;
using System.Collections;

public class CameraFade : MonoBehaviour
{
    const string GAME_OBJECT_NAME = "Camera Fade";

    public static CameraFade Main { get; private set; }

    public bool Processing { get; private set; }

    public RenderTexture tex;
    public Material mat;

    public Camera camFromDebug,
                  camToDebug,
                  camKeepRendering;

    // Initialize the texture, background-style and initial color:
    void Awake()
    {
        if (!Main)
            Main = this;

        enabled = false;
    }

    static CameraFade GetInstance()
    {
        GameObject go = new GameObject(GAME_OBJECT_NAME);
        return go.AddComponent<CameraFade>();
    }

    // Draw the texture and perform the fade:
    void OnGUI()
    {
        if (Event.current.type.Equals(EventType.Repaint) && tex && mat)
        {
            Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), tex, mat);

            if (camKeepRendering && camKeepRendering.gameObject.activeSelf)
            {
                camKeepRendering.Render();
            }
        }
    }

    public static void Fade(Camera camFrom, Camera camTo, Camera keepRendering, float time)
    {
        if (!Main)
            Main = GetInstance();

        if (Main)
        {
            keepRendering = GetCorrectCamera(keepRendering);

            Main.camKeepRendering = keepRendering;

            Main.StartCoroutine(Main.CamerasFade(camFrom, camTo, time));
        }
        else
            Debug.LogError("Couldn't create the instance of CameraFade");
    }

    IEnumerator CamerasFade(Camera camFrom, Camera camTo, float time)
    {
        if (Processing)
            yield break;

        Processing = true;

        CameraSetup(ref camFrom, ref camTo);

        Main.camFromDebug = camFrom;
        Main.camToDebug = camTo;

        RenderTexture texFrom = new RenderTexture(Screen.width, Screen.height, 24);
        RenderTexture texTo = new RenderTexture(Screen.width, Screen.height, 24);

        tex = texFrom;

        camFrom.targetTexture = texFrom;
        camTo.targetTexture = texTo;

        mat = new Material(Shader.Find("Blend 2 Textures"));
        mat.SetTexture("_MainTex", texFrom);
        mat.SetTexture("_Texture2", texTo);
        mat.SetFloat("_Blend", 0);

        enabled = true;

        float timer = 0;

        while (timer <= 1)
        {
            timer += Time.deltaTime / time;
            mat.SetFloat("_Blend", Mathf.Clamp01(timer));

            yield return 0;
        }

        camFrom.targetTexture = null;
        camTo.targetTexture = null;

        texFrom.Release();
        texTo.Release();

        enabled = false;
        CameraCleanup(camFrom, camTo);
        Processing = false;
    }

    void CameraSetup(ref Camera camFrom, ref Camera camTo)
    {
        camFrom = GetCorrectCamera(camFrom);
        camTo = GetCorrectCamera(camTo);

        camFrom.gameObject.SetActive(true);
        camTo.gameObject.SetActive(true);

        AudioListener listener = camTo.GetComponent<AudioListener>();
        if (listener)
            listener.enabled = false;
    }

    static Camera GetCorrectCamera(Camera cam)
    {
        if (ResolutionDetector.Main)
        {
            if (!cam.gameObject.activeInHierarchy)
            {
                if (cam.transform.parent && !cam.transform.parent.gameObject.activeInHierarchy)
                {
                    if (cam.transform.parent.gameObject.name == "Anchor")
                    {
                        bool cam2D = false;

                        if (cam.transform.parent.parent && !cam.transform.parent.parent.gameObject.activeInHierarchy)
                        {
                            if (cam.transform.parent.parent.name.Contains("2D"))
                            {
                                cam2D = true;
                            }
                        }

                        Transform cameraReplacement = null;

                        if (cam2D)
                        {
                            cameraReplacement = ResolutionDetector.Main.Gui2D.Find("Anchor/" + cam.name);
                        }
                        else
                        {
                            cameraReplacement = ResolutionDetector.Main.Gui3D.Find("Anchor/" + cam.name);
                        }

                        if (cameraReplacement)
                        {
                            return cameraReplacement.GetComponent<Camera>();
                        }
                        else
                            Debug.LogError("Couldn't replace " + cam.name);
                    }
                }
            }
        }

        return cam;
    }

    void CameraCleanup(Camera camFrom, Camera camTo)
    {
        AudioListener listener = camTo.GetComponent<AudioListener>();
        if (listener)
            listener.enabled = true;

        camFrom.gameObject.active = false;
    }


}
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkyboxToggle : MonoBehaviour
{
    public Camera myBackgroundCamera;
    private Toggle myToggle;

    void Awake()
    {
        myToggle = GetComponent<Toggle>();
    }

    void Start()
    {
        myToggle.isOn = myBackgroundCamera.clearFlags == CameraClearFlags.Skybox;
        myToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    public void OnToggleChanged(bool isOn)
    {
        if (isOn)
            myBackgroundCamera.clearFlags = CameraClearFlags.Skybox;
        else
            myBackgroundCamera.clearFlags = CameraClearFlags.SolidColor;
    }
}

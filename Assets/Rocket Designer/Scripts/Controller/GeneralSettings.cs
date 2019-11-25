using UnityEngine;

public class GeneralSettings : MonoBehaviour
{
    public bool runInBackground = false;

    void Awake()
    {
        Application.runInBackground = runInBackground;
    }
}

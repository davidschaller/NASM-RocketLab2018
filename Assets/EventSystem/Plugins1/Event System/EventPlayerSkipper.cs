using UnityEngine;
using System.Collections.Generic;

public class EventPlayerSkipper : MonoBehaviour
{
    public Transform epsContainer;

    public Camera skipToCam;

    public List<EventPlayer> epsToSkip;

    public EventPlayer epToStart;

    public List<KeyCode> keyCodes;

    void Awake()
    {
        if (epsToSkip == null || epsToSkip.Count == 0)
            epsToSkip = new List<EventPlayer>();

        if (epsContainer)
        {
            EventPlayer[] eps = epsContainer.GetComponentsInChildren<EventPlayer>();

            if (eps != null && eps.Length > 0)
            {
                epsToSkip.AddRange(eps);
            }
        }

        if (epsToSkip.Count == 0 || !epToStart)
        {
            enabled = false;
        }
    }

    void Start()
    {
        if (keyCodes == null || keyCodes.Count == 0)
        {
            keyCodes = new List<KeyCode>();
            keyCodes.Add(KeyCode.Space);
        }
    }

    void Update()
    {
        if (Time.deltaTime == 0)
            return;

        bool skipped = false;
        foreach (KeyCode code in keyCodes)
        {
            if (Input.GetKey(code))
            {
                skipped = true;
                break;
            }
        }

        if (skipped)
        {
            Skip();
            enabled = false;
        }
    }

    void Skip()
    {
        if (skipToCam)
        {
            Camera[] cameras = (Camera[])GameObject.FindObjectsOfType(typeof(Camera));
            foreach (Camera cam in cameras)
                if (cam != skipToCam)
                    cam.gameObject.active = false;

            skipToCam.gameObject.active = true;
        }

        foreach (EventPlayer ep in epsToSkip)
        {
            if (ep.HasTriggered)
            {
                ep.Stop();
            }

            ep.StopAllCoroutines();
        }

        epToStart.PlayerTriggered();

        enabled = false;
    }
}

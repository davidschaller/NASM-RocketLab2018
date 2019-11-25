using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InactivityTimer : MonoBehaviour
{
    public int InactivitySecond = 120;

    private float startTime = 0;

    private void StartCountDown()
    {
        startTime = Time.time;
    }

    private void CheckTime()
    {
        if ((Time.time - startTime) > InactivitySecond)
            Done();
    }

    Vector3 oldMousePosition;

    private void CheckActivity()
    {
        if (!Input.mousePosition.Equals(oldMousePosition) || Input.touchCount > 0)
        {
            oldMousePosition = Input.mousePosition;
            StartCountDown();
        }
    }

    private void Done()
    {
        this.gameObject.GetComponent<MissionController>().HomeClick();
    }

    void Start()
    {
        StartCountDown();
    }

    void Update()
    {
        if (InactivitySecond > 0)
        {
            CheckActivity();

            CheckTime();
        }
    }
}
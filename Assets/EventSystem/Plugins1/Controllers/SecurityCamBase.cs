using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class SecurityCamBase : GUIBase
{
    private static bool hasWarned = true;
    protected static bool HasWarned 
    {
        get
        {
            return hasWarned;
        }
        set
        {
            hasWarned = true;
        }
    }

    public static int Counter {get; set;}
    public static int SneakCounter { get; set; }
    public static bool inGaze { get; set; }


    protected void GazeIntered(bool shift)
    {
        if (Counter == 0 && SneakCounter == 0)
            hasWarned = false;

        if (shift)
        {
            SneakCounter++;
        }
        else
            Counter++;
        inGaze = true;
    }

    protected void GazeExit()
    {
        inGaze = false;
    }
}

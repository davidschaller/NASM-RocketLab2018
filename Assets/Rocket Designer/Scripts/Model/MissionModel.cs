using UnityEngine;
using System.Collections;

public static class MissionModel
{
    public static int launchAttempts { get; private set; }
    public static int failedLaunchAttempts { get; private set; }

    public static float[] Improvements { get; private set; }

    public static void SaveAttempts(int attempts, int failedattempts)
    {
        launchAttempts = attempts;
        failedLaunchAttempts = failedattempts;
    }

    public static void SaveImprovements(float[] improvements)
    {
        Improvements = improvements;
    }

    public static void ResetAll()
    {
        launchAttempts = 0;
        Improvements = null;
    }
}

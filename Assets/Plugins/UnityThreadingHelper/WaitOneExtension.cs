using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class WaitOneExtension
{
    private static System.Reflection.MethodInfo WaitOneMilliseconds;
    private static System.Reflection.MethodInfo WaitOneTimeSpan;
    internal static bool isWebPlayer;

    static WaitOneExtension()
    {
        var type = typeof(System.Threading.ManualResetEvent);
		WaitOneMilliseconds = type.GetMethod("WaitOne", new System.Type[1] { typeof(int) } );
		WaitOneTimeSpan = type.GetMethod("WaitOne", new System.Type[1] { typeof(TimeSpan) } );
    }

    public static bool InterWaitOne(this System.Threading.ManualResetEvent that, int ms)
    {
        return isWebPlayer ? (bool)WaitOneMilliseconds.Invoke(that, new object[1] { ms }) : that.WaitOne(0, false);
    }

    public static bool InterWaitOne(this System.Threading.ManualResetEvent that, TimeSpan duration)
    {
        return isWebPlayer ? (bool)WaitOneTimeSpan.Invoke(that, new object[1] { duration }) : that.WaitOne(duration, false);
    }
}

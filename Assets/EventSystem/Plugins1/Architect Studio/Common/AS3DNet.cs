using UnityEngine;
using System.Collections;

public class AS3DNet : MonoBehaviour
{
    protected string serverUrl = "http://architectstudio3d.org/DB2/";//"http://74.207.232.171:8100/";

    protected void Results(string p_results)
    {
        Debug.Log("Results: " + p_results);
    }

    protected void SetGameID(string p_results)
    {
        int result = -1;

        int.TryParse(p_results, out result);

        if (result >= 0)
            LoadAS3DManager.oGameData.GameId = result;
        else
            LoadAS3DManager.oGameData.GameId = -500;

        if (result >= 0)
            Debug.Log("RegisterNewGame result: GameID = " + result);
        else
            Debug.LogError("Coudl't get GameID from the server. Saving is failed");
    }

    public delegate void PostCallback(string result);

    private static int numOfProcess = 0;
    public bool RequestFinished
    {
        get
        {
            return numOfProcess > 0 ? false: true;
        }
    }


    protected IEnumerator Post(string p_action, WWWForm p_form, PostCallback p_func)
    {
        numOfProcess++;

        Debug.Log("Posting data: " + p_action);
        WWW w = null;

        if (p_form != null)
        {
            w = new WWW(serverUrl + p_action, p_form);
        }
        else
            w = new WWW(serverUrl + p_action);

        while (!w.isDone)
            yield return w;

        if (w.error != null)
        {
            Debug.LogError("WWW ERROR: " + w.error);
            if (p_func != null)
                p_func(w.error);
        }
        else
        {
            if (p_func != null)
                p_func(w.text);
        }

        Debug.Log("num of process " + numOfProcess);

        numOfProcess--;
    }
}


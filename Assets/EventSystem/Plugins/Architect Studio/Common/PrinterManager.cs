using UnityEngine;
using System;

public class PrinterManager : AS3DNet
{
    private bool isFinished = false;
    public bool IsFinished
    {
        get
        {
            return isFinished;
        }
        set
        {
            isFinished = value;
        }
    }

    private bool isInProcess = false;
    public bool IsInProcess
    {
        get
        {
            return isInProcess;
        }
        set
        {
            isInProcess = value;
        }
    }

    private bool isSuccess = false;
    public bool IsSuccess
    {
        get
        {
            return isSuccess;
        }
    }

    private bool canShowNotification = false;
    public bool CanShowNotification
    {
        get
        {
            return canShowNotification;
        }
        set
        {
            canShowNotification = value;
        }
    }

    private void Status(string p_input)
    {
        if (p_input.Contains("success"))
        {
            isSuccess = true;
        }

        isInProcess = false;
        isFinished = true;
        canShowNotification = false;
    }

    public void PreparePrint(string p_id, byte[] p_bytes)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", p_id);

        string img = Convert.ToBase64String(p_bytes, 0, p_bytes.Length);

        form.AddField("picture", img);

        StartCoroutine(Post("PreparePrint", form, Status));
    }
}

using UnityEngine;

public class LoadLevelRendererBase : MonoBehaviour
{
    public bool IsShowed { get; private set; }

    protected float ProgressValue { get; private set; }

    public virtual void Show(TextAsset xml)
    {
        IsShowed = true;
    }

    public virtual void Hide()
    {
        IsShowed = false;
    }

    public virtual void ProgressChanged()
    {
    }

    public void UpdateProgress(float progressValue)
    {
        ProgressValue = progressValue;
        ProgressChanged();
    }
}

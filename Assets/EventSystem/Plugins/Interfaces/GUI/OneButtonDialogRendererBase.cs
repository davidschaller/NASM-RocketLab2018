using UnityEngine;
using System.Collections;

public class OneButtonDialogRendererBase : MonoBehaviour
{
    public const string GO_NAME = "One Button Dialog";

    public delegate void SimpleVoidDelegate();

    public SimpleVoidDelegate clickCallback;

    public bool IsShowed { get; private set; }

    public virtual void Show(TextAsset xmlFile, SimpleVoidDelegate callback)
    {
        clickCallback += callback;

        IsShowed = true;
    }

    public virtual void Hide()
    {
        IsShowed = false;
    }
}

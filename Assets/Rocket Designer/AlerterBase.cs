using UnityEngine;

public abstract class AlerterBase : MonoBehaviour
{
    public TextAsset xml;

    public enum OneButtonAlerts
    {
        NotEnoughMoney,
        ControlIsTooLow,
        ThristWeightIsTooLow,
        Success,
        FailedComponentIsFixed,
        TooManyComponentsFailed
    }

    public enum TwoButtonAlerts
    {
        ThrustWeightIsTooLow,
        HasSpentAllTheirMoney
    }



    public abstract bool IsActive { get; }

    public delegate void SimpleClickCallback();
    public delegate void TwoButtonsClickCallback(bool decision);

    public SimpleClickCallback simpleClickCallback;
    public SimpleClickCallback simpleClickCallbackInfo;
    public TwoButtonsClickCallback twoButtonsClickCallback;

    public abstract void ShowOneButton(OneButtonAlerts alertType, string[] values, SimpleClickCallback callback, bool allowAlternative);
    public abstract void ShowOneButton(OneButtonAlerts alertType, string[] values, SimpleClickCallback callback, SimpleClickCallback callbackInfo, bool allowAlternative);

    public abstract void ShowTwoButtons(TwoButtonAlerts alertType, string[] values, TwoButtonsClickCallback callback);

    public abstract void Hide(bool isHide);
}

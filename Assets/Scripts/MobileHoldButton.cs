using UnityEngine;
using UnityEngine.EventSystems;

public class MobileHoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public enum Control { Accelerate, Reverse, Left, Right, Boost }
    public Control control;
    public CarController target;
    public NetworkedCarController networkTarget;

    public void OnPointerDown(PointerEventData eventData) => SetPressed(true);
    public void OnPointerUp(PointerEventData eventData) => SetPressed(false);
    public void OnPointerExit(PointerEventData eventData) => SetPressed(false);

    private void SetPressed(bool pressed)
    {
        float throttle = 0f;
        float steering = 0f;
        bool boost = false;

        switch (control)
        {
            case Control.Accelerate: throttle = pressed ? 1f : 0f; break;
            case Control.Reverse: throttle = pressed ? -1f : 0f; break;
            case Control.Left: steering = pressed ? -1f : 0f; break;
            case Control.Right: steering = pressed ? 1f : 0f; break;
            case Control.Boost: boost = pressed; break;
        }

        if (target != null)
        {
            if (control == Control.Accelerate || control == Control.Reverse)
                target.throttle = throttle;
            else if (control == Control.Left || control == Control.Right)
                target.steering = steering;
            else if (control == Control.Boost)
                target.boostPressed = boost;
        }

        if (networkTarget != null)
        {
            if (control == Control.Accelerate || control == Control.Reverse)
                networkTarget.SetMobileThrottle(throttle);
            else if (control == Control.Left || control == Control.Right)
                networkTarget.SetMobileSteering(steering);
            else if (control == Control.Boost)
                networkTarget.SetMobileBoost(boost);
        }
    }

    private void OnDisable() => SetPressed(false);
}

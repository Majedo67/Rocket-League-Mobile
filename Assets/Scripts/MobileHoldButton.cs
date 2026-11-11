using UnityEngine;
using UnityEngine.EventSystems;

public class MobileHoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public enum Control { Accelerate, Reverse, Left, Right, Boost }
    public Control control;
    public CarController target;

    public void OnPointerDown(PointerEventData eventData) => SetPressed(true);
    public void OnPointerUp(PointerEventData eventData) => SetPressed(false);
    public void OnPointerExit(PointerEventData eventData) => SetPressed(false);

    private void SetPressed(bool pressed)
    {
        if (target == null) return;

        switch (control)
        {
            case Control.Accelerate:
                target.throttle = pressed ? 1f : 0f;
                break;
            case Control.Reverse:
                target.throttle = pressed ? -1f : 0f;
                break;
            case Control.Left:
                target.steering = pressed ? -1f : 0f;
                break;
            case Control.Right:
                target.steering = pressed ? 1f : 0f;
                break;
            case Control.Boost:
                target.boostPressed = pressed;
                break;
        }
    }

    void OnDisable() => SetPressed(false);
}

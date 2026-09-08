using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform _background;
    [SerializeField] private RectTransform _knob;
    [SerializeField] private float _deadzone = 0.15f;
    [SerializeField] private List<MonoBehaviour> _controllables = new();

    public Vector2 Direction { get; private set; }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _background, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        float radius = _background.sizeDelta.x * 0.5f;
        Vector2 inputVector = Vector2.ClampMagnitude(localPoint / radius, 1f);
        _knob.anchoredPosition = inputVector * radius;
        Direction = inputVector.magnitude >= _deadzone ? inputVector : Vector2.zero;
        NotifyControllables();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Direction = Vector2.zero;
        _knob.anchoredPosition = Vector2.zero;
        NotifyControllables();
    }

    private void NotifyControllables()
    {
        if (Direction == Vector2.zero) return;
        float angle = Mathf.Atan2(Direction.x, Direction.y) * Mathf.Rad2Deg;
        foreach (IJoystickControllable controllable in _controllables)
            controllable.SetJoystickValue(angle);
    }

    private void Awake()
    {
        for(int i = _controllables.Count - 1; i >= 0; --i)
            if (!(_controllables[0] is IJoystickControllable joystickControllable))
                _controllables.RemoveAt(i);
    }
}

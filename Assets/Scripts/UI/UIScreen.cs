using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIScreen : MonoBehaviour, IPointerClickHandler
{
    public event Action OnTapped;

    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _hideDuration = 0.3f;

    public virtual void Show()
    {
        gameObject.SetActive(true);
        _canvasGroup.DOKill();
        _canvasGroup.DOFade(1f, _hideDuration);
    }

    public virtual void Hide()
    {
        _canvasGroup.DOKill();
        _canvasGroup.DOFade(0f, _hideDuration).OnComplete(() => gameObject.SetActive(false));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnTapped?.Invoke();
    }
}

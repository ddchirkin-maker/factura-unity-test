using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public abstract class TextPopupUI : CombatFeedbackUI
{
    [SerializeField] protected TextMeshPro _text;
    [SerializeField] protected float _moveUpDistance = 1f;

    public override async UniTask ShowInfo()
    {
        if (_text == null)
            return;

        SetAlpha(1f);
        await UniTask.Delay((int)(_displayDuration * 1000));

        Vector3 startPosition = transform.localPosition;
        Vector3 endPosition = startPosition + Vector3.up * _moveUpDistance;
        transform.localPosition = startPosition;

        _changeColorTween.Restart();
        await transform.DOLocalMove(endPosition, _disappearDuration).AsyncWaitForCompletion();
    }

    protected void SetText(string value)
    {
        if (_text != null)
            _text.text = value;
    }

    #region Private

    private Tweener _changeColorTween;

    private void SetAlpha(float alpha)
    {
        Color color = _text.color;
        color.a = alpha;
        _text.color = color;
    }

    private void Awake()
    {
        _changeColorTween = DOTween.To(() => _text.color.a, SetAlpha, 0f, _disappearDuration).SetEase(Ease.InQuad).SetAutoKill(false).Pause();
    }

    #endregion
}

using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : CombatFeedbackUI
{
    [SerializeField] private Image _fillImage;
    [SerializeField] private Gradient _gradient;
    [SerializeField] private Vector3 _followOffset = new(0f, 2f, 0f);

    [SerializeField, HideInPlayMode] private bool _useAnimation = false;

    public void SetFollowTarget(Transform target)
    {
        _followTarget = target;
        UpdatePosition();
    }

    public void SetHealth(float normalizedHealth)
    {
        if (_fillImage == null) return;
        if (_useAnimation)
            _healthBarChangeTween.Goto(normalizedHealth);
        else
        {
            _fillImage.color = _gradient.Evaluate(normalizedHealth);
            _fillImage.fillAmount = normalizedHealth;
        }
    }

    public void ExtendVisibility()
    {
        _hideAt = Time.time + _displayDuration;
    }

    public void HideNow()
    {
        _hideAt = Time.time;
    }

    public override async UniTask ShowInfo()
    {
        ExtendVisibility();
        await UniTask.WaitUntil(() => Time.time >= _hideAt);
    }

    private float _hideAt;
    private const float TweenTime = 1f;

    private Transform _followTarget;
    private Tweener _healthBarChangeTween = null;

    private void Awake()
    {
        if(_useAnimation)
            _healthBarChangeTween = DOTween.To(()=> 0f, x => SetHealthToFill(x), 1f, TweenTime).SetAutoKill(false).Pause();
    }

    private void SetHealthToFill(float percent)
    {
        _fillImage.color = _gradient.Evaluate(percent);
        _fillImage.fillAmount = percent;
    }

    private void LateUpdate()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (_followTarget != null)
            transform.position = _followTarget.position + _followOffset;
    }
}

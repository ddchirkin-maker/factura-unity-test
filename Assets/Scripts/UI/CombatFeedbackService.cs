using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CombatFeedbackService : ICombatFeedbackService
{
    private readonly ComponentPool<DamageNumberUI> _damagePool;
    private readonly ComponentPool<HealthBarUI> _healthBarPool;
    private readonly ComponentPool<DeathEffect> _deathEffectPool;

    private readonly Dictionary<Transform, HealthBarUI> _activeHealthBars = new();

    public CombatFeedbackService(
        ComponentPool<DamageNumberUI> damagePool,
        ComponentPool<HealthBarUI> healthBarPool,
        ComponentPool<DeathEffect> deathEffectPool)
    {
        _damagePool = damagePool;
        _healthBarPool = healthBarPool;
        _deathEffectPool = deathEffectPool;
    }

    public async UniTask ShowDamage(int amount, Vector3 worldPosition)
    {
        DamageNumberUI popup = _damagePool.Get();
        popup.transform.position = worldPosition;
        popup.SetAmount(amount);
        await popup.ShowInfo();
        _damagePool.Release(popup);
    }

    public async UniTask ShowDeathEffect(Vector3 worldPosition)
    {
        DeathEffect effect = _deathEffectPool.Get();
        effect.transform.position = worldPosition;
        await effect.Play();
        _deathEffectPool.Release(effect);
    }

    public void UpdateHealthBar(Transform owner, float normalizedHealth)
    {
        if (_activeHealthBars.TryGetValue(owner, out HealthBarUI bar))
        {
            bar.SetHealth(normalizedHealth);
            bar.ExtendVisibility();
            return;
        }
        bar = _healthBarPool.Get();
        bar.SetFollowTarget(owner);
        bar.SetHealth(normalizedHealth);
        _activeHealthBars[owner] = bar;
        TrackHealthBar(owner, bar).Forget();
    }

    public void HideHealthBar(Transform owner)
    {
        if (_activeHealthBars.TryGetValue(owner, out HealthBarUI bar))
            bar.HideNow();
    }

    private async UniTaskVoid TrackHealthBar(Transform owner, HealthBarUI bar)
    {
        await bar.ShowInfo();
        if (_activeHealthBars.TryGetValue(owner, out HealthBarUI current) && current == bar)
        {
            _activeHealthBars.Remove(owner);
            bar.SetFollowTarget(null);
            _healthBarPool.Release(bar);
        }
    }
}

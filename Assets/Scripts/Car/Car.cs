using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

public class Car : MonoBehaviour, IDamageable
{
    public event Action OnDied;

    [SerializeField] private CarHealthComponent _health;
    [SerializeField] private Turret _turret;

    [SerializeField, BoxGroup("Effects")] private List<TrailRenderer> _trailRenderer;
    [SerializeField, BoxGroup("Effects")] private ParticleSystem _trailParticle;

    [SerializeField, BoxGroup("Hit")] private Vector3 _hitPunchScale = new(0.2f, 0.2f, 0.2f);
    [SerializeField, BoxGroup("Hit")] private float _hitPunchDuration = 0.3f;
    [SerializeField, BoxGroup("Hit")] private AudioSource _damageSource;

    public void TakeDamage(float damage)
    {
        _damageSource.Play();
        _health.TakeDamage(damage);
        _hitPunchTween.Restart();
    }

    public void StartShooting()
    {
        _turret.StartShooting();
    }

    public void StopShooting()
    {
        _turret.StopShooting();
    }

    public void ResetState()
    {
        _health.ResetHealth();
        _turret.ResetState();

        foreach (TrailRenderer trail in _trailRenderer)
            trail.Clear();

        if (_trailParticle != null)
        {
            _trailParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _trailParticle.Play();
        }
    }

    private Tween _hitPunchTween;

    #region Init

    private void Awake()
    {
        _health.OnDied += () => OnDied?.Invoke();

        _hitPunchTween = transform.DOPunchScale(_hitPunchScale, _hitPunchDuration, vibrato: 8, elasticity: 1f)
            .SetAutoKill(false).Pause();
    }

    [Inject]
    public void Construct(ComponentPool<Bullet> bulletPool, IColliderRegistry<Enemy> enemyRegistry)
    {
        _turret.Init(bulletPool, enemyRegistry);
    }

    #endregion
}

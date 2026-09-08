using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _lifetime = 3f;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private TrailRenderer _trailRenderer;

    private ComponentPool<Bullet> _pool;
    private IColliderRegistry<Enemy> _enemyRegistry;
    private CancellationTokenSource _cts;

    private bool _hasPendingHit;
    private Enemy _pendingEnemy;
    private float _pendingDamage;

    public void Launch(ComponentPool<Bullet> pool, IColliderRegistry<Enemy> enemyRegistry, Vector3 velocity, float gravityForce)
    {
        _pool = pool;
        _enemyRegistry = enemyRegistry;

        _rigidbody.isKinematic = false;
        _rigidbody.linearVelocity = velocity;
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.AddForce(Vector3.down * gravityForce, ForceMode.Acceleration);

        _trailRenderer.Clear();
        _trailRenderer.enabled = true;

        _cts = new CancellationTokenSource();
        AutoRelease(_cts.Token).Forget();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_enemyRegistry == null || !_enemyRegistry.TryGetOwner(collision.collider, out Enemy enemy))
        {
            if (!_hasPendingHit)
                Release();
            return;
        }
        float damage = collision.collider.GetComponent<CollisionTracker>().DamageCoef;
        if (!_hasPendingHit || damage > _pendingDamage)
        {
            _pendingEnemy = enemy;
            _pendingDamage = damage;
        }
        if (!_hasPendingHit)
        {
            _hasPendingHit = true;
            ResolveHit().Forget();
        }
    }

    private async UniTaskVoid ResolveHit()
    {
        await UniTask.Yield();

        if (_pendingEnemy != null && _pendingEnemy.isActiveAndEnabled)
        {
            _pendingEnemy.TakeDamage(_pendingDamage);
        }
        _hasPendingHit = false;
        Release();
    }

    private async UniTaskVoid AutoRelease(CancellationToken token)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_lifetime), cancellationToken: token);
        Release();
    }

    private void Release()
    {
        if (_pool == null) return;

        _trailRenderer.enabled = false;
        _pendingDamage = 0f;
        _cts?.Cancel();
        _cts = null;

        ComponentPool<Bullet> pool = _pool;
        _pool = null;
        pool.Release(this);
    }
}

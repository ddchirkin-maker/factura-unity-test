using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class Turret : MonoBehaviour, IJoystickControllable
{
    [SerializeField, BoxGroup("Laser")] private LineRenderer _laser;
    [SerializeField, BoxGroup("Laser")] private float _laserFadeInDuration = 0.8f;
    [SerializeField, BoxGroup("Laser")] private float _laserLength = 20f;

    [SerializeField] private AudioSource _audioSource;

    [SerializeField] private Transform _originPoint;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private float _gravityForce;
    [SerializeField] private float _fireInterval = 0.5f;
    [SerializeField] private LayerMask _enemyLayerMask;
    [SerializeField] private float _aimRaycastRange = 50f;
    [SerializeField] private float _rotationLerpSpeed = 10f;
    [SerializeField] private float _maxAngle = 90f;


    public void Init(ComponentPool<Bullet> bulletPool, IColliderRegistry<Enemy> enemyRegistry)
    {
        _bulletPool = bulletPool;
        _enemyRegistry = enemyRegistry;
    }

    public void SetJoystickValue(float value)
    {
        _targetAngle = Mathf.Clamp(value, -_maxAngle, _maxAngle);
    }

    public void ResetState()
    {
        _targetAngle = 0f;
        transform.localRotation = Quaternion.identity;
    }

    public void StartShooting()
    {
        if (_cts != null) return;
        _cts = new CancellationTokenSource();
        _laserTween?.PlayForward();
        ShootLoop(_cts.Token).Forget();
    }

    public void StopShooting()
    {
        _cts?.Cancel();
        _laserTween?.PlayBackwards();
        _cts = null;
    }

    #region Private

    private Tween _laserTween = null;
    private Vector3[] _laserPoints = new Vector3[2] { Vector3.zero, Vector3.zero };

    private ComponentPool<Bullet> _bulletPool;
    private IColliderRegistry<Enemy> _enemyRegistry;
    private CancellationTokenSource _cts;
    private float _targetAngle;

    private void Shoot()
    {
        Bullet bullet = _bulletPool.Get();
        bullet.transform.SetPositionAndRotation(_firePoint.position, Quaternion.identity);

        Vector3 direction = _firePoint.forward;
        float gravityForce = _gravityForce;
        bool hitEnemy = Physics.Raycast(_originPoint.position, _originPoint.forward, out RaycastHit hit, _aimRaycastRange, _enemyLayerMask)
            && _enemyRegistry.TryGetOwner(hit.collider, out _);

        if (hitEnemy)
        {
            direction = (hit.point - _firePoint.position).normalized;
            gravityForce = 0f;
        }

        Vector3 velocity = direction * _bulletSpeed;
        bullet.Launch(_bulletPool, _enemyRegistry, velocity, gravityForce);
        _audioSource.Play();
    }

    private void ShowLaser(float fadePercent)
    {
        _laserPoints[1] = Vector3.forward * fadePercent * _laserLength;
        _laser.SetPositions(_laserPoints);
    }

    private async UniTaskVoid ShootLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            Shoot();
            await UniTask.Delay(TimeSpan.FromSeconds(_fireInterval), cancellationToken: token);
        }
    }

    private void Update()
    {
        Quaternion targetRotation = Quaternion.Euler(0f, _targetAngle, 0f);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, _rotationLerpSpeed * Time.deltaTime);
    }

    private void Awake()
    {
        _laserTween = DOTween.To(() => 0f, x => ShowLaser(x), 1f, _laserFadeInDuration).SetAutoKill(false).Pause();        
    }

    private void OnDestroy()
    {
        StopShooting();
    }

    #endregion
}

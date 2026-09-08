using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

public class Enemy : MonoBehaviour
{
    public Transform UIPoint => _uiPoint;

    [SerializeField] private AudioSource _audioSource;
    [FormerlySerializedAs("_UIPoint")][SerializeField] private Transform _uiPoint;
    [SerializeField] private List<CollisionTracker> _collisionTrackers;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _turnSpeed = 8f;
    [SerializeField] private float _attackRange = 1.5f;
    [SerializeField] private EnemyHealthComponent _health;
    [SerializeField] private float _carDamageRatio = 0.05f;
    [SerializeField] private string _carTargetLayerName = "Car";

    public void TakeDamage(float damage)
    {
        _audioSource.Play();
        _health.TakeDamage(damage);
    }

    public void Init(ComponentPool<Enemy> pool, IColliderRegistry<Enemy> colliderRegistry, ICombatFeedbackService combatFeedbackService)
    {
        _pool = pool;
        _colliderRegistry = colliderRegistry;
        _combatFeedbackService = combatFeedbackService;
        _health.Construct(combatFeedbackService);
        RegisterColliders();
    }

    public void ForceRelease()
    {
        if (_pool != null)
            _pool.Release(this);
        else
            gameObject.SetActive(false);
    }

    public void MovingToTarget(GameObject target)
    {
        if (_target != null) return;

        _target = target.transform;
        _animator.SetBool("IsMovingToTarget", true);
    }

    [Inject]
    public void Construct(IColliderRegistry<Enemy> colliderRegistry)
    {
        _colliderRegistry = colliderRegistry;
        RegisterColliders();
    }

    #region Private 

    private int _carTargetMask;
    private IColliderRegistry<Enemy> _colliderRegistry;
    private ComponentPool<Enemy> _pool;
    private ICombatFeedbackService _combatFeedbackService;
    private Transform _target;
    private bool _isRegistered;

    private void RegisterColliders()
    {
        if (_isRegistered) return;
        _isRegistered = true;

        foreach (CollisionTracker tracker in _collisionTrackers)
            _colliderRegistry.Register(tracker.Collider, this);
    }

    private void Die()
    {
        _combatFeedbackService?.HideHealthBar(transform);
        _combatFeedbackService?.ShowDeathEffect(transform.position).Forget();
        ForceRelease();
    }

    private void OnTrigger(Collider collider)
    {
        if (((1 << collider.gameObject.layer) & _carTargetMask) != 0)
        {
            _target?.GetComponent<IDamageable>()?.TakeDamage(_carDamageRatio);
            Die();
        }
    }

    private void Awake()
    {
        _health.OnDied += Die;

        for (int i = 0; i < _collisionTrackers.Count; ++i)
            _collisionTrackers[i].OnTrigger += OnTrigger;
        _carTargetMask = LayerMask.GetMask(_carTargetLayerName);
    }

    private void OnDisable()
    {
        _target = null;
        _health.ResetHealth();
        _animator.SetBool("IsMovingToTarget", false);
    }


    private void Update()
    {
        if (_target == null) return;

        Vector3 direction = _target.position - transform.position;
        direction.y = 0f;
        float distance = direction.magnitude;
        if (distance < 0.0001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _turnSpeed * Time.deltaTime);

        if (distance > _attackRange)
            transform.position += transform.forward * _moveSpeed * Time.deltaTime;
    }

    private void OnDestroy()
    {
        if (_colliderRegistry == null) return;

        foreach (CollisionTracker tracker in _collisionTrackers)
            _colliderRegistry.Unregister(tracker.Collider);
    }

    #endregion

    #region InEditorMode

    [Button, HideInPlayMode]
    private void GetCollisionTrackers()
    {
        _collisionTrackers = GetComponentsInChildren<CollisionTracker>().ToList();
    }

    #endregion
}

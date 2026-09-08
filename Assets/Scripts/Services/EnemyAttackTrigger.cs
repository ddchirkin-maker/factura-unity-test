using UnityEngine;
using VContainer;

public class EnemyAttackTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _attackTarget;

    private IColliderRegistry<Enemy> _enemyColliders;

    [Inject]
    public void Construct(IColliderRegistry<Enemy> enemyColliders)
    {
        _enemyColliders = enemyColliders;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_enemyColliders.TryGetOwner(other, out Enemy enemy))
            enemy.MovingToTarget(_attackTarget);
    }
}

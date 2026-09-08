using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

public class EnemyHealthComponent : HealthComponent
{
    [SerializeField] private float _damageNumberSpread = 0.3f;

    private ICombatFeedbackService _combatFeedbackService;

    [Inject]
    public void Construct(ICombatFeedbackService combatFeedbackService)
    {
        _combatFeedbackService = combatFeedbackService;
    }

    protected override void OnHealthChanged(float currentHealth, float damage)
    {
        if (currentHealth > 0)
            _combatFeedbackService?.UpdateHealthBar(transform, currentHealth);
        if (damage > 0f)
        {
            Vector3 randomOffset = GetRandomUIOffset();
            _combatFeedbackService?.ShowDamage((int)(damage * 100), transform.position + randomOffset).Forget();
        }
    }

    private Vector3 GetRandomUIOffset()
    {
        return new Vector3(Random.Range(-_damageNumberSpread, _damageNumberSpread),
            Random.Range(-_damageNumberSpread, _damageNumberSpread),
            Random.Range(-_damageNumberSpread, _damageNumberSpread));
    }
}

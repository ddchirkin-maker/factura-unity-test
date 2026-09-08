using UnityEngine;

public class CarHealthComponent : HealthComponent
{
    [SerializeField] private HealthBarUI _healthBarUI;

    protected override void OnHealthChanged(float currentHealth, float damage)
    {
        _healthBarUI.SetHealth(currentHealth);
    }

    protected override void OnReset(float currentHealth)
    {
        _healthBarUI.SetHealth(currentHealth);
    }
}

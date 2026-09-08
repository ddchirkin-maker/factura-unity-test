using System;
using UnityEngine;

public abstract class HealthComponent : MonoBehaviour
{
    public event Action OnDied;

    public void ResetHealth()
    {
        _currentHealth = FullHealth;
        OnReset(_currentHealth);
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        OnHealthChanged(_currentHealth, damage);

        if (_currentHealth <= 0)
            OnDied?.Invoke();
    }

    protected abstract void OnHealthChanged(float currentHealth, float damage);
    protected virtual void OnReset(float currentHealth) { }

    private const float FullHealth = 1f;
    private float _currentHealth = FullHealth;
}

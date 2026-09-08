using Cysharp.Threading.Tasks;
using UnityEngine;

public interface ICombatFeedbackService
{
    UniTask ShowDamage(int amount, Vector3 worldPosition);
    UniTask ShowDeathEffect(Vector3 worldPosition);
    void UpdateHealthBar(Transform owner, float normalizedHealth);
    void HideHealthBar(Transform owner);
}

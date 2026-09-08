using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class CombatFeedbackUI : MonoBehaviour
{
    [SerializeField] protected float _appearDuration = 0.5f;
    [SerializeField] protected float _displayDuration = 2f;
    [SerializeField] protected float _disappearDuration = .5f;

    public abstract UniTask ShowInfo();

}

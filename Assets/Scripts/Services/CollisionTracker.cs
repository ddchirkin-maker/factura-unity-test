using UnityEngine;

public class CollisionTracker : MonoBehaviour
{
    public delegate void OnTriggerEvent(Collider other);

    public OnTriggerEvent OnTrigger;

    public Collider Collider => _collider;
    public float DamageCoef => _damageCoef;

    [SerializeField] private Collider _collider;
    [Range(0f, 1f), SerializeField] private float _damageCoef = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
        OnTrigger?.Invoke(other);
    }

}

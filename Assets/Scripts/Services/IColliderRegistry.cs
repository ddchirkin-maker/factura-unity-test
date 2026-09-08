using UnityEngine;

public interface IColliderRegistry<T>
{
    void Register(Collider collider, T owner);
    void Unregister(Collider collider);
    bool TryGetOwner(Collider collider, out T owner);
}

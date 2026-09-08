using System.Collections.Generic;
using UnityEngine;

public class ColliderRegistry<T> : IColliderRegistry<T> where T : MonoBehaviour
{
    private readonly Dictionary<Collider, T> _owners = new();

    public void Register(Collider collider, T owner)
    {
        if (collider != null)
            _owners[collider] = owner;
    }

    public void Unregister(Collider collider)
    {
        if (collider != null)
            _owners.Remove(collider);
    }

    public bool TryGetOwner(Collider collider, out T owner)
    {
        return _owners.TryGetValue(collider, out owner);
    }
}

using UnityEngine;
using UnityEngine.Pool;

public class ComponentPool<T> where T : Component
{
    public ComponentPool(T prefab, Transform container, int defaultCapacity = 8, int maxSize = 32)
    {
        _prefab = prefab;
        _container = container;
        _pool = new ObjectPool<T>(createFunc: Create, actionOnGet: OnGet, actionOnRelease: OnRelease,
            actionOnDestroy: OnDestroyItem, collectionCheck: false, defaultCapacity: defaultCapacity,
            maxSize: maxSize);
    }

    public int CountActive => _pool.CountActive;

    public T Get() => _pool.Get();

    public void Release(T item) => _pool.Release(item);

    private readonly T _prefab;
    private readonly Transform _container;
    private readonly ObjectPool<T> _pool;

    private T Create() => Object.Instantiate(_prefab, _container);

    private void OnGet(T item) => item.gameObject.SetActive(true);

    private void OnRelease(T item)
    {
        item.gameObject.SetActive(false);
        item.transform.SetParent(_container);
    }

    private void OnDestroyItem(T item)
    {
        if (item != null)
            Object.Destroy(item.gameObject);
    }
}

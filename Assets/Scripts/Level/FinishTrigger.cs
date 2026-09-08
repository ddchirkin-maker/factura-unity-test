using System;
using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    public event Action OnCarEntered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<Car>() != null)
            OnCarEntered?.Invoke();
    }
}

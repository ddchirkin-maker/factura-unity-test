using UnityEngine;

public class MovingBlock : MonoBehaviour
{
    [SerializeField] private Transform _finishLine;
    [SerializeField] private float _minSpeed = 0.01f;
    [SerializeField] private float _maxSpeed = 0.03f;

    public void ResetPosition()
    {
        transform.SetPositionAndRotation(_startPosition, _startRotation);
    }

    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private float _totalDistance;


    private void FixedUpdate()
    {
        transform.position += transform.forward * CurrentSpeed();
    }

    private float CurrentSpeed()
    {
        if (_totalDistance <= 0f)
            return _minSpeed;
        float progress = Vector3.Distance(_startPosition, transform.position) / _totalDistance;
        return Mathf.Lerp(_minSpeed, _maxSpeed, Mathf.Clamp01(progress));
    }

    private void Awake()
    {
        _startPosition = transform.position;
        _startRotation = transform.rotation;
        _totalDistance = Vector3.Distance(_startPosition, _finishLine.position);
    }

}

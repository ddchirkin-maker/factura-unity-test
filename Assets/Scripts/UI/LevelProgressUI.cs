using UnityEngine;
using UnityEngine.UI;

public class LevelProgressUI : MonoBehaviour
{
    [SerializeField] private Transform _tracked;
    [SerializeField] private Transform _finishLine;
    [SerializeField] private Image _fillImage;

    private Vector3 _startPosition;

    private void Awake()
    {
        _startPosition = _tracked.position;
    }

    private void Update()
    {
        _fillImage.fillAmount = GetProgress();
    }

    private float GetProgress()
    {
        float total = Vector3.Distance(_startPosition, _finishLine.position);
        if (total <= 0f)  return 1f;

        float traveled = Vector3.Distance(_startPosition, _tracked.position);
        return Mathf.Clamp01(traveled / total);
    }
}

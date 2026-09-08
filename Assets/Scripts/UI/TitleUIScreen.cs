using System.Threading;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class TitleUIScreen : UIScreen
{
    [SerializeField] private TextMeshProUGUI _subtitleText;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private string _title;

    [SerializeField] private float _startScale = 1.6f;
    [SerializeField] private float _duration = .8f;

    [Button]
    public override void Show()
    {
        base.Show();
        ShowTitle(_title);
    }

    private void ShowTitle(string text, CancellationToken ct = default)
    {
        _titleText.text = text;
        _titleText.transform.DOScale(Vector3.one, _duration);
        _subtitleText.DOFade(1f, _duration);
    }

    private void OnEnable()
    {
        _subtitleText.color = GetColor(_subtitleText.color, 0f);
        _titleText.transform.localScale = Vector3.one * _startScale;
    }

    private Color GetColor(Color color, float alpha)
    {
        color.a = alpha;
        return color;
    }
}

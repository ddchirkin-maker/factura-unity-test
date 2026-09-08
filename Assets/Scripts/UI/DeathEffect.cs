using Cysharp.Threading.Tasks;
using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private AudioSource _audioSource;
    public async UniTask Play()
    {
        _audioSource?.Play();
        if (_particleSystem == null) return;
        
        _particleSystem.Clear(true);
        _particleSystem.Play(true);
        
        await UniTask.WaitUntil(() => !_particleSystem.IsAlive(true));
    }
}

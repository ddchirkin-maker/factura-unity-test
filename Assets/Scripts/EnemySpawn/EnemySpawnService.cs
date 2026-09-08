using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemySpawnService : IEnemySpawnService
{
    public EnemySpawnService(ComponentPool<Enemy> enemyPool, IColliderRegistry<Enemy> enemyRegistry, Car car,
        EnemySpawnController controller, ICombatFeedbackService combatFeedbackService)
    {
        _enemyPool = enemyPool;
        _enemyRegistry = enemyRegistry;
        _car = car;
        _controller = controller;
        _combatFeedbackService = combatFeedbackService;
    }

    public void StartSpawning()
    {
        if (_cts != null) return;

        _levelStartPosition = _car.transform.position;
        _totalDistance = _controller.FinishLine != null ? Vector3.Distance(_levelStartPosition, _controller.FinishLine.position) : 0f;
        _cts = new CancellationTokenSource();
        SpawnLoop(_cts.Token).Forget();
    }

    public void StopSpawning()
    {
        _cts?.Cancel();
        _cts = null;
    }

    public void ClearActiveEnemies()
    {
        Transform container = _controller.EnemyContainer;
        if (container == null) return;

        for (int i = container.childCount - 1; i >= 0; i--)
        {
            Transform child = container.GetChild(i);
            if (child.gameObject.activeSelf && child.TryGetComponent(out Enemy enemy))
                enemy.ForceRelease();
        }
    }

    #region Private

    private const int MaxSpawnAttempts = 5;
    private readonly ComponentPool<Enemy> _enemyPool;
    private readonly IColliderRegistry<Enemy> _enemyRegistry;
    private readonly Car _car;
    private readonly EnemySpawnController _controller;
    private readonly ICombatFeedbackService _combatFeedbackService;

    private CancellationTokenSource _cts;
    private Vector3 _levelStartPosition;
    private float _totalDistance;

    private async UniTaskVoid SpawnLoop(CancellationToken token)
    {
        SpawnInitialGroup();

        while (!token.IsCancellationRequested)
        {
            if (_car == null) return;

            Vector3 groupStartPosition = _car.transform.position;
            await WaitUntilTraveled(groupStartPosition, _controller.GroupSpacing, token);
            await SpawnGroup(token, _controller.SpawnDistanceAhead);
        }
    }

    private async UniTask WaitUntilTraveled(Vector3 fromPosition, float distance, CancellationToken token)
    {
        while (!token.IsCancellationRequested && _car != null && Vector3.Distance(fromPosition, _car.transform.position) < distance)
            await UniTask.Yield(PlayerLoopTiming.Update, token);
    }

    private void SpawnInitialGroup()
    {
        int groupSize = CurrentGroupSize();
        List<Vector3> existingPositions = GetActiveEnemyPositions();

        for (int i = 0; i < groupSize; i++)
        {
            if (_enemyPool.CountActive < _controller.MaxActiveEnemies)
                SpawnOne(_controller.InitialSpawnDistance, existingPositions);
        }
    }

    private async UniTask SpawnGroup(CancellationToken token, float spawnDistance)
    {
        int groupSize = CurrentGroupSize();
        List<Vector3> existingPositions = GetActiveEnemyPositions();

        for (int i = 0; i < groupSize; i++)
        {
            if (token.IsCancellationRequested)
                return;
            if (_enemyPool.CountActive < _controller.MaxActiveEnemies)
                SpawnOne(spawnDistance, existingPositions);
            await UniTask.Delay(System.TimeSpan.FromSeconds(_controller.SpawnStagger), cancellationToken: token);
        }
    }

    private int CurrentGroupSize()
    {
        if(_car == null) return 0;

        float progress = _totalDistance > 0f ? Vector3.Distance(_levelStartPosition, _car.transform.position) / _totalDistance : 0f;
        return Mathf.RoundToInt(Mathf.Lerp(_controller.MinGroupSize, _controller.MaxGroupSize, Mathf.Clamp01(progress)));
    }

    private void SpawnOne(float spawnDistance, List<Vector3> existingPositions)
    {
        Transform carTransform = _car.transform;
        if (!TryFindSpawnPosition(carTransform, spawnDistance, existingPositions, out Vector3 spawnPosition))
            return;
        existingPositions.Add(spawnPosition);
        Enemy enemy = _enemyPool.Get();
        enemy.Init(_enemyPool, _enemyRegistry, _combatFeedbackService);
        enemy.transform.SetPositionAndRotation(spawnPosition, Quaternion.LookRotation(-carTransform.forward));
    }

    private bool TryFindSpawnPosition(Transform carTransform, float spawnDistance, List<Vector3> existingPositions, out Vector3 spawnPosition)
    {
        spawnPosition = default;

        Vector3 origin = carTransform.position;
        Vector3 forward = carTransform.forward;
        Vector3 right = carTransform.right;

        float maxForwardDistance = float.MaxValue;
        if (_controller.FinishLine != null)
            maxForwardDistance = Vector3.Dot(_controller.FinishLine.position - origin, forward);
        float minForwardDistance = spawnDistance - _controller.SpawnDistanceJitter;
        if (maxForwardDistance < minForwardDistance)
            return false;
        for (int attempt = 0; attempt < MaxSpawnAttempts; attempt++)
        {
            float lateralOffset = Random.Range(-_controller.RoadHalfWidth, _controller.RoadHalfWidth);
            float forwardDistance = spawnDistance + Random.Range(-_controller.SpawnDistanceJitter, _controller.SpawnDistanceJitter);
            forwardDistance = Mathf.Min(forwardDistance, maxForwardDistance);
            Vector3 candidate = origin + forward * forwardDistance + right * lateralOffset;
            if (IsFarEnough(candidate, existingPositions))
            {
                spawnPosition = candidate;
                return true;
            }
        }
        return false;
    }

    private bool IsFarEnough(Vector3 candidate, List<Vector3> existingPositions)
    {
        for (int i = 0; i < existingPositions.Count; i++)
        {
            if (Vector3.Distance(candidate, existingPositions[i]) < _controller.MinSpawnSpacing)
                return false;
        }
        return true;
    }

    private List<Vector3> GetActiveEnemyPositions()
    {
        List<Vector3> positions = new List<Vector3>();
        Transform container = _controller.EnemyContainer;
        if (container == null) return positions;
        for (int i = 0; i < container.childCount; i++)
        {
            Transform child = container.GetChild(i);
            if (child.gameObject.activeSelf)
                positions.Add(child.position);
        }
        return positions;
    }

    #endregion
}

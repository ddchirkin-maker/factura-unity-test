using UnityEngine;

// Scene wiring only (prefab/container/finish line). Tunable numbers live in the EnemySpawnConfig asset.
public class EnemySpawnController : MonoBehaviour
{
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private Transform _enemyContainer;
    [SerializeField] private Transform _finishLine;
    [SerializeField] private EnemySpawnConfig _config;

    public Enemy EnemyPrefab => _enemyPrefab;
    public Transform EnemyContainer => _enemyContainer;
    public Transform FinishLine => _finishLine;

    public float SpawnDistanceAhead => _config.SpawnDistanceAhead;
    public float SpawnDistanceJitter => _config.SpawnDistanceJitter;
    public float InitialSpawnDistance => _config.InitialSpawnDistance;
    public float RoadHalfWidth => _config.RoadHalfWidth;
    public float MinSpawnSpacing => _config.MinSpawnSpacing;
    public int MaxActiveEnemies => _config.MaxActiveEnemies;

    public int MinGroupSize => _config.MinGroupSize;
    public int MaxGroupSize => _config.MaxGroupSize;
    public float GroupSpacing => _config.GroupSpacing;
    public float SpawnStagger => _config.SpawnStagger;
}

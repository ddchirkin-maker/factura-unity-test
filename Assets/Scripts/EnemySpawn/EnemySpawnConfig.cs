using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnConfig", menuName = "Config/Enemy Spawn Config")]
public class EnemySpawnConfig : ScriptableObject
{
    [Header("Route")]
    [SerializeField] private float _spawnDistanceAhead = 35f;
    [SerializeField] private float _spawnDistanceJitter = 5f;
    [SerializeField] private float _initialSpawnDistance = 20f;
    [SerializeField] private float _roadHalfWidth = 3f;
    [SerializeField] private float _minSpawnSpacing = 4f;

    [Header("Groups")]
    [SerializeField] private int _minGroupSize = 3;
    [SerializeField] private int _maxGroupSize = 10;
    [SerializeField] private float _groupSpacing = 25f;
    [SerializeField] private float _spawnStagger = 0.15f;

    [Header("Runtime")]
    [SerializeField] private int _maxActiveEnemies = 20;

    public float SpawnDistanceAhead => _spawnDistanceAhead;
    public float SpawnDistanceJitter => _spawnDistanceJitter;
    public float InitialSpawnDistance => _initialSpawnDistance;
    public float RoadHalfWidth => _roadHalfWidth;
    public float MinSpawnSpacing => _minSpawnSpacing;
    public int MaxActiveEnemies => _maxActiveEnemies;

    public int MinGroupSize => _minGroupSize;
    public int MaxGroupSize => _maxGroupSize;
    public float GroupSpacing => _groupSpacing;
    public float SpawnStagger => _spawnStagger;
}

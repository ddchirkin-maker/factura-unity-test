using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private DamageNumberUI _damageNumberPrefab;
    [SerializeField] private HealthBarUI _healthBarPrefab;
    [SerializeField] private DeathEffect _deathEffectPrefab;
    [SerializeField] private Transform _floatingUIContainer;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private Transform _bulletContainer;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(new ComponentPool<DamageNumberUI>(_damageNumberPrefab, _floatingUIContainer));
        builder.RegisterInstance(new ComponentPool<HealthBarUI>(_healthBarPrefab, _floatingUIContainer));
        builder.RegisterInstance(new ComponentPool<DeathEffect>(_deathEffectPrefab, _floatingUIContainer));
        builder.RegisterInstance(new ComponentPool<Bullet>(_bulletPrefab, _bulletContainer));

        builder.Register<ICombatFeedbackService, CombatFeedbackService>(Lifetime.Singleton);
        builder.Register<IColliderRegistry<Enemy>, ColliderRegistry<Enemy>>(Lifetime.Singleton);

        builder.RegisterComponentInHierarchy<EnemySpawnController>();

        builder.Register<ComponentPool<Enemy>>(resolver =>
        {
            EnemySpawnController controller = resolver.Resolve<EnemySpawnController>();
            return new ComponentPool<Enemy>(controller.EnemyPrefab, controller.EnemyContainer);
        }, Lifetime.Singleton);

        builder.Register<IEnemySpawnService>(resolver => new EnemySpawnService(
            resolver.Resolve<ComponentPool<Enemy>>(),
            resolver.Resolve<IColliderRegistry<Enemy>>(),
            resolver.Resolve<Car>(),
            resolver.Resolve<EnemySpawnController>(),
            resolver.Resolve<ICombatFeedbackService>()), Lifetime.Singleton);

        builder.RegisterComponentInHierarchy<EnemyAttackTrigger>();
        builder.RegisterComponentInHierarchy<Car>();
        builder.RegisterComponentInHierarchy<GameController>();
    }
}

using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

public class GameController : MonoBehaviour
{
    [GUIColor("blue"), BoxGroup("UI")][SerializeField]
    private UIScreen _tapUI;
    [GUIColor("blue"), BoxGroup("UI")][SerializeField] 
    private UIScreen _levelUI;
    [GUIColor("blue"), BoxGroup("UI")][SerializeField] 
    private TitleUIScreen _winUI;
    [GUIColor("blue"), BoxGroup("UI")][SerializeField] 
    private TitleUIScreen _loseUI;

    [SerializeField] private MovingBlock _movingBlock;
    [SerializeField] private Car _car;
    [SerializeField] private FinishTrigger _finishTrigger;
    [SerializeField] private float _resultGraceDuration = 2f;

    [Inject]
    public void Construct(IEnemySpawnService enemySpawnService)
    {
        _enemySpawnService = enemySpawnService;
    }

    private IEnemySpawnService _enemySpawnService;
    private bool _resultTappable;

    private void Awake()
    {
        _car.OnDied += OnCarDied;
        _finishTrigger.OnCarEntered += OnFinishReached;

        _tapUI.OnTapped += OnTap;
        _winUI.OnTapped += () => OnResultTapped(_winUI);
        _loseUI.OnTapped += () => OnResultTapped(_loseUI);
    }

    private void Start()
    {
        WaitForTap();
    }

    private void OnTap()
    {
        _tapUI.Hide();
        StartLevel();
    }

    private void StartLevel()
    {
        _levelUI.Show();
        _movingBlock.enabled = true;
        _enemySpawnService.StartSpawning();
        _car.StartShooting();
    }

    private void OnCarDied()
    {
        EndLevel(won: false);
    }

    private void OnFinishReached()
    {
        EndLevel(won: true);
    }

    private void WaitForTap()
    {
        _movingBlock.enabled = false;
        _car.StopShooting();
        _levelUI.Hide();
        _winUI.Hide();
        _loseUI.Hide();
        _tapUI.Show();
    }

    private void EndLevel(bool won)
    {
        _movingBlock.enabled = false;
        _car.StopShooting();
        _enemySpawnService.StopSpawning();
        _levelUI.Hide();

        TitleUIScreen resultUI = won ? _winUI : _loseUI;
        resultUI.Show();

        _resultTappable = false;
        EnableResultTapAfterGrace().Forget();
    }

    private async UniTaskVoid EnableResultTapAfterGrace()
    {
        await UniTask.Delay((int)(_resultGraceDuration * 1000));
        _resultTappable = true;
    }

    private void OnResultTapped(TitleUIScreen resultUI)
    {
        if (!_resultTappable) return;

        resultUI.Hide();
        ResetLevel();
        StartLevel();
    }

    private void ResetLevel()
    {
        _movingBlock.ResetPosition();
        _car.ResetState();
        _enemySpawnService.ClearActiveEnemies();
    }
}

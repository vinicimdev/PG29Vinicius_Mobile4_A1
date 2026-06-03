using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private BallController _ball;
    [SerializeField] private ARCameraBackground _arCamera;
    [SerializeField] private HUDController _hud;
    [SerializeField] private ScoreZone[] _scoreZones;

    [Header("Settings")]
    [SerializeField] private float _gameDuration = 60f;

    private int _score;
    private float _timeLeft;
    private bool _gameRunning;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        // Subscribe to every ScoreZone
        foreach (var zone in _scoreZones)
            zone.OnCollected += AddScore;

        ShowStartMenu();
    }

    private void Update()
    {
        if (!_gameRunning) return;

        _timeLeft -= Time.deltaTime;
        _hud.SetTimer(_timeLeft);

        if (_timeLeft <= 0f)
            EndGame();
    }

    /// <summary>Called by the Start button in the HUD.</summary>
    public void StartGame()
    {
        _score    = 0;
        _timeLeft = _gameDuration;

        foreach (var zone in _scoreZones)
            zone.Reset();

        _ball.ResetBall();
        _ball.SetActive(true);
        _arCamera.StartFeed();

        _hud.SetScore(_score);
        _hud.SetTimer(_timeLeft);
        _hud.ShowGameplay();

        _gameRunning = true;
    }

    /// <summary>Called by the Restart button in the HUD.</summary>
    public void RestartGame()
    {
        _arCamera.StopFeed();
        StartGame();
    }

    private void AddScore(int points)
    {
        _score += points;
        _hud.SetScore(_score);
    }

    private void EndGame()
    {
        _gameRunning = false;
        _ball.SetActive(false);
        _arCamera.StopFeed();
        _hud.ShowGameOver(_score);
    }

    private void ShowStartMenu()
    {
        _ball.SetActive(false);
        _hud.ShowStartMenu();
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject _startMenuPanel;
    [SerializeField] private GameObject _gameplayPanel;
    [SerializeField] private GameObject _gameOverPanel;

    [Header("Start Menu")]
    [SerializeField] private Button _startButton;

    [Header("Gameplay")]
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _timerText;

    [Header("Game Over")]
    [SerializeField] private TMP_Text _finalScoreText;
    [SerializeField] private Button _restartButton;

    private int _lastScore = -1;

    private void Awake()
    {
        _startButton.onClick.AddListener(() => GameManager.Instance.StartGame());
        _restartButton.onClick.AddListener(() => GameManager.Instance.RestartGame());
    }

    public void ShowStartMenu()
    {
        SetPanels(start: true, gameplay: false, gameOver: false);
    }

    public void ShowGameplay()
    {
        SetPanels(start: false, gameplay: true, gameOver: false);
    }

    public void ShowGameOver(int finalScore)
    {
        _finalScoreText.SetText("Score: {0}", finalScore);
        SetPanels(start: false, gameplay: false, gameOver: true);
    }

    /// <summary>Updates score display only when value changes.</summary>
    public void SetScore(int score)
    {
        if (score == _lastScore) return;
        _lastScore = score;
        _scoreText.SetText("Score: {0}", score); // no GC alloc
    }

    /// <summary>Updates timer display every frame.</summary>
    public void SetTimer(float seconds)
    {
        int s = Mathf.CeilToInt(Mathf.Max(seconds, 0f));
        int m = s / 60;
        s %= 60;
        _timerText.SetText("{0}:{1:00}", m, s);
    }

    private void SetPanels(bool start, bool gameplay, bool gameOver)
    {
        _startMenuPanel.SetActive(start);
        _gameplayPanel.SetActive(gameplay);
        _gameOverPanel.SetActive(gameOver);
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private const string BestScoreKey = "BestScore";

    [Header("Score UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text bestScoreText;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text finalScoreText;

    [Header("Buttons")]
    [SerializeField] private Button restartButton;      // nút trong panel Game Over
    [SerializeField] private Button restartButtonHud;   // nút chơi lại luôn hiện (không bắt buộc)

    private int score;
    private int bestScore;

    public bool IsGameOver { get; private set; }

    private void Awake()
    {
        bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);

        if (restartButton != null) restartButton.onClick.AddListener(Restart);
        if (restartButtonHud != null) restartButtonHud.onClick.AddListener(Restart);

        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        UpdateUI();
    }

    // Điểm = số ô đã đặt + thưởng theo số hàng/cột xóa cùng lúc (combo tăng nhanh)
    public void AddScore(int placedCells, int clearedLines)
    {
        score += placedCells + clearedLines * clearedLines * 10;

        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt(BestScoreKey, bestScore);
            PlayerPrefs.Save();
        }

        UpdateUI();
    }

    public void GameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;

        if (finalScoreText != null) finalScoreText.text = score.ToString();
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void UpdateUI()
    {
        if (scoreText != null) scoreText.text = score.ToString();
        if (bestScoreText != null) bestScoreText.text = bestScore.ToString();
    }
}

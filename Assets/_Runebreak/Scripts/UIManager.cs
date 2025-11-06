using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages all UI elements and updates
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI movesText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI pairsText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    
    [Header("Buttons")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button[] levelButtons;
    
    [Header("Level Configurations")]
    [SerializeField] private Vector2Int[] levelGridSizes = new Vector2Int[]
    {
        new Vector2Int(2, 2),
        new Vector2Int(2, 3),
        new Vector2Int(4, 4),
        new Vector2Int(4, 5),
        new Vector2Int(5, 6)
    };
    
    private GameManager gameManager;
    private ScoreManager scoreManager;
    
    private void Start()
    {
        gameManager = GameManager.Instance;
        scoreManager = ScoreManager.Instance;
        
        if (scoreManager != null)
        {
            scoreManager.OnScoreChanged += UpdateScore;
            scoreManager.OnMovesChanged += UpdateMoves;
            scoreManager.OnComboChanged += UpdateCombo;
        }
        
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartClicked);
        }
        
        if (newGameButton != null)
        {
            newGameButton.onClick.AddListener(OnNewGameClicked);
        }
        
        // Setup level buttons
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i] != null && i < levelGridSizes.Length)
            {
                int index = i;
                levelButtons[i].onClick.AddListener(() => OnLevelButtonClicked(index));
            }
        }
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        UpdateAllUI();
    }
    
    private void Update()
    {
        if (gameManager != null)
        {
            UpdatePairs(gameManager.RemainingPairs);
            
            if (!gameManager.IsGameActive && gameOverPanel != null && !gameOverPanel.activeSelf)
            {
                ShowGameOver();
            }
        }
    }
    
    private void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }
    
    private void UpdateMoves(int moves)
    {
        if (movesText != null)
        {
            movesText.text = $"Moves: {moves}";
        }
    }
    
    private void UpdateCombo(int combo)
    {
        if (comboText != null)
        {
            if (combo > 1)
            {
                comboText.text = $"Combo x{combo}!";
                comboText.gameObject.SetActive(true);
            }
            else
            {
                comboText.gameObject.SetActive(false);
            }
        }
    }
    
    private void UpdatePairs(int pairs)
    {
        if (pairsText != null)
        {
            pairsText.text = $"Pairs Left: {pairs}";
        }
    }
    
    private void UpdateAllUI()
    {
        if (scoreManager != null)
        {
            UpdateScore(scoreManager.CurrentScore);
            UpdateMoves(scoreManager.TotalMoves);
            UpdateCombo(scoreManager.CurrentCombo);
        }
        
        if (gameManager != null)
        {
            UpdatePairs(gameManager.RemainingPairs);
        }
    }
    
    private void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            if (gameOverScoreText != null && scoreManager != null)
            {
                gameOverScoreText.text = $"Final Score: {scoreManager.CurrentScore}\nMoves: {scoreManager.TotalMoves}";
            }
        }
    }
    
    private void OnRestartClicked()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        if (gameManager != null)
        {
            gameManager.RestartGame();
        }
        
        UpdateAllUI();
    }
    
    private void OnNewGameClicked()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        if (gameManager != null)
        {
            gameManager.RestartGame();
        }
        
        UpdateAllUI();
    }
    
    private void OnLevelButtonClicked(int levelIndex)
    {
        if (levelIndex < levelGridSizes.Length && gameManager != null)
        {
            Vector2Int gridSize = levelGridSizes[levelIndex];
            
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }
            
            gameManager.StartNewGame(gridSize.y, gridSize.x);
            UpdateAllUI();
        }
    }
    
    private void OnDestroy()
    {
        if (scoreManager != null)
        {
            scoreManager.OnScoreChanged -= UpdateScore;
            scoreManager.OnMovesChanged -= UpdateMoves;
            scoreManager.OnComboChanged -= UpdateCombo;
        }
    }
}


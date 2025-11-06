using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main game controller managing game flow and card matching logic
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("References")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private SaveLoadManager saveLoadManager;
    
    [Header("Game Settings")]
    [SerializeField] private int defaultRows = 4;
    [SerializeField] private int defaultColumns = 4;
    [SerializeField] private float mismatchDelay = 1f;
    
    private List<Card> flippedCards = new List<Card>();
    private bool isProcessingMatch = false;
    private bool isGameActive = false;
    private float playTime = 0f;
    
    public bool IsGameActive => isGameActive;
    public int RemainingPairs { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Get references if not assigned
        if (gridManager == null)
            gridManager = FindObjectOfType<GridManager>();
        
        if (scoreManager == null)
            scoreManager = FindObjectOfType<ScoreManager>();
        
        if (audioManager == null)
            audioManager = FindObjectOfType<AudioManager>();
        
        if (saveLoadManager == null)
            saveLoadManager = FindObjectOfType<SaveLoadManager>();
    }
    
    private void Start()
    {
        // Try to load saved game, otherwise start new game
        if (saveLoadManager != null && saveLoadManager.SaveFileExists())
        {
            LoadGame();
        }
        else
        {
            StartNewGame(defaultRows, defaultColumns);
        }
    }
    
    private void Update()
    {
        if (isGameActive)
        {
            playTime += Time.deltaTime;
        }
    }
    
    public void StartNewGame(int rows, int columns)
    {
        flippedCards.Clear();
        isProcessingMatch = false;
        playTime = 0f;
        
        if (gridManager != null)
        {
            gridManager.CreateGrid(rows, columns);
            RemainingPairs = gridManager.TotalPairs;
            
            // Subscribe to card click events
            List<Card> cards = gridManager.GetAllCards();
            foreach (Card card in cards)
            {
                card.OnCardClicked += OnCardClicked;
            }
        }
        
        if (scoreManager != null)
        {
            scoreManager.ResetScore();
        }
        
        isGameActive = true;
    }
    
    public void RestartGame()
    {
        if (saveLoadManager != null)
        {
            saveLoadManager.DeleteSave();
        }
        
        StartNewGame(defaultRows, defaultColumns);
    }
    
    private void OnCardClicked(Card card)
    {
        if (!isGameActive || isProcessingMatch)
            return;
        
        // Flip the card
        card.Flip(true);
        flippedCards.Add(card);
        
        if (audioManager != null)
        {
            audioManager.PlayFlipSound();
        }
        
        // Check for match when two cards are flipped
        if (flippedCards.Count == 2)
        {
            scoreManager?.AddMove();
            StartCoroutine(CheckMatch());
        }
    }
    
    private IEnumerator CheckMatch()
    {
        isProcessingMatch = true;
        
        // Small delay to let player see both cards
        yield return new WaitForSeconds(0.5f);
        
        Card card1 = flippedCards[0];
        Card card2 = flippedCards[1];
        
        if (card1.CardId == card2.CardId)
        {
            // Match found!
            card1.SetMatched();
            card2.SetMatched();
            
            if (audioManager != null)
            {
                audioManager.PlayMatchSound();
            }
            
            if (scoreManager != null)
            {
                scoreManager.AddMatch();
            }
            
            RemainingPairs--;
            
            // Check for game over
            if (RemainingPairs <= 0)
            {
                yield return new WaitForSeconds(0.5f);
                GameOver();
            }
        }
        else
        {
            // No match
            if (audioManager != null)
            {
                audioManager.PlayMismatchSound();
            }
            
            if (scoreManager != null)
            {
                scoreManager.BreakCombo();
            }
            
            // Wait before flipping back
            yield return new WaitForSeconds(mismatchDelay);
            
            card1.Flip(false);
            card2.Flip(false);
        }
        
        flippedCards.Clear();
        isProcessingMatch = false;
    }
    
    private void GameOver()
    {
        isGameActive = false;
        
        if (audioManager != null)
        {
            audioManager.PlayGameOverSound();
        }
        
        Debug.Log($"Game Over! Score: {scoreManager?.CurrentScore}, Moves: {scoreManager?.TotalMoves}");
        
        // Delete save file on completion
        if (saveLoadManager != null)
        {
            saveLoadManager.DeleteSave();
        }
    }
    
    public void SaveGame()
    {
        if (saveLoadManager == null || gridManager == null || scoreManager == null)
            return;
        
        GameSaveData saveData = new GameSaveData
        {
            gridData = gridManager.GetGridData(),
            scoreData = scoreManager.GetScoreData(),
            playTime = playTime
        };
        
        saveLoadManager.SaveGame(saveData);
    }
    
    public void LoadGame()
    {
        if (saveLoadManager == null)
            return;
        
        GameSaveData saveData = saveLoadManager.LoadGame();
        
        if (saveData == null)
        {
            StartNewGame(defaultRows, defaultColumns);
            return;
        }
        
        flippedCards.Clear();
        isProcessingMatch = false;
        playTime = saveData.playTime;
        
        if (gridManager != null && saveData.gridData != null)
        {
            gridManager.LoadGridData(saveData.gridData);
            
            // Calculate remaining pairs
            RemainingPairs = 0;
            List<Card> cards = gridManager.GetAllCards();
            foreach (Card card in cards)
            {
                card.OnCardClicked += OnCardClicked;
                if (!card.IsMatched)
                {
                    RemainingPairs++;
                }
            }
            RemainingPairs /= 2;
        }
        
        if (scoreManager != null && saveData.scoreData != null)
        {
            scoreManager.LoadScoreData(saveData.scoreData);
        }
        
        isGameActive = true;
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && isGameActive)
        {
            SaveGame();
        }
    }
    
    private void OnApplicationQuit()
    {
        if (isGameActive)
        {
            SaveGame();
        }
    }
}

using UnityEngine;
using System;

/// <summary>
/// Manages scoring, combo system, and game statistics
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    
    [Header("Scoring Settings")]
    [SerializeField] private int matchPoints = 100;
    [SerializeField] private int comboMultiplier = 50;
    [SerializeField] private float comboTimeWindow = 3f;
    
    public int CurrentScore { get; private set; }
    public int CurrentCombo { get; private set; }
    public int TotalMoves { get; private set; }
    public int TotalMatches { get; private set; }
    
    public event Action<int> OnScoreChanged;
    public event Action<int> OnComboChanged;
    public event Action<int> OnMovesChanged;
    
    private float lastMatchTime;
    
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
    }
    
    public void ResetScore()
    {
        CurrentScore = 0;
        CurrentCombo = 0;
        TotalMoves = 0;
        TotalMatches = 0;
        lastMatchTime = 0f;
        
        OnScoreChanged?.Invoke(CurrentScore);
        OnComboChanged?.Invoke(CurrentCombo);
        OnMovesChanged?.Invoke(TotalMoves);
    }
    
    public void AddMove()
    {
        TotalMoves++;
        OnMovesChanged?.Invoke(TotalMoves);
    }
    
    public void AddMatch()
    {
        // Check if within combo time window
        float timeSinceLastMatch = Time.time - lastMatchTime;
        
        if (timeSinceLastMatch <= comboTimeWindow && CurrentCombo > 0)
        {
            CurrentCombo++;
        }
        else
        {
            CurrentCombo = 1;
        }
        
        lastMatchTime = Time.time;
        TotalMatches++;
        
        // Calculate score with combo bonus
        int scoreToAdd = matchPoints + (CurrentCombo - 1) * comboMultiplier;
        CurrentScore += scoreToAdd;
        
        OnScoreChanged?.Invoke(CurrentScore);
        OnComboChanged?.Invoke(CurrentCombo);
    }
    
    public void BreakCombo()
    {
        if (CurrentCombo > 0)
        {
            CurrentCombo = 0;
            OnComboChanged?.Invoke(CurrentCombo);
        }
    }
    
    public ScoreData GetScoreData()
    {
        return new ScoreData
        {
            score = CurrentScore,
            moves = TotalMoves,
            matches = TotalMatches
        };
    }
    
    public void LoadScoreData(ScoreData data)
    {
        CurrentScore = data.score;
        TotalMoves = data.moves;
        TotalMatches = data.matches;
        CurrentCombo = 0;
        
        OnScoreChanged?.Invoke(CurrentScore);
        OnMovesChanged?.Invoke(TotalMoves);
        OnComboChanged?.Invoke(CurrentCombo);
    }
}

[System.Serializable]
public class ScoreData
{
    public int score;
    public int moves;
    public int matches;
}


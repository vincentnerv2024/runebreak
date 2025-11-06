using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Manages the card grid layout with dynamic sizing and positioning
/// UI-based implementation using GridLayoutGroup for automatic layout
/// </summary>
public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private RectTransform gridContainer;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private float spacing = 10f;
    [SerializeField] private Vector2 maxCardSize = new Vector2(150f, 200f);
    
    [Header("Card Sprites")]
    [SerializeField] private Sprite[] cardFrontSprites;
    [SerializeField] private Sprite cardBackSprite;
    
    private List<Card> cards = new List<Card>();
    private int rows;
    private int columns;
    
    public int TotalCards => rows * columns;
    public int TotalPairs => TotalCards / 2;
    
    private void Awake()
    {
        // Get GridLayoutGroup if not assigned
        if (gridLayoutGroup == null && gridContainer != null)
        {
            gridLayoutGroup = gridContainer.GetComponent<GridLayoutGroup>();
        }
    }
    
    public void CreateGrid(int gridRows, int gridColumns)
    {
        ClearGrid();
        
        rows = gridRows;
        columns = gridColumns;
        
        if (rows * columns % 2 != 0)
        {
            Debug.LogError("Grid must have an even number of cards!");
            return;
        }
        
        // Configure GridLayoutGroup
        ConfigureGridLayout();
        
        // Generate card pairs
        List<int> cardIds = GenerateCardIds();
        
        // Create cards
        for (int i = 0; i < cardIds.Count; i++)
        {
            CreateCard(cardIds[i]);
        }
    }
    
    private void ConfigureGridLayout()
    {
        if (gridLayoutGroup == null || gridContainer == null)
            return;
        
        // Set constraint to column count
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = columns;
        gridLayoutGroup.spacing = new Vector2(spacing, spacing);
        
        // Calculate optimal card size
        RectTransform containerRect = gridContainer;
        float availableWidth = containerRect.rect.width - (columns - 1) * spacing;
        float availableHeight = containerRect.rect.height - (rows - 1) * spacing;
        
        float cardWidth = Mathf.Min(availableWidth / columns, maxCardSize.x);
        float cardHeight = Mathf.Min(availableHeight / rows, maxCardSize.y);
        
        // Keep aspect ratio (3:4 is typical for cards)
        float aspectRatio = 0.75f;
        if (cardWidth / cardHeight > aspectRatio)
        {
            cardWidth = cardHeight * aspectRatio;
        }
        else
        {
            cardHeight = cardWidth / aspectRatio;
        }
        
        gridLayoutGroup.cellSize = new Vector2(cardWidth, cardHeight);
        
        // Center alignment
        gridLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
    }
    
    private List<int> GenerateCardIds()
    {
        int pairCount = TotalPairs;
        List<int> ids = new List<int>();
        
        // Create pairs
        for (int i = 0; i < pairCount; i++)
        {
            ids.Add(i);
            ids.Add(i);
        }
        
        // Shuffle using Fisher-Yates algorithm for better randomization
        for (int i = ids.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = ids[i];
            ids[i] = ids[j];
            ids[j] = temp;
        }
        
        return ids;
    }
    
    private void CreateCard(int cardId)
    {
        if (cardPrefab == null)
        {
            Debug.LogError("Card prefab is not assigned!");
            return;
        }
        
        if (gridContainer == null)
        {
            Debug.LogError("Grid container is not assigned!");
            return;
        }
        
        GameObject cardObj = Instantiate(cardPrefab, gridContainer);
        
        Card card = cardObj.GetComponent<Card>();
        if (card == null)
        {
            Debug.LogError("Card prefab must have a Card component!");
            return;
        }
        
        // Assign sprites
        Sprite frontSprite = GetCardSprite(cardId);
        card.Initialize(cardId, frontSprite, cardBackSprite);
        
        cards.Add(card);
    }
    
    private Sprite GetCardSprite(int cardId)
    {
        if (cardFrontSprites != null && cardFrontSprites.Length > 0)
        {
            return cardFrontSprites[cardId % cardFrontSprites.Length];
        }
        return null;
    }
    
    public void ClearGrid()
    {
        foreach (Card card in cards)
        {
            if (card != null)
            {
                Destroy(card.gameObject);
            }
        }
        cards.Clear();
    }
    
    public List<Card> GetAllCards()
    {
        return new List<Card>(cards);
    }
    
    public void ResetAllCards()
    {
        foreach (Card card in cards)
        {
            card.ResetCard();
        }
    }
    
    public GridData GetGridData()
    {
        GridData data = new GridData
        {
            rows = rows,
            columns = columns,
            cardStates = new List<CardState>()
        };
        
        foreach (Card card in cards)
        {
            data.cardStates.Add(new CardState
            {
                cardId = card.CardId,
                isMatched = card.IsMatched
            });
        }
        
        return data;
    }
    
    public void LoadGridData(GridData data)
    {
        CreateGrid(data.rows, data.columns);
        
        for (int i = 0; i < cards.Count && i < data.cardStates.Count; i++)
        {
            if (data.cardStates[i].isMatched)
            {
                cards[i].Flip(true);
                cards[i].SetMatched();
            }
        }
    }
}

[System.Serializable]
public class GridData
{
    public int rows;
    public int columns;
    public List<CardState> cardStates;
}

[System.Serializable]
public class CardState
{
    public int cardId;
    public bool isMatched;
}


using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Manages the card grid layout with dynamic sizing and positioning
/// </summary>
public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform gridContainer;
    [SerializeField] private Vector2 padding = new Vector2(0.2f, 0.2f);
    [SerializeField] private Vector2 displayArea = new Vector2(10f, 10f);
    
    [Header("Card Sprites")]
    [SerializeField] private Sprite[] cardFrontSprites;
    [SerializeField] private Sprite cardBackSprite;
    
    private List<Card> cards = new List<Card>();
    private int rows;
    private int columns;
    
    public int TotalCards => rows * columns;
    public int TotalPairs => TotalCards / 2;
    
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
        
        // Calculate card size and spacing
        float cardWidth = (displayArea.x - (columns + 1) * padding.x) / columns;
        float cardHeight = (displayArea.y - (rows + 1) * padding.y) / rows;
        float cardSize = Mathf.Min(cardWidth, cardHeight);
        
        // Calculate starting position (centered)
        float totalWidth = columns * cardSize + (columns - 1) * padding.x;
        float totalHeight = rows * cardSize + (rows - 1) * padding.y;
        float startX = -totalWidth / 2f + cardSize / 2f;
        float startY = totalHeight / 2f - cardSize / 2f;
        
        // Generate card pairs
        List<int> cardIds = GenerateCardIds();
        
        // Create cards
        int cardIndex = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 position = new Vector3(
                    startX + col * (cardSize + padding.x),
                    startY - row * (cardSize + padding.y),
                    0
                );
                
                CreateCard(position, cardSize, cardIds[cardIndex]);
                cardIndex++;
            }
        }
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
        
        // Shuffle
        return ids.OrderBy(x => Random.value).ToList();
    }
    
    private void CreateCard(Vector3 position, float size, int cardId)
    {
        if (cardPrefab == null)
        {
            Debug.LogError("Card prefab is not assigned!");
            return;
        }
        
        GameObject cardObj = Instantiate(cardPrefab, position, Quaternion.identity, gridContainer);
        cardObj.transform.localPosition = position;
        cardObj.transform.localScale = Vector3.one * size;
        
        Card card = cardObj.GetComponent<Card>();
        if (card == null)
        {
            card = cardObj.AddComponent<Card>();
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


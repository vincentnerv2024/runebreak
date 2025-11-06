using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Represents a single card in the memory game with flip animations and state management
/// </summary>
public class Card : MonoBehaviour
{
    [Header("Card Settings")]
    [SerializeField] private SpriteRenderer frontSprite;
    [SerializeField] private SpriteRenderer backSprite;
    [SerializeField] private float flipDuration = 0.3f;
    
    public int CardId { get; private set; }
    public bool IsFlipped { get; private set; }
    public bool IsMatched { get; private set; }
    public bool IsFlipping { get; private set; }
    
    public event Action<Card> OnCardClicked;
    
    private Vector3 frontRotation = new Vector3(0, 0, 0);
    private Vector3 backRotation = new Vector3(0, 180, 0);
    
    public void Initialize(int id, Sprite frontImage, Sprite backImage)
    {
        CardId = id;
        
        if (frontSprite != null)
            frontSprite.sprite = frontImage;
        
        if (backSprite != null)
            backSprite.sprite = backImage;
        
        ResetCard();
    }
    
    public void ResetCard()
    {
        IsFlipped = false;
        IsMatched = false;
        IsFlipping = false;
        
        if (frontSprite != null)
            frontSprite.enabled = false;
        
        if (backSprite != null)
            backSprite.enabled = true;
        
        transform.localRotation = Quaternion.Euler(backRotation);
    }
    
    public void Flip(bool showFront, Action onComplete = null)
    {
        if (IsFlipping || IsMatched)
            return;
        
        StartCoroutine(FlipAnimation(showFront, onComplete));
    }
    
    private IEnumerator FlipAnimation(bool showFront, Action onComplete)
    {
        IsFlipping = true;
        
        Vector3 startRotation = transform.localEulerAngles;
        Vector3 targetRotation = showFront ? frontRotation : backRotation;
        
        float elapsed = 0f;
        
        // Flip to 90 degrees (edge view)
        while (elapsed < flipDuration / 2)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (flipDuration / 2);
            float yRotation = Mathf.Lerp(startRotation.y, startRotation.y + 90, t);
            transform.localRotation = Quaternion.Euler(0, yRotation, 0);
            yield return null;
        }
        
        // Switch sprite visibility at midpoint
        if (frontSprite != null)
            frontSprite.enabled = showFront;
        
        if (backSprite != null)
            backSprite.enabled = !showFront;
        
        // Complete the flip
        elapsed = 0f;
        while (elapsed < flipDuration / 2)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (flipDuration / 2);
            float yRotation = Mathf.Lerp(startRotation.y + 90, targetRotation.y, t);
            transform.localRotation = Quaternion.Euler(0, yRotation, 0);
            yield return null;
        }
        
        transform.localRotation = Quaternion.Euler(targetRotation);
        IsFlipped = showFront;
        IsFlipping = false;
        
        onComplete?.Invoke();
    }
    
    public void SetMatched()
    {
        IsMatched = true;
        StartCoroutine(MatchedAnimation());
    }
    
    private IEnumerator MatchedAnimation()
    {
        // Simple pulse effect for matched cards
        Vector3 originalScale = transform.localScale;
        float duration = 0.3f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float scale = 1f + Mathf.Sin(elapsed / duration * Mathf.PI) * 0.2f;
            transform.localScale = originalScale * scale;
            yield return null;
        }
        
        transform.localScale = originalScale;
    }
    
    private void OnMouseDown()
    {
        if (!IsFlipping && !IsMatched && !IsFlipped)
        {
            OnCardClicked?.Invoke(this);
        }
    }
}


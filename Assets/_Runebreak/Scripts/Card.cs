using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;

/// <summary>
/// Represents a single card in the memory game with flip animations and state management
/// UI-based implementation with layered visuals (image, frame, glow)
/// </summary>
[RequireComponent(typeof(Button))]
public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Card Visual Layers")]
    [SerializeField] private Image frontImage;      // Main card image (front)
    [SerializeField] private Image backImage;       // Main card image (back)
    [SerializeField] private Image frameImage;      // Frame/border (always visible)
    [SerializeField] private Image glowImage;       // Glow effect (for highlights)
    
    [Header("Card Settings")]
    [SerializeField] private float flipDuration = 0.3f;
    [SerializeField] private bool preserveAspect = true; // Preserve sprite aspect ratio
    [SerializeField] private bool enableGlowOnHover = true;
    
    public int CardId { get; private set; }
    public bool IsFlipped { get; private set; }
    public bool IsMatched { get; private set; }
    public bool IsFlipping { get; private set; }
    
    public event Action<Card> OnCardClicked;
    
    private Button button;
    private RectTransform rectTransform;
    private Vector3 frontRotation = new Vector3(0, 0, 0);
    private Vector3 backRotation = new Vector3(0, 180, 0);
    
    private void Awake()
    {
        button = GetComponent<Button>();
        rectTransform = GetComponent<RectTransform>();
        
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
    }
    
    public void Initialize(int id, Sprite frontSprite, Sprite backSprite)
    {
        CardId = id;
        
        if (frontImage != null && frontSprite != null)
        {
            frontImage.sprite = frontSprite;
            frontImage.preserveAspect = preserveAspect;
        }
        
        if (backImage != null && backSprite != null)
        {
            backImage.sprite = backSprite;
            backImage.preserveAspect = preserveAspect;
        }
        
        ResetCard();
    }
    
    public void ResetCard()
    {
        IsFlipped = false;
        IsMatched = false;
        IsFlipping = false;
        
        if (frontImage != null)
            frontImage.enabled = false;
        
        if (backImage != null)
            backImage.enabled = true;
        
        if (rectTransform != null)
            rectTransform.localRotation = Quaternion.Euler(backRotation);
        
        if (button != null)
            button.interactable = true;
        
        // Reset glow
        SetGlowActive(false);
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
        
        if (button != null)
            button.interactable = false;
        
        Vector3 startRotation = rectTransform.localEulerAngles;
        Vector3 targetRotation = showFront ? frontRotation : backRotation;
        
        float elapsed = 0f;
        
        // Flip to 90 degrees (edge view)
        while (elapsed < flipDuration / 2)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (flipDuration / 2);
            float yRotation = Mathf.Lerp(startRotation.y, startRotation.y + 90, t);
            rectTransform.localRotation = Quaternion.Euler(0, yRotation, 0);
            yield return null;
        }
        
        // Switch image visibility at midpoint
        if (frontImage != null)
            frontImage.enabled = showFront;
        
        if (backImage != null)
            backImage.enabled = !showFront;
        
        // Complete the flip
        elapsed = 0f;
        while (elapsed < flipDuration / 2)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (flipDuration / 2);
            float yRotation = Mathf.Lerp(startRotation.y + 90, targetRotation.y, t);
            rectTransform.localRotation = Quaternion.Euler(0, yRotation, 0);
            yield return null;
        }
        
        rectTransform.localRotation = Quaternion.Euler(targetRotation);
        IsFlipped = showFront;
        IsFlipping = false;
        
        if (!IsMatched && button != null)
            button.interactable = true;
        
        onComplete?.Invoke();
    }
    
    public void SetMatched()
    {
        IsMatched = true;
        
        if (button != null)
            button.interactable = false;
        
        StartCoroutine(MatchedAnimation());
    }
    
    private IEnumerator MatchedAnimation()
    {
        // Pulse the glow during match
        SetGlowActive(true);
        
        // Simple pulse effect for matched cards
        Vector3 originalScale = rectTransform.localScale;
        float duration = 0.5f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Scale pulse
            float scale = 1f + Mathf.Sin(t * Mathf.PI) * 0.2f;
            rectTransform.localScale = originalScale * scale;
            
            // Glow pulse
            if (glowImage != null)
            {
                Color glowColor = glowImage.color;
                glowColor.a = Mathf.Sin(t * Mathf.PI * 2) * 0.5f + 0.5f; // Pulsing alpha
                glowImage.color = glowColor;
            }
            
            yield return null;
        }
        
        rectTransform.localScale = originalScale;
        
        // Keep glow on for matched cards
        SetGlowActive(true, 0.3f);
    }
    
    public void SetGlowActive(bool active, float alpha = 1f)
    {
        if (glowImage != null)
        {
            glowImage.enabled = active;
            if (active)
            {
                Color glowColor = glowImage.color;
                glowColor.a = alpha;
                glowImage.color = glowColor;
            }
        }
    }
    
    public void SetFrameColor(Color color)
    {
        if (frameImage != null)
        {
            frameImage.color = color;
        }
    }
    
    // Pointer events for hover glow
    public void OnPointerEnter()
    {
        if (enableGlowOnHover && !IsFlipped && !IsMatched && !IsFlipping)
        {
            SetGlowActive(true, 0.5f);
        }
    }
    
    public void OnPointerExit()
    {
        if (enableGlowOnHover && !IsMatched)
        {
            SetGlowActive(false);
        }
    }
    
    private void OnButtonClicked()
    {
        if (!IsFlipping && !IsMatched && !IsFlipped)
        {
            OnCardClicked?.Invoke(this);
        }
    }
    
    // Interface implementations for hover effects
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnter();
    }
    
    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        OnPointerExit();
    }
    
    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClicked);
        }
    }
}


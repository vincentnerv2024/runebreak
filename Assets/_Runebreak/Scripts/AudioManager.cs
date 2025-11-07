using UnityEngine;

/// <summary>
/// Manages all audio playback for the game
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    
    [Header("Sound Effects")]
    [SerializeField] private AudioClip flipSound;
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip matchSound;
    [SerializeField] private AudioClip mismatchSound;
    [SerializeField] private AudioClip gameOverSound;
    
    [Header("Settings")]
    [SerializeField] private float volume = 1f;
    [SerializeField] private float hoverPitchMin = 0.95f;
    [SerializeField] private float hoverPitchMax = 1.05f;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Create audio source if not assigned
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
            }
            
            sfxSource.volume = volume;
            sfxSource.playOnAwake = false;
            sfxSource.loop = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void PlayFlipSound()
    {
        PlaySound(flipSound);
    }

    public void PlayHoverSound()
    {
        float pitch = Mathf.Clamp(Random.Range(hoverPitchMin, hoverPitchMax), 0.1f, 3f);
        AudioClip clip = hoverSound != null ? hoverSound : flipSound;
        if (clip == null)
            return;

        PlaySound(clip, pitch);
    }
    
    public void PlayMatchSound()
    {
        PlaySound(matchSound);
    }
    
    public void PlayMismatchSound()
    {
        PlaySound(mismatchSound);
    }
    
    public void PlayGameOverSound()
    {
        PlaySound(gameOverSound);
    }
    
    private void PlaySound(AudioClip clip, float pitch = 1f)
    {
        if (clip != null && sfxSource != null)
        {
            float originalPitch = sfxSource.pitch;
            sfxSource.pitch = pitch;
            sfxSource.PlayOneShot(clip);
            sfxSource.pitch = originalPitch;
        }
    }
    
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        if (sfxSource != null)
        {
            sfxSource.volume = volume;
        }
    }
}


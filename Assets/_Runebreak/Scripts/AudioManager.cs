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
    [SerializeField] private AudioClip matchSound;
    [SerializeField] private AudioClip mismatchSound;
    [SerializeField] private AudioClip gameOverSound;
    
    [Header("Settings")]
    [SerializeField] private float volume = 1f;
    
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
    
    private void PlaySound(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
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


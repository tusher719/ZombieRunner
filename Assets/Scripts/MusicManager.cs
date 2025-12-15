using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Background Music")]
    public AudioClip menuMusic;
    [Range(0f, 1f)]
    public float musicVolume = 0.3f;

    private AudioSource audioSource;
    private bool isMuted = false;

    void Awake()
    {
        // Singleton pattern - only one music manager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Don't destroy when loading new scene
            
            // Setup AudioSource
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = true;
            audioSource.volume = musicVolume;
            
            // Load saved settings
            LoadMusicSettings();
            
            // Start playing music if not muted
            if (menuMusic != null)
            {
                audioSource.clip = menuMusic;
                if (!isMuted)
                {
                    audioSource.Play();
                    Debug.Log("[MusicManager] Menu music started");
                }
                else
                {
                    Debug.Log("[MusicManager] Music muted from settings");
                }
            }
        }
        else
        {
            // Another music manager exists, destroy this one
            Destroy(gameObject);
            Debug.Log("[MusicManager] Duplicate music manager destroyed");
        }
    }

    void LoadMusicSettings()
    {
        // Load mute state from PlayerPrefs (0 = not muted, 1 = muted)
        isMuted = PlayerPrefs.GetInt("BackgroundMusicMuted", 0) == 1;
    }

    public void SetMuted(bool muted)
    {
        isMuted = muted;
        
        if (audioSource != null)
        {
            if (isMuted)
            {
                audioSource.Pause();
                Debug.Log("[MusicManager] Music muted");
            }
            else
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.UnPause();
                }
                Debug.Log("[MusicManager] Music unmuted");
            }
        }
    }

    public bool IsMuted()
    {
        return isMuted;
    }

    // Optional: Methods to control music
    public void SetVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (audioSource != null)
        {
            audioSource.volume = musicVolume;
        }
    }

    public void PauseMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (audioSource != null && !audioSource.isPlaying && !isMuted)
        {
            audioSource.UnPause();
        }
    }

    public void StopMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
}
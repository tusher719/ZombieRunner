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
    private bool isStoppedForGameplay = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = true;
            audioSource.volume = musicVolume;
            
            LoadMusicSettings();
            
            if (menuMusic != null)
            {
                audioSource.clip = menuMusic;
                if (!isMuted)
                {
                    audioSource.Play();
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LoadMusicSettings()
    {
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
            }
            else
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.UnPause();
                }
            }
        }
    }

    public bool IsMuted()
    {
        return isMuted;
    }

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
        if (audioSource != null && !isMuted)
        {
            if (isStoppedForGameplay)
            {
                return;
            }
            
            if (!audioSource.isPlaying)
            {
                if (audioSource.time > 0)
                {
                    audioSource.UnPause();
                }
                else
                {
                    audioSource.Play();
                }
            }
        }
    }

    public void AllowMusicResume()
    {
        isStoppedForGameplay = false;
        ResumeMusic();
    }

    public void StopMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            isStoppedForGameplay = true;
        }
    }

    public bool IsPlaying()
    {
        return audioSource != null && audioSource.isPlaying;
    }
}
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;       
    public AudioSource sfxSource; // Ensure this is assigned in Inspector!

    [Header("Music Lists")]
    public AudioClip[] menuMusicList;      
    public AudioClip[] careerMusicList;    
    public AudioClip[] infiniteMusicList;  

    // --- CONFIGURATION ---
    private const float MAX_MUSIC_VOLUME = 0.5f; // Slider at 100% = 0.5 actual volume
    private const float DEFAULT_SLIDER_VALUE = 0.04f; // 0.04 * 0.5 = 0.02 (Your requested default)

    // --- STATE VARIABLES ---
    private bool isMuted = false; 
    private float savedSliderValue = 1f; // Stores the 0.0 to 1.0 slider position

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // --- LOAD SAVED SETTINGS ---
            // Load Mute State
            isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;

            // Load Saved Slider Position (Default to 0.04 if not found)
            savedSliderValue = PlayerPrefs.GetFloat("SavedSliderValue", DEFAULT_SLIDER_VALUE); 

            // Apply immediately
            ApplyVolume();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- VOLUME CONTROL ---
    
    public void ToggleSound()
    {
        isMuted = !isMuted;
        ApplyVolume(); 
        
        PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ChangeVolume(float sliderValue)
    {
        // If muted, we still update the "saved" value, but we don't apply it yet
        if (isMuted) return;

        savedSliderValue = sliderValue;
        ApplyVolume();

        PlayerPrefs.SetFloat("SavedSliderValue", savedSliderValue);
        PlayerPrefs.Save();
    }

    // Helper function to calculate and set the volume
    private void ApplyVolume()
    {
        if (isMuted)
        {
            musicSource.volume = 0; 
            // Note: We do NOT mute SFX here based on your request. 
            // If you want to mute SFX too, add: sfxSource.volume = 0;
        }
        else
        {
            // MATH: Slider (0 to 1) * MaxVolume (0.5)
            // Example: Slider 1.0 * 0.5 = 0.5 Volume
            // Example: Slider 0.04 * 0.5 = 0.02 Volume
            musicSource.volume = savedSliderValue * MAX_MUSIC_VOLUME;
            
            // Restore SFX volume (assuming SFX should always be full volume)
            if(sfxSource != null) sfxSource.volume = 1.0f; 
        }
    }

    public bool IsMuted()
    {
        return isMuted;
    }

    public float GetSavedVolume()
    {
        return savedSliderValue;
    }

    // --- MUSIC PLAYING LOGIC ---
    // (Kept exactly the same)
    public void PlayMenuMusic() { PlayMusicFromList(menuMusicList); }
    public void PlayCareerMusic() { PlayMusicFromList(careerMusicList); }
    public void PlayInfiniteMusic() { PlayMusicFromList(infiniteMusicList); }

    private void PlayMusicFromList(AudioClip[] playlist)
    {
        if (playlist.Length == 0) return;
        int index = Random.Range(0, playlist.Length);
        AudioClip newClip = playlist[index];
        
        // Prevent restarting the same song
        if (musicSource.clip == newClip && musicSource.isPlaying) return;
        
        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.volume = isMuted ? 0 : savedSliderValue * MAX_MUSIC_VOLUME; // Ensure volume is correct on start
        musicSource.Play();
    }
}
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;       

    [Header("Music Lists")]
    public AudioClip[] menuMusicList;      
    public AudioClip[] careerMusicList;    
    public AudioClip[] infiniteMusicList;  

    // --- VARIABLES ---
    private bool isMuted = false; 
    private float savedVolume = 1f; // NEW: Remembers volume (0.0 to 1.0)

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // --- LOAD SAVED SETTINGS ---
            isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;
            savedVolume = PlayerPrefs.GetFloat("SavedVolume", 1f); // Load saved slider value

            // Apply immediately
            ApplyVolume();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- NEW FUNCTIONS FOR BUTTON & SLIDER ---
    
    public void ToggleSound()
    {
        isMuted = !isMuted;
        
        // Use helper to restore previous volume instead of hard reset to 1
        ApplyVolume(); 

        // Save the setting
        PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ChangeVolume(float volume)
    {
        // 1. If we are muted, DO NOT change the volume (keeps it silent)
        if (isMuted) return;

        // 2. Remember this new volume
        savedVolume = volume;
        
        // 3. Apply it
        AudioListener.volume = savedVolume;

        // 4. Save it
        PlayerPrefs.SetFloat("SavedVolume", savedVolume);
        PlayerPrefs.Save();
    }

    // Helper function to decide actual volume
    private void ApplyVolume()
    {
        if (isMuted)
        {
            AudioListener.volume = 0; // Silence
        }
        else
        {
            AudioListener.volume = savedVolume; // Restore saved volume (e.g. 0.5)
        }
    }

    public bool IsMuted()
    {
        return isMuted;
    }

    public float GetSavedVolume()
    {
        return savedVolume;
    }

    // ... (Keep the rest of your PlayMusic functions exactly the same) ...
    public void PlayMenuMusic() { PlayMusicFromList(menuMusicList); }
    public void PlayCareerMusic() { PlayMusicFromList(careerMusicList); }
    public void PlayInfiniteMusic() { PlayMusicFromList(infiniteMusicList); }

    private void PlayMusicFromList(AudioClip[] playlist)
    {
        if (playlist.Length == 0) return;
        int index = Random.Range(0, playlist.Length);
        AudioClip newClip = playlist[index];
        if (musicSource.clip == newClip && musicSource.isPlaying) return;
        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();
    }

}
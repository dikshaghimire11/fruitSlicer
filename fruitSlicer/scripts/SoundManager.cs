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

    // --- NEW VARIABLE ---
    private bool isMuted = false; 

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // --- LOAD SAVED SETTING ON START ---
            // 0 = Sound On, 1 = Sound Off
            isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;
            AudioListener.volume = isMuted ? 0 : 1; 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- NEW FUNCTIONS FOR BUTTON ---
    
    public void ToggleSound()
    {
        isMuted = !isMuted;
        
        // 0 means Silent, 1 means Full Volume
        AudioListener.volume = isMuted ? 0 : 1; 

        // Save the setting
        PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool IsMuted()
    {
        return isMuted;
    }

    // ... (Keep the rest of your PlayMenuMusic, PlaySFX, etc. functions exactly the same) ...
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
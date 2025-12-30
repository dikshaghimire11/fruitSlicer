using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [Header("UI References")]
    public Image soundButtonIcon; 
    public Slider volumeSlider;   

    [Header("Sprites")]
    public Sprite soundOnSprite;  
    public Sprite soundOffSprite; 

    void Start()
    {
        // 1. Setup the Slider Listener
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        // 2. Update the UI correctly when the scene starts
        UpdateUI();
    }

    // Called when dragging the slider
    public void OnVolumeChanged(float value)
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.ChangeVolume(value);
        }
    }

    // Called when clicking the Mute Button
    public void OnSoundButtonClicked()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.ToggleSound();
            UpdateUI(); // <--- CRITICAL: Refreshes the slider lock state
        }
    }

    // Updates the Icons and Locks/Unlocks the Slider
    void UpdateUI()
    {
        if (SoundManager.instance == null) return;

        bool isMuted = SoundManager.instance.IsMuted();

        // 1. Update the Button Icon
        if (soundButtonIcon != null)
        {
            soundButtonIcon.sprite = isMuted ? soundOffSprite : soundOnSprite;
        }

        // 2. Lock or Unlock the Slider
        if (volumeSlider != null)
        {
            if (isMuted)
            {
                // IF MUTED: Lock the slider and show 0 volume
                volumeSlider.interactable = false; 
                volumeSlider.value = 0; 
            }
            else
            {
                // IF UNMUTED: Unlock slider and show saved volume
                volumeSlider.interactable = true;
                volumeSlider.value = SoundManager.instance.GetSavedVolume();
            }
        }
    }
}
using UnityEngine;
using UnityEngine.UI; // Required for UI buttons and images

public class SettingManager : MonoBehaviour
{
    [Header("UI References")]
    public Image soundButtonIcon; // Drag the Image component of your button here

    [Header("Sprites")]
    public Sprite soundOnSprite;  // Drag your "Speaker" icon here
    public Sprite soundOffSprite; // Drag your "Mute" icon here

    void Start()
    {
        // Update the visual state immediately when scene loads
        UpdateSoundIcon();
    }

    public void OnSoundButtonClicked()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.ToggleSound();
            UpdateSoundIcon();
        }
    }

    void UpdateSoundIcon()
    {
        if (SoundManager.instance != null && soundButtonIcon != null)
        {
            bool isMuted = SoundManager.instance.IsMuted();
            
            // If Muted -> Show Off Sprite. If Not Muted -> Show On Sprite.
            soundButtonIcon.sprite = isMuted ? soundOffSprite : soundOnSprite;
        }
    }
}
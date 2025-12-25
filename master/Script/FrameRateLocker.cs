using UnityEngine;

public class FrameRateLocker : MonoBehaviour
{
    void Awake()
    {
        // 1. Disable VSync (Essential: Unity ignores targetFrameRate if VSync is on)
        QualitySettings.vSyncCount = 0;

        // 2. Lock the frame rate to 60
        Application.targetFrameRate = 60;
    }
}
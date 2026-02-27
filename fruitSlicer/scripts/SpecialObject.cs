using UnityEngine;

public class SpecialObject : MonoBehaviour
{
    public AudioClip breakSound;
    private static AudioSource sliceSource;
    private bool isSliced = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (sliceSource == null)
        {
            GameObject audioObj = GameObject.Find("SliceAudio");
            if (audioObj != null)
            {
                sliceSource = audioObj.GetComponent<AudioSource>();
            }
        }
    }

    // Update is called once per frame
    public void Slice(Vector2 sliceDirection)
    {
        if (isSliced) return;
        isSliced = true;

        // 🔊 INSTANT SOUND (NO DELAY)
        if (breakSound != null && sliceSource != null)
        {
            sliceSource.pitch = Random.Range(0.95f, 1.05f);
            sliceSource.PlayOneShot(breakSound);
        }
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.addSpecialReward();
        }
        Destroy(gameObject);

    }

}

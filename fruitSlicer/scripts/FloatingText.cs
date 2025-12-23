using UnityEngine;
using TMPro; // We need this for TextMeshPro

public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float destroyTime = 1f;
   public TMP_Text textMesh;

    void Awake()
    {
        // Auto-find component if not assigned
        if (textMesh == null) textMesh = GetComponent<TMP_Text>();
    }

    void Start()
    {
        // Destroy the text object after 1 second
        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        // Move the text UP constantly
        transform.Translate(Vector3.up * moveSpeed * Time.unscaledDeltaTime);
    }

    // Call this to change the text and color instantly
    public void Setup(string message, Color color)
    {
        if (textMesh != null)
        {
            textMesh.text = message;
            textMesh.color = color;
            textMesh.fontSize = 6; // Make it big!
        }
    }
}
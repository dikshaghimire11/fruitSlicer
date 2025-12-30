using UnityEngine;

// public enum FruitType { Apple, Orange, Watermelon, Banana, Pineapple, Mango ,Kiwi ,Coconut}

public class Fruit : MonoBehaviour
{
    // --- ASSIGN THIS IN INSPECTOR ---
    // public FruitType fruitType; 
    
    public GameObject leftHalf;
    public GameObject rightHalf;
    public float sliceForce = 5f;
    public float rotationForce = 20f;
    public AudioClip sliceSound;
    public int points = 10;
    public float missYPosition = -8f;

    private bool isSliced = false;

    public void Slice(Vector2 sliceDirection)
    {
        if (isSliced) return;
        isSliced = true;

        if (sliceSound != null) AudioSource.PlayClipAtPoint(sliceSound, transform.position, 1.0f);

        GameObject mainFruit = this.gameObject;
        mainFruit.SetActive(false);

        GameObject leftInst = Instantiate(leftHalf, transform.position, transform.rotation);
        GameObject rightInst = Instantiate(rightHalf, transform.position, transform.rotation);

        Rigidbody2D leftRb = leftInst.GetComponent<Rigidbody2D>();
        Rigidbody2D rightRb = rightInst.GetComponent<Rigidbody2D>();

        leftRb.AddForce((-sliceDirection + new Vector2(-0.5f, 0)) * sliceForce, ForceMode2D.Impulse);
        rightRb.AddForce((sliceDirection + new Vector2(0.5f, 0)) * sliceForce, ForceMode2D.Impulse);

        float currentTorque = Random.Range(rotationForce * 0.8f, rotationForce * 1.2f);
        leftRb.AddTorque(currentTorque, ForceMode2D.Impulse);
        rightRb.AddTorque(-currentTorque, ForceMode2D.Impulse);

        Destroy(gameObject); 
        Destroy(leftInst, 4f);
        Destroy(rightInst, 4f);
    }

    void Update()
    {
        if (transform.position.y < missYPosition)
        {
            // ONLY LOSE LIFE IN INFINITE MODE
            if (ModeManager.Instance.currentMode == GameMode.Infinite)
            {
                if (ScoreManager.instance != null) ScoreManager.instance.LoseLife();
            }
            Destroy(gameObject);
        }
    }
}
using UnityEngine;

public class Fruit : MonoBehaviour
{
    public GameObject leftHalf;
    public GameObject rightHalf;
    
    public float sliceForce = 5f;
    
    // 1. Add a variable to control how fast they spin
    public float rotationForce = 20f; 
    public AudioClip sliceSound;

    private bool isSliced = false;

    public void Slice(Vector2 sliceDirection)
    {
        if (isSliced) return;
        isSliced = true;
        if (sliceSound != null)
        {
            AudioSource.PlayClipAtPoint(sliceSound, transform.position, 1.0f);
        }

        GameObject mainFruit = this.gameObject;
        Transform mainFruitTransform = mainFruit.transform;
        
        // Disable the whole fruit immediately so it swaps cleanly
        mainFruit.SetActive(false); 

        // 2. Instantiate and CAPTURE the new objects in variables
        // We need to store them in 'leftInst' and 'rightInst' to access THEIR Rigidbodies
        GameObject leftInst = Instantiate(leftHalf, mainFruitTransform.position, mainFruitTransform.rotation);
        GameObject rightInst = Instantiate(rightHalf, mainFruitTransform.position, mainFruitTransform.rotation);

        Rigidbody2D leftRb = leftInst.GetComponent<Rigidbody2D>();
        Rigidbody2D rightRb = rightInst.GetComponent<Rigidbody2D>();

        // 3. Add Linear Force (Pushing them apart)
        // I kept your logic here, pushing them generally up and away from the cut
        leftRb.AddForce((-sliceDirection + new Vector2(-0.5f,0)) * sliceForce, ForceMode2D.Impulse);
        rightRb.AddForce((sliceDirection + new Vector2(0.5f,0)) * sliceForce, ForceMode2D.Impulse);

        // 4. Add Rotation (Torque)
        // We apply torque based on the rotationForce. 
        // We create a slight randomness so every cut looks unique.
        float currentTorque = Random.Range(rotationForce * 0.8f, rotationForce * 1.2f);

        // Apply positive rotation to one and negative to the other 
        // This makes them spin away from each other (like a real cut)
        leftRb.AddTorque(currentTorque, ForceMode2D.Impulse);
        rightRb.AddTorque(-currentTorque, ForceMode2D.Impulse);

        // Destroy the original whole fruit
        Destroy(gameObject);
        
        // Optional: Destroy the slices after 3-4 seconds so they don't clutter the scene
        Destroy(leftInst, 4f);
        Destroy(rightInst, 4f);
    }
}
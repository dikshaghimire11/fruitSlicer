using UnityEngine;

public class FrozenFruitEffect : MonoBehaviour
{

    public GameObject leftHalf;
    public GameObject rightHalf;

    public float rotationForce;
    public float sliceForce;

    public AudioClip iceBreakSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

   public void SpawnSlicedParts(Vector2 sliceDirection)
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.playClip(iceBreakSound);

        }
        GameObject leftInst = Instantiate(leftHalf, transform.position, transform.rotation);
        GameObject rightInst = Instantiate(rightHalf, transform.position, transform.rotation);

        Rigidbody2D leftRb = leftInst.GetComponent<Rigidbody2D>();
        Rigidbody2D rightRb = rightInst.GetComponent<Rigidbody2D>();

        if (leftRb && rightRb)
        {
            leftRb.AddForce((-sliceDirection + new Vector2(-0.5f, 0)) * sliceForce, ForceMode2D.Impulse);
            rightRb.AddForce((sliceDirection + new Vector2(0.5f, 0)) * sliceForce, ForceMode2D.Impulse);

            float torque = Random.Range(rotationForce * 0.8f, rotationForce * 1.2f);
            leftRb.AddTorque(torque, ForceMode2D.Impulse);
            rightRb.AddTorque(-torque, ForceMode2D.Impulse);
        }

        Destroy(leftInst, 4f);
        Destroy(rightInst, 4f);
    }
}

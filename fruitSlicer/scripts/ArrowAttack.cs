using UnityEngine;

public class ArrowAttack : MonoBehaviour
{
    public float shootPower = 30f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject.Destroy(this.gameObject, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.up * shootPower * Time.deltaTime);
    }
}

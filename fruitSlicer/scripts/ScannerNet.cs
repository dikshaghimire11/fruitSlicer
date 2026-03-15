using UnityEngine;

public class ScannerNet : MonoBehaviour
{

    public SpriteRenderer spriteRenderer;
    private float opacityValue = 1;
    public float speed = 2f;

    public float minSpeed = 1;
    public float maxSpeed = 5;

    private Color color;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        color = Color.cyan;
    }

    // Update is called once per frame
    void Update()
    {
        float opacityValue = Mathf.PingPong(Time.time * speed, 1f);
        color.a = opacityValue;
        spriteRenderer.color = color;

    }

    public void setSpeed(float speed)
    {
        this.speed = speed;
    }

    public void setColor(Color color)
    {
        this.color = color;
    }
}

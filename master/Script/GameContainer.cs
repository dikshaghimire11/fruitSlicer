using UnityEngine;

public class GameContainer : MonoBehaviour
{
    private Camera mainCamera;

    public float offsetMultiplier=100;

   void Start()
{
    mainCamera = Camera.main;
    transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, 0);
    
    Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(Vector3.zero) * offsetMultiplier;
    Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(mainCamera.rect.width, mainCamera.rect.height)) * offsetMultiplier;

    Debug.Log("Bottom Left: " + bottomLeft);
    Debug.Log("top Right: " + topRight);
    Vector3 screenSize = topRight - bottomLeft;
    float screenRatio = screenSize.x / screenSize.y;
    float desiredRatio = transform.localScale.x / transform.localScale.y;
    Debug.Log("Desired: " + desiredRatio);

    if (screenRatio > desiredRatio)
    {
        float height = screenSize.y;
        transform.localScale = new Vector3(height * desiredRatio, height);
        Debug.Log(transform.localScale);
    }
    else
    {
        float width = screenSize.x;
        transform.localScale = new Vector3(width, width / desiredRatio);
        Debug.Log(transform.localScale);
    }
}
    }

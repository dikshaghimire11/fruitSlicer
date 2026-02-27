using UnityEngine;

public class BowLocationButtons : MonoBehaviour
{
       public static BowLocationButtons instance;

       public Transform leftButton;
       public Transform rightButton;
       public Transform middleButton;

       public Transform hiddenPositionButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
}

using TMPro;
using UnityEngine;

public class BladePowerup : MonoBehaviour
{
     public float effectDuration = 5f;
     public GameObject powerBladePrefab;
     public TextMeshProUGUI bladeTime;

     void Start()
     {
          bladeTime.text = effectDuration.ToString();
     }
}

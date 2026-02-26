using System;
using System.Diagnostics;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class Arrow : MonoBehaviour
{
    private Collider2D collider;

    public float shootPower;


    void Awake()
    {
        collider = transform.GetComponent<Collider2D>();
    }


    public void ShootArrow(Transform releasedArrows)
    {

        StartCoroutine(moveArrow(1f));
        transform.SetParent(releasedArrows);
        collider.isTrigger = false;
        GameObject.Destroy(transform.gameObject, 1.5f);
    }
    IEnumerator moveArrow(float delay)
    {
        float startTime = Time.time;

        while (Time.time < startTime + delay) // run for 2 seconds
        {
            transform.Translate(UnityEngine.Vector3.right * shootPower * Time.deltaTime);

            yield return null; // wait until next frame
        }


    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        CheckHit(collision.collider);
    }

    void CheckHit(Collider2D other)
    {
        if (!other.enabled) return;

        Fruit fruit = other.GetComponent<Fruit>();
        if (fruit != null)
        {
            other.enabled = false;
            fruit.Slice(new UnityEngine.Vector2(0, 0));
            bool isCorrectFruit = false;
            if (ModeManager.Instance.currentMode == GameMode.Infinite)
            {
                ScoreManager.instance?.AddScore(fruit.points);
                // ShowFloatingText("+" + fruit.points, Color.cyan, fruit.transform.position, 0.5f, 0.2f);
            }
            else if (ModeManager.Instance.currentMode == GameMode.JuiceMaking)
            {
                isCorrectFruit = JuiceManager.instance?.CheckFruit(fruit.name) ?? false;
                if (isCorrectFruit)
                {
                    // ShowFloatingText("PERFECT!", Color.cyan, fruit.transform.position, 0.6f, 0.2f);
                }
                else
                {
                    // ShowFloatingText("X", Color.red, fruit.transform.position, 1.5f, 0.2f);
                }


            }

            // HandleCombo(fruit, isCorrectFruit);
            return;
        }

        Bomb bomb = other.GetComponent<Bomb>();
        if (bomb != null)
        {
            other.enabled = false;
            bomb.Explode();
            // ShowFloatingText("BOOM!", Color.red, bomb.transform.position, 0.6f, 0.25f);
            ScoreManager.instance?.HitBomb();
            // comboCount = 0;
            return;
        }

        Ice ice = other.GetComponent<Ice>();
        if (ice != null)
        {
            other.enabled = false;
            ice.Slice(new UnityEngine.Vector2(0, 0));
            // ShowFloatingText("FREEZE!", Color.cyan, ice.transform.position, 0.5f, 0.2f);
        }
    }

    public void ShowFloatingText(string message, Color color, Vector3 position, float size, float yOffset)
    {
        if (floatingTextPrefab == null) return;
        GameObject obj = Instantiate(
            floatingTextPrefab,
            position + new Vector3(0f, yOffset, 0f),
            Quaternion.identity
        );

        obj.transform.localScale = Vector3.one * size;

        FloatingText ft = obj.GetComponent<FloatingText>();
        if (ft != null)
            ft.Setup(message, color);
    }
}
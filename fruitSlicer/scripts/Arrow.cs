using System;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class Arrow : MonoBehaviour
{
    private Collider2D collider;

    public float shootPower;
    public GameObject floatingTextPrefab;
    public float maxComboDelay = 0.2f;
    private int comboCount;
    private float lastHitTime;


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
            if (ModeManager.Instance.currentMode == GameMode.Archery)
            {
                ScoreManager.instance?.AddScore(fruit.points);
                ShowFloatingText("+" + fruit.points, Color.cyan, fruit.transform.position, 0.5f, 0.2f);
            }
            HandleCombo(fruit, isCorrectFruit);
            return;
        }
        SpecialObject special = other.GetComponent<SpecialObject>();
        if (special != null)
        {
            special.Slice(new UnityEngine.Vector2(0, 0));
            return;
        }

        // Bomb bomb = other.GetComponent<Bomb>();
        // if (bomb != null)
        // {
        //     other.enabled = false;
        //     bomb.Explode();
        //     ShowFloatingText("BOOM!", Color.red, bomb.transform.position, 0.6f, 0.25f);
        //     ScoreManager.instance?.HitBomb();
        //     comboCount = 0;
        //     return;
        // }

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

    void HandleCombo(Fruit fruit, bool isCorrectFruit)
    {
        if (Time.time - lastHitTime > maxComboDelay)
            comboCount = 0;

        lastHitTime = Time.time;


        comboCount++;


        if (comboCount < 2) return;

        float textSize = 0.6f;
        int bonus;

        if (ModeManager.Instance.currentMode == GameMode.Archery)
        {
            bonus = comboCount * 5;
            ScoreManager.instance?.AddScore(bonus);
            ShowFloatingText("COMBO", Color.yellow, fruit.transform.position + new Vector3(-0.2f, 0f, 0f), textSize, 0f);
            ShowFloatingText("+" + bonus, Color.yellow, fruit.transform.position + new Vector3(0.2f, 0f, 0f), textSize, 0f);
        }

        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayComboSound(comboCount);
        }
    }

}
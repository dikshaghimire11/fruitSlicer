using System;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class BowController : MonoBehaviour
{
    public Transform leftPoint;
    public Transform rightPoint;
    public Transform arrow;

    public Transform arrowPoint;

    public LineRenderer line;


    private Transform releasedArrows;

    public float coolDownTimer;

    // private Rigidbody2D arrowRb;
    private Vector2 startPosition;
    private bool isDragging = false;

    private Camera mainCamera;

    public GameObject arrowPrefab;

    void Start()
    {
        mainCamera = Camera.main;
        // arrowRb = arrow.GetComponent<Rigidbody2D>();
        startPosition = arrow.position;
        releasedArrows = GameObject.Find("ReleasedArrows").transform;
        // arrowRb.isKinematic = true;
        ReleaseRubber();
    }

    public void setdefaultArrowPositon()
    {
        // arrow.transform.localPosition = new Vector2(210, 0);
        ReleaseRubber();
    }

    public void setStreachArrowPosition()
    {
        arrow.transform.localPosition = new Vector2(-35, 0);
        StreachRubber();
    }

    void Update()
    {

        updateBowRotation();
        if (IsInputDown())
        {
            isDragging = true;
        }
        if (IsInputUp())
        {
            isDragging = false;
            Shoot();
        }

        if (isDragging && arrow != null)
        {
            setStreachArrowPosition();
        }
        else
        {
            setdefaultArrowPositon();
        }



    }

    void StreachRubber()
    {
        {
            if (line.positionCount != 3)
                line.positionCount = 3;

            line.SetPosition(0, new Vector3(leftPoint.transform.position.x, leftPoint.transform.position.y, -14));
            line.SetPosition(1, new Vector3(arrowPoint.transform.position.x, arrowPoint.transform.position.y, -14));
            line.SetPosition(2, new Vector3(rightPoint.transform.position.x, rightPoint.transform.position.y, -14));
        }
    }

    void ReleaseRubber()
    {
        {
            if (line.positionCount != 2)
                line.positionCount = 2;

            line.SetPosition(0, new Vector3(leftPoint.transform.position.x, leftPoint.transform.position.y, -14));
            line.SetPosition(1, new Vector3(rightPoint.transform.position.x, rightPoint.transform.position.y, -14));
        }
    }

    // void Shoot()
    // {
    //     arrowRb.isKinematic = false;
    //     Vector2 force = (startPosition - (Vector2)arrow.position) * shootPower;
    //     arrowRb.AddForce(force, ForceMode2D.Impulse);
    // }

    public void Shoot()
    {
        if (arrow == null)
        {
            return;
        }
        Arrow arrowScript = arrow.GetComponent<Arrow>();
        arrowScript.ShootArrow(releasedArrows);
        arrow = null;
        arrowPoint = null;
        StartCoroutine(AttachNewArrow(coolDownTimer));

    }

    IEnumerator AttachNewArrow(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        arrow = Instantiate(arrowPrefab, transform).transform;
        arrow.transform.localPosition = new Vector2(210, 0);
        arrowPoint = arrow.GetChild(0);


    }


    bool IsInputDown()
    {
        return Input.GetMouseButtonDown(0) ||
              (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
    }

    bool IsInputUp()
    {
        return Input.GetMouseButtonUp(0) ||
              (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended);
    }

    Vector3 GetInputPosition()
    {
        Vector3 screenPos = Input.touchCount > 0
            ? (Vector3)Input.GetTouch(0).position
            : Input.mousePosition;

        screenPos.z = Mathf.Abs(mainCamera.transform.position.z);
        return mainCamera.ScreenToWorldPoint(screenPos);
    }



    public void updateBowRotation()
    {
        Vector3 mousePos = GetInputPosition();
        mousePos.z = 0f;

        Vector3 direction = transform.position - mousePos;

        transform.right = direction;
    }






}
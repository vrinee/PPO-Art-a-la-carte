using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Toastable : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;

    private SpriteRenderer spriteRenderer;
    private bool isColliding = false;
    private bool isCoroutineRunning = false;

    private bool isDragging = false;

    private bool isCooked = false;

    private bool isBurnt = false;

    private string trashTag = "Lixo";

    public string targetTag = "Prato"; 
    public int cookingTime = 5;

    private int currentState = 0;

    public Sprite[] states;

    public Transform spawnPoint;

    public Transform selfTransform;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        selfTransform = GetComponent<Transform>();
        if (states.Length > 0)
        {
            spriteRenderer.sprite = states[0];
        }
    }

    void Update()
    {
        
    }
    void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        isDragging = false;
    }

    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;
        isDragging = true;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == targetTag && isDragging )
        {
            isColliding = true;

            
            if (!isCoroutineRunning)
            {
                StartCoroutine(CheckIfStillColliding());
            }
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.tag == targetTag)
        {
            isColliding = false;
        }
    }

    IEnumerator CheckIfStillColliding()
    {
        isCoroutineRunning = true; 
        int timeBtwStates = cookingTime / states.Length;
        while (isColliding && currentState < states.Length)
        {
            spriteRenderer.sprite = states[currentState];
            currentState++;
            if(currentState == states.Length - 1 )
            {
                isCooked = true;
                Debug.Log("Food is cooked!");
            }
            if(currentState == states.Length)
            {
                isCooked = false;
                isBurnt = true;
                Debug.Log("Food is burnt!");
            }
            yield return new WaitForSeconds(timeBtwStates);
        }



       /*  yield return new WaitForSeconds(cookingTime);

        if (isColliding)
        {
            Debug.Log("Still colliding with " + target.name + " after " + cookingTime + " seconds.");
        }
        */
        isCoroutineRunning = false;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == trashTag)
        {
            isBurnt = false;
            isCooked = false;
            currentState = 0;
            spriteRenderer.sprite = states[0];
            selfTransform.position = spawnPoint.position;

        }
    }
}
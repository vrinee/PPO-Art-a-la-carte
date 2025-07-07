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

    private string finishTag = "Final";

    public string targetTag = "Prato"; 

    public string foodTag = "Hamburguer";
    public int cookingTime = 5;

    private int currentState = 0;

    public Sprite[] states;

    public Transform spawnPoint;

    public Transform selfTransform;

    [Header("Drag Constraints")]
    public LayerMask obstacleLayerMask = -1; // What layers to check for obstacles
    public float checkRadius = 0.5f; // Radius for collision checking
    
    private Camera mainCamera;
    private Vector3 screenBounds;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        selfTransform = GetComponent<Transform>();
        mainCamera = Camera.main;
        CalculateScreenBounds();
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
        
        // Apply constraints
        curPosition = ApplyDragConstraints(curPosition);
        
        transform.position = curPosition;
        isDragging = true;
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(!isDragging)
        {
            if (col.gameObject.tag == targetTag)
            {
                isColliding = true;

                
                if (!isCoroutineRunning)
                {
                    StartCoroutine(CheckIfStillColliding());
                }
            }
            else if (col.gameObject.tag == finishTag)
            {
                if (isCooked && !isBurnt)
                {
                    CookingBehaviour cookingBehaviour = FindFirstObjectByType<CookingBehaviour>();
                    if (cookingBehaviour != null)
                    {
                        cookingBehaviour.complete(foodTag);
                        Debug.Log("Called complete function in CookingBehaviour with tag: " + foodTag);
                    }
                    else
                    {
                        Debug.LogError("CookingBehaviour not found in the scene!");
                    }
                }
                else if (isBurnt)
                {
                    Debug.Log("Food is burnt!");
                }
                else
                {
                    Debug.Log("Food is not cooked yet!");
                }
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
    
    void CalculateScreenBounds()
    {
        if (mainCamera == null) return;
        
        // Calculate screen bounds in world coordinates
        Vector3 screenBottomLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, screenPoint.z));
        Vector3 screenTopRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, screenPoint.z));
        
        screenBounds = new Vector3(
            screenTopRight.x - screenBottomLeft.x, // width
            screenTopRight.y - screenBottomLeft.y, // height
            0
        );
    }
    
    Vector3 ApplyDragConstraints(Vector3 targetPosition)
    {
        Vector3 constrainedPosition = targetPosition;
        
        // 1. Check screen boundaries
        constrainedPosition = ClampToScreenBounds(constrainedPosition);
        
        // 2. Check for collisions with other objects
        constrainedPosition = AvoidCollisions(constrainedPosition);
        
        return constrainedPosition;
    }
    
    Vector3 ClampToScreenBounds(Vector3 position)
    {
        if (mainCamera == null) return position;
        
        // Get current object bounds
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null) return position;
        
        Vector3 objectSize = renderer.bounds.size;
        
        // Calculate screen boundaries in world coordinates
        Vector3 screenBottomLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, mainCamera.WorldToScreenPoint(position).z));
        Vector3 screenTopRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.WorldToScreenPoint(position).z));
        
        // Clamp position to keep object fully within screen
        position.x = Mathf.Clamp(position.x, 
            screenBottomLeft.x + objectSize.x / 2, 
            screenTopRight.x - objectSize.x / 2);
        position.y = Mathf.Clamp(position.y, 
            screenBottomLeft.y + objectSize.y / 2, 
            screenTopRight.y - objectSize.y / 2);
        
        return position;
    }
    
    Vector3 AvoidCollisions(Vector3 targetPosition)
    {
        // Check if the target position would cause a collision
        Collider2D hit = Physics2D.OverlapCircle(targetPosition, checkRadius, obstacleLayerMask);
        
        // If there's a collision and it's not this object, don't move there
        if (hit != null && hit.gameObject != gameObject)
        {
            // Return current position (don't move)
            return transform.position;
        }
        
        return targetPosition;
    }
}
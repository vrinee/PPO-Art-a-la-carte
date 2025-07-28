using UnityEngine;

public class BowlItem : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;

    private bool isDragging = false;

    [Header("Drag Constraints")]
    public LayerMask obstacleLayerMask = -1; // What layers to check for obstacles
    public float checkRadius = 0.5f; // Radius for collision checking

    public string selfTag = "BowlItem"; // Tag for this item

    private bool isDisplaced = false;
    
    private Camera mainCamera;
    private Vector3 screenBounds;

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
        isDisplaced = true;
    }

    void OnMouseUp()
    {
        isDragging = false;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDragging) return;
        

        if (other.CompareTag("Prato"))
        {

            Bowl bowl = FindFirstObjectByType<Bowl>();
            bowl.enterItem(selfTag);
            Destroy(gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
        CalculateScreenBounds();
    }
    
    void CalculateScreenBounds()
    {
        if (mainCamera == null) return;
        
        // Calculate screen bounds in world coordinates
        Vector3 screenBottomLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, screenPoint.z));
        Vector3 screenTopRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, screenPoint.z));
        
        screenBounds = new Vector3(
            screenTopRight.x - screenBottomLeft.x,
            screenTopRight.y - screenBottomLeft.y, 
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

    // Update is called once per frame
    void Update()
    {
        if (gameObject.transform.position.x < 68 && !isDisplaced)
        {
            float x = gameObject.transform.position.x + 0.01f;
            gameObject.transform.position = new Vector3(x, gameObject.transform.position.y, gameObject.transform.position.z);
        }else if (!isDisplaced && gameObject.transform.position.x >= 68)
        {
            isDisplaced = true;
        }

    }
}

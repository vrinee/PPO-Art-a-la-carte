using UnityEngine;

public class Washable : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;

    public bool isDragging = false;
    
    public bool dragable = true;

    private bool isWashed = false;

    public string sinkTag = "Torneira";
    public string targetTag = "Prato";

    public Sprite washedSprite;

    private SpriteRenderer spriteRenderer;

    private CookingBehaviour cookingBehaviour;

    [Header("Drag Constraints")]
    public LayerMask obstacleLayerMask = -1; // What layers to check for obstacles
    public float checkRadius = 0.5f; // Radius for collision checking
    
    private Camera mainCamera;
    private Vector3 screenBounds;

    void OnMouseDown()
    {
        if (!dragable) return;
        screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    void OnMouseDrag()
    {
        if (!dragable) return;
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        
        // Apply constraints
        curPosition = ApplyDragConstraints(curPosition);
        
        transform.position = curPosition;
        isDragging = true;
    }

    void OnMouseUp()
    {
        if (!dragable) return;
        if (isDragging)
        {
            isDragging = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(sinkTag) && !isWashed)
        {
            spriteRenderer.sprite = washedSprite;
            Debug.Log("Lavado um alface, ou oq quer que seja");
            isWashed = true;
        }
        if (!isWashed) return;
        if (isDragging) return;
        if (other.CompareTag(targetTag))
        {
            cookingBehaviour.WashableDone();
            Destroy(gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        cookingBehaviour = FindAnyObjectByType<CookingBehaviour>();
        mainCamera = Camera.main;
        CalculateScreenBounds();
        
        // Disable BowlItem component if not washed initially
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

    // Update is called once per frame
    void Update()
    {
        
    }
}

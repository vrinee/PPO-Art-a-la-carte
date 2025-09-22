using UnityEngine;

public class BowlItem : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;

    private bool isDragging = false;

    [Header("Drag Constraints")]
    public LayerMask obstacleLayerMask = -1; // What layers to check for obstacles
    public float checkRadius = 0.5f; // Radius for collision checking

    [Header("Item Properties")]
    public string selfTag = "BowlItem"; // Tag for this item
    public bool isDraggingEnabled = true; // Toggle to enable/disable dragging
    public string targetBowlName = "Bowl"; // Name of the specific bowl to find

    private bool isDisplaced = false;
    
    private Camera mainCamera;
    private Vector3 screenBounds;

    void OnMouseDown()
    {
        if (!isDraggingEnabled) return; // Exit if dragging is disabled
        
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        isDragging = false;
    }

    void OnMouseDrag()
    {
        if (!isDraggingEnabled) return; // Exit if dragging is disabled
        
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
        if (!isDraggingEnabled) return; // Exit if dragging is disabled
        
        isDragging = false;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        //if (isDragging) return;
        
        // Check if any other script on this object is dragging
        //if (IsAnyComponentDragging()) return;

        if (other.CompareTag("Prato"))
        {
            Bowl bowl = FindBowlByName(targetBowlName);
            
            // Check if the item is still washed (if it has a Washable component)
            Washable washable = GetComponent<Washable>();
            bool isWashed = true; // Default to true if no Washable component
            if (washable != null)
            {
                // Use reflection to check the private isWashed field
                System.Type washableType = washable.GetType();
                System.Reflection.FieldInfo isWashedField = washableType.GetField("isWashed", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (isWashedField != null)
                {
                    isWashed = (bool)isWashedField.GetValue(washable);
                }
            }
            
            if (bowl != null && isWashed)
            {
                bowl.enterItem(selfTag);
                if (!isDraggingEnabled) return;
                Destroy(gameObject);
            }
            else if (bowl != null && !isWashed)
            {
                Debug.LogWarning("Item must be washed before entering the bowl!");
            }
            else
            {
                Debug.LogError("Bowl with name '" + targetBowlName + "' not found in the scene!");
            }
        }
    }
    
    bool IsAnyComponentDragging()
    {
        // Get all MonoBehaviour components on this GameObject
        MonoBehaviour[] components = GetComponents<MonoBehaviour>();
        
        foreach (MonoBehaviour component in components)
        {
            // Skip this script to avoid checking itself
            if (component == this) continue;
            
            // Use reflection to check for isDragging field
            System.Type componentType = component.GetType();
            System.Reflection.FieldInfo isDraggingField = componentType.GetField("isDragging", 
                System.Reflection.BindingFlags.Public | 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);
            
            if (isDraggingField != null && isDraggingField.FieldType == typeof(bool))
            {
                bool otherIsDragging = (bool)isDraggingField.GetValue(component);
                if (otherIsDragging)
                {
                    Debug.Log($"Component {componentType.Name} is dragging, skipping trigger.");
                    return true;
                }
            }
        }
        
        return false;
    }
    
    Bowl FindBowlByName(string bowlName)
    {
        Bowl[] allBowls = FindObjectsByType<Bowl>(FindObjectsSortMode.None);
        
        foreach (Bowl bowl in allBowls)
        {
            if (bowl.gameObject.name == bowlName)
            {
                return bowl;
            }
        }
        
        return null; // Return null if no bowl with the specified name is found
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
        if(!isDraggingEnabled) return; // Exit if dragging is disabled
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
        if (gameObject.transform.position.x < 68 && !isDisplaced && isDraggingEnabled)
        {
            float x = gameObject.transform.position.x + 0.01f;
            gameObject.transform.position = new Vector3(x, gameObject.transform.position.y, gameObject.transform.position.z);
        }else if (!isDisplaced && gameObject.transform.position.x >= 68)
        {
            isDisplaced = true;
        }

    }
}

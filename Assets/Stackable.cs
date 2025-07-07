using UnityEngine;


public class Stackable : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;

    public string stackableName = "StackableObject";
    public string targetName = "Stackable";

    public bool hasAbove = true;

    private bool isDragging = false;

    public bool isStacked = false;

    [Header("Drag Constraints")]
    public LayerMask obstacleLayerMask = -1; // What layers to check for obstacles
    public float checkRadius = 0.5f; // Radius for collision checking
    
    private Camera mainCamera;
    private Vector3 screenBounds;

    void Start()
    {
        mainCamera = Camera.main;
        CalculateScreenBounds();
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
    void OnCollisionEnter2D (Collision2D collision)
    {
        if(!isDragging){
            if (collision.gameObject.CompareTag("Stackable") && collision.gameObject.GetComponent<Stackable>().isStacked)
            {
                if (collision.transform.position.y < transform.position.y){
                    if(collision.gameObject.GetComponent<Stackable>().stackableName == targetName){
                        transform.position = collision.gameObject.transform.position + new Vector3(0, 0.5f, 0);
                        isStacked = true;
                        if(!hasAbove){
                            Debug.Log("Stacked on top of " + collision.gameObject.name);
                            CookingBehaviour cookingBehaviour = FindFirstObjectByType<CookingBehaviour>();
                            cookingBehaviour.EndRecipe();

                        }
                    }
                }
                
            }else if (collision.gameObject.CompareTag("Prato") && targetName == "Prato"){
                if (collision.transform.position.y < transform.position.y){
                    transform.position = collision.gameObject.transform.position + new Vector3(0, 0.5f, 0);
                    isStacked = true;
                }
            }
        }
    }

    void OnCollisionExit2D (Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Stackable") || collision.gameObject.CompareTag("Prato")){
            isStacked = false;
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

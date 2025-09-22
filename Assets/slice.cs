using UnityEngine;

public class slice : MonoBehaviour
{

    private Vector3 screenPoint;
    private Vector3 offset;

    public bool dragable = false;

    public string targetTag = "Prato";

    private bool isDragging = false;

    private CookingBehaviour cookingBehaviour;

    [Header("Drag Constraints")]
    public LayerMask obstacleLayerMask = -1; 
    public float checkRadius = 0.5f; 
    
    private Camera mainCamera;
    private Vector3 screenBounds;

    void OnMouseDown()
    {
        if (!dragable) return;
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    void OnMouseDrag()
    {
        if (!dragable) return;
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        

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
        if (!dragable) return;
        //if (isDragging) return;
        if (other.CompareTag(targetTag))
        {
            cookingBehaviour.FinnishSlice();
            Destroy(gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cookingBehaviour = FindAnyObjectByType<CookingBehaviour>();
        mainCamera = Camera.main;
        CalculateScreenBounds();
    }
    
    void CalculateScreenBounds()
    {
        if (mainCamera == null) return;
        
  
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
        

        constrainedPosition = ClampToScreenBounds(constrainedPosition);
        

        constrainedPosition = AvoidCollisions(constrainedPosition);
        
        return constrainedPosition;
    }
    
    Vector3 ClampToScreenBounds(Vector3 position)
    {
        if (mainCamera == null) return position;
        

        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null) return position;
        
        Vector3 objectSize = renderer.bounds.size;
        

        Vector3 screenBottomLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, mainCamera.WorldToScreenPoint(position).z));
        Vector3 screenTopRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.WorldToScreenPoint(position).z));
        

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

        Collider2D hit = Physics2D.OverlapCircle(targetPosition, checkRadius, obstacleLayerMask);
        

        if (hit != null && hit.gameObject != gameObject)
        {
        
            return transform.position;
        }
        
        return targetPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

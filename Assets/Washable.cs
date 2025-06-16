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
        if (other.CompareTag(targetTag) && other.transform.position.y < transform.position.y )
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

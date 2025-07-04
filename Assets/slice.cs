using UnityEngine;

public class slice : MonoBehaviour
{

    private Vector3 screenPoint;
    private Vector3 offset;

    public bool dragable = false;

    public string targetTag = "Prato";

    private bool isDragging = false;

    private CookingBehaviour cookingBehaviour;

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
        if (isDragging) return;
        if (other.CompareTag(targetTag) && other.transform.position.y < transform.position.y)
        {
            cookingBehaviour.FinnishSlice();
            Destroy(gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cookingBehaviour = FindAnyObjectByType<CookingBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

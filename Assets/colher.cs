using UnityEngine;

public class colher : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;
    private bool isDragging = false;
    public bool isDraggingEnabled = true;
    private Rigidbody2D rb;

    private panela panelaScript;

    void OnMouseDown()
    {
        if (!isDraggingEnabled) return;
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        isDragging = false;
    }

    void OnMouseDrag()
    {
        if (!isDraggingEnabled) return;
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        
        // Use Rigidbody2D if available, otherwise use transform
        if (rb != null)
        {
            rb.MovePosition(curPosition);
        }
        else
        {
            transform.position = curPosition;
        }
        
        isDragging = true;
    }

    void OnMouseUp()
    {
        if (!isDraggingEnabled) return;
        isDragging = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PanelaLeft"))
        {
            panelaScript.StirTrigger("left");
        }
        else if (other.CompareTag("PanelaRight"))
        {
            panelaScript.StirTrigger("right");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get Rigidbody2D if it exists
        rb = GetComponent<Rigidbody2D>();
        panelaScript = FindObjectOfType<panela>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}

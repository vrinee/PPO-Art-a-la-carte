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
}

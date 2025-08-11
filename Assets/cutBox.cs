using UnityEngine;

public class cutBox : MonoBehaviour
{

    private sliceable parentSliceable;
    private SliceableFrango parentSliceableFrango;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        parentSliceable = GetComponentInParent<sliceable>();
        parentSliceableFrango = GetComponentInParent<SliceableFrango>();
    }

    void OnMouseDown()
    {
        if (parentSliceable != null)
        {
            parentSliceable.Cutted(gameObject);
            Destroy(gameObject);
        }
        else if (parentSliceableFrango != null)
        {
            parentSliceableFrango.Cutted(gameObject);
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("Parent sliceable not found for cutBox!");
        }
    }
}

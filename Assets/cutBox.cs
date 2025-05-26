using UnityEngine;

public class cutBox : MonoBehaviour
{

    private sliceable parentSliceable;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        parentSliceable = GetComponentInParent<sliceable>();
    }

    void OnMouseDown()
    {
        if (parentSliceable != null)
        {
            parentSliceable.Cutted(gameObject);
        }
        else
        {
            Debug.LogWarning("Parent sliceable not found for cutBox!");
        }
    }
}

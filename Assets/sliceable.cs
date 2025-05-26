using UnityEngine;

public class sliceable : MonoBehaviour
{

    [SerializeField] private GameObject slice1and2hitbox;
    [SerializeField] private GameObject slice2and3hitbox;

    [SerializeField] private GameObject unslicedObject;
    [SerializeField] private GameObject slicedObject;

    [SerializeField] private GameObject slice1;

    [SerializeField] private GameObject slice2;

    [SerializeField] private GameObject slice3;
    
    private bool isSliced = false;

    private bool isSlice1Cut = false;

    private bool isSlice3Cut = false;

    public void Cutted(GameObject cutBox)
    {
        if (!isSliced)
        {
            isSliced = true;
            Slice();
        }
        if (cutBox == slice1and2hitbox)
        {
            var sliceScript = slice1.GetComponent<slice>();
            sliceScript.dragable = true;
            sliceScript.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            isSlice1Cut = true;
        }
        else if (cutBox == slice2and3hitbox)
        {
            var sliceScript = slice3.GetComponent<slice>();
            sliceScript.dragable = true;
            sliceScript.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            isSlice3Cut = true; 
        }
        if (isSlice1Cut && isSlice3Cut)
        {
            var sliceScript = slice2.GetComponent<slice>();
            sliceScript.dragable = true;
            sliceScript.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }
    }
    void Slice()
    {
        unslicedObject.SetActive(false);
        slicedObject.SetActive(true);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

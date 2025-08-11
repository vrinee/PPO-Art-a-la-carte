using UnityEngine;

public class SliceableFrango : MonoBehaviour
{

    [SerializeField] private GameObject slice1and2hitbox;
    [SerializeField] private GameObject slice2and3hitbox;
    [SerializeField] private GameObject slice3and4hitbox;
    [SerializeField] private GameObject slice4and5hitbox;
    [SerializeField] private GameObject slice5and6hitbox;

    [SerializeField] private GameObject unslicedObject;
    [SerializeField] private GameObject slicedObject;

    [SerializeField] private GameObject slice1;

    [SerializeField] private GameObject slice2;

    [SerializeField] private GameObject slice3;

    [SerializeField] private GameObject slice4;

    [SerializeField] private GameObject slice5;

    [SerializeField] private GameObject slice6;
    
    private bool isSliced = false;

    private bool isSlice1Cut = false;

    private bool isSlice2Cut = false;

    private bool isSlice3Cut = false;

    private bool isSlice4Cut = false;

    private bool isSlice5Cut = false;

    private bool isSlice6Cut = false;

    public void Cutted(GameObject cutBox)
    {
        if (!isSliced)
        {
            isSliced = true;
            Slice();
        }
        
        // Handle cutting between slice 1 and slice 2
        if (cutBox == slice1and2hitbox)
        {
            // Enable slice 1 (left of cut)
            if (!isSlice1Cut)
            {
                var sliceScript1 = slice1.GetComponent<slice>();
                sliceScript1.dragable = true;
                sliceScript1.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                isSlice1Cut = true;
            }
            
            // Enable slice 2 (right of cut)
            if (!isSlice2Cut)
            {
                var sliceScript2 = slice2.GetComponent<slice>();
                sliceScript2.dragable = true;
                sliceScript2.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                isSlice2Cut = true;
            }
        }
        // Handle cutting between slice 2 and slice 3
        else if (cutBox == slice2and3hitbox)
        {
            // Enable slice 2 (left of cut) if not already enabled
            if (!isSlice2Cut)
            {
                var sliceScript2 = slice2.GetComponent<slice>();
                sliceScript2.dragable = true;
                sliceScript2.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                isSlice2Cut = true;
            }
            
            // Enable slice 3 (right of cut)
            if (!isSlice3Cut)
            {
                var sliceScript3 = slice3.GetComponent<slice>();
                sliceScript3.dragable = true;
                sliceScript3.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                isSlice3Cut = true; 
            }
        }
        // Handle cutting between slice 3 and slice 4
        else if (cutBox == slice3and4hitbox)
        {
            // Enable slice 3 (left of cut) if not already enabled
            if (!isSlice3Cut)
            {
                var sliceScript3 = slice3.GetComponent<slice>();
                sliceScript3.dragable = true;
                sliceScript3.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                isSlice3Cut = true;
            }
            
            // Enable slice 4 (right of cut)
            if (!isSlice4Cut)
            {
                var sliceScript4 = slice4.GetComponent<slice>();
                sliceScript4.dragable = true;
                sliceScript4.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                isSlice4Cut = true;
            }
        }
        // Handle cutting between slice 4 and slice 5
        else if (cutBox == slice4and5hitbox)
        {
            // Enable slice 4 (left of cut) if not already enabled
            if (!isSlice4Cut)
            {
                var sliceScript4 = slice4.GetComponent<slice>();
                sliceScript4.dragable = true;
                sliceScript4.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                isSlice4Cut = true;
            }
            
            // Enable slice 5 (right of cut)
            if (!isSlice5Cut)
            {
                var sliceScript5 = slice5.GetComponent<slice>();
                sliceScript5.dragable = true;
                sliceScript5.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                isSlice5Cut = true;
            }
        }
        // Handle cutting between slice 5 and slice 6
        else if (cutBox == slice5and6hitbox)
        {
            // Enable slice 5 (left of cut) if not already enabled
            if (!isSlice5Cut)
            {
                var sliceScript5 = slice5.GetComponent<slice>();
                sliceScript5.dragable = true;
                sliceScript5.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                isSlice5Cut = true;
            }
            
            // Enable slice 6 (right of cut)
            if (!isSlice6Cut)
            {
                var sliceScript6 = slice6.GetComponent<slice>();
                sliceScript6.dragable = true;
                sliceScript6.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                isSlice6Cut = true;
            }
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

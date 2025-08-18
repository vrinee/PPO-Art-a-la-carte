using UnityEngine;

public class panela : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int stirsPerPhase = 5;

    public int currentPhase = 0;

    public Sprite[] phaseSprites;

    [Header("Side to Side Sprites (each two belong to a phase)")]
    public Sprite[] sideToSideSprites;

    public Sprite[] arrowSprites;

    private int currentStirs = 0;

    private SpriteRenderer spriteRenderer;

    private SpriteRenderer arrow;

    private CookingBehaviour cookingBehaviour;
    
    private string lastSide = ""; // Track the last side that was triggered

    public void StirTrigger(string side)
    {
        Debug.Log("StirTrigger called with side: " + side + ", lastSide: " + lastSide);
        
        // If this is the first trigger or we're switching sides
        if (lastSide == "" || lastSide != side)
        {
            if (lastSide == "")
            {
                // First trigger - just set the arrow and remember the side
                if (side == "left")
                {
                    arrow.sprite = arrowSprites[0];
                }
                else
                {
                    arrow.sprite = arrowSprites[1];
                }
                lastSide = side;
            }
            else
            {
                // We switched sides - this completes a stir
                if (side == "left" && lastSide == "right")
                {
                    changeStir("rightToLeft");
                }
                else if (side == "right" && lastSide == "left")
                {
                    changeStir("leftToRight");
                }
                lastSide = side;
            }
        }
    }

    void changeStir(string direction)
    {
        currentStirs++;
        
        // Change arrow sprite each revolution (stir)
        if (arrowSprites.Length > 0)
        {
            if (arrow.sprite == arrowSprites[0])
            {
                arrow.sprite = arrowSprites[1];
            }
            else
            {
                arrow.sprite = arrowSprites[0];
            }
        }
        
        if (direction == "rightToLeft")
        {
            Debug.Log("Changing stir from right to left");
            spriteRenderer.sprite = sideToSideSprites[currentPhase * 2 + 1];
        }
        else if (direction == "leftToRight")
        {
            Debug.Log("Changing stir from left to right");
            spriteRenderer.sprite = sideToSideSprites[currentPhase * 2];
        }
        if (currentStirs >= stirsPerPhase)
        {
            currentPhase++;
            currentStirs = 0;
            if (currentPhase < phaseSprites.Length)
            {
                spriteRenderer.sprite = phaseSprites[currentPhase];
            }
            if (currentPhase >= phaseSprites.Length)
            {
                // Process completed - delete colher and complete cooking
                GameObject colher = GameObject.FindWithTag("colher");
                if (colher != null)
                {
                    Destroy(colher);
                }
                cookingBehaviour.complete("Frango");
                return;
            }
        }
    }
    void Start()
    {
        spriteRenderer = GameObject.Find("Conteudo").GetComponent<SpriteRenderer>();
        arrow = GameObject.Find("Arrow").GetComponent<SpriteRenderer>();
        cookingBehaviour = FindObjectOfType<CookingBehaviour>();
        spriteRenderer.sprite = phaseSprites[currentPhase];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

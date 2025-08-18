using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PDollarGestureRecognizer;
public class Book : MonoBehaviour
{
    private bool isOpen = false;
    private bool isRunning = false;
    private Animator animator;

    private string[] recipeNames;

    private string[][] recipesDescription;

    private string[] recipesGest;

    private SellingGame sellingGame;

    public GameObject recipeStripPrefab;
    public GameObject recipeStripParent;
    public void ChangeState()
    {
        if (!isRunning)
        {
            isRunning = true;
            StartCoroutine(OpenClose());
        }
    }

    IEnumerator OpenClose()
    {
        if (isOpen)
        {
            animator.SetTrigger("Exit");
            isOpen = false;
        }
        else
        {
            animator.SetTrigger("Enter");
            isOpen = true;
        }
        yield return new WaitForSeconds(1f);
        isRunning = false;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        sellingGame = GameObject.Find("Main Camera").GetComponent<SellingGame>();
        if (sellingGame == null)
        {
            Debug.LogError("SellingGame component not found on Main Camera!");
            return;
        }
        recipeNames = sellingGame.GetRecipes();
        if (recipeNames == null || recipeNames.Length == 0)
        {
            Debug.LogError("No recipes found!");
            return;
        }
        recipesDescription = sellingGame.GetRecipeDescription();
        recipesGest = sellingGame.GetRecipesGest();

        if (recipeStripPrefab == null)
        {
            Debug.LogError("recipeStripPrefab is not assigned!");
            
        }
        if (recipeStripParent == null)
        {
            Debug.LogError("recipeStripParent is not assigned!");
            
        }
        Debug.Log(recipeNames.Length);
        for (int i = 0; i < recipeNames.Length; i++)
        {
            GameObject recipeStrip = Instantiate(recipeStripPrefab, recipeStripParent.transform);
            if (recipeStrip == null)
                Debug.LogError("RecipeStrip prefab not found!");
            var stripComponent = recipeStrip.GetComponent<RecipeStrip>();
            if (stripComponent == null)
                Debug.LogError("RecipeStrip component not found on prefab!");
            
            // Find the correct index for this recipe in the full arrays
            int recipeIndex = FindRecipeIndex(recipeNames[i]);
            if (recipeIndex != -1)
            {
                stripComponent.SetRecipeName(recipeNames[i]);
                stripComponent.SetRecipeDescription(recipesDescription[recipeIndex]);
                stripComponent.SetRecipeGest(recipesGest[recipeIndex]);
            }
            else
            {
                Debug.LogError("Recipe index not found for: " + recipeNames[i]);
            }
        }
    }

    int FindRecipeIndex(string recipeName)
    {
        // Get all recipe names (not just unlocked ones) to find the correct index
        string[] allRecipeNames = sellingGame.GetAllRecipeNames();
        if (allRecipeNames == null) return -1;
        
        for (int i = 0; i < allRecipeNames.Length; i++)
        {
            if (allRecipeNames[i] == recipeName)
            {
                return i;
            }
        }
        return -1; // Recipe not found
    }
}

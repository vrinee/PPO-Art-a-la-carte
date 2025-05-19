using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeStrip : MonoBehaviour
{

    private string recipeName;
    private string[] recipeDescription;

    private string recipeGest;

    private TMP_Text recipeNameText;
    private TMP_Text recipeDescriptionText;

    private GameObject recipeGestImage; //needs to be done(no current sprite)
    private GameObject recipeImage; //needs to be done(no current sprite)
    
    public void SetRecipeName(string recipeName)
    {
        this.recipeName = recipeName;
    }

    public void SetRecipeDescription(string[] recipeDescription)
    {
        this.recipeDescription = recipeDescription;
    }
    public void SetRecipeGest(string recipeGest)
    {
        this.recipeGest = recipeGest;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        recipeNameText = GameObject.Find("Nome").GetComponent<TMP_Text>();
        recipeDescriptionText = GameObject.Find("Descricao").GetComponent<TMP_Text>();
        recipeGestImage = GameObject.Find("Gesto");
        recipeImage = GameObject.Find("Imagem");

        recipeNameText.text = recipeName;
        recipeDescriptionText.text = "É " + recipeDescription[0] + ", " + recipeDescription[1] + " e " + recipeDescription[2] + ".";
        Debug.Log(recipeGest);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

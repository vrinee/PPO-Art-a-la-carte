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
        // Find components within this RecipeStrip's hierarchy, not globally
        recipeNameText = transform.Find("Nome").GetComponent<TMP_Text>();
        recipeDescriptionText = transform.Find("Descricao").GetComponent<TMP_Text>();
        recipeGestImage = transform.Find("Gesto").gameObject;
        recipeImage = transform.Find("Imagem").gameObject;
        
        if (recipeGestImage == null)
        {
            Debug.LogError("RecipeGestImage GameObject not found in " + gameObject.name + "!");
            return;
        }
        
        if (recipeNameText == null)
        {
            Debug.LogError("RecipeNameText not found in " + gameObject.name + "!");
            return;
        }
        
        recipeNameText.text = recipeName;
        recipeDescriptionText.text = "É " + recipeDescription[0] + ", " + recipeDescription[1] + " e " + recipeDescription[2] + ".";
        recipeGestImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Gestures/" + FormatNameForFile(recipeGest));
        recipeImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Foods/Finished/" + FormatNameForFile(recipeName));
        Debug.Log(recipeGest + " loaded for recipe: " + recipeName + " in object: " + gameObject.name);
    }
    
    // Função para formatar nomes para busca de arquivos
    private string FormatNameForFile(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;
        return name.Replace(" ", "_").ToLower();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

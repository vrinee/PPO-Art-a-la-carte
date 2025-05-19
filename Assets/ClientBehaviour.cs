using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
public class ClientBehaviour : MonoBehaviour
{   

    private string sprite;
    private string gestureName;

    private string recipeName;
    private string[] recipeDescription;

    public TMP_Text chatBox;
    
    private System.Random rng = new System.Random();

    private bool ran = false;

        


    public void SetSprite(string sprite)
    {
        this.sprite = sprite;
        gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Clientes/" + sprite);
    }
    public void SetGestureName(string gestureName)
    {
        this.gestureName = gestureName;
    }
    public void SetRecipeName(string recipeName)
    {
        this.recipeName = recipeName;
    }

    public void SetRecipeDescription(string[] recipeDescription)
    {
        this.recipeDescription = recipeDescription;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        chatBox = GameObject.Find("Text (Chat)").GetComponent<TMP_Text>();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!ran && recipeDescription != null)
        {
            ran = true;
            for (int i = recipeDescription.Length - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                string temp = recipeDescription[i];
                recipeDescription[i] = recipeDescription[j];
                recipeDescription[j] = temp;
            }
            if (rng.Next(0, 2) == 0)
            {
                chatBox.text = "Eu quero algo que seja " + recipeDescription[0] + ", também " + recipeDescription[1] + " e " + recipeDescription[2] + ".";
            }
            else
            {
                chatBox.text = "Só me passa um " + recipeName + " Bro.";
            }
        }
    }
}

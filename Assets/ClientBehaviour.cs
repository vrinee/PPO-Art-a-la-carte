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
        gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Clientes/" + sprite);
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
            // Embaralha a descrição da receita
            for (int i = recipeDescription.Length - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                string temp = recipeDescription[i];
                recipeDescription[i] = recipeDescription[j];
                recipeDescription[j] = temp;
            }
            
            // Escolhe aleatoriamente entre diferentes tipos de diálogo
            int dialogType = rng.Next(0, 8);
            
            switch (dialogType)
            {
                case 0:
                    chatBox.text = "Eu quero algo que seja " + recipeDescription[0] + ", também " + recipeDescription[1] + " e " + recipeDescription[2] + ".";
                    break;
                case 1:
                    chatBox.text = "Só me passa um " + recipeName + " Bro.";
                    break;
                case 2:
                    chatBox.text = "Oi! Você pode me fazer algo " + recipeDescription[0] + " e " + recipeDescription[1] + "? Precisa ser " + recipeDescription[2] + " também!";
                    break;
                case 3:
                    chatBox.text = "Estou com bucho vazio! Quero um " + recipeName + " bem caprichado!";
                    break;
                case 4:
                    chatBox.text = "Me vê aí um prato que seja " + recipeDescription[0] + ", " + recipeDescription[1] + " e " + recipeDescription[2] + ", por favor!";
                    break;
                case 5:
                    chatBox.text = "Olá! Gostaria de pedir algo " + recipeDescription[2] + " e " + recipeDescription[0] + ". Pode ser " + recipeName + "?";
                    break;
                case 6:
                    chatBox.text = "Ei, chef! Tô precisando de algo bem " + recipeDescription[0] + ", " + recipeDescription[1] + " e " + recipeDescription[2] + "!";
                    break;
                case 7:
                    chatBox.text = "Por favor, me faz " + recipeName + " ae Tô morrendo de fome aqui!";
                    break;
            }
        }
    }
}

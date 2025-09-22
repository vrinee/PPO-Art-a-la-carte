using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PDollarGestureRecognizer;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class RecipeData
{
    public string name;
    public int cost;
    public string[] description;
}

[System.Serializable]
public class RecipeDataList
{
    public RecipeData[] recipes;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private new GameObject camera;
    private SellingGame sellingGame;

    private int money = 150;

    private string[] recipes;
    private int[] recipeCosts;

    private string[][] recipeDescription;
    private bool[] isRecipeReady;

    private TMP_Text moneyText;

    private bool modalOpen = false;

    public void SetModalOpen(bool open)
    {
        modalOpen = open;
    }
    public bool IsModalOpen()
    {
        return modalOpen;
    }

    public void SetMoney(int money)
    {
        this.money = money;
        Debug.Log("Money set to: " + money + " in GameManager.");
    }


    public int GetRecipeCost(string RecipeName)
    {
        for (int i = 0; i < recipes.Length; i++)
        {
            if (RecipeName == recipes[i])
            {
                return recipeCosts[i];
            }
        }
        Debug.LogError("Recipe not found: " + RecipeName);
        return 0;
    }

    public string[] GetRecipesReady()
    {
        List<string> readyRecipes = new List<string>();
        for (int i = 0; i < recipes.Length; i++)
        {
            if (isRecipeReady[i])
            {
                readyRecipes.Add(recipes[i]);
            }
        }
        return readyRecipes.ToArray();
    }

    public string[][] GetRecipeDescription()
    {
        return recipeDescription;
    }

    public string[] GetAllRecipeNames()
    {
        return recipes;
    }

    public int GetMoney()
    {
        return money;
    }

    public void AddMoney(int amount)
    {
        money += amount;
        Debug.Log("Money added: " + amount + ". New total: " + money);
        UpdateMoney();
    }
    void UpdateMoney()
    {
        moneyText.text = "Dinheiro:" + money.ToString();
    }

    void LoadRecipesFromJson()
    {
        // Load the JSON file from Resources
        TextAsset jsonText = Resources.Load<TextAsset>("recipes");
        if (jsonText == null)
        {
            Debug.LogError("recipes.json not found in Resources folder!");
            return;
        }

        // Parse the JSON
        RecipeDataList recipeList = JsonUtility.FromJson<RecipeDataList>(jsonText.text);

        // Assign to arrays
        int count = recipeList.recipes.Length;
        recipes = new string[count];
        recipeCosts = new int[count];
        recipeDescription = new string[count][];
        for (int i = 0; i < count; i++)
        {
            recipes[i] = recipeList.recipes[i].name;
            recipeCosts[i] = recipeList.recipes[i].cost;
            recipeDescription[i] = recipeList.recipes[i].description;
        }
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Prevent this object from being destroyed when loading a new scene
            SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the sceneLoaded event
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    void Start()
    {
        // This will only run once when the GameManager is first created
        Debug.Log("GameManager Start method called.");
        LoadRecipesFromJson();

        isRecipeReady = new bool[recipes.Length];
        for (int i = 0; i < isRecipeReady.Length; i++)
        {
            isRecipeReady[i] = false; //should be false as default, change to true to unlock every recipe without needing to do the minigame
        }
        //isRecipeReady[1] = true; // Set the first recipe as ready for testing
        moneyText = GameObject.Find("Text (Dinheiro)").GetComponent<TMP_Text>();
        UpdateMoney();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // This will run every time a new scene is loaded
        Debug.Log("Scene loaded: " + scene.name);

        if (scene.name == "StoreGame")
        {
            camera = GameObject.Find("Main Camera");
            if (camera != null)
            {
                sellingGame = camera.GetComponent<SellingGame>();
                if (sellingGame != null)
                {
                    sellingGame.UpdateMoney(money);
                }
                else
                {
                    Debug.LogError("sellingGame component not found on Main Camera!");
                }
            }
            else
            {
                Debug.LogError("Main Camera not found in the scene!");
            }
        }
        if (scene.name == "GameMenu")
        {
            moneyText = GameObject.Find("Text (Dinheiro)").GetComponent<TMP_Text>();
            UpdateMoney();
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from the sceneLoaded event to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void SetRecipeReady(string recipeName)
    {
        for (int i = 0; i < recipes.Length; i++)
        {
            if (recipeName == recipes[i])
            {
                isRecipeReady[i] = true;
                Debug.Log("Recipe " + recipeName + " is ready.");
                break;
            }
        }
    }

    public bool IsRecipeReady(string recipeName)
    {
        for (int i = 0; i < recipes.Length; i++)
        {
            if (recipeName == recipes[i])
            {
                return isRecipeReady[i];
            }
        }
        Debug.LogError("Recipe " + recipeName + " not found.");
        return false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // Handle the return key press - Reset game state
            Debug.Log("Resetting game state to default values...");
            
            // Reset money to default value
            money = 150;
            
            // Reset all recipes to not ready (false as default)
            if (isRecipeReady != null)
            {
                for (int i = 0; i < isRecipeReady.Length; i++)
                {
                    isRecipeReady[i] = false;
                }
            }
            
            Debug.Log("Game state reset complete. Money: " + money + ", All recipes locked.");
            
            // Load the GameMenu scene
            SceneManager.LoadScene("GameMenu");
        }
    }
}
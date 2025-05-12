using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PDollarGestureRecognizer;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private GameObject camera;
    private sellingGame sellingGame;

    private int money = 1000;

    public string[] recipes;
    public int[] recipeCosts;
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


    public int GetRecipeCost(string RecipeName){
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
    void UpdateMoney(){
        moneyText.text = "Dinheiro:" + money.ToString();
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

        isRecipeReady = new bool[recipes.Length];
        for (int i = 0; i < isRecipeReady.Length; i++)
        {
            isRecipeReady[i] = false;
        }
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
                sellingGame = camera.GetComponent<sellingGame>();
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

    public void SetRecipeReady(string recipeName){
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

    public bool IsRecipeReady(string recipeName){
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
}
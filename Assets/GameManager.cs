using UnityEngine;
using PDollarGestureRecognizer;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private GameObject camera;
    private sellingGame sellingGame;

    private int money = 0;

    public void SetMoney(int money)
    {
        this.money = money;
        Debug.Log("Money set to: " + money + " in GameManager.");
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
    }

    void OnDestroy()
    {
        // Unsubscribe from the sceneLoaded event to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    private GameObject camera = GameObject.Find("Main Camera");
    private Demo sellingGame = camera.GetComponent<Demo>();

    private int money;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void setMoney(int money)
    {
        this.money = money;
        debug.Log("Money set to: " + money);
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Prevent this object from being destroyed when loading a new scene
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }
    void Start()
    {
        sellingGame.UpdateMoney(money);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

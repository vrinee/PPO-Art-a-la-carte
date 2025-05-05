using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class CookingBehaviour : MonoBehaviour
{

    public string[] ToastableTags;
    private bool[] readyToBuild;
    public Transform cameraTransform;

    private Camera mainCamera;

    public string recipeName;
    public int recipeCost;
    public GameObject modalPrefab;

    private GameObject modalInstance;
    private Text lucroText;
    private Text TotalText;

    private Button TerminarButton;

    private LevelLoader levelLoader;

    private GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void complete(string tag)
    {
        for (int i = 0; i < ToastableTags.Length; i++)
        {
            if (tag == ToastableTags[i])
            {
                readyToBuild[i] = true;
                break;
            }
        }
        isComplete();
    }

    void isComplete()
    {
        for (int i = 0; i < readyToBuild.Length; i++)
        {
            if (readyToBuild[i] == false)
            {
                return;
            }
        }
        // Start a coroutine to smoothly move the camera
        
        if (levelLoader != null)
        {
            levelLoader.CompleteTransition();
        }
        else
        {
            Debug.LogError("LevelLoader not found in the scene!");
        }
        StartCoroutine(SmoothMoveCamera());
    }

    IEnumerator SmoothMoveCamera()
    {   
        yield return new WaitForSeconds(1f); // Wait for 0.5 seconds before starting the camera movement
        Vector3 startPosition = mainCamera.transform.position;
        Vector3 targetPosition = cameraTransform.position;
        targetPosition.z = mainCamera.transform.position.z;
        mainCamera.transform.position = targetPosition;
    }
    void Start()
    {
        readyToBuild = new bool[ToastableTags.Length];
        for (int i = 0; i < readyToBuild.Length; i++)
        {
            readyToBuild[i] = false;
        }
        mainCamera = Camera.main;
        gameManager = GameManager.Instance;
        levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();

        if (gameManager == null)
        {
             Debug.LogError("GameManager instance not found!");
                return;
        }
        if (levelLoader == null)
        {
            Debug.LogError("LevelLoader not found in the scene!");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void EndRecipe(){
        modalInstance = Instantiate(modalPrefab, transform.position, Quaternion.identity);

        lucroText = modalInstance.transform.Find("Lucro").GetComponent<Text>();
        TotalText = modalInstance.transform.Find("Total").GetComponent<Text>();
        TerminarButton = modalInstance.transform.Find("Button").GetComponent<Button>();
        if (TerminarButton == null)
        {
            Debug.LogError("TerminarButton not found in the modal instance!");
            return;
        }

        lucroText.text = "Receita: " + recipeName;
        TotalText.text = "Valor: " + recipeCost;
        Debug.LogError("TerminarButton found in the modal instance!");
        TerminarButton.onClick.AddListener(() => {
            levelLoader.LoadNextLevel("GameMenu");
            Debug.LogError("TerminarButton found in the modal instance!33333333333");
            });
        Debug.LogError("TerminarButton found in the modal instance! 2222");
        gameManager.SetRecipeReady(recipeName);
    }
}

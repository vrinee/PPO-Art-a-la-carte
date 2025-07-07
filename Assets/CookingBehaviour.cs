using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class CookingBehaviour : MonoBehaviour
{

    public string[] ToastableTags;
    private bool[] readyToBuild;
    public Transform cameraTransformToast;

    public Transform cameraTransformWash;

    public Transform cameraTransformSlice;

    private Camera mainCamera;

    public string recipeName;
    public int recipeCost;

    public Transform sliceableSpawnPoint;

    public Transform washableSpawnPoint;
    public GameObject modalPrefab;

    public GameObject washablePrefab;

    public GameObject sliceablePrefab;

    public GameObject[] slicesPrefabs;

    public int slicesAmount;
    private int slicesController = 0;

    public int sliceablesAmount;
    private int sliceablesController = 0;

    private int washablesController = 0;

    private GameObject modalInstance;
    private Text lucroText;
    private Text TotalText;

    private Button TerminarButton;

    private LevelLoader levelLoader;

    private GameManager gameManager;

    private GameObject sliceable;

    private bool IsToasted = false;
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
        IsToasted = true;
        StartCoroutine(SmoothMoveCamera(cameraTransformToast));
    }


    IEnumerator SmoothMoveCamera(Transform target)
    {
        yield return new WaitForSeconds(1f); // Wait for 0.5 seconds before starting the camera movement
        Vector3 startPosition = mainCamera.transform.position;
        Vector3 targetPosition = target.position;
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


        if (levelLoader == null)
        {
            Debug.LogError("LevelLoader not found in the scene!");
            return;
        }
        if (recipeName == "Salada")
        {
            Instantiate(washablePrefab, washableSpawnPoint.position, Quaternion.identity);
            SpawnSliceable();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void EndRecipe()
    {
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
        TerminarButton.onClick.AddListener(() => { levelLoader.LoadNextLevel("GameMenu"); });
        gameManager.SetRecipeReady(recipeName);
    }

    private void SpawnSliceable()
    {
        if (sliceableSpawnPoint != null && sliceablePrefab != null)
        {
            sliceable = Instantiate(sliceablePrefab, sliceableSpawnPoint.position, Quaternion.identity);
            sliceable.transform.SetParent(sliceableSpawnPoint);
        }
        else
        {
            Debug.LogError("Sliceable spawn point or prefab is not set!");
        }
    }

    public void RestartSliceable()
    {
        if (sliceable != null)
        {
            Destroy(sliceable);
            slicesController = 0;
            SpawnSliceable();
        }
        else
        {
            Debug.LogError("Sliceable not found to restart!");
        }
    }

    public void FinnishSlice()
    {
        slicesController++;
        if (slicesController >= slicesAmount)
        {
            slicesController = 0;
            sliceablesController++;
            if (sliceablesController >= sliceablesAmount)
            {
                Destroy(sliceable);
                Debug.Log("All slices completed for the recipe: " + recipeName);
                levelLoader.CompleteTransition();
                StartCoroutine(SmoothMoveCamera(cameraTransformSlice));
                return;
            }
            RestartSliceable();

        }
    }

    public void WashableDone()
    {
        washablesController++;
        if (washablesController >= sliceablesAmount)
        {
            levelLoader.CompleteTransition();
            StartCoroutine(SmoothMoveCamera(cameraTransformWash));
            return;
        }
        Instantiate(washablePrefab, washableSpawnPoint.position, Quaternion.identity);
    }

    public bool IsRecipeToasted()
    {
        return IsToasted;
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class CookingBehaviour : MonoBehaviour
{

    public string[] ToastableTags;

    public bool ToastablePanning = false;
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

    public GameObject[] sliceablePrefab;

    public int[] slicesAmount;
    public int slicesController = 0;

    public int[] sliceablesAmount;
    public int sliceablesController = 0;

    public GameObject UIcursor;
    private GameObject cursorInstance;

    private int washablesController = 0;

    private GameObject modalInstance;
    private Text lucroText;
    private Text TotalText;

    private Button TerminarButton;

    private LevelLoader levelLoader;

    private GameManager gameManager;

    private GameObject sliceable;

    public int sliceableIndex = 0;

    public int sliceIndex = 0;

    public GameObject[] sliceBowls;

    public GameObject escapePoint;

    private Transform bowl1Transform;

    private Transform bowl2Transform;

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

    IEnumerator MoveBowlsSmooth()
    {
        if (bowl1Transform == null || bowl2Transform == null || escapePoint == null)
        {
            Debug.LogError("Bowl transforms or escape point not set!");
            yield break;
        }

        // Store initial positions
        Vector3 bowl1StartPos = bowl1Transform.position;
        Vector3 bowl2StartPos = bowl2Transform.position;
        
        // Target positions
        Vector3 bowl1TargetPos = escapePoint.transform.position;
        Vector3 bowl2TargetPos = bowl1StartPos;

        float duration = 1.0f; // Movement duration in seconds
        float elapsedTime = 0f;

        // Smooth movement over time
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            // Use smooth curve for better animation
            t = Mathf.SmoothStep(0f, 1f, t);

            // Move bowl1 to escape point
            bowl1Transform.position = Vector3.Lerp(bowl1StartPos, bowl1TargetPos, t);
            
            // Move bowl2 to bowl1's original position
            bowl2Transform.position = Vector3.Lerp(bowl2StartPos, bowl2TargetPos, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final positions are exact
        bowl1Transform.position = bowl1TargetPos;
        bowl2Transform.position = bowl2TargetPos;

        // Wait a brief moment then destroy bowl1
        yield return new WaitForSeconds(0.3f);
        
        if (bowl1Transform != null && bowl1Transform.gameObject != null)
        {
            Destroy(bowl1Transform.gameObject);
        }
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
        if (recipeName == "Salada Caesar")
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
            Debug.Log("Spawning sliceable: " + sliceablePrefab[sliceableIndex].name);
            sliceable = Instantiate(sliceablePrefab[sliceableIndex], sliceableSpawnPoint.position, Quaternion.identity);
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
        if (slicesController >= slicesAmount[sliceIndex])
        {
            slicesController = 0;
            sliceablesController++;
            if (sliceablesController >= sliceablesAmount[sliceIndex])
            {
                sliceablesController = 0;
                sliceableIndex++;
                if (sliceBowls.Length > 0 && !(sliceIndex >= sliceablePrefab.Length - 1))
                {
                    bowl1Transform = sliceBowls[sliceIndex].transform;
                    bowl2Transform = sliceBowls[sliceIndex + 1].transform;
                    
                    // Start smooth movement for bowls
                    StartCoroutine(MoveBowlsSmooth());
                }
                if (sliceIndex >= sliceablePrefab.Length - 1)
                {
                    Destroy(sliceable);
                    Debug.Log("All slices completed for the recipe: " + recipeName);
                    levelLoader.CompleteTransition();
                    StartCoroutine(SmoothMoveCamera(cameraTransformSlice));
                    Destroy(cursorInstance);
                    return;

                }
                sliceIndex++;
            }
            RestartSliceable();

        }
    }

    public void WashableDone()
    {
        washablesController++;
        if (washablesController >= sliceablesAmount[0])
        {
            levelLoader.CompleteTransition();
            StartCoroutine(SmoothMoveCamera(cameraTransformWash));
            CreateCursorInstance();
            return;
        }
        Instantiate(washablePrefab, washableSpawnPoint.position, Quaternion.identity);
    }

    public bool IsRecipeToasted()
    {
        return IsToasted;
    }

    private void CreateCursorInstance()
    {
        if (UIcursor != null)
        {
            cursorInstance = Instantiate(UIcursor);
        }
        else
        {
            Debug.LogError("UIcursor prefab is not assigned!");
        }
    }
}

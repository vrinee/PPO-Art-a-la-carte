using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CookingBehaviour : MonoBehaviour
{

    public string[] ToastableTags;
    private bool[] readyToBuild;
    public Transform cameraTransform;

    private Camera mainCamera;
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
    LevelLoader levelLoader = FindObjectOfType<LevelLoader>();
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

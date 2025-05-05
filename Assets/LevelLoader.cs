using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class LevelLoader : MonoBehaviour
{
    public Animator transition; // Assign the Animator in the Inspector
    public float transitionTime = 1f;

    public void LoadNextLevel(string sceneName)
    {
        StartCoroutine(LoadLevel(sceneName));
    }

    IEnumerator LoadLevel(string sceneName)
    {
        Debug.Log("Starting transition animation...");
        
        // Play animation
        if (transition != null)
        {
            transition.SetTrigger("Start");
            Debug.Log("Trigger 'Start' set.");
        }
        else
        {
            Debug.LogError("Transition Animator is not assigned!");
            yield break;
        }

        // Wait for the transition animation to complete
        Debug.Log("Waiting for transition time: " + transitionTime);
        yield return new WaitForSeconds(transitionTime);

        // Load the scene
        Debug.Log("Loading scene: " + sceneName);
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene name is null or empty!");
        }
    }

    public void CompleteTransition()
    {
        StartCoroutine(PlayTransitionAnimation());
    }

    IEnumerator PlayTransitionAnimation()
    {
        if (transition != null)
        {
            transition.SetTrigger("Start");
            Debug.Log("Trigger 'Start' set for transition animation.");
        }
        else
        {
            Debug.LogError("Transition Animator is not assigned!");
            yield break;
        }

        // Wait for the transition animation to complete
        Debug.Log("Waiting for transition time: " + transitionTime);
        yield return new WaitForSeconds(transitionTime);

        transition.SetTrigger("End");
        Debug.Log("Trigger 'End' set for transition animation.");
        // Load the next level or perform any other action here
    }
}
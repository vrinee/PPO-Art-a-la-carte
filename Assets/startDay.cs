using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class startDay : MonoBehaviour
{
    private GameManager gameManager;

    private LevelLoader levelLoader;

    private bool hasRecipes = false;

    private Button button;

    private bool warningShown = false;
    public GameObject warningPrefab;

    private string[] recipes;


    void ShowWarning()
    {
        if (!warningShown)
        {
            warningShown = true;
            GameObject warningInstance = Instantiate(warningPrefab, transform.position, Quaternion.identity);
            Destroy(warningInstance, 2f);
            StartCoroutine(WaitForWarning(2f));
        }
    
    }

    IEnumerator WaitForWarning(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        warningShown = false;
    }
    void SetButtonAppearance(bool enabled)
    {
        ColorBlock colors = button.colors;

        if (enabled)
        {
            // Set the normal color to the default enabled color
            colors.normalColor = Color.white; // Or any color you want for enabled state
            colors.highlightedColor = Color.white;
            colors.selectedColor = Color.white;
        }
        else
        {
            // Set the normal color to a "disabled" color
            colors.normalColor = Color.gray; // Or any color you want for disabled state
            colors.highlightedColor = Color.gray;
            colors.selectedColor = Color.gray;
        }

        button.colors = colors;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        levelLoader = FindFirstObjectByType<LevelLoader>();
        button = GetComponent<Button>();
        
    }

    // Update is called once per frame
    void Update()
    {
        recipes = gameManager.GetRecipesReady();

        if (recipes == null || recipes.Length == 0)
        {
            hasRecipes = false;
        }
        else
        {
            hasRecipes = true;
        }
        if (hasRecipes)
        {
            SetButtonAppearance(true);
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                levelLoader.LoadNextLevel("StoreGame");
            });
        }
        else
        {
            SetButtonAppearance(false);
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                ShowWarning();
            });
        }

    }
}

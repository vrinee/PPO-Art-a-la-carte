using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Recipe : MonoBehaviour
{
    public string recipeName;
    
    public GameObject warningPrefab;

    public GameObject modalPrefab;
    private GameObject modalInstance;
    private GameObject Empty;
    private Button yesButton;
    private Button noButton;
    private Animator animator;
    private int recipeCost;

    private Button button;
    private GameManager gameManager;

    private Text buttonText;

    private LevelLoader levelLoader;



    private bool warningShown = false;

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

    void goToRecipe(){
        if (gameManager.IsModalOpen()) return; // Prevent opening multiple modals
        gameManager.SetModalOpen(true);
        modalInstance = Instantiate(modalPrefab, transform.position, Quaternion.identity);
        Empty = modalInstance.transform.Find("base").gameObject;
        yesButton = Empty.transform.Find("uh huh").GetComponent<Button>();
        noButton = Empty.transform.Find("nuh uh").GetComponent<Button>();
        animator = modalInstance.GetComponent<Animator>();
        if (yesButton == null || noButton == null)
        {
            Debug.LogError("Buttons not found in the modal instance!");
            return;
        }
        yesButton.onClick.AddListener(() => {
            gameManager.AddMoney(-recipeCost);
            levelLoader.LoadNextLevel(recipeName);
            animator.SetTrigger("Exit");
            Destroy(modalInstance,1f);
            gameManager.SetModalOpen(false);

        });
        noButton.onClick.AddListener(() => {
            Destroy(modalInstance,1f);
            animator.SetTrigger("Exit");
            gameManager.SetModalOpen(false);
        });
    }

    void Start()
    {
        button = GetComponentInChildren<Button>();
        buttonText = button.GetComponentInChildren<Text>();
        gameManager = GameManager.Instance;
        levelLoader = FindObjectOfType<LevelLoader>();
        recipeCost = gameManager.GetRecipeCost(recipeName);
        if (recipeCost == 0)
        {
            Debug.LogError("Recipe cost not found for: " + recipeName);
            return;
        }

        buttonText.text = recipeCost.ToString();
    }

    void Update()
    {

        if (gameManager != null && button != null )
        {   
            if(!gameManager.IsRecipeReady(recipeName)){
                
                if (gameManager.GetMoney() >= recipeCost)
                {
                    SetButtonAppearance(true);
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(()=>{goToRecipe();});
                }
                else
                {
                    SetButtonAppearance(false);
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(()=>{ShowWarning();});

                }
            }else{
                button.interactable = false;
                buttonText.text = "???";
            }
        }
    }
}

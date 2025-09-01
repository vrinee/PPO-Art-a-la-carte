using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bowl : MonoBehaviour
{

    public string[] itemTags;
    public int[] itemAmounts;

    public Sprite[] states;

    public bool spawnsItems = true;
    public Transform spawnPoint;

    public GameObject[] itemPrefab;
    public bool isFinishBowl = false;

    private CookingBehaviour cookingBehaviour;

    private int currentState = 0;

    private int lastAdded = -1;

    private int currentAmount = 0;

    private bool isStarted = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cookingBehaviour = FindFirstObjectByType<CookingBehaviour>();

    }

    // Update is called once per frame
    void Update()
    {
        if (cookingBehaviour.IsRecipeToasted() && !isStarted && isFinishBowl)
        {
            Instantiate(itemPrefab[0], spawnPoint.position, Quaternion.identity);
            isStarted = true;
        }
    }

    public void enterItem(string tag)
    {
        for (int i = 0; i < itemTags.Length; i++)
        {
            if (tag == itemTags[i])
            {   
                if (currentAmount == 0) currentAmount = itemAmounts[i];
                itemAmounts[i]--;
                GetComponent<SpriteRenderer>().sprite = states[currentState];
                currentState++;
                if (i == itemTags.Length - 1 && itemAmounts[i] <= 0 && isFinishBowl)
                {
                    cookingBehaviour.EndRecipe();
                    return;
                }
                if (!spawnsItems) return;
                if (itemAmounts[i] <= 0)
                {
                    Instantiate(itemPrefab[i + 1], spawnPoint.position, Quaternion.identity);
                    return;
                }
                Instantiate(itemPrefab[i], spawnPoint.position, Quaternion.identity);
            }
        }
    }

    public void resetState()
    {
        for (int i = 0; i < currentAmount; i++)
        {
            GetComponent<SpriteRenderer>().sprite = states[currentState];
            currentState--;
        }
        currentAmount = 0;
    }
    
}

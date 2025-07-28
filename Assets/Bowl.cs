using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bowl : MonoBehaviour
{

    public string[] itemTags;
    public int[] itemAmounts;

    public Sprite[] states;

    public Transform spawnPoint;

    public GameObject[] itemPrefab;

    private CookingBehaviour cookingBehaviour;

    private int currentState = 0;

    private bool isStarted = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cookingBehaviour = FindFirstObjectByType<CookingBehaviour>();

    }

    // Update is called once per frame
    void Update()
    {
        if (cookingBehaviour.IsRecipeToasted() && !isStarted)
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
                itemAmounts[i]--;
                GetComponent<SpriteRenderer>().sprite = states[currentState];
                currentState++;
                if (i == itemTags.Length - 1 && itemAmounts[i] <= 0)
                {
                    cookingBehaviour.EndRecipe();
                    return;
                }
                if (itemAmounts[i] <= 0)
                {
                    Instantiate(itemPrefab[i + 1], spawnPoint.position, Quaternion.identity);
                    return;    
                }
                Instantiate(itemPrefab[i], spawnPoint.position, Quaternion.identity);
            }
        }
    }
    
}

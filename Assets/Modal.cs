using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Modal : MonoBehaviour
{
    private Animator animator;

    private bool previouslyActiveState = false;
    public void Show()
    {
        if(!this.gameObject.activeSelf){
            this.gameObject.SetActive(true);
            previouslyActiveState = false;
        }else{
            previouslyActiveState = true;
        }
        StartCoroutine(StateChange());
    }
    IEnumerator StateChange()
    {
        if(!previouslyActiveState){
            
            animator.SetTrigger("Start");
            yield return new WaitForSeconds(1f);
            Debug.Log("Modal is now active.");
            animator.SetTrigger("end");
        }else{
            animator.SetTrigger("StartExit");
            yield return new WaitForSeconds(1f);
            this.gameObject.SetActive(false);
        }
        
        
        
    }

    // Start is called before the first frame update
    void Start()
    {
        // Hide the modal by default
        animator = GetComponent<Animator>();
        this.gameObject.SetActive(false);
        
    }
}
using UnityEngine;
using UnityEngine.UI;

public class ClientBehaviour : MonoBehaviour
{   

    private string sprite;
    private string gestureName;

    public Text chatBox;


        


    public void SetSprite(string sprite)
    {
        this.sprite = sprite;
        gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Clientes/"+ sprite);
    }
    public void SetGestureName(string gestureName)
    {
        this.gestureName = gestureName;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        chatBox = GameObject.Find("Text (Chat)").GetComponent<Text>();
        chatBox.text = "HAIII, eu quero uma: " + gestureName + "!";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

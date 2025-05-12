using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PDollarGestureRecognizer
{
    public class sellingGame : MonoBehaviour
    {
        public Transform gestureOnScreenPrefab;

        private List<Gesture> trainingSet = new List<Gesture>();

        private List<Point> points = new List<Point>();
        private int strokeId = -1;

        private Vector3 virtualKeyPosition = Vector2.zero;
        public RectTransform drawAreaImage;

        private RuntimePlatform platform;
        private int vertexCount = 0;

        private List<LineRenderer> gestureLinesRenderer = new List<LineRenderer>();
        private LineRenderer currentGestureLineRenderer;

        // GUI
        private string message;
        private bool recognized;
        private string newGestureName = "";

        private string wanted;

        // UI Buttons and Text
        public Button recognizeButton;
        public Button clearButton;
        public Text messageText;

        public GameObject Cliente;

        public Transform ClienteSpawn;
        private string[] ClientesGest = 
            { "whirl", "ball", "T"};

        private string[] ClientesSprites = 
            {"AdorableCutieChiikawa", "SweetBabyHachiware2", "SweetieMomonga", "YahaUsagi"}; // Sprites
        

        private GameObject newCliente;

        private int money;
        private int moneyPerOrder = 100;
        public Text moneyText;
        public Text chatBox;

        private int LastSpite = -1;
        private int LastGesture = -1;

        private GameManager gameManager; // Reference to GameManager
        
        private System.Random rng = new System.Random();

        private int startingMoney = 0;
    
        public void UpdateMoney(int money)
        {
            this.money = money;
            startingMoney = money;
            moneyText.text = "Dinheiro: " + money.ToString();
            Debug.Log("Money updated to: " + money + " in sellingGame.");
        }

        public GameObject modalPrefab;

        private GameObject modalInstance;
        private Text lucroText;
        private Text TotalText;

        private Button TerminarButton;

        private LevelLoader levelLoader; 
        void Start()
        {
            // Access the GameManager singleton
            gameManager = GameManager.Instance;
            levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();

            if (gameManager == null)
            {
                Debug.LogError("GameManager instance not found!");
                return;
            }

            platform = Application.platform;

            // Load pre-made gestures
            TextAsset[] gesturesXml = Resources.LoadAll<TextAsset>("GestureSet/10-stylus-MEDIUM/");
            foreach (TextAsset gestureXml in gesturesXml)
                trainingSet.Add(GestureIO.ReadGestureFromXML(gestureXml.text));

            // Load user custom gestures
            string[] filePaths = Directory.GetFiles(Application.dataPath + @"/\PDollar\Resources\GestureSet\10-stylus-MEDIUM", "*.xml");
            foreach (string filePath in filePaths)
                trainingSet.Add(GestureIO.ReadGestureFromFile(filePath));

            // Add listeners to the buttons
            recognizeButton.onClick.AddListener(OnRecognizeButtonClick);
            clearButton.onClick.AddListener(OnClearButtonClick);

            // Start the game automatically
            StartGame(); // Call the StartGame method to start the game automatically
        }

        void Update()
        {
            if (platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touchCount > 0)
                {
                    virtualKeyPosition = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
                }
            }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    virtualKeyPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
                }
            }

            if (RectTransformUtility.RectangleContainsScreenPoint(drawAreaImage, virtualKeyPosition, Camera.main))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (recognized)
                    {
                        recognized = false;
                        strokeId = -1;

                        points.Clear();

                        foreach (LineRenderer lineRenderer in gestureLinesRenderer)
                        {
                            lineRenderer.positionCount = 0;
                            Destroy(lineRenderer.gameObject);
                        }

                        gestureLinesRenderer.Clear();
                    }

                    ++strokeId;

                    Transform tmpGesture = Instantiate(gestureOnScreenPrefab, transform.position + new Vector3(0, 0, 0), transform.rotation) as Transform;
                    currentGestureLineRenderer = tmpGesture.GetComponent<LineRenderer>();

                    gestureLinesRenderer.Add(currentGestureLineRenderer);

                    vertexCount = 0;
                }

                if (Input.GetMouseButton(0))
                {
                    points.Add(new Point(virtualKeyPosition.x, -virtualKeyPosition.y, strokeId));

                    currentGestureLineRenderer.positionCount = ++vertexCount;
                    currentGestureLineRenderer.SetPosition(vertexCount - 1, Camera.main.ScreenToWorldPoint(new Vector3(virtualKeyPosition.x, virtualKeyPosition.y, 10)));
                }
            }
        }

        void OnRecognizeButtonClick()
        {
            recognized = true;
            Gesture candidate = new Gesture(points.ToArray());
            Result gestureResult = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());

            if (gestureResult.GestureClass == wanted)
            {
                message = "Acertou";
                money += moneyPerOrder;
                moneyText.text = "Dinheiro: " + money.ToString();
                moneyPerOrder = 100;
                chatBox.text = "";
                Destroy(newCliente);
                //reseta o gesto
                wanted = null;
                // Atualiza o dineiro no GameManager
                gameManager.SetMoney(money);
            }
            else
            {
                message = "Errou";
                moneyPerOrder -= 10;
            }

            points.Clear();

            foreach (LineRenderer lineRenderer in gestureLinesRenderer)
            {
                lineRenderer.positionCount = 0;
                Destroy(lineRenderer.gameObject);
            }

            gestureLinesRenderer.Clear();
            strokeId = -1;

            if (messageText != null)
            {
                messageText.text = message;
            }
        }

        void OnClearButtonClick()
        {
            points.Clear();

            foreach (LineRenderer lineRenderer in gestureLinesRenderer)
            {
                lineRenderer.positionCount = 0;
                Destroy(lineRenderer.gameObject);
            }

            gestureLinesRenderer.Clear();
            strokeId = -1;

            Debug.Log("Cleared!");
        }

        void StartGame()
        {
            // Start the coroutine to spawn clients
            StartCoroutine(SpawnClients());
        }
        IEnumerator SpawnClients()
        {
            int numClientes = 10; // numeros de clientes para spawnar
            for(int i =0 ; i < numClientes; i++)
            {
                SpawnClient();
                
                yield return new WaitUntil(() => wanted == null); // Wait until the client is served
            }

            modalInstance = Instantiate(modalPrefab, transform.position, Quaternion.identity);

            lucroText = modalInstance.transform.Find("Lucro").GetComponent<Text>();
            TotalText = modalInstance.transform.Find("Total").GetComponent<Text>();
            TerminarButton = modalInstance.transform.Find("Button").GetComponent<Button>();

            lucroText.text = "Lucro: " + (money - startingMoney).ToString();
            TotalText.text = "Total: " + money.ToString();
            gameManager.SetMoney(money);
            TerminarButton.onClick.AddListener(() => levelLoader.LoadNextLevel("GameMenu"));
        }
        void SpawnClient()
        {
            
            newCliente = Instantiate(Cliente, ClienteSpawn.position, Quaternion.identity);
            int gesture = rng.Next(ClientesGest.GetLength(0) - 1); // Random gesture index
            int spriteIndx = rng.Next(ClientesSprites.GetLength(0) - 1); // Random sprite index
            while (spriteIndx == LastSpite) 
            {
                spriteIndx = rng.Next(ClientesSprites.GetLength(0) - 1);
            }
            while (gesture == LastGesture) 
            {
                gesture = rng.Next(ClientesGest.GetLength(0) - 1);
            }

            ClientBehaviour clienteScript = newCliente.GetComponent<ClientBehaviour>();
            if (clienteScript != null)
            {
                clienteScript.SetSprite(ClientesSprites[spriteIndx]);
                clienteScript.SetGestureName(ClientesGest[gesture]);
                wanted = ClientesGest[gesture];
            }
            else
            {
                Debug.LogError("ClientBehaviour script is not attached to the Cliente prefab!");
            }

            LastGesture = gesture; // Store the last gesture index
            LastSpite = spriteIndx; // Store the last sprite index
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;

namespace PDollarGestureRecognizer
{
    public class SellingGame : MonoBehaviour
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
        //private string newGestureName = "";

        private string wanted;

        // UI Buttons and Text
        public Button recognizeButton;
        public Button clearButton;
        public Text messageText;

        public GameObject Cliente;

        public Transform ClienteSpawn;
        private string[] ClientesGest =
            { "whirl", "ball", "triangle","horizontalline","verticalline"}; // Gestures

        private string[] Recipes;

        private string[] RecipesGest;

        //private int[] RecipesCost;

        private string[][] RecipeDescriptions;

        private int recipeIndex;

        private string[] ClientesSprites =
            {"AdorableCutieChiikawa", "SweetBabyHachiware2", "SweetieMomonga", "YahaUsagi"}; // Sprites
        

        private GameObject newCliente;

        private int money;
        private int moneyPerOrder = 100;
        private int streak = 0;
        private int streakMoney = 0;
        public Text moneyText;
        public TMP_Text chatBox;
        public Text streakText;

        private int LastSpite = -1;

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

        public string[] GetRecipes()
        {
            return Recipes;
        }
        public string[] GetRecipesGest()
        {
            return RecipesGest;
        }
        public string[][] GetRecipeDescription()
        {
            return RecipeDescriptions;
        }
        void AssignGestures()
        {
            RecipesGest = new string[Recipes.Length];
            // Create a mutable list of available gestures
            List<string> gesturePool = new List<string>(ClientesGest);

            System.Random rng = new System.Random();

            for (int i = 0; i < Recipes.Length; i++)
            {
                string chosenGesture;
                if (gesturePool.Count > 0)
                {
                    // Pick a random gesture from the pool
                    int index = rng.Next(gesturePool.Count);
                    chosenGesture = gesturePool[index];
                    gesturePool.RemoveAt(index); // Remove so it's not reused
                }
                else
                {
                    // Pool is empty, reuse any gesture from the original set
                    int index = rng.Next(ClientesGest.Length);
                    chosenGesture = ClientesGest[index];
                }
                RecipesGest[i] = chosenGesture;
            }

            Debug.Log("Gestures assigned: " + string.Join(", ", RecipesGest));
        }   
        void Awake()
        {
            // Access the GameManager singleton
            gameManager = GameManager.Instance;
            levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();

            if (gameManager == null)
            {
                Debug.LogError("GameManager instance not found!");
                return;
            }

            Recipes = gameManager.GetRecipesReady();
            RecipeDescriptions = gameManager.GetRecipeDescription();
            AssignGestures();

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
            
            Debug.Log(gestureResult.GestureClass);
            if (gestureResult.GestureClass == wanted)
            {
                streak += 1;
                message = "Acertou";
                if(streak < 2) streakMoney = 0;
                else if(streak < 4) streakMoney = 1;
                else if(streak < 6) streakMoney = 2;
                else streakMoney = 3;
                streakText.text = "Sequência: " + streak.ToString() + "\nBônus: " + streakMoney.ToString();
                money += moneyPerOrder + streakMoney;
                moneyText.text = "Dinheiro: " + money.ToString();
                moneyPerOrder = 69; //this is a default, the actual value is set in spawnClient
                chatBox.text = "";
                Destroy(newCliente);
                //reseta o gesto
                wanted = null;
                // Atualiza o dineiro no GameManager
                gameManager.SetMoney(money);
                
            }
            else
            {
                streak = 0;
                streakMoney = 0;
                streakText.text = "Sequência: " + streak.ToString() + "\nBônus: " + streakMoney.ToString();
                message = "Errou";
                moneyPerOrder -= 1;
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
            recipeIndex = rng.Next(Recipes.Length); // Random recipe index
            moneyPerOrder = (gameManager.GetRecipeCost(Recipes[recipeIndex]))/10;//here is where the moneyperorder is set
            int spriteIndx = rng.Next(ClientesSprites.GetLength(0) - 1); // Random sprite index
            while (spriteIndx == LastSpite) 
            {
                spriteIndx = rng.Next(ClientesSprites.GetLength(0) - 1);
            }
            ClientBehaviour clienteScript = newCliente.GetComponent<ClientBehaviour>();
            if (clienteScript != null)
            {
                clienteScript.SetSprite(ClientesSprites[spriteIndx]);
                clienteScript.SetGestureName(RecipesGest[recipeIndex]);
                Debug.Log(RecipesGest[recipeIndex] + " this is the gesture " + recipeIndex);
                clienteScript.SetRecipeName(Recipes[recipeIndex]);
                Debug.Log(Recipes[recipeIndex] + " this is the recipe " + recipeIndex);
                clienteScript.SetRecipeDescription(RecipeDescriptions[recipeIndex]);
                wanted = RecipesGest[recipeIndex];
            }
            else
            {
                Debug.LogError("ClientBehaviour script is not attached to the Cliente prefab!");
            }

            LastSpite = spriteIndx; // Store the last sprite index
        }
    }
}
/*

código original do assets

 using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PDollarGestureRecognizer
{
	public class Demo : MonoBehaviour
	{
		public Transform gestureOnScreenPrefab;

		private List<Gesture> trainingSet = new List<Gesture>();

		private List<Point> points = new List<Point>();
		private int strokeId = -1;

		private Vector3 virtualKeyPosition = Vector2.zero;
		private Rect drawArea;

		private RuntimePlatform platform;
		private int vertexCount = 0;

		private List<LineRenderer> gestureLinesRenderer = new List<LineRenderer>();
		private LineRenderer currentGestureLineRenderer;

		//GUI
		private string message;
		private bool recognized;
		private string newGestureName = "";

		private string wanted;
		public Texture2D buttonImage;


		void Start()
		{
			platform = Application.platform;
			drawArea = new Rect(0, 0, Screen.width - Screen.width / 2, Screen.height);

			//Load pre-made gestures
			TextAsset[] gesturesXml = Resources.LoadAll<TextAsset>("GestureSet/10-stylus-MEDIUM/");
			foreach (TextAsset gestureXml in gesturesXml)
				trainingSet.Add(GestureIO.ReadGestureFromXML(gestureXml.text));

			//Load user custom gestures
			string[] filePaths = Directory.GetFiles(Application.dataPath + @"/\PDollar\Resources\GestureSet\10-stylus-MEDIUM", "*.xml");
			foreach (string filePath in filePaths)
				trainingSet.Add(GestureIO.ReadGestureFromFile(filePath));
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

			if (drawArea.Contains(virtualKeyPosition))
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

					Transform tmpGesture = Instantiate(gestureOnScreenPrefab, transform.position+ new Vector3(0,0,0), transform.rotation) as Transform;
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
	
		void OnGUI()
		{

			GUI.Box(drawArea, "Draw Area");

			GUI.Label(new Rect(10, Screen.height - 40, 500, 50), message);

			if (GUI.Button(new Rect(Screen.width - 100, 10, 100, 30), buttonImage))
			{
				recognized = true;
				wanted = "pica";
				Gesture candidate = new Gesture(points.ToArray());
				Result gestureResult = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());
				// esta aqui meu querido filho copilot

				if(gestureResult.GestureClass == wanted){
					message = "Acertou";
				}else{
					message = "Errou";
				}
				//message = gestureResult.GestureClass + " " + gestureResult.Score;
			}

			GUI.Label(new Rect(Screen.width - 200, 150, 70, 30), "Add as: ");
			newGestureName = GUI.TextField(new Rect(Screen.width - 150, 150, 100, 30), newGestureName);

			if (GUI.Button(new Rect(Screen.width - 50, 150, 50, 30), "Add") && points.Count > 0 && newGestureName != "")
			{
				string fileName = String.Format("{0}/{1}-{2}.xml", Application.dataPath + @"/\PDollar\Resources\GestureSet\10-stylus-MEDIUM", newGestureName, DateTime.Now.ToFileTime());

#if !UNITY_WEBPLAYER
				GestureIO.WriteGesture(points.ToArray(), newGestureName, fileName);
#endif

				trainingSet.Add(new Gesture(points.ToArray(), newGestureName));

				newGestureName = "";
			}
		}
	}
} */

using UnityEngine;
using UnityEngine.UI; // Import UnityEngine.UI for UI components
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PDollarGestureRecognizer
{
    public class Demo : MonoBehaviour
    {
        public Transform gestureOnScreenPrefab;

        private List<Gesture> trainingSet = new List<Gesture>();

        private List<Point> points = new List<Point>();
        private int strokeId = -1;

        private Vector3 virtualKeyPosition = Vector2.zero;
        public Rect drawArea;

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
        public Button recognizeButton; // coloca o botao (reconhecer) pela UI da unity
        public Button clearButton;     // coloca o botao (limpar) pela UI da unity
        public Text messageText;       // Drag a Text UI element to display messages

        public Button StartButton;  // coloca o botao (começar) pela UI da unity

        public GameObject Cliente; // prefab do cliente
        private string[,] Clientes = {
        {"pica","whirl","ball","T"}, // gestos
        {"AdorableCutieChiikawa","SweetBabyHachiware2","SweetieMomonga","YahaUsagi"}
        };

        private GameObject newCliente; // cliente instanciado

        private int money = 0; // dinheiro do usuario
        private int moneyPerOrder = 100;
        public Text moneyText; // texto que mostra o dinheiro

        public Text chatBox; // caixa de texto do cliente

        private GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        public void UpdateMoneyText(int money)
        {
            this.money = money; // Atualiza o valor do dinheiro
            moneyText.text = "Dinheiro: " + this.money.ToString(); // Atualiza o texto do dinheiro
        }

        void sendMoney(){
            gameManager.setMoney(money);

        }
        void Start()
        {
            platform = Application.platform;
            drawArea = new Rect(0, 0, Screen.width - Screen.width / 2, Screen.height);

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
            StartButton.onClick.AddListener(OnStartButtonClick);
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

            if (drawArea.Contains(virtualKeyPosition))
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
                money += moneyPerOrder; // Adiciona dinheiro ao usuário
                moneyText.text = "Dinheiro: " + money.ToString(); // Atualiza o texto do dinheiro
                moneyPerOrder = 100; // Reseta o dinheiro por pedido
                chatBox.text = ""; // Limpa a caixa de texto do cliente
                Destroy(newCliente); // Destroi o cliente após acertar
            }
            else
            {
                message = "Errou";
                moneyPerOrder -= 10; // Diminui o dinheiro por pedido
            }

            points.Clear();

            foreach (LineRenderer lineRenderer in gestureLinesRenderer)
            {
                lineRenderer.positionCount = 0;
                Destroy(lineRenderer.gameObject);
            }

            gestureLinesRenderer.Clear();
            strokeId = -1;

            // atualiza o texto na tela
            if (messageText != null)
            {
                messageText.text = message;
            }
        }

        void OnClearButtonClick()
        {
            //limpa a tela
            points.Clear();

            foreach (LineRenderer lineRenderer in gestureLinesRenderer)
            {
                lineRenderer.positionCount = 0;
                Destroy(lineRenderer.gameObject);
            }

            gestureLinesRenderer.Clear();
            strokeId = -1;

            Debug.Log("Pq tá olhando o console? Meliante!");
        }

        void OnStartButtonClick()
        {
            // Instantiate the Cliente prefab
            newCliente = Instantiate(Cliente, new Vector3(0, 0, 2), Quaternion.identity);

            // Get the script attached to the Cliente prefab
            ClientBehaviour clienteScript = newCliente.GetComponent<ClientBehaviour>();
            if (clienteScript != null)
            {
                // Set the sprite and gesture name
                clienteScript.SetSprite(Clientes[1, 0]); // Pass the sprite name
                clienteScript.SetGestureName(Clientes[0, 1]); // Pass the gesture name
                wanted = Clientes[0, 1]; // Set the wanted gesture
            }
            else
            {
                Debug.LogError("ClienteScript is not attached to the Cliente prefab!");
            }
        }
    }
}
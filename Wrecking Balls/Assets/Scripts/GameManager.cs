using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static playerMove;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public enum GameState { Loading, readyForTheShot, preparingNewLevel, inTheShot, gameOver}

    public GameState currentState;
    public GameState CurrentState => currentState;

    Vector3 pos1 = new Vector3(-3, 6, -0.5f);
    Vector3 pos2 = new Vector3(-2, 6, -0.5f);
    Vector3 pos3 = new Vector3(-1, 6, -0.5f);
    Vector3 pos4 = new Vector3(0, 6, -0.5f);
    Vector3 pos5 = new Vector3(1, 6, -0.5f);
    Vector3 pos6 = new Vector3(2, 6, -0.5f);
    Vector3 pos7 = new Vector3(3, 6, -0.5f);

    [SerializeField] List<ScaleButton> ButtonScaleList = new List<ScaleButton>();
    private List<Vector3> posList = new List<Vector3>();
    private List<bool> occupiedPositionList = new List<bool>();
    public List<GameObject> cubeList = new List<GameObject>();
    public List<GameObject> powerUpList = new List<GameObject>();
    public List<Ball> ballList = new List<Ball>();
    public List<Ball> ballMoving = new List<Ball>();
    public List<Ball> ballCatches = new List<Ball>();

    public GameObject cubePrefab;
    public GameObject ballPrefab;
    public GameObject ballAddPrefab;
    public GameObject coinPrefab;
  
    public int level = 1;
    public int record = 1;

    [SerializeField] TextMeshProUGUI FPSText;
    [SerializeField] TextMeshProUGUI MultiplierText;
    [SerializeField] TextMeshProUGUI score;
    [SerializeField] TextMeshProUGUI recordText;
    [SerializeField] TextMeshProUGUI coinsCount;

    public int frameRate;
    public int frameCount;
    public int speed = 10;
    public float shotFrequency = 1f / 10;

    [SerializeField] MainMenuControl mainMenu;
    [SerializeField] GameObject gameArea;
    [SerializeField] GameObject UI;
    public MoveToPointShot ballCount;
    [SerializeField] GameObject shootingGuide;
    [SerializeField] GameObject rewardAdsMenu;
    [SerializeField] GameObject setting;
    [SerializeField] GameObject cannon;
    [SerializeField] GameObject pauseMenu;

    public Vector3 direction;
    public Vector3 directionBall;

    public int ballForAdd;
    public int coinForAdd;

    public string version;
    public int gameOver;
    public bool allShoot;
    bool shot = false;

    public GameData gameData;

    public string isSpeedLocked = "true";
    public int timeScale = 1;


    public AudioControl audioControl;
    [SerializeField] LookAt camerasc;

    [SerializeField] playerMove player;

    [SerializeField] GameObject mainCamera;

    [SerializeField] Ads ads;
    int subordinates = 0;

    [SerializeField] UnityGameService gameService;


    private void Awake()
    {
        timeScale = 1;
        //Se listan las posisiones iniciales de los cubos.
        posList.Add(pos1);
        posList.Add(pos2);
        posList.Add(pos3);
        posList.Add(pos4);
        posList.Add(pos5);
        posList.Add(pos6);
        posList.Add(pos7);
        foreach (var item in posList)
        {
            occupiedPositionList.Add(false);
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        AdjustCameraZoom();
        gameData = FindObjectOfType<GameData>();
        gameOver = PlayerPrefs.GetInt("GameOver", 0);
        isSpeedLocked = "true";

        //Se invoca repetidamente la funcion del contador de frames.
        //InvokeRepeating("FrameCount", 1, 1);
        //Se establece el objetivo de frame del juego.
        Application.targetFrameRate = PlayerPrefs.GetInt("FPS", 120);
        QualitySettings.vSyncCount = 0;
        //Se muestran los textos inicales en pantalla.
        MultiplierText.text = $"X{speed / 10}";
        score.text = $"Score: {level}";
        record = PlayerPrefs.GetInt("record");
        recordText.text = $"Record: {record}";
        coinsCount.text = coinForAdd.ToString();

        setting.SetActive(true);
    }

    public void PlayGame()
    {
        if (PlayerPrefs.GetInt("GameOver", 0) == 1)
        {
            LoadGame();
        }
        else
        {
            NewGame();
        }
    }

    public void NewGame()
    {
        ballCatches.Clear();
        ballMoving.Clear();
        level = 1;
        if (level > record) PlayerPrefs.SetInt("record", level);
        record = PlayerPrefs.GetInt("record");
        //Se instancia la primera bola.
        ballList.Add(Instantiate(ballPrefab.GetComponent<Ball>()));
        //Establece el estado del juego para preparar el primer nivel.
        currentState = GameState.preparingNewLevel;
        //Se invocan los cubos del primer nivel.
        Invoke("SpawnCube", 0.5f);
        //no hay game over para que se cargue la partida la proxima vez.
        gameOver = 1;
        //Se establece el tipo de movimiento del jugador.
        player.currentMovement = Movement.Transform;
        player.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -1.17f);
        player.agent.enabled = false;
        //Se establece la posision del jugador donde esta la bola.
        player.SetPositionForShot(ballList[0].transform.position);
        //Se establece la posision de la camara donde esta la bola.
        camerasc.MoveCamera(ballList[0].transform.position.x);
        //Mostrar botones de cambiar la velocidad
        ShowButton();
    }

    public void LoadGame()
    {
        ballCatches.Clear();
        ballMoving.Clear();
        //Se llama a la funcion load para recoger los datos guardados y poner los objetos en su lugar.
        gameData.Load();
        //Se establece el estado del juego en cargando para preparar la escena.
        currentState = GameState.Loading;
        //Carga para saber si el jgo se cerro durante un tiro y volver a ejecutarlo.
        Loading();
        //Se establece el tipo de movimiento del jugador.
        player.currentMovement = Movement.Transform;
        player.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -1.17f);
        player.agent.enabled = false;
        //Se establece la posision del jugador donde esta la bola.
        player.SetPositionForShot(ballList[0].transform.position);
        //Se establece la posision de la camara donde esta la bola.
        camerasc.MoveCamera(ballList[0].transform.position.x);

        ballCount.UpdateValues();

        //Mostrar botones de cambiar la velocidad
        ShowButton();
        score.text = $"Score: {level}";
        coinsCount.text = coinForAdd.ToString();
    }

    /// <summary>
    /// Cuenta los frames por segundo en el juego y los actualiza en la pantalla.
    /// </summary>
    void FrameCount()
    {
        frameRate = Time.frameCount - frameCount;
        frameCount = Time.frameCount;
        FPSText.text = frameRate.ToString();
    }
    // Update is called once per frame
    void Update()
    {
        
        //Intercambia las acciones del juego segun el estado actual.
        switch (currentState)
        {
            case GameState.readyForTheShot:
                ReadyForTheShot();
                break;
            case GameState.preparingNewLevel:
                break;
            case GameState.inTheShot:
                InTheShoot();
                break;
            case GameState.gameOver:
                //GameOver();
                break;
            case GameState.Loading:
                break;
            default:
                break;
        }
       
    }
    
    void Loading()
    {
        if (gameData.direction != Vector3.zero)
        {
            directionBall = gameData.direction;
            direction = gameData.direction;
            Invoke("StartShot", 0.5f);
        }
        else
        {
            currentState = GameState.readyForTheShot;
        }
    }
    /// <summary>
    /// Se encarga de ralizar el tiro segun la entrada del usuario.
    /// </summary>
    void ReadyForTheShot()
    {
        if (timeScale == 0) return;

        if (Input.GetMouseButton(0)) // Se hizo clic con el botón izquierdo del mouse
        {
            direction = Input.mousePosition;

            // Convierte la posición del toque de pantalla a un rayo en el mundo 3D
            Ray ray = Camera.main.ScreenPointToRay(direction);

            // Dispara un rayo y realiza un RaycastHit para obtener información del objeto impactado
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.layer == 3)
                {
                    // Obtén la dirección normalizada desde la posición actual de la bola hacia el punto de impacto
                    direction = (hit.point - ballList[0].transform.position).normalized;
                    direction.z = 0;
                    if (direction.y >= 0.2f) directionBall = direction;
                    shootingGuide.SetActive(true);
                    ShootingGuidePos();
                    shot = true;
                    cannon.transform.rotation = shootingGuide.transform.rotation;
                }
                else
                {
                    direction = Vector3.zero;
                    shootingGuide.SetActive(false);
                    shot = false;
                }
            }
            
        }
        if (Input.GetMouseButtonUp(0))//ejecuta el codigo cuando se deja de tocar la pantalla.
        {
            if(shot)
            {
                StartShot();
                shot = false;
            }
        }

    }
    /// <summary>
    /// Update cuando el tiro se esta ejecutando.
    /// </summary>
    void InTheShoot()
    {
        if(cubeList.Count == 0)
        {
            currentState = GameState.Loading;
            if (allShoot)
            {
                CollectAllBalls();
            }
        }
        if (ballCatches.Count >= 1 && player.positionForShot != ballCatches[0].transform.position)
        {
            player.SetPositionForShot(ballCatches[0].transform.position);
            camerasc.MoveCamera(ballCatches[0].transform.position.x);
        }
    }
    void CollectAllBalls()
    {
        foreach (var item in ballList)
        {
            if(item.isMoving)
            {
                RaycastHit hitInfo;
                if (Physics.Raycast(transform.position, new Vector3(0, -3.41f, -0.4f), out hitInfo))
                {
                    item.point = hitInfo.point;
                    //Mantiene la bola en la misma posision en el eje z.
                    item.point = new Vector3(item.point.x, item.point.y, -0.4f);
                    item.target = hitInfo.collider.gameObject;
                }
            }
            
        }
        foreach (var item in powerUpList)
        {
            item.GetComponent<PowerUp>().Invoke("GetPowerUp", 0.01f);
        }
    }
    /// <summary>
    /// Termina la partida.
    /// </summary>
    public void GameOver()
    {
        currentState = GameState.gameOver;
        if(mainMenu.active == false)
        {
            PlayerPrefs.SetInt("GameOver", 0);
            mainMenu.Invoke("ShowButton",2);
            ads.Invoke("ShowInterstitial", 2);
            HideButton();
            camerasc.MoveCamera(0);
            player.SetPositionForShot(Vector3.zero);
            foreach (var item in cubeList)
            {
                Cube x = item.GetComponent<Cube>();
                x.mat.SetColor("_EmissionColor", new Color(32f, 0, 0, 1000f));
                x.text.font = x.fontRed;
                x.Invoke("GameOver", Random.Range(0.1f,2f));
            }
            foreach (var item in powerUpList)
            {
                item.GetComponent<PowerUp>().Invoke("GameOver",0.1f);
            }            

            PlayerPrefs.SetInt("Coin", coinForAdd + PlayerPrefs.GetInt("Coin",0));
            recordText.text = $"Record: {record}";
            score.text = $"Score: {level}";
            gameService.AddScore(level, false);

            gameOver = 0;
            isSpeedLocked = "true";
            //gameData.Save(Vector3.zero, isSpeedLocked);


            foreach (var item in ballList)
            {
                Destroy(item.gameObject, 0.1f);
            }
            ballList.Clear();
            coinForAdd = 0;
            ballForAdd = 0;
            speed = 10;
            shotFrequency = 1f / 10;
            MultiplierText.text = $"X{speed / 10}";
            UpdateCoinText();
            currentState = GameState.Loading;

        }
            
    }
    /// <summary>
    /// Spawn de cubos aleatorios.
    /// </summary>
    void SpawnCube()
    {
        for (int i = 0; i < posList.Count; i++)
        {
            int ranadom = Random.Range(0, 2);
            if(ranadom == 0)
            {
                occupiedPositionList[i] = true;
            }
            else
            {
                occupiedPositionList[i] = false;
            }
        }
        bool allEqual = true;
        foreach (bool value in occupiedPositionList)
        {
            if (value != occupiedPositionList[0])
            {
                allEqual = false;
                break;
            }
        }
        if (allEqual)
        {
            SpawnCube();
        }
        else
        {
            for (int i = 0; i < occupiedPositionList.Count; i++)
            {
                if (occupiedPositionList[i])
                {
                    var cube = Instantiate(cubePrefab, posList[i], transform.rotation);
                    cubeList.Add(cube);
                }
            }
            powerUpList.Add(Instantiate(ballAddPrefab, posList[SpawnAddBallPos()], transform.rotation));

            int CoinPos = CoinSpawnPos();
            if (CoinPos != -1)
            {
                powerUpList.Add(Instantiate(coinPrefab, posList[CoinPos], transform.rotation));
            }

            foreach (var item in powerUpList)
            {
                item.layer = 2;
            }
            if (audioControl.isActiveAndEnabled) audioControl.AppearCube();
            StartCoroutine("MoveDownObject");
        }
       
    }
    int SpawnAddBallPos()
    {
        int random = Random.Range(0, 7);
        if (occupiedPositionList[random] == true)
        {
            return SpawnAddBallPos();
        }
        else
        {
            occupiedPositionList[random] = true;
            return random;
        }
    }

    int CoinSpawnPos()
    {
        int random = Random.Range(0, 7);
        if (occupiedPositionList[random] == false)
        {
            return random;
        }
        else
        {
            return -1;
        }
    }

    /// <summary>
    /// Meve los objetos de la lista hacia abajo.
    /// </summary>
    IEnumerator MoveDownObject()
    {

        foreach (var item in cubeList)
        {
            item.GetComponent<MoveDown>().NewPos();
        }
        foreach (var item in powerUpList)
        {
            item.GetComponent<MoveDown>().NewPos();
        }
        
        ballCatches.Clear();

        yield return new WaitForSeconds(0.1f);
        /*foreach (var item in cubeList)
        {
            bool a = item.GetComponent<Cube>().CheckGameOver();
        }*/
        foreach (var item in powerUpList)
        {
            item.GetComponent<PowerUp>().StartCoroutine("CheckPosition");
        }

        StartCoroutine("SpawnBall");
        StartCoroutine(CheckObjectPosition());
    }

    IEnumerator CheckObjectPosition()
    {
        bool inPosition = true;
        foreach (var item in cubeList)
        {
            
            if (!item.GetComponent<MoveDown>().inPosition)
            {
                inPosition = false;
                yield return new WaitForSeconds(0.1f);
                StartCoroutine(CheckObjectPosition());
                break;
            }
        }

        if(inPosition)
        {
            bool gameOver = false;
            foreach (var item in cubeList)
            {
                if (item.GetComponent<Cube>().CheckGameOver())
                {
                    currentState = GameState.gameOver;
                    gameOver = true;
                    GameOver();
                    break;
                }
            }
            if (!gameOver)
            {
                NewShot();
            }
        }
    }

    public void NewShot()
    {
        if (currentState != GameState.gameOver)
        {
            currentState = GameState.readyForTheShot;
            ballCount.gameObject.SetActive(true);
            ballCount.UpdateValues();

            gameData.Save(Vector3.zero, isSpeedLocked);
            recordText.text = $"Record: {record}";
            score.text = $"Score: {level}";
        }
    }

    IEnumerator SpawnBall()
    {
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < ballForAdd; i++)
        {
            ballList.Add(Instantiate(ballPrefab.GetComponent<Ball>(),
            new Vector3(ballList[0].transform.position.x, -3.41f, -0.4f),
            ballPrefab.transform.rotation));
            ballList[ballList.Count-1].numBall = ballList.Count - 1;
        }
        ballForAdd = 0;
        ballCount.UpdateValues();
    }
    public Vector3 saveDirection;
    /// <summary>
    /// Lanza todas las bolas de la lista.
    /// </summary>
    /// <param name="direction">Recibe la direccion a la q debe moverse la bola</param>
    /// <returns>Espera un tiempo determinado para lanzar la siguiebte bola.</returns>
    public IEnumerator ShootBalls(Vector3 direction)
    {
        while (timeScale == 0)
        {
            yield return null;
        }
        gameData.Save(direction, isSpeedLocked);
        saveDirection = direction;
        currentState = GameState.inTheShot;
        foreach (var item in powerUpList)
        {
            item.layer = 0;
        }
        allShoot = false;

        subordinates = 0;
        for (int i = 0; i < ballList.Count; i++)
        {
            if (currentState == GameState.inTheShot)
            {
                ballList[i].gameObject.SetActive(true);
                ballList[i].ShootBall(direction);
                ballMoving.Add(ballList[i]);
                ballCount.SubtractBall();

                if (speed < 80)
                {
                    yield return new WaitForSeconds(shotFrequency);
                }
                else if(speed == 80)
                {
                    yield return new WaitForFixedUpdate();
                }
                else if(speed == 160)
                {
                    if(subordinates < 1)
                    {
                        subordinates++;
                        continue;
                    }
                    else
                    {
                        ballList[i].isSubordinate = true;
                        ballList[i-subordinates].subordinates.Add(ballList[i]);
                        subordinates = 0;
                        yield return new WaitForFixedUpdate();
                    }
                    
                }
                else if (speed == 320)
                {

                    if (subordinates == 0)
                    {
                        subordinates++;
                    }
                    else if(subordinates < 3)
                    {
                        ballList[i].isSubordinate = true;
                        ballList[i - subordinates].subordinates.Add(ballList[i]);
                        subordinates++;
                    }
                    else if(subordinates == 3)
                    {
                        ballList[i].isSubordinate = true;
                        ballList[i - subordinates].subordinates.Add(ballList[i]);
                        subordinates = 0;
                        yield return new WaitForFixedUpdate();
                    }
                    
                    
                }
                else if(speed == 640)
                {
                    if (subordinates == 0)
                    {
                        subordinates++;
                    }
                    else if (subordinates < 7)
                    {
                        ballList[i].isSubordinate = true;
                        ballList[i - subordinates].subordinates.Add(ballList[i]);
                        subordinates++;
                    }
                    else if (subordinates == 7)
                    {
                        ballList[i].isSubordinate = true;
                        ballList[i - subordinates].subordinates.Add(ballList[i]);
                        subordinates = 0;
                        yield return new WaitForFixedUpdate();
                    }
                }
            }
            else
            {
                ballList[i].gameObject.SetActive(true);
                ballList[i].ShootBall(direction);
                ballMoving.Add(ballList[i]);
                ballCount.SubtractBall();
            }
            while (timeScale == 0)
            {
                yield return null;
            }
        }
        
        allShoot = true;

        if (currentState == GameState.Loading)
        {
            CollectAllBalls();
        }
    }

    /// <summary>
    /// Comienza un nuevo nivel.
    /// </summary>
    public void NewLevel()
    {
        currentState = GameState.preparingNewLevel;
        player.SetPositionForShot(ballList[0].transform.position);
        camerasc.MoveCamera(ballList[0].transform.position.x);
        level++;
        if (level % 50 == 0) ads.ShowInterstitial();
        if (level > record) PlayerPrefs.SetInt("record", level);
        record = PlayerPrefs.GetInt("record");
        Invoke("SpawnCube", 0.5f);
    }


    /// <summary>
    /// Disminiye la velocidad de las bolas.
    /// </summary>
    public void MinusSpeed()
    {
        if (speed > 10)
        {
            speed /= 2;
            shotFrequency = 1f / speed;
            MultiplierText.text = $"X{speed / 10}";
            foreach (var item in ballList)
            {
                item.UpdateSpeed();
            }
            subordinates = 0;
        }
    }
    /// <summary>
    /// Aumenta la belocidad de las bolas.
    /// </summary>
    public void MaxSpeed()
    {
        isSpeedLocked= "false";
        if(isSpeedLocked == "false")
        {
            if (speed < 640)
            {
                speed *= 2;
                shotFrequency = 1f / speed;
                MultiplierText.text = $"X{speed / 10}";
                foreach (var item in ballList)
                {
                    item.UpdateSpeed();
                }
                subordinates = 0;
            }
        }
        else
        {
            rewardAdsMenu.SetActive(true);
            timeScale = 0;
        }
    }

    public void RewardAd()
    {
        isSpeedLocked = "false";
        MaxSpeed();
    }

    
    
    /// <summary>
    /// Comienza el tiro.
    /// </summary>
    void StartShot()
    {
        if(directionBall.y >= 0.2f)
        {
            ballCount.gameObject.SetActive(true);
            ballCount.UpdateValues();
            shootingGuide.SetActive(false);
            StartCoroutine(ShootBalls(directionBall));
            direction = Vector3.zero;
        }
        
    }
    /// <summary>
    /// Actualiza la posision de la guia de tiro.
    /// </summary>
    void ShootingGuidePos()
    {
        shootingGuide.transform.up = directionBall;
        shootingGuide.transform.position = ballList[0].transform.position;
        
        if (directionBall.y < 0.2f)
        {
            if (directionBall.x < 0)
            {
                directionBall = new Vector3(-1, 0.2f, directionBall.z);
            }
            else
            {
                directionBall = new Vector3(1, 0.2f, directionBall.z);
            }

        }
    }

    public void UpdateCoinText()
    {
        coinsCount.text = coinForAdd.ToString();
    }

    void AdjustCameraZoom()
    {
        float currentAspectRatio = (float)Screen.height / Screen.width;

        if (currentAspectRatio <= 16f / 9f)
        {
            mainCamera.transform.position = new Vector3(mainCamera.transform.position.x,
                mainCamera.transform.position.y, -14);
        }
        else if(currentAspectRatio <= 18f / 9f)
        {
            mainCamera.transform.position = new Vector3(mainCamera.transform.position.x,
                mainCamera.transform.position.y, -15f);
        }
        else if (currentAspectRatio <= 20f / 9f)
        {
            mainCamera.transform.position = new Vector3(mainCamera.transform.position.x,
                mainCamera.transform.position.y, -15.5f);
        }
    }

    public void ShowButton()
    {
        foreach (var button in ButtonScaleList)
        {
            if (button.gameObject.activeInHierarchy)
            {
                button.ShowButton();
            }
        }
    }

    public void HideButton()
    {
        foreach (var button in ButtonScaleList)
        {
            if (button.gameObject.activeInHierarchy)
            {
                button.HideButton();
            }
        }
    }

    public void MainMenu()
    {
        StartCoroutine(MainMenuCoroutine());
    }

    IEnumerator MainMenuCoroutine()
    {
        while (currentState == GameState.preparingNewLevel)
        {
            yield return null;
        }

        pauseMenu.SetActive(false);
        currentState = GameState.Loading;

        camerasc.MoveCamera(0);

        foreach (var item in cubeList)
        {
            item.GetComponent<Cube>().Invoke("GameOver", 0.1f);
        }
        foreach (var item in powerUpList)
        {
            item.GetComponent<PowerUp>().Invoke("GameOver", 0.1f);
        }

        foreach (var item in ballList)
        {
            Destroy(item.gameObject, 0.1f);
        }

        coinForAdd = 0;
        ballForAdd = 0;
        speed = 10;
        shotFrequency = 1f / 10;
        MultiplierText.text = $"X{speed / 10}";
        UpdateCoinText();
        HideButton();
        mainMenu.Invoke("ShowButton", 0);
        ballCount.gameObject.SetActive(false);
        ballList.Clear();
        timeScale = 1;
    }

    public ScaleButton continueButton;
    public ScaleButton retryButton;

    public void Pause()
    {
        timeScale = 0;
        pauseMenu.SetActive(true);
    }

    public void Continue()
    {
        timeScale = 1;
        pauseMenu.SetActive(false);
    }

    public void Restart()
    {
        pauseMenu.SetActive(false);
        level = 1;
        score.text = $"Score: {level}";
        camerasc.MoveCamera(0);

        foreach (var item in cubeList)
        {
            item.GetComponent<Cube>().Invoke("GameOver", 0.1f);
        }
        foreach (var item in powerUpList)
        {
            item.GetComponent<PowerUp>().Invoke("GameOver",0.1f);
        }

        foreach (var item in ballList)
        {
            Destroy(item.gameObject, 0.1f);
        }
        coinForAdd = 0;
        ballForAdd = 0;
        speed = 10;
        shotFrequency = 1f / 10;
        MultiplierText.text = $"X{speed / 10}";
        UpdateCoinText();
        ballList.Clear();
        ballCatches.Clear();
        ballMoving.Clear();
        timeScale = 1;
        NewGame();
    }
}

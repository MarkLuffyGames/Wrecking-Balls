using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static GameManager;

public class GameData : MonoBehaviour
{
    public GameManager gameManager;
    public Vector3 direction = Vector3.zero;
    public void Save(Vector3 direction, string isSpeedLocked)
    {
        PlayerPrefs.SetString("isSpeedLocked", isSpeedLocked);

        PlayerPrefs.SetFloat("DirectionX", direction.x);
        PlayerPrefs.SetFloat("DirectionY", direction.y);
        PlayerPrefs.SetFloat("DirectionZ", direction.z);

        gameManager = FindObjectOfType<GameManager>();

        //Guardar la cantidad de monedas acumuladas en la partida actual.
        PlayerPrefs.SetInt("CoinForAdd", gameManager.coinForAdd);

        // Guardar la cantidad de bolas restantes.
        PlayerPrefs.SetInt("Balls", gameManager.ballList.Count);
        PlayerPrefs.SetFloat("BallPosX", gameManager.ballList[0].transform.position.x);
        PlayerPrefs.SetInt("Level", gameManager.level);
        PlayerPrefs.SetInt("GameOver", gameManager.gameOver);
        if(gameManager.level>gameManager.record)PlayerPrefs.SetInt("record", gameManager.level);
        int i = 0;
        while (PlayerPrefs.HasKey("BlockX" + i))
        {
            PlayerPrefs.DeleteKey("BlockX" + i);
            PlayerPrefs.DeleteKey("BlockY" + i);
            PlayerPrefs.DeleteKey("BlockZ" + i);
            i++;
        }
        i = 0;
        while (PlayerPrefs.HasKey("PowerUpX" + i))
        {
            PlayerPrefs.DeleteKey("PowerUpX" + i);
            PlayerPrefs.DeleteKey("PowerUpY" + i);
            PlayerPrefs.DeleteKey("PowerUpZ" + i);
            PlayerPrefs.DeleteKey("Type" + i);
            i++;
        }
        // Guardar la información de los bloques.
        for (i = 0; i < gameManager.cubeList.Count; i++)
        {
            GameObject block = gameManager.cubeList[i];
            
            PlayerPrefs.SetFloat("BlockX" + i, block.transform.position.x);
            PlayerPrefs.SetFloat("BlockY" + i, block.transform.position.y);
            PlayerPrefs.SetFloat("BlockZ" + i, block.transform.position.z);
            PlayerPrefs.SetInt("BlockHealth" + i, block.GetComponent<Cube>().point);

        }
        for (i = 0; i < gameManager.powerUpList.Count; i++)
        {
            GameObject powerUp = gameManager.powerUpList[i];
            
            PlayerPrefs.SetFloat("PowerUpX" + i, powerUp.transform.position.x);
            PlayerPrefs.SetFloat("PowerUpY" + i, powerUp.transform.position.y);
            PlayerPrefs.SetFloat("PowerUpZ" + i, powerUp.transform.position.z);
            PlayerPrefs.SetString("Type"+i, powerUp.GetComponent<PowerUp>().type);
        }
        
        PlayerPrefs.Save();
    }
    public void Load()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.level = PlayerPrefs.GetInt("Level", 1);
        gameManager.record = PlayerPrefs.GetInt("record",1);
        gameManager.isSpeedLocked = PlayerPrefs.GetString("isSpeedLocked");
        gameManager.coinForAdd = PlayerPrefs.GetInt("CoinForAdd", 0);

        // Cargar la cantidad de bolas restantes.
        int i = 0;
        for (i = 0; i < PlayerPrefs.GetInt("Balls"); i++)
        {
            Vector3 position = new Vector3( PlayerPrefs.GetFloat("BallPosX"), -3.41f, -0.4f);
            Ball ball = Instantiate(gameManager.ballPrefab.GetComponent<Ball>());
            ball.transform.position = position;
            gameManager.ballList.Add(ball);
            ball.numBall = gameManager.ballList.Count -1;
        }
        // Cargar la información de los bloques.
        gameManager.cubeList = new List<GameObject>();
        i = 0;
        while (PlayerPrefs.HasKey("BlockX" + i))
        {
            Vector3 position = new Vector3(
                PlayerPrefs.GetFloat("BlockX" + i),
                PlayerPrefs.GetFloat("BlockY" + i),
                PlayerPrefs.GetFloat("BlockZ" + i));
            int health = PlayerPrefs.GetInt("BlockHealth" + i);
            GameObject cube = Instantiate(gameManager.cubePrefab, position, Quaternion.identity);
            cube.GetComponent<Cube>().point = health;
            cube.GetComponent<MoveDown>().targetPos = position;
            gameManager.cubeList.Add(cube);
            i++;
        }
        i = 0;
        while (PlayerPrefs.HasKey("PowerUpX" + i))
        {
            Vector3 position = new Vector3(
                PlayerPrefs.GetFloat("PowerUpX" + i),
                PlayerPrefs.GetFloat("PowerUpY" + i),
                PlayerPrefs.GetFloat("PowerUpZ" + i));
            GameObject powerUp;
            if (PlayerPrefs.GetString("Type" + i) == "AddBall")
            {
                powerUp = Instantiate(gameManager.ballAddPrefab, position, Quaternion.identity);
            }
            else
            {
                powerUp = Instantiate(gameManager.coinPrefab , position, Quaternion.identity);
            }
            powerUp.GetComponent<MoveDown>().targetPos = position;
            powerUp.layer = 2;
            gameManager.powerUpList.Add(powerUp);
            i++;
        }
        gameManager.ballCount.gameObject.SetActive(true);


        direction = new Vector3(
            PlayerPrefs.GetFloat("DirectionX"),
            PlayerPrefs.GetFloat("DirectionY"),
            PlayerPrefs.GetFloat("DirectionZ"));
    }
}

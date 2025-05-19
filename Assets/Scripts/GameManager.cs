using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject wall;
    [SerializeField] GameObject coin;
    [SerializeField] GameObject obsticle;
    [SerializeField] GameObject illusion;
    [SerializeField] GameObject selectDifficulty;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] TextMeshProUGUI statusText;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI gameOverText;
    [SerializeField] GameObject restartButton;
    [SerializeField] TextMeshProUGUI infoText;
    float wallZ = 0f;
    float obsticleAndCoinZ = 15f;
    float spawndistance = 120f;
    private float laneDistance = 1.5f;
    private int minObsticleCountPerRow = 3;
    private int maxObsticleCountPerRow = 4;
    private int minCoinCountPerRow = 1;
    private int maxCoinCountPerRow = 2;
    private float illusionRate;
    private int coinCount = 0;
    private long life = 1;
    private bool isGameStarted = false;
    private bool isGamePaused = false;
    private float gameStartTime;
    private float lastTimeDamaged;
    private float immunityDuration = 1;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameStarted)
        {
            if (player.gameObject.transform.position.z + spawndistance > wallZ)
            {
                SpawnWall();
            }
            if (player.gameObject.transform.position.z + spawndistance > obsticleAndCoinZ)
            {
                SpawnObsticlesAndCoins();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Pause(!isGamePaused);
            }
        }
        if (life <= 0) GameOver();

        statusText.SetText("Coins: " + coinCount + "\nDist: " + (int)player.transform.position.z);

        DisableTextAfterSeconds(infoText, gameStartTime, 5);
    }
    private void SpawnWall()
    {
        Instantiate(wall,
            new Vector3(8f, -3f, wallZ),
            wall.transform.rotation);
        Instantiate(wall,
            new Vector3(-8f, -3f, wallZ),
            wall.transform.rotation);
        wallZ += 7.5f;
    }

    private void SpawnObsticlesAndCoins()
    {
        SpawnObsticle(Random.Range(minObsticleCountPerRow,maxObsticleCountPerRow + 1));
        SpawnCoin(Random.Range(minCoinCountPerRow,maxCoinCountPerRow + 1));
        obsticleAndCoinZ += 5;
    }

    private void SpawnObsticle(int ObsticlesPerRow)
    {
        List<int> xPosList = new();
        int randomPos;
        while (xPosList.Count < ObsticlesPerRow)
        {
            randomPos = Random.Range(-2, 3);
            if (!xPosList.Contains(randomPos))
                xPosList.Add(randomPos);
        }
        foreach (var xPos in xPosList)
        {
            if (Random.Range(0f, 1f) < illusionRate)
            {
                Instantiate(illusion,
                new Vector3(xPos * laneDistance, 0, obsticleAndCoinZ),
                illusion.transform.rotation);
            }
            else
            {
                Instantiate(obsticle,
                new Vector3(xPos * laneDistance, 0, obsticleAndCoinZ),
                obsticle.transform.rotation);
            }
        }
    }

    private void SpawnCoin(int CoinsPerRow)
    {
        List<int> xPosList = new();
        int randomPos;
        while (xPosList.Count < CoinsPerRow)
        {
            randomPos = Random.Range(-2, 3);
            if (!xPosList.Contains(randomPos))
                xPosList.Add(randomPos);
        }
        foreach (var xPos in xPosList)
        {
            Instantiate(coin,
            new Vector3(xPos * laneDistance, 3f, obsticleAndCoinZ),
            coin.transform.rotation);
        }
    }
    private void GameOver()
    {
        player.GetComponent<PlayerControl>().SetMovement(false);
        gameOverText.gameObject.SetActive(true);
        statusText.gameObject.SetActive(false);
        gameOverText.SetText("Game Over\nCoins: " + coinCount + "\nDistance: " + (int)player.transform.position.z);
        restartButton.SetActive(true);
    }

    public void AddCoin()
    {
        coinCount++;
    }
    public void RemoveLife()
    {
        if (Time.time > lastTimeDamaged + immunityDuration)
        {
            life--;
            healthText.SetText("Health: " + life);
            lastTimeDamaged = Time.time;
        }
    }

    public void SetGodMode()
    {
        SetEasy();
        life = long.MaxValue;
        spawndistance = 4000;
        player.GetComponent<PlayerControl>().SetPlayerSpeed(400);
        StartGame();
    }

    public void SetEasy()
    {
        life = 6;
        minObsticleCountPerRow = 1;
        maxObsticleCountPerRow = 3;
        minCoinCountPerRow = 1;
        maxCoinCountPerRow = 3;
        illusionRate = 0.2f;
        player.GetComponent<PlayerControl>().SetJumpResetTime(0);
        StartGame();
    }
    public void SetMedium()
    {
        life = 4;
        minObsticleCountPerRow = 1;
        maxObsticleCountPerRow = 4;
        minCoinCountPerRow = 1;
        maxCoinCountPerRow = 2;
        illusionRate = 0.15f;
        infoText.gameObject.SetActive(true);
        StartGame();
    }
    public void SetHard()
    {
        life = 2;
        minObsticleCountPerRow = 3;
        maxObsticleCountPerRow = 4;
        minCoinCountPerRow = 0;
        maxCoinCountPerRow = 2;
        illusionRate = 0.1f;
        infoText.gameObject.SetActive(true);
        StartGame();
    }

    private void StartGame()
    {
        selectDifficulty.gameObject.SetActive(false);
        statusText.gameObject.SetActive(true);
        healthText.gameObject.SetActive(true);
        player.GetComponent<PlayerControl>().SetMovement(true);
        isGameStarted = true;
        gameStartTime = Time.time;
        healthText.SetText("Health: " + life);
    }

    private void DisableTextAfterSeconds(TextMeshProUGUI text,float start,float duration)
    {
        if (Time.time - start > duration) text.gameObject.SetActive(false);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Pause(bool isPaused)
    {
        isGamePaused = isPaused;
        if (isPaused) 
        {
            //player.GetComponent<PlayerControl>().SetMovement(false);
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }
        else
        {
            //player.GetComponent<PlayerControl>().SetMovement(true);
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
        }
    }

    public float GetLaneDistance() { return laneDistance; }

    public bool IsGameStarted() { return isGameStarted; }

    public bool IsGamePaused() { return isGamePaused; }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Referenced Components")]
    public PlayerController playerControllerPrefab;
    public TMPro.TextMeshProUGUI currentScoreTMP;
    public TMPro.TextMeshProUGUI highScoreTMP;

    [Space(10)]

    public Vector2 spawningAreaCenter = Vector2.zero;
    public Vector2 spawnAreaSize = new Vector2(1, 1);

    public int initialSpawnCount = 10;
    public float spawnFrequency = 1.0f;

    private float spawnTimer = 0f;

    private bool initialized = false;

    private const float kScoreDecimalOffset = 100f;

    private static GameManager instance;

    public static bool TryGetInstance(out GameManager gameManagerInstance)
    {
        bool success = false;
        gameManagerInstance = null;

        if (instance != null)
        {
            success = true;
            gameManagerInstance = instance;
        }

        return success;
    }

    #region MonoBehaviour

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && !initialized)
        {
            InitializeGame();
        }

        if (initialized)
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= spawnFrequency)
            {
                SpawnPlayer();
                spawnTimer -= spawnFrequency;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawCube(Vector3.zero, new Vector3(spawnAreaSize.x, spawnAreaSize.y, 1));
    }

    #endregion

    public void UpdateScore(float currentScore, float highestScore)
    {
        if(currentScoreTMP != null)
        {
            currentScoreTMP.text = (currentScore * kScoreDecimalOffset).ToString("0");
        }

        if (highScoreTMP != null)
        {
            highScoreTMP.text = (highestScore * kScoreDecimalOffset).ToString("0");
        }
    }

    #region Private Methods

    private void SpawnPlayer()
    {
        Instantiate(playerControllerPrefab, new Vector3(Random.Range(spawningAreaCenter.x - spawnAreaSize.x / 2f, spawningAreaCenter.x + spawnAreaSize.x / 2f),
            Random.Range(spawningAreaCenter.y - spawnAreaSize.y / 2f, spawningAreaCenter.y + spawnAreaSize.y / 2f), 0f), Quaternion.identity);
    }

    private void InitializeGame()
    {
        initialized = true;

        for (int i = 0; i < initialSpawnCount; i++)
        {
            SpawnPlayer();
        }
    }

    #endregion
}

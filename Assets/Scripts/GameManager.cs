using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake() 
    {
        Instance = this;    
    }
    
    [Header("Unit Parent")]
    public Transform weaponLootParent;
    public Transform characterParent;

    [Header("Spawn")]
    [SerializeField] private Vector2 minMax_X, minMax_Z;
    [SerializeField] private GameObject rifleLootPrefab;
    [SerializeField] private GameObject ammoLootPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject dummyEnemyPrefab;


    [Header("Progress BattleRoyale")]
    public int totalAliveCharacters;
    public int currentAliveCharacters;

    public int CurrentAliveCharacters
    {
        get => currentAliveCharacters;
        set
        {
            currentAliveCharacters = value;
            characterAliveText.text = $"Total Alive : {currentAliveCharacters} / {totalAliveCharacters}";
            if (currentAliveCharacters <= 1) 
            {
                playerWin = true;
                GameOver();
            }
        }
    }

    [Header("Game Over")]
    public TextMeshProUGUI characterAliveText;
    public GameObject gameOverCanvas;
    public TextMeshProUGUI rankText;
    public GameObject winCanvas;
    public GameObject loseCanvas;

    public bool playerWin;

    private void Start()
    {
        CurrentAliveCharacters = totalAliveCharacters;
        SpawnLogic();
    }

    private void Update() 
    {
        RestartGame();    
    }

    public void DecreaseAliveCharacter(Transform deadCharacter)
    {
        deadCharacter.parent = null;
        CurrentAliveCharacters--;
    }

    public void GameOver()
    {
        Time.timeScale = 0.5f;

        gameOverCanvas.SetActive(true);
        
        if (playerWin)
        {
            rankText.text = $"Rank : <color=green>1</color> / {totalAliveCharacters}";
            winCanvas.SetActive(true);
        }
        else
        {
            rankText.text = $"Rank : <color=red>{CurrentAliveCharacters + 1}</color> / {totalAliveCharacters}";
            loseCanvas.SetActive(true);
        }
        print("Game Over");
    }

    public void RestartGame()
    {
        if (!gameOverCanvas.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void SpawnLogic()
    {
        List<Transform> dummyEnemyTransforms = new List<Transform>();
        for (int i = 0; i < totalAliveCharacters; i++)
        {
            GameObject enemyDummy = SpawnObjectAtRandomPos(dummyEnemyPrefab);
            dummyEnemyTransforms.Add(enemyDummy.transform);
            // enemy.transform.parent = characterParent;

            GameObject rifleLoot = SpawnObjectAtRandomPos(rifleLootPrefab);
            rifleLoot.transform.parent = weaponLootParent;
            
            SpawnObjectAtRandomPos(ammoLootPrefab);
        }

        StartCoroutine(SpawnRealEnemyCoroutine(dummyEnemyTransforms));
    }

    IEnumerator SpawnRealEnemyCoroutine(List<Transform> dummyEnemyTransforms)
    {
        yield return new WaitForSeconds(1.5f);
        foreach (var dummyEnemyTransform in dummyEnemyTransforms)
        {
            Vector3 dummyPos = dummyEnemyTransform.position;
            Destroy(dummyEnemyTransform.gameObject);
            var enemy= Instantiate(enemyPrefab, dummyPos, Quaternion.identity);
            enemy.transform.parent = characterParent;
        }
    }

    public GameObject SpawnObjectAtRandomPos(GameObject objectToSpawn)
    {
        var randomSpawnPos = new Vector3(UnityEngine.Random.Range(minMax_X.x, minMax_X.y), 5, UnityEngine.Random.Range(minMax_Z.x, minMax_Z.y));
        return Instantiate(objectToSpawn, randomSpawnPos, Quaternion.identity);
    }

}

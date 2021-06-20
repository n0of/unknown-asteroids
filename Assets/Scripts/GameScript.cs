using System;
using System.Collections;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    public static GameScript instance;

    public GameObject level;
    public GameObject playerObject;
    public GameObject objectsPool;
    public GameObject menu;
    public GameObject audioHandler;
    public GameScriptableObject gameScriptableObject;

    [NonSerialized]
    public GameObject currentPlayer = null;

    private GameObject _currentLevel;
    private GameObject _objectPool;


    void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
        GameState.InitState(gameScriptableObject);
        _objectPool = Instantiate(objectsPool);
    }

    void Update()
    {
        if (!ObjectsPool.instance.isInited || GameState.IsPaused)
            return;
        if (!currentPlayer.activeSelf)
            StopLevel();
        if (!currentPlayer.activeSelf || Input.GetKey(KeyCode.Escape))
            menu.gameObject.SetActive(true);
        if (!ObjectsPool.instance.HasEnemies())
        {
            GameState.Level++;
            StartNewLevel(2);
        }
    }

    public void StartGame()
    {
        ObjectsPool.instance.DisableObjects();
        GameState.ResetState(gameScriptableObject);
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
            currentPlayer = null;
        }
        if (currentPlayer == null)
            currentPlayer = Instantiate(playerObject);

        StartNewLevel();
    }

    private void StartNewLevel(int delayS = 0)
    {
        if (_currentLevel != null)
            Destroy(_currentLevel);
        var neededTrees = GameState.Settings.AsteroidsInitCount + GameState.Level;
        var currentTrees = ObjectsPool.instance.treesCount;
        for (; currentTrees < neededTrees; currentTrees++)
            ObjectsPool.instance.AddAsteroidsTree();

        StartCoroutine(DelayedLevelStart(delayS));
    }

    private void StopLevel()
    {
        ObjectsPool.instance.StopAllCoroutines();
        ObjectsPool.instance.isInited = false;
    }

    private IEnumerator DelayedLevelStart(int delayS)
    {
        StopLevel();
        yield return new PausedYield(delayS);
        _currentLevel = Instantiate(level);
        ObjectsPool.instance.SpawnEnemy();
    }
}

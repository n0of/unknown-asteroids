using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectsPool : MonoBehaviour
{
    public static ObjectsPool instance;
    public bool isInited = false;
    public int treesCount = 0;

    public GameObject asteroidObject;
    public GameObject enemyObject;
    public GameObject bulletObject;

    private GameObject _enemy;
    private Dictionary<ShooterId, List<(GameObject, Collider2D)>> _bulletsById = new Dictionary<ShooterId, List<(GameObject, Collider2D)>>();
    private Dictionary<int, List<(GameObject, Collider2D)>> _asteroidsByLevel = new Dictionary<int, List<(GameObject, Collider2D)>>();

    private readonly int _initBulletsCount = 10;

    private void Awake()
    {
        instance = this;

        for (var i = 0; i < GameState.Settings.AsteroidsInitCount; i++)
            AddAsteroidsTree();

        foreach (var id in (ShooterId[])Enum.GetValues(typeof(ShooterId)))
        {
            _bulletsById.Add(id, new List<(GameObject, Collider2D)>());
            for (var i = 0; i < _initBulletsCount; i++)
            {
                var bullet = CreateInactiveObject(bulletObject);
                BulletScript.SetColor(bullet.Item1, id);
                _bulletsById[id].Add(bullet);
            }
        }

        _enemy = CreateInactiveObject(enemyObject).Item1;
    }

    public void AddAsteroidsTree()
    {
        var subAsteroidsCount = GameState.Settings.SubAsteroidsCount;
        CreateAsteroids(1, 0, 1);
        if (subAsteroidsCount != 0)
        {
            CreateAsteroids(subAsteroidsCount, 1, 1f / subAsteroidsCount);
            CreateAsteroids(subAsteroidsCount * subAsteroidsCount, 2, 1f / (subAsteroidsCount * subAsteroidsCount));
        }
        treesCount++;
    }

    public void DisableObjects()
    {
        foreach (var bullets in _bulletsById)
            foreach (var bullet in bullets.Value)
                bullet.Item1.SetActive(false);

        foreach (var asteroids in _asteroidsByLevel)
            foreach (var asteroid in asteroids.Value)
                asteroid.Item1.SetActive(false);

        _enemy.SetActive(false);
    }

    public List<(GameObject, Collider2D)> GetAsteroidsByLevel(int level)
    {
        _asteroidsByLevel.TryGetValue(level, out var result);
        return result;
    }

    public GameObject GetAsteroidByLevel(int level)
    {
        var asteroidsByLevel = GetAsteroidsByLevel(level);
        return asteroidsByLevel?.FirstOrDefault(x => !x.Item1.activeSelf).Item1;
    }

    public bool HasEnemies()
    {
        return _asteroidsByLevel.Any(x => x.Value.Any(y => y.Item1.activeSelf)) || _enemy.activeSelf;
    }

    public GameObject GetBullet(ShooterId id)
    {
        var firstInactiveBullet = _bulletsById[id].FirstOrDefault(x => !x.Item1.activeSelf);
        if (firstInactiveBullet.Item1 == null)
        {
            firstInactiveBullet = CreateInactiveObject(bulletObject);
            BulletScript.SetColor(firstInactiveBullet.Item1, id);
            _bulletsById[id].Add(firstInactiveBullet);
        }
        return firstInactiveBullet.Item1;
    }

    public ShooterId? IsHitBySomeBullet(Collider2D collider, ShooterId? id = null)
    {
        foreach (var keyValue in _bulletsById)
        {
            if (keyValue.Key == id && !GameState.Settings.isFriendlyFire)
                continue;

            var bullet = keyValue.Value.FirstOrDefault(x => IsActiveHit(x, collider));
            if (bullet.Item1 != null)
            {
                bullet.Item1.SetActive(false);
                return keyValue.Key;
            }
        }
        return null;
    }

    public bool IsHitBySomeAsteroid(Collider2D collider)
    {
        foreach (var keyValue in _asteroidsByLevel)
        {
            var asteroid = keyValue.Value.FirstOrDefault(x => IsActiveHit(x, collider));
            if (asteroid.Item1 != null)
            {
                asteroid.Item1.SetActive(false);
                return true;
            }
        }
        return false;
    }

    public void SpawnEnemy()
    {
        StartCoroutine(EnemySpawnTimer());
    }

    public IEnumerator EnemySpawnTimer()
    {
        yield return new PausedYield(UnityEngine.Random.Range(GameState.Settings.EnemyMinSpawnDelayS, GameState.Settings.EnemyMaxSpawnDelayS));
        _enemy.SetActive(true);
    }

    private void CreateAsteroids(int count, int level, float scale)
    {
        if (!_asteroidsByLevel.ContainsKey(level))
            _asteroidsByLevel.Add(level, new List<(GameObject, Collider2D)>());
        for (var i = 0; i < count; i++)
            _asteroidsByLevel[level].Add(CreateInactiveObject(asteroidObject, scale));
    }

    private bool IsActiveHit((GameObject, Collider2D) gameObject, Collider2D collider)
    {
        return gameObject.Item1.activeSelf && gameObject.Item2.bounds.Intersects(collider.bounds);
    }

    private (GameObject, Collider2D) CreateInactiveObject(GameObject gameObject, float scale = 1)
    {
        var result = Instantiate(gameObject, transform);
        result.transform.localScale = new Vector2(scale, scale);
        result.SetActive(false);

        return (result, result.GetComponent<Collider2D>());
    }
}

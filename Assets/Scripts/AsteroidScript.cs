using System.Collections.Generic;
using UnityEngine;

public class AsteroidScript : BaseObject
{
    public int level;

    private static readonly Dictionary<int, int> HitAwards = new Dictionary<int, int>()
    {
        { 0, 20 },
        { 1, 50 },
        { 2, 100 }
    };

    protected override void OnStart()
    {
        Collider = transform.GetComponent<Collider2D>();
        CanBeHitByBullets = true;
    }

    protected override void OnHit(ShooterId? id)
    {
        if (id == ShooterId.Player)
        {
            HitAwards.TryGetValue(level, out var hitAward);
            GameState.Score += hitAward;
        }
        if (GameState.Settings.SubAsteroidsCount > 0)
            CreateSubAsteroids();
        
        gameObject.SetActive(false);
    }

    private void CreateSubAsteroids()
    {
        var turns = GameState.Settings.SubAsteroidsCount - 1;
        var turnRate = GameState.Settings.AsteroidSplatterAngle / (turns == 0 ? 1 : turns);
        var turn = 0f;
        var subAsteroidsSpeed = Random.Range(GameState.Settings.AsteroidMinSpeed, GameState.Settings.AsteroidMaxSpeed);
        for (var i = 0; i < GameState.Settings.SubAsteroidsCount; i++)
        {
            var subAsteroid = ObjectsPool.instance.GetAsteroidByLevel(level + 1);
            if (subAsteroid == null)
                continue;

            subAsteroid.transform.position = transform.position;
            subAsteroid.transform.rotation = Quaternion.identity;
            subAsteroid.transform.Rotate(new Vector3(0, 0, 1), transform.rotation.eulerAngles.z - GameState.Settings.AsteroidSplatterAngle / 2 + turn);
            SetProps(subAsteroid, level + 1, subAsteroidsSpeed);
            turn += turnRate;
            subAsteroid.SetActive(true);
        }
    }

    public static void SetProps(GameObject asteroid, int level, float speed)
    {
        var asteroidScript = asteroid.GetComponent<AsteroidScript>();
        asteroidScript.level = level;
        asteroidScript.Speed = speed;
    }
}

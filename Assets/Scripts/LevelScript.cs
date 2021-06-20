using UnityEngine;

public class LevelScript : MonoBehaviour
{
    void Start()
    {
        var initAsteroids = ObjectsPool.instance.GetAsteroidsByLevel(0);
        for (var i = 0; i < GameState.Settings.AsteroidsInitCount + GameState.Level; i++)
        {
            var asteroid = initAsteroids[i];
            asteroid.Item1.transform.position = GetSpawnPoint();
            asteroid.Item1.transform.rotation = Quaternion.identity;
            asteroid.Item1.transform.Rotate(new Vector3(0, 0, 1), Random.Range(-180, 180));
            AsteroidScript.SetProps(asteroid.Item1, 0, Random.Range(GameState.Settings.AsteroidMinSpeed, GameState.Settings.AsteroidMaxSpeed));
            asteroid.Item1.SetActive(true);
        }
        ObjectsPool.instance.isInited = true;
    }

    private Vector2 GetSpawnPoint()
    {
        var screenTopLeft = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
        var screenBottomRight = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        var spawnBorder = Random.Range(0, 3);
        switch (spawnBorder)
        {
            case (int)SpawnBorder.Top:
            case (int)SpawnBorder.Bottom:
                var x = Random.Range(screenTopLeft.x, screenBottomRight.x);
                var y = spawnBorder == (int)SpawnBorder.Top ? screenTopLeft.y : screenBottomRight.y;
                return new Vector2(x, y);
            case (int)SpawnBorder.Right:
            case (int)SpawnBorder.Left:
            default:
                x = spawnBorder == (int)SpawnBorder.Left ? screenTopLeft.x : screenBottomRight.x;
                y = Random.Range(screenTopLeft.y, screenBottomRight.y);
                return new Vector2(x, y);
        }
    }

    private enum SpawnBorder
    {
        Top = 0,
        Right = 1,
        Bottom = 2,
        Left = 3
    }
}

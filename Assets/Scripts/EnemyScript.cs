using System.Collections;
using UnityEngine;

public class EnemyScript : BaseObject
{
    protected override ShooterId? Id { get; } = ShooterId.Enemy;

    public AudioClip fireAudio;

    private int _directionModifier;

    void OnEnable()
    {
        Spawn();
        StartCoroutine(HandleFireTimer());
    }

    void OnDisable()
    {
        StopAllCoroutines();
        if (ObjectsPool.instance.gameObject.activeSelf)
            ObjectsPool.instance.SpawnEnemy();
    }

    protected override void OnStart()
    {
        Collider = GetComponent<Collider2D>();
        CanBeHitByAsteroids = true;
        CanBeHitByBullets = true;
    }

    protected override void OnHit(ShooterId? id)
    {
        if (id == ShooterId.Player)
            GameState.Score += 200;

        gameObject.SetActive(false);
    }

    protected override void HandleMove()
    {
        transform.Translate(new Vector2(_directionModifier * GameState.Settings.EnemySpeed * Time.deltaTime, 0));
    }

    private void Spawn()
    {
        _directionModifier = Random.value < 0.5f ? -1 : 1;
        var border = (_screenBottomRight.y - _screenTopLeft.y) / 5;
        var y = Random.Range(_screenTopLeft.y + border, _screenBottomRight.y - border);
        transform.SetPositionAndRotation(new Vector2(_screenTopLeft.x, y), Quaternion.identity);
    }

    private void FireAtPlayer()
    {
        var bullet = ObjectsPool.instance.GetBullet(Id.Value);
        bullet.transform.position = new Vector2(transform.position.x, transform.position.y);
        var angle = CalculeSignedAngle(bullet.transform.position, GameScript.instance.currentPlayer.transform.position);
        bullet.transform.rotation = Quaternion.identity;
        bullet.transform.Rotate(new Vector3(0, 0, 1), angle);
        bullet.transform.Translate(new Vector2(0, Collider.bounds.extents.x + 0.2f));
        bullet.SetActive(true);
        AudioHandler.instance.PlaySound(fireAudio);
        StartCoroutine(HandleFireTimer());
    }

    private IEnumerator HandleFireTimer()
    {
        yield return new PausedYield(Random.Range(GameState.Settings.EnemyMinFireDelayS, GameState.Settings.EnemyMaxFireDelayS));
        FireAtPlayer();
    }
}

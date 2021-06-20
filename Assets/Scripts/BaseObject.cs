using UnityEngine;

public abstract class BaseObject : MonoBehaviour
{
    protected virtual ShooterId? Id { get; } = null;

    protected float Speed = 0f;
    protected bool CanBeHitByBullets = false;
    protected bool CanBeHitByAsteroids = false;
    protected Collider2D Collider = null;
    protected Vector2 _screenTopLeft;
    protected Vector2 _screenBottomRight;

    void Awake()
    {
        _screenTopLeft = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
        _screenBottomRight = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        OnStart();
    }

    void Update()
    {
        if (GameState.IsPaused)
            return;

        HandleHits();
        if (CanMove())
            HandleMove();

        HandleScreenBounds();
        HandleFire();
    }

    protected virtual void HandleMove()
    {
        var distance = Speed * Time.deltaTime;
        transform.Translate(new Vector2(0, distance));
        OnMove(distance);
    }

    protected virtual void OnMove(float distance) { }

    protected virtual void OnStart() { }

    protected virtual bool CanMove() => true;

    protected virtual void OnHit(ShooterId? id) { }

    protected virtual void HandleFire() { }

    protected float CalculeSignedAngle(Vector2 center, Vector2 to)
    {
        var radian = Mathf.Atan2((to.y - center.y), (to.x - center.x));
        var angle = radian * Mathf.Rad2Deg - 90;
        return ToSignedAngle(angle);
    }

    protected float ToSignedAngle(float angle)
    {
        var modifier = Mathf.Sign(angle) * Mathf.Floor(Mathf.Abs(angle) / 360);
        var result = angle - 360 * modifier;
        if (result > 180)
            result -= 360;
        if (result < -180)
            result += 360;

        return result;
    }

    private void HandleHits()
    {
        if (Collider == null)
            return;

        var isHitByAsteroid = CanBeHitByAsteroids && ObjectsPool.instance.IsHitBySomeAsteroid(Collider);
        var hitByBulletWithId = CanBeHitByBullets ? ObjectsPool.instance.IsHitBySomeBullet(Collider, Id) : null;
        if (!isHitByAsteroid && hitByBulletWithId == null)
            return;

        OnHit(hitByBulletWithId);
    }

    private void HandleScreenBounds()
    {
        Vector2? newPosition = null;

        if (transform.position.y < _screenTopLeft.y)
            newPosition = new Vector2(transform.position.x, _screenBottomRight.y - 0.1f);
        if (transform.position.x < _screenTopLeft.x)
            newPosition = new Vector2(_screenBottomRight.x - 0.1f, transform.position.y);
        if (transform.position.y > _screenBottomRight.y)
            newPosition = new Vector2(transform.position.x, _screenTopLeft.y + 0.1f);
        if (transform.position.x > _screenBottomRight.x)
            newPosition = new Vector2(_screenTopLeft.x + 0.1f, transform.position.y);

        if (newPosition.HasValue)
            transform.SetPositionAndRotation(newPosition.Value, transform.rotation);
    }
}

public enum ShooterId
{
    Player,
    Enemy,
}

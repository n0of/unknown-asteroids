using System.Collections;
using UnityEngine;

public class PlayerScript : BaseObject
{
    protected override ShooterId? Id { get; } = ShooterId.Player;

    public AudioClip fireAudio;
    public AudioClip deathAudio;
    public AudioClip accelerationAudio;

    private float _currentSpeed = 0f;
    private bool _canShoot = true;
    private GameObject _visualObject;
    private Renderer _renderer;

    private static readonly string _accelerationLoopkey = "accelerationPlayer";

    protected override void OnStart()
    {
        CanBeHitByAsteroids = true;
        CanBeHitByBullets = true;
        _visualObject = transform.GetChild(0).gameObject;
        _renderer = _visualObject.GetComponent<Renderer>();
        Collider = _visualObject.GetComponent<Collider2D>();
    }

    protected override void OnHit(ShooterId? id)
    {
        AudioHandler.instance.PlaySound(deathAudio);
        GameState.Health--;
        if (GameState.Health == 0)
        {
            gameObject.SetActive(false);
            return;
        }
        transform.position = new Vector2(0, 0);
        StartCoroutine(HandleInvulnerability());
    }

    protected override void HandleMove()
    {
        var shouldAccelerate = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) || !GameState.IsKeyboardOnly && Input.GetMouseButton(1);
        var angleModifier = GameState.IsKeyboardOnly ? GetKeyboardAngleModifier() : GetMouseAngleModifier();
        var isTurning = _visualObject.transform.rotation.z != 0;

        if (angleModifier != 0 || (isTurning && shouldAccelerate))
        {
            var transformToRotate = shouldAccelerate ? transform : _visualObject.transform;
            if (shouldAccelerate && isTurning)
            {
                transform.rotation = _visualObject.transform.rotation;
                _visualObject.transform.rotation = new Quaternion(0, 0, 0, 0);
            }
            if (angleModifier != 0)
                transformToRotate.Rotate(new Vector3(0, 0, 1), angleModifier * GameState.Settings.PlayerTurnRate * Time.deltaTime);
        }

        if (shouldAccelerate)
        {
            AudioHandler.instance.PlaySound(accelerationAudio, _accelerationLoopkey);
            _currentSpeed = GameState.Settings.PlayerSpeed;
        }
        else
        {
            AudioHandler.instance.StopLoop(_accelerationLoopkey);
            if (_currentSpeed != 0f)
                _currentSpeed = GameState.Settings.PlayerSpeed * GameState.Settings.PlayerDriftModifier;
        }

        if (_currentSpeed != 0)
            transform.Translate(new Vector2(0, _currentSpeed * Time.deltaTime));
    }

    protected override void HandleFire()
    {
        var isFirePressed = Input.GetKeyDown(KeyCode.Space) || (!GameState.IsKeyboardOnly && Input.GetMouseButtonDown(0));
        if (!isFirePressed || !_canShoot)
            return;

        var bullet = ObjectsPool.instance.GetBullet(Id.Value);
        bullet.transform.position = new Vector2(transform.position.x, transform.position.y);
        bullet.transform.rotation = _visualObject.transform.rotation;
        bullet.transform.Translate(new Vector2(0, Collider.bounds.extents.y + 0.2f));
        bullet.SetActive(true);
        AudioHandler.instance.PlaySound(fireAudio);
        StartCoroutine(HandleFireDelay());
    }

    private int GetKeyboardAngleModifier()
    {
        var angleModifier = 0;

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            angleModifier--;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            angleModifier++;

        return angleModifier;
    }

    private int GetMouseAngleModifier()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var angle = CalculeSignedAngle(transform.position, mousePosition);
        angle = ToSignedAngle(angle - _visualObject.transform.rotation.eulerAngles.z);
        if (Mathf.Abs(angle) < 0.5f)
            return 0;
        return (int)Mathf.Sign(angle);
    }

    private IEnumerator HandleInvulnerability()
    {
        CanBeHitByAsteroids = false;
        CanBeHitByBullets = false;
        var endTime = Time.time + GameState.Settings.PlayerInvulnerabilityTimeS;
        while (Time.time < endTime)
        {
            _renderer.enabled = false;
            yield return new PausedYield(0.2f);
            _renderer.enabled = true;
            yield return new PausedYield(0.3f);
        }
        CanBeHitByAsteroids = true;
        CanBeHitByBullets = true;
    }

    private IEnumerator HandleFireDelay()
    {
        _canShoot = false;
        yield return new PausedYield(GameState.Settings.PlayerFireDelayS);
        _canShoot = true;
    }
}

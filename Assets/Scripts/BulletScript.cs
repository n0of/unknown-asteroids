using System.Collections.Generic;
using UnityEngine;

public class BulletScript : BaseObject
{
    private float _traveledLength = 0f;
    private float _maxTravelLength = 0f;
    private static readonly Dictionary<ShooterId, Color> ColorsById = new Dictionary<ShooterId, Color>()
    {
        { ShooterId.Player, Color.green },
        { ShooterId.Enemy, Color.red }
    };

    protected override void OnStart()
    {
        Speed = GameState.Settings.BulletSpeed;
        _maxTravelLength = _screenBottomRight.x - _screenTopLeft.x;
    }

    protected override bool CanMove()
    {
        if (_traveledLength < _maxTravelLength)
            return true;

        _traveledLength = 0f;
        gameObject.SetActive(false);
        return false;
    }

    protected override void OnMove(float distance)
    {
        _traveledLength += distance;
    }

    public static void SetColor(GameObject bullet,  ShooterId id)
    {
        ColorsById.TryGetValue(id, out var color);
        if (color != null)
            bullet.GetComponent<SpriteRenderer>().color = color;
    }
}

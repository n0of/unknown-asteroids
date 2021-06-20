using UnityEngine;

public class PausedYield : CustomYieldInstruction
{
    private float _timer;
    private readonly float _duration;

    public PausedYield(float duration)
    {
        _duration = duration;
    }

    public override bool keepWaiting
    {
        get
        {
            if (GameState.IsPaused)
                return true;

            _timer += Time.deltaTime;
            return _timer < _duration;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    public static AudioHandler instance;

    public GameObject audioObject;

    private AudioSource _audioSource;
    private Dictionary<string, Coroutine> _loopedSounds = new Dictionary<string, Coroutine>();

    void Awake()
    {
        instance = this;
        _audioSource = Instantiate(audioObject).GetComponent<AudioSource>();
    }

    void OnDisable()
    {
        StopAllCoroutines();
        _loopedSounds = new Dictionary<string, Coroutine>();
    }

    public void PlaySound(AudioClip clip, string loopId = null)
    {
        if (string.IsNullOrEmpty(loopId) || !_loopedSounds.ContainsKey(loopId))
            _audioSource.PlayOneShot(clip, 1);
        if (!string.IsNullOrEmpty(loopId) && !_loopedSounds.ContainsKey(loopId))
            _loopedSounds.Add(loopId, StartCoroutine(LoopSound(clip, loopId)));
    }

    public void StopLoop(string loopId)
    {
        if (!_loopedSounds.TryGetValue(loopId, out var coroutine))
            return;

        StopCoroutine(coroutine);
        _loopedSounds.Remove(loopId);
    }

    private IEnumerator LoopSound(AudioClip clip, string loopId)
    {
        yield return new PausedYield(clip.length);
        _loopedSounds.Remove(loopId);
        PlaySound(clip, loopId);
    }
}

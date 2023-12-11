using Effects.Audio;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class AmbientMusicManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] _backgroundMusicFiles;

    [SerializeField]
    private AudioSource _audioSource;

    private bool _isInFocus;

    [SerializeField]
    private float _maxVolume;

    private void Awake()
    {

    }

    private void Start()
    {
        PlayNextMusic();
    }

    private void OnApplicationFocus(bool isInFocus)
    {
        Debug.Log("Application focus: " + isInFocus);
        _isInFocus = isInFocus;
    }

    void Update()
    {
        if (_isInFocus && !_audioSource.isPlaying)
        {
            PlayNextMusic();
        }
    }

    private void PlayNextMusic()
    {
        AudioClip audioClip = GetRandomClip();

        _audioSource.clip = audioClip;

        // Start game music
        StartCoroutine(AudioFade.FadeIn(_audioSource, 10f, _maxVolume));
        //StartCoroutine(AudioFade.FadeOut(_audioSource, 5f));
    }

    private AudioClip GetRandomClip()
    {
        AudioClip audioClip = _backgroundMusicFiles[Random.Range(0, _backgroundMusicFiles.Length)];

        if (audioClip == _audioSource.clip)
        {
            return GetRandomClip();
        }

        return audioClip;
    }
}


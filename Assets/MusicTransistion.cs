using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTransistion : MonoBehaviour
{
    public List<AudioSource> chillMusicSources;
    public List<AudioSource> battleMusicSources;

    private AudioSource currentChillMusic;
    private AudioSource currentBattleMusic;

    public float crossfadeTime = 2.0f; // Time in seconds to crossfade music
    private Coroutine currentCrossfadeCoroutine;

    private bool isCrossfading = false;

    private void Start()
    {
        // Preload and pause all chill music sources
        foreach (var source in chillMusicSources)
        {
            if (source.clip != null)
            {
                source.Play();
                source.Pause();
            }
        }

        // Preload and pause all battle music sources
        foreach (var source in battleMusicSources)
        {
            if (source.clip != null)
            {
                source.Play();
                source.Pause();
            }
        }

        currentChillMusic = GetRandomMusic(chillMusicSources);
        currentBattleMusic = GetRandomMusic(battleMusicSources);

        if (currentChillMusic != null)
        {
            currentChillMusic.UnPause();  // Unpause the selected track
        }

        if (currentBattleMusic != null && currentBattleMusic != currentChillMusic)
        {
            currentBattleMusic.Pause();  // Ensure it remains paused
        }
        PlayMusic(currentChillMusic, true);
        PlayMusic(currentBattleMusic, false);

        GameStateManager.Instance.OnBattleStateChanged += HandleBattleStateChanged;
    }

    private AudioSource GetRandomMusic(List<AudioSource> sources)
    {
        if (sources == null || sources.Count == 0) return null;
        int randomIndex = UnityEngine.Random.Range(0, sources.Count);
        return sources[randomIndex];
    }

    private void PlayMusic(AudioSource source, bool play)
    {
        if (source != null)
        {
            if (play)
            {
                if (source.isPlaying)
                {
                    // If the source is already playing, just continue
                    Debug.Log("Continuing music: " + source.clip.name);
                }
                else
                {
                    // If the source is not playing, unpause it to continue from where it was stopped
                    Debug.Log("Resuming music: " + source.clip.name);
                    source.UnPause();
                }
            }
            else
            {
                Debug.Log("Pausing music: " + source.clip.name);
                source.Pause();
                source.volume = 0f;
            }
        }
        else
        {
            Debug.LogWarning("Attempted to play a null AudioSource");
        }
    }


    private void HandleBattleStateChanged(bool isInBattle)
    {
        if (isCrossfading)
        {
            StopCoroutine(currentCrossfadeCoroutine);
            isCrossfading = false;
        }

        if (isInBattle)
        {
            currentBattleMusic = GetRandomMusic(battleMusicSources);
            currentCrossfadeCoroutine = StartCoroutine(CrossfadeMusic(currentChillMusic, currentBattleMusic, true));
        }
        else
        {
            currentChillMusic = GetRandomMusic(chillMusicSources);
            currentCrossfadeCoroutine = StartCoroutine(CrossfadeMusic(currentBattleMusic, currentChillMusic, false));
        }
    }

    private IEnumerator CrossfadeMusic(AudioSource fadeOutSource, AudioSource fadeInSource, bool resume)
    {
        isCrossfading = true;

        float timer = 0;
        float startVolumeFadeOut = fadeOutSource.volume;
        float startVolumeFadeIn = fadeInSource.volume;

        fadeInSource.Play();

        while (timer < crossfadeTime)
        {
            timer += Time.deltaTime;
            fadeOutSource.volume = Mathf.Lerp(startVolumeFadeOut, 0, timer / crossfadeTime);
            fadeInSource.volume = Mathf.Lerp(startVolumeFadeIn, 1, timer / crossfadeTime);
            yield return null;
        }

        fadeOutSource.volume = 0;
        fadeInSource.volume = 1f;

        if (!resume)
        {
            fadeOutSource.Pause();
        }

        isCrossfading = false;
    }

    void OnDestroy()
    {
        GameStateManager.Instance.OnBattleStateChanged -= HandleBattleStateChanged;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTransistion : MonoBehaviour
{
    public AudioSource defaultMusicSource;
    public AudioSource battleMusicSource;
    public float crossfadeTime = 2.0f; // Time in seconds to crossfade music
    private Coroutine currentCrossfadeCoroutine;

    private bool isCrossfading = false;

    private void Start()
    {
        // Start with default music
        defaultMusicSource.Play();
        battleMusicSource.Play();
        battleMusicSource.Pause(); // Pause battle music immediately after it starts playing
        battleMusicSource.volume = 0f; // Ensure battle music starts muted

        GameStateManager.Instance.OnBattleStateChanged += HandleBattleStateChanged;

    }

    private void HandleBattleStateChanged(bool isInBattle)
    {
        if (isCrossfading)
        {
            // Immediately stop the current crossfade if a new state change occurs
            StopCoroutine(currentCrossfadeCoroutine);
            // Reset volumes to appropriate states before starting a new crossfade
            if (isInBattle)
            {
                defaultMusicSource.volume = 1f;
                battleMusicSource.volume = 0f;
                battleMusicSource.UnPause();
            }
            else
            {
                defaultMusicSource.volume = 0f;
                battleMusicSource.volume = 1f;
                defaultMusicSource.UnPause();
            }
            isCrossfading = false;
        }

        if (isInBattle)
        {
            currentCrossfadeCoroutine = StartCoroutine(CrossfadeMusic(defaultMusicSource, battleMusicSource, true));
        }
        else
        {
            currentCrossfadeCoroutine = StartCoroutine(CrossfadeMusic(battleMusicSource, defaultMusicSource, false));
        }
    }

    public void CrossfadeToBattleMusic()
    {
        if (!isCrossfading)
        {
            StartCoroutine(CrossfadeMusic(defaultMusicSource, battleMusicSource, resume: true));
        }
    }    
    
    public void CrossfadeToDefaultMusic()
    {
        if (!isCrossfading)
        {
            StartCoroutine(CrossfadeMusic(battleMusicSource, defaultMusicSource, resume: false));
        }
    }

    private IEnumerator CrossfadeMusic(AudioSource fadeOutSource, AudioSource fadeInSource, bool resume)
    {
        isCrossfading = true;

        float timer = 0;

        //Get current volume levels
        float startVolumeFadeOut = fadeOutSource.volume;
        float startVolumeFadeIn = fadeInSource.volume;

        // Resume paused music or just continue if it's already playing
        if (resume)
        {
            fadeInSource.UnPause();
        }

        while (timer < crossfadeTime)
        {
            timer += Time.deltaTime;
            //Lerp volune levels based on timer
            fadeOutSource.volume = Mathf.Lerp(startVolumeFadeOut, 0, timer / crossfadeTime);
            fadeInSource.volume = Mathf.Lerp(startVolumeFadeIn, 1, timer / crossfadeTime);

            yield return null;
        }

        // Ensure final volumes are set correctly
        fadeOutSource.volume = 0;
        fadeInSource.volume = 1f;

        // Only pause the fadeOutSource if it's the battle music that's fading out
        if (!resume)
        {
            fadeOutSource.Pause();
        }
        isCrossfading = false;
    }

    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        GameStateManager.Instance.OnBattleStateChanged -= HandleBattleStateChanged;
    }
}

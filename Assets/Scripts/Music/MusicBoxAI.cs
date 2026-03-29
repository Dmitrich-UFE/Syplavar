using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Для работы с List

public class MusicBoxAI : MonoBehaviour
{
    [Header("Источники звука")]
    [SerializeField] private AudioSource peacefulSource;
    [SerializeField] private AudioSource combatSource;
    
    [Header("Плейлисты")]
    [SerializeField] private List<AudioClip> peacefulTracks;
    [SerializeField] private List<AudioClip> combatTracks;

    [Header("Настройки переходов")]
    [SerializeField] private float fastFadeTime = 0.5f;
    [SerializeField] private float slowFadeTime = 2.0f;
    [SerializeField] private float silenceDelay = 1.0f;

    private Coroutine _musicCoroutine;


    void Awake()
    {
        SwitchToClassicMusic();
        BattleStatusTracker._OnBattleModeOn += SwitchToBattleMusic;
        BattleStatusTracker._OnBattleModeOff += SwitchToClassicMusic;
    }

    void SwitchToBattleMusic()
    {
        SwitchMode(true);
    }

    void SwitchToClassicMusic()
    {
        SwitchMode(false);
    }

    //BattleStatusTracker.BattleMode

    void OnDestroy()
    {
        BattleStatusTracker._OnBattleModeOn -= SwitchToBattleMusic;
        BattleStatusTracker._OnBattleModeOff -= SwitchToClassicMusic;
    }


    void SwitchMode(bool isCombat)
    {
        if (_musicCoroutine != null) StopCoroutine(_musicCoroutine);

        if (isCombat)
            // В бой врываемся быстро (без пауз)
            _musicCoroutine = StartCoroutine(QuickCombatTransition());
        else
            // В мир выходим через "затухание -> пауза -> появление"
            _musicCoroutine = StartCoroutine(SlowPeacefulTransition());
    }

    // Быстрый кроссфейд для боя
    private IEnumerator QuickCombatTransition()
    {
        PrepareSource(combatSource, combatTracks);
        
        float time = 0;
        float startPeacefulVol = peacefulSource.volume;

        while (time < fastFadeTime)
        {
            time += Time.deltaTime;
            float ratio = time / fastFadeTime;
            
            peacefulSource.volume = Mathf.Lerp(startPeacefulVol, 0, ratio);
            combatSource.volume = Mathf.Lerp(0, 1, ratio);
            yield return null;
        }
        peacefulSource.Stop();
    }

    // Последовательный переход для мира
    private IEnumerator SlowPeacefulTransition()
    {
        // 1. Затухание боевой музыки
        float time = 0;
        float startCombatVol = combatSource.volume;
        while (time < slowFadeTime)
        {
            time += Time.deltaTime;
            combatSource.volume = Mathf.Lerp(startCombatVol, 0, time / slowFadeTime);
            yield return null;
        }
        combatSource.Stop();

        // 2. Пауза тишины
        yield return new WaitForSeconds(silenceDelay);

        // 3. Плавное появление мирной музыки
        PrepareSource(peacefulSource, peacefulTracks);
        time = 0;
        while (time < slowFadeTime)
        {
            time += Time.deltaTime;
            peacefulSource.volume = Mathf.Lerp(0, 1, time / slowFadeTime);
            yield return null;
        }
    }

    private void PrepareSource(AudioSource source, List<AudioClip> playlist)
    {
        if (playlist == null || playlist.Count == 0) return;
        
        source.clip = playlist[Random.Range(0, playlist.Count)];
        source.volume = 0;
        source.Play();
    }
}
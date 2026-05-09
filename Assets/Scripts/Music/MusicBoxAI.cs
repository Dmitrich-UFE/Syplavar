using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    [SerializeField] private float transitionSilence = 1.0f;

    [Header("Задержка между треками")]
    [SerializeField] private float minDelayBetweenTracks = 20.0f;
    [SerializeField] private float maxDelayBetweenTracks = 30.0f;

    private Coroutine _transitionCoroutine;
    private Coroutine _waitTrackCoroutine;
    private Coroutine _monitorCoroutine;

    private bool _isCombatMode;
    private bool _isWaitingForNextTrack;

    void Awake()
    {
        BattleStatusTracker._OnBattleModeOn += SwitchToBattleMusic;
        BattleStatusTracker._OnBattleModeOff += SwitchToClassicMusic;
    }

    void Start()
    {
        // Запускаем монитор один раз при старте
        _monitorCoroutine = StartCoroutine(PlaylistMonitor());
        // Устанавливаем начальное состояние без лишних проверок
        _isCombatMode = false;
        _transitionCoroutine = StartCoroutine(SlowPeacefulTransition());
    }

    private IEnumerator PlaylistMonitor()
    {
        var checkWait = new WaitForSeconds(1.0f);
        while (true)
        {
            // Проверяем условия для запуска нового трека
            if (_transitionCoroutine == null && !_isWaitingForNextTrack)
            {
                bool combatFinished = _isCombatMode && !combatSource.isPlaying;
                bool peacefulFinished = !_isCombatMode && !peacefulSource.isPlaying;

                if (combatFinished)
                    _waitTrackCoroutine = StartCoroutine(WaitAndPlayNext(combatSource, combatTracks));
                else if (peacefulFinished)
                    _waitTrackCoroutine = StartCoroutine(WaitAndPlayNext(peacefulSource, peacefulTracks));
            }
            yield return checkWait;
        }
    }

    private IEnumerator WaitAndPlayNext(AudioSource source, List<AudioClip> playlist)
    {
        _isWaitingForNextTrack = true;
        float delay = Random.Range(minDelayBetweenTracks, maxDelayBetweenTracks);
        yield return new WaitForSeconds(delay);

        if (_transitionCoroutine == null)
        {
            PlayNextTrack(source, playlist);
        }

        _isWaitingForNextTrack = false;
        _waitTrackCoroutine = null;
    }

    private void PlayNextTrack(AudioSource source, List<AudioClip> playlist)
    {
        if (playlist == null || playlist.Count == 0) return;
        source.clip = playlist[Random.Range(0, playlist.Count)];
        source.volume = 1f; 
        source.Play();
    }

    void SwitchToBattleMusic() { if (!_isCombatMode) SwitchMode(true); }
    void SwitchToClassicMusic() { if (_isCombatMode) SwitchMode(false); }

    void SwitchMode(bool isCombat)
    {
        _isCombatMode = isCombat;

        // Останавливаем активные переходы и ожидания, но НЕ трогаем PlaylistMonitor
        if (_transitionCoroutine != null) StopCoroutine(_transitionCoroutine);
        if (_waitTrackCoroutine != null) 
        {
            StopCoroutine(_waitTrackCoroutine);
            _isWaitingForNextTrack = false;
        }

        if (isCombat)
            _transitionCoroutine = StartCoroutine(QuickCombatTransition());
        else
            _transitionCoroutine = StartCoroutine(SlowPeacefulTransition());
    }

    private IEnumerator QuickCombatTransition()
    {
        PrepareSource(combatSource, combatTracks);
        float time = 0;
        float startPeacefulVol = peacefulSource.volume;

        while (time < fastFadeTime)
        {
            time += Time.deltaTime;
            peacefulSource.volume = Mathf.Lerp(startPeacefulVol, 0, time / fastFadeTime);
            combatSource.volume = Mathf.Lerp(combatSource.volume, 1, time / fastFadeTime);
            yield return null;
        }
        peacefulSource.Stop();
        _transitionCoroutine = null;
    }

    private IEnumerator SlowPeacefulTransition()
    {
        float time = 0;
        float startCombatVol = combatSource.volume;
        
        // Угасание боевой музыки
        while (time < slowFadeTime)
        {
            time += Time.deltaTime;
            combatSource.volume = Mathf.Lerp(startCombatVol, 0, time / slowFadeTime);
            yield return null;
        }
        combatSource.Stop();

        yield return new WaitForSeconds(transitionSilence);

        // Плавное появление мирной музыки
        PrepareSource(peacefulSource, peacefulTracks);
        time = 0;
        while (time < slowFadeTime)
        {
            time += Time.deltaTime;
            peacefulSource.volume = Mathf.Lerp(0, 1, time / slowFadeTime);
            yield return null;
        }
        _transitionCoroutine = null;
    }

    private void PrepareSource(AudioSource source, List<AudioClip> playlist)
    {
        if (playlist == null || playlist.Count == 0) return;
        source.clip = playlist[Random.Range(0, playlist.Count)];
        source.volume = 0;
        source.Play();
    }

    void OnDestroy()
    {
        BattleStatusTracker._OnBattleModeOn -= SwitchToBattleMusic;
        BattleStatusTracker._OnBattleModeOff -= SwitchToClassicMusic;
    }
}

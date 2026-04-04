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
    [SerializeField] private float transitionSilence = 1.0f; // Пауза при смене режима

    [Header("Задержка между треками")]
    [SerializeField] private float minDelayBetweenTracks = 2.0f;
    [SerializeField] private float maxDelayBetweenTracks = 5.0f;

    private Coroutine _transitionCoroutine;
    private bool _isCombatMode;
    private bool _isWaitingForNextTrack; // Флаг, чтобы монитор не запускал задержку дважды

    void Awake()
    {
        BattleStatusTracker._OnBattleModeOn += SwitchToBattleMusic;
        BattleStatusTracker._OnBattleModeOff += SwitchToClassicMusic;
    }

    void Start()
    {
        StartCoroutine(PlaylistMonitor());
        SwitchToClassicMusic();
    }

    private IEnumerator PlaylistMonitor()
    {
        var checkWait = new WaitForSeconds(1.0f);
        while (true)
        {
            // Проверяем, нужно ли запустить новый трек
            // Условие: переход не идет, задержка между треками не активна, и музыка затихла
            if (_transitionCoroutine == null && !_isWaitingForNextTrack)
            {
                if (_isCombatMode && !combatSource.isPlaying)
                {
                    StartCoroutine(WaitAndPlayNext(combatSource, combatTracks));
                }
                else if (!_isCombatMode && !peacefulSource.isPlaying)
                {
                    StartCoroutine(WaitAndPlayNext(peacefulSource, peacefulTracks));
                }
            }
            yield return checkWait;
        }
    }

    private IEnumerator WaitAndPlayNext(AudioSource source, List<AudioClip> playlist)
    {
        _isWaitingForNextTrack = true;

        // Случайная пауза между треками
        float delay = Random.Range(minDelayBetweenTracks, maxDelayBetweenTracks);
        yield return new WaitForSeconds(delay);

        // Если за время паузы режим не сменился (или мы все еще в том же режиме)
        if (_transitionCoroutine == null)
        {
            PlayNextTrack(source, playlist);
        }

        _isWaitingForNextTrack = false;
    }

    private void PlayNextTrack(AudioSource source, List<AudioClip> playlist)
    {
        if (playlist == null || playlist.Count == 0) return;
        
        source.clip = playlist[Random.Range(0, playlist.Count)];
        source.volume = 1f; 
        source.Play();
    }

    void SwitchToBattleMusic() { _isCombatMode = true; SwitchMode(true); }
    void SwitchToClassicMusic() { _isCombatMode = false; SwitchMode(false); }

    void OnDestroy()
    {
        BattleStatusTracker._OnBattleModeOn -= SwitchToBattleMusic;
        BattleStatusTracker._OnBattleModeOff -= SwitchToClassicMusic;
        StopAllCoroutines();
    }

    void SwitchMode(bool isCombat)
    {
        // Прерываем всё: и фейды, и ожидание следующего трека
        StopAllCoroutines(); 
        _isWaitingForNextTrack = false;
        
        // Заново запускаем монитор, так как StopAllCoroutines его убил
        StartCoroutine(PlaylistMonitor());

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
            combatSource.volume = Mathf.Lerp(0, 1, time / fastFadeTime);
            yield return null;
        }
        peacefulSource.Stop();
        _transitionCoroutine = null;
    }

    private IEnumerator SlowPeacefulTransition()
    {
        float time = 0;
        float startCombatVol = combatSource.volume;
        while (time < slowFadeTime)
        {
            time += Time.deltaTime;
            combatSource.volume = Mathf.Lerp(startCombatVol, 0, time / slowFadeTime);
            yield return null;
        }
        combatSource.Stop();

        yield return new WaitForSeconds(transitionSilence);

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
}

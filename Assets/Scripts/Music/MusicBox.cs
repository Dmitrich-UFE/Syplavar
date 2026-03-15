using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Audio;
using Unity.Mathematics;

public class MusicBox : MonoBehaviour
{
    [SerializeField] private List<AudioClip> MusicForBattle;

    [SerializeField] private List<AudioClip> MusicForClassic;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Queue<AudioClip> Clips;

    [SerializeField] private Coroutine musicCoroutine;


    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Clips = new Queue<AudioClip>();
        BattleStatusTracker.SetBattleMode(false); //здесь идет вызов событий
        BattleStatusTracker._OnBattleModeOn += SwitchMusicToBattleMode;
        BattleStatusTracker._OnBattleModeOff += SwitchMusicToClassicMode;
    }

    void Start()
    {
        musicCoroutine =  StartCoroutine(PlayMusic());
    }

    void SwitchMusicToBattleMode()
    {
        Clips.Clear();
        SetVolume(0f, 14f);
        AddMusicForBattleMode();
        SetVolume(1f, 1f);
    }

    void SwitchMusicToClassicMode()
    {
        Clips.Clear();
        SetVolume(0f, 34f);
        AddMusicForClassicMode();
        SetVolume(0f, 1f);
    }



    void AddMusicForClassicMode()
    {
        do
        {
            Clips.Enqueue(MusicForClassic[UnityEngine.Random.Range(0, MusicForClassic.Count)]);
        }
        while (Clips.Count < 3);
    }   

    void AddMusicForBattleMode()
    {
        do
        {
            Clips.Enqueue(MusicForBattle[UnityEngine.Random.Range(0, MusicForBattle.Count)]);
        }
        while (Clips.Count < 3);
    }


    void OnDestroy()
    {
        BattleStatusTracker._OnBattleModeOn -= SwitchMusicToBattleMode;
        BattleStatusTracker._OnBattleModeOff -= SwitchMusicToClassicMode;
        StopCoroutine(musicCoroutine);
    }

    IEnumerator PlayMusic()
    {
        if (Clips.Count < 1)
        {
            if (BattleStatusTracker.BattleMode)
            {
                AddMusicForBattleMode();
            }
            else
            {
                AddMusicForClassicMode();
            }
        }

        _audioSource.clip = Clips.Peek();
        _audioSource.Play();


        yield return new WaitForSecondsRealtime(Clips.Dequeue().length + UnityEngine.Random.Range(30, 50));
    }

    //fadetime здесь 1 - это 0.05 секунд 
    void SetVolume(float targetvolume, float fadetime)
    {
        if (fadetime == 0) fadetime = 1;
        StartCoroutine(SetVolumeEnumerator(targetvolume, (targetvolume - _audioSource.volume) / fadetime));
    }

    IEnumerator SetVolumeEnumerator(float targetvolume, float deltavolume)
    {
        _audioSource.volume += deltavolume;

        if (math.abs(_audioSource.volume - targetvolume) < 0.01) { yield break; }

        yield return new WaitForSecondsRealtime(0.05f);
    }

}

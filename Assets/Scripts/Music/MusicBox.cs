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
    [SerializeField] private Coroutine checkingBattleModeCoroutine;

    private bool CheckingBattleMode;
    private bool isStartMusicCalled;
    private bool isSetToBattleModeCalled;
    private bool isSetToClassicModeCalled;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Clips = new Queue<AudioClip>();
        BattleStatusTracker.SetBattleMode(false); //здесь идет вызов событий
        //BattleStatusTracker._OnBattleModeOn += SwitchMusicToBattleMode;
        //BattleStatusTracker._OnBattleModeOff += SwitchMusicToClassicMode;
    }

    void Start()
    {
        StartMusic();
        checkingBattleModeCoroutine = StartCoroutine(CheckBattleMode());
    }
    
    void StartMusic()
    {
        if (!isStartMusicCalled)
        {
            SetVolume(1f, 1f);
            musicCoroutine = StartCoroutine(PlayMusic());
            isStartMusicCalled = true;
        }
    }

    void CheckBattleStatus1()
    {
        if (BattleStatusTracker.BattleMode == CheckingBattleMode)
        {
            StartMusic();
            CheckingBattleMode = BattleStatusTracker.BattleMode;
            isSetToClassicModeCalled = false;
        }
    }

    void CheckBattleStatus2()
    {
        if (BattleStatusTracker.BattleMode == CheckingBattleMode)
        {
            StartMusic();
            CheckingBattleMode = BattleStatusTracker.BattleMode;
            isSetToBattleModeCalled = false;
        }
    }

    void SwitchMusicToBattleMode()
    {
        Clips.Clear();
        SetVolume(0f, 9f);

        StopCoroutine(musicCoroutine);
        //CheckingBattleMode = BattleStatusTracker.BattleMode;
        
        //if (!isSetToBattleModeCalled) Invoke("CheckBattleStatus2", 1.25f);
        Invoke("StartMusic", 1.25f);
        //isSetToBattleModeCalled = true;
        
    }

    void SwitchMusicToClassicMode()
    {
        if (!BattleStatusTracker.BattleMode)
        {
            Clips.Clear();
            SetVolume(0f, 24f);

            StopCoroutine(musicCoroutine);
        //CheckingBattleMode = BattleStatusTracker.BattleMode;
        
        //if (!isSetToClassicModeCalled) Invoke("CheckBattleStatus1", 7f);
        //isSetToClassicModeCalled = true;
            Invoke("StartMusic", 7f);
        }
        
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
        //BattleStatusTracker._OnBattleModeOn -= SwitchMusicToBattleMode;
        //BattleStatusTracker._OnBattleModeOff -= SwitchMusicToClassicMode;
        StopCoroutine(musicCoroutine);
        StopCoroutine(checkingBattleModeCoroutine);
    }

    IEnumerator CheckBattleMode()
    {
        while (true)
        {
            if (BattleStatusTracker.BattleMode == CheckingBattleMode)
            {
                if (BattleStatusTracker.BattleMode && !isStartMusicCalled)
                {
                    SwitchMusicToBattleMode();
                }
                else if (!BattleStatusTracker.BattleMode && !isStartMusicCalled)
                {
                    Invoke("SwitchMusicToClassicMode", 3f);
                }

                
            }
            else
            {
                isStartMusicCalled = false;
            }

            CheckingBattleMode = BattleStatusTracker.BattleMode;

            yield return new WaitForSecondsRealtime(0.5f);
        }
    }

    IEnumerator PlayMusic()
    {
        while(true)
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
    }

    //fadetime здесь 1 - это 0.05 секунд 
    void SetVolume(float targetvolume, float fadetime)
    {
        if (fadetime == 0) fadetime = 1;
        StartCoroutine(SetVolumeEnumerator(targetvolume, (targetvolume - _audioSource.volume) / fadetime));
    }

    IEnumerator SetVolumeEnumerator(float targetvolume, float deltavolume)
    {
        while (true)
        {
            _audioSource.volume += deltavolume;

            if (math.abs(_audioSource.volume - targetvolume) < 0.01) { yield break; }

            yield return new WaitForSecondsRealtime(0.05f);
        }
    }

}

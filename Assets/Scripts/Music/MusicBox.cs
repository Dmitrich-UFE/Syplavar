using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;

public class MusicBox : MonoBehaviour
{
    [SerializeField] private List<AudioSource> MusicForBattle;

    [SerializeField] private List<AudioSource> MusicForClassic;


    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        
    }

}

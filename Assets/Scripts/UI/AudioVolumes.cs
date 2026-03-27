using UnityEngine;
using UnityEngine.Audio;

public class AudioVolumes : MonoBehaviour
{
    [SerializeField] private AudioMixer ambientMixer, musicMixer, soundMixer;

    internal float MusicVolume {get; private set;}
    internal float AmbientVolume {get; private set;}
    internal float SoundVolume {get; private set;}

    void Awake()
    {
        MusicVolume = 1f;
        AmbientVolume = 1f;
        SoundVolume = 1f;
    }

    public void SetAmbientVolume(float sval)
    {
        float dbVal = -Mathf.Pow(sval - 1, 2) * 80;
        AmbientVolume = sval;
        ambientMixer.SetFloat("aVolume", dbVal);
    } 

    public void SetMusicVolume(float sval)
    {
        float dbVal = -Mathf.Pow(sval - 1, 2) * 80;
        MusicVolume = sval;
        musicMixer.SetFloat("mVolume", dbVal);
    } 

    public void SetSoundVolume(float sval)
    {
        float dbVal = -Mathf.Pow(sval - 1, 2) * 80;
        SoundVolume = sval;
        soundMixer.SetFloat("sVolume", dbVal);
    } 
}

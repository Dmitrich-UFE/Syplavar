using UnityEngine;
using UnityEngine.Audio;

public class AudioVolumes : MonoBehaviour
{
    [SerializeField] private AudioMixer ambientMixer, musicMixer, soundMixer;

    public void SetAmbientVolume(float sval)
    {
        float dbVal = -Mathf.Pow(sval - 1, 2) * 80;
        ambientMixer.SetFloat("aVolume", dbVal);
    } 

    public void SetMusicVolume(float sval)
    {
        float dbVal = -Mathf.Pow(sval - 1, 2) * 80;
        musicMixer.SetFloat("mVolume", dbVal);
    } 

    public void SetSoundVolume(float sval)
    {
        float dbVal = -Mathf.Pow(sval - 1, 2) * 80;
        soundMixer.SetFloat("sVolume", dbVal);
    } 
}

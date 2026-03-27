using UnityEngine;
using UnityEngine.Audio;
using System.IO;

public class AudioVolumes : MonoBehaviour
{
    [SerializeField] private AudioMixer ambientMixer, musicMixer, soundMixer;

    internal float MusicVolume {get; private set;}
    internal float AmbientVolume {get; private set;}
    internal float SoundVolume {get; private set;}

    internal static AudioVolumes audioVolumes;
    public AudioSettingsData settings = new AudioSettingsData();
    private string filePath;

    void Awake()
    {
        if (audioVolumes == null)
        {
            audioVolumes = this;
            DontDestroyOnLoad(gameObject);
            filePath = Path.Combine(Application.persistentDataPath, "AudioSettings.json");
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }


        MusicVolume = 1f;
        AmbientVolume = 1f;
        SoundVolume = 1f;
    }

    public void SaveSettings()
    {
        //AudioVolumes.audioVolumes.settings.MusicVolume = MusicVolume;
        //AudioVolumes.audioVolumes.settings.AmbientVolume = AmbientVolume;
        //AudioVolumes.audioVolumes.settings.SoundVolume = SoundVolume;
        string json = JsonUtility.ToJson(settings, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Настройки сохранены в: " + filePath);
    }

    public void LoadSettings()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            settings = JsonUtility.FromJson<AudioSettingsData>(json);
            Debug.Log("Настройки загружены");
            
            MusicVolume = AudioVolumes.audioVolumes.settings.MusicVolume;
            AmbientVolume = AudioVolumes.audioVolumes.settings.AmbientVolume;
            SoundVolume = AudioVolumes.audioVolumes.settings.SoundVolume;
        }
    }




    public void SetAmbientVolume(float sval)
    {
        float dbVal = -Mathf.Pow(sval - 1, 2) * 80;
        AmbientVolume = sval;
        AudioVolumes.audioVolumes.settings.AmbientVolume = sval;
        ambientMixer.SetFloat("aVolume", dbVal);
        AudioVolumes.audioVolumes.SaveSettings();
    } 

    public void SetMusicVolume(float sval)
    {
        float dbVal = -Mathf.Pow(sval - 1, 2) * 80;
        MusicVolume = sval;
        AudioVolumes.audioVolumes.settings.MusicVolume = sval;
        musicMixer.SetFloat("mVolume", dbVal);
        AudioVolumes.audioVolumes.SaveSettings();
    } 

    public void SetSoundVolume(float sval)
    {
        float dbVal = -Mathf.Pow(sval - 1, 2) * 80;
        SoundVolume = sval;
        AudioVolumes.audioVolumes.settings.SoundVolume = sval;
        soundMixer.SetFloat("sVolume", dbVal);
        AudioVolumes.audioVolumes.SaveSettings();
    } 
}

//класс для хранения данных, связанных с музыкой
[System.Serializable]
public class AudioSettingsData
{
    public float MusicVolume  = 1f;
    public float AmbientVolume  = 1f;
    public float SoundVolume  = 1f;

}

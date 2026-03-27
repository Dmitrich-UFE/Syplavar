using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UIHandlerMainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _settingsMenu;
    [SerializeField] private AudioVolumes _audioVolumes;

    [Header("Элементы настроек")]
    [SerializeField] private Toggle fsToggle;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _ambientSlider;
    [SerializeField] private Slider _soundSlider;


    public void OpenGameWorld()
    {
        SceneManager.LoadScene("UFETestScene");
    }

    public void OpenSettings()
    {
        AudioVolumes.audioVolumes.LoadSettings();
        _settingsMenu.SetActive(true);
        fsToggle.isOn = Screen.fullScreen;
        _ambientSlider.value = AudioVolumes.audioVolumes.AmbientVolume;
        _musicSlider.value = AudioVolumes.audioVolumes.MusicVolume;
        _soundSlider.value = AudioVolumes.audioVolumes.SoundVolume;
    }

    public void CloseSettings()
    {
        AudioVolumes.audioVolumes.SaveSettings();
        _settingsMenu.SetActive(false);
    }
}

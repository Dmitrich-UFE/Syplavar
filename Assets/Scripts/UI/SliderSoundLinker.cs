using UnityEngine;
using UnityEngine.UI;
public class SliderSoundLinker : MonoBehaviour
{
    private Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();
        
        // 1. Устанавливаем положение слайдера из глобальных данных
        slider.value = AudioVolumes.audioVolumes.SoundVolume;

        // 2. Подписываем менеджер на изменения этого слайдера
        slider.onValueChanged.AddListener(AudioVolumes.audioVolumes.SetSoundVolume);
    }
}
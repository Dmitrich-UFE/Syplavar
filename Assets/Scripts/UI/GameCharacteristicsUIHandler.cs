using UnityEngine;
using UnityEngine.UI;

public class GameCharacteristicsUIHandler : MonoBehaviour
{

    private PlayerHealth _playerHealth;
    private PlayerMind _playerMind;
    [SerializeField] private Image _healthBar;
    [SerializeField] private Image _mindBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerHealth = PlayerSeeker.GetPlayerHealth();
        _playerMind = PlayerSeeker.GetPlayerMind();
        _playerHealth.OnHealthChanged += SetHealthUI;
        _playerMind.OnMindChanged += SetMindUI;
        SetHealthUI();
        SetMindUI();
    }

    void SetHealthUI()
    {
        _healthBar.fillAmount = _playerHealth.HealthInPercents;
    }

    void SetMindUI()
    {
        _mindBar.fillAmount = _playerMind.MindPercent;
    }


    void OnDestroy()
    {
        _playerHealth.OnHealthChanged -= SetHealthUI;
        _playerMind.OnMindChanged -= SetMindUI;
    }
}

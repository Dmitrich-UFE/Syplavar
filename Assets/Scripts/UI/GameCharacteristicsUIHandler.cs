using UnityEngine;
using UnityEngine.UI;

public class GameCharacteristicsUIHandler : MonoBehaviour
{

    private PlayerHealth _playerHealth;
    [SerializeField] private Image _healthBar;
    [SerializeField] private Image _mindBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerHealth = PlayerSeeker.GetPlayerHealth();
        _playerHealth.OnHealthChanged += SetHealthUI;
        SetHealthUI();
    }

    void SetHealthUI()
    {
        _healthBar.fillAmount = _playerHealth.HealthInPercents;
    }


    void OnDestroy()
    {
        _playerHealth.OnHealthChanged -= SetHealthUI;
    }
}

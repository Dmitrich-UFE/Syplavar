using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UIHandler : MonoBehaviour
{
    private PlayerInputActions _playerInputActions;
    [SerializeField] private InventoryAI _inventoryAI;
    [SerializeField] private AudioVolumes _audioVolumes;
    [SerializeField] private GameObject _bigInventory;
    [SerializeField] private GameObject _lowerInventory;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _settingsMenu;
    [SerializeField] private GameObject _helpMenu;

    [Header("Элементы настроек")]
    [SerializeField] private Toggle fsToggle;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _ambientSlider;
    [SerializeField] private Slider _soundSlider;
    
    bool isBigInvOpen;
    bool isPauseMenuOpen;

    void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        isBigInvOpen = false;
        isPauseMenuOpen = false;
        fsToggle.isOn = Screen.fullScreen;
        fsToggle.onValueChanged.AddListener(SetFullScreen);
    }

    public void ClosePauseMenu()
    {
        isPauseMenuOpen = false;
        _settingsMenu.SetActive(false);
        _helpMenu.SetActive(false);
        _pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    void OpenBigInventory(InputAction.CallbackContext context)
    {
        if (isBigInvOpen)
        {
            _inventoryAI.DrawLowerInventory();
            _lowerInventory.SetActive(true);
            _bigInventory.SetActive(false);
            isBigInvOpen = false;
        }
        else if (!isPauseMenuOpen)
        {
            _lowerInventory.SetActive(false);
            _bigInventory.SetActive(true);
            _inventoryAI.DrawInventory();
            isBigInvOpen = true;
        }
    }

    void OpenPauseMenu(InputAction.CallbackContext context)
    {
        if (!isPauseMenuOpen && !isBigInvOpen)
        {
            _lowerInventory.SetActive(false);
            _pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            isPauseMenuOpen = true;
        }
        else if (isPauseMenuOpen)
        {
            _lowerInventory.SetActive(true);
            _settingsMenu.SetActive(false);
            _helpMenu.SetActive(false);
            _pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            isPauseMenuOpen = false;
            _lowerInventory.SetActive(true);
        }
    }

    public void OpenSettings()
    {
        _settingsMenu.SetActive(true);
        fsToggle.isOn = Screen.fullScreen;
        _ambientSlider.value = _audioVolumes.AmbientVolume;
        _musicSlider.value = _audioVolumes.MusicVolume;
        _soundSlider.value = _audioVolumes.SoundVolume;
    }

    public void OpenHelpMenu()
    {
        _helpMenu.SetActive(true);
    }

    public void EscapeToMainMenu()
    {

    }

    public void SetFullScreen(bool isfscreen)
    {
        Screen.fullScreen = isfscreen;
        Debug.Log("Fullscreen mode is now: " + isfscreen);
    }

    private void OnEnable()
    {
        _playerInputActions.Player.OpenBigInventory.performed += OpenBigInventory;
        _playerInputActions.Player.EscapeTo.performed += OpenPauseMenu;
        _playerInputActions.Enable();
    }

    private void OnDisable()
    {
        _playerInputActions.Player.OpenBigInventory.performed -= OpenBigInventory;
        _playerInputActions.Player.EscapeTo.performed += OpenPauseMenu;
        _playerInputActions.Disable();
    }
}

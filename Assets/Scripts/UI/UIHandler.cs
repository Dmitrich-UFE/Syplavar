using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;


public class UIHandler : MonoBehaviour
{
    private PlayerInputActions _playerInputActions;
    [SerializeField] private InventoryAI _inventoryAI;
    //[SerializeField] private AudioVolumes _audioVolumes;
    [SerializeField] private GameObject _cursor;
    [SerializeField] private GameObject _attackCursor;
    [SerializeField] private GameObject _bigInventory;
    [SerializeField] private GameObject _lowerInventory;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _settingsMenu;
    [SerializeField] private GameObject _helpMenu;
    [SerializeField] private GameObject _craftMenu;
    [SerializeField] private GameObject _storyUI;

    [Header("Окно с задачей")]
    [SerializeField] private GameObject _TaskWindow;
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private TMP_Text logoText;
    [SerializeField] private TMP_Text descText;
    [SerializeField] private TMP_Text goalText;


    [Header("Элементы настроек")]
    [SerializeField] private Toggle fsToggle;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _ambientSlider;
    [SerializeField] private Slider _soundSlider;

    
    bool isBigInvOpen;
    bool isPauseMenuOpen;
    bool isDeathMenuOpen;
    bool isTaskMenuOpen;

    void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        isBigInvOpen = false;
        isPauseMenuOpen = false;
        fsToggle.isOn = Screen.fullScreen;
        fsToggle.onValueChanged.AddListener(SetFullScreen);
    }

    internal void SetDeath(bool param)
    {
        isDeathMenuOpen = param;

        if (param)
        {
            ClosePauseMenu();
            CloseBigInventory();
        }
    }

    public void ClosePauseMenu()
    {
        isPauseMenuOpen = false;
        _settingsMenu.SetActive(false);
        _helpMenu.SetActive(false);
        _pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        _storyUI.SetActive(true);
        _lowerInventory.SetActive(true);

        SetOnCursor();
    }

    public void CloseBigInventory()
    {
        if (isBigInvOpen)
        {
            _inventoryAI.DrawLowerInventory();
            Time.timeScale = 1f;
            _lowerInventory.SetActive(true);
            _storyUI.SetActive(true);
            _bigInventory.SetActive(false);
            isBigInvOpen = false;

            SetOnCursor();
        }
    }

    void OpenBigInventory(InputAction.CallbackContext context)
    {
        if (isBigInvOpen)
        {
            _inventoryAI.DrawLowerInventory();
            Time.timeScale = 1f;
            _lowerInventory.SetActive(true);
            _bigInventory.SetActive(false);
            _storyUI.SetActive(true);
            isBigInvOpen = false;

            SetOnCursor();
        }
        else if (!isPauseMenuOpen && !isDeathMenuOpen && !isTaskMenuOpen)
        {
            _craftMenu.SetActive(false);
            _lowerInventory.SetActive(false);
            _storyUI.SetActive(false);
            _bigInventory.SetActive(true);
            Time.timeScale = 0f;
            _inventoryAI.DrawInventory();
            isBigInvOpen = true;

            SetOffCursor();
        }
    }

    void OpenPauseMenu(InputAction.CallbackContext context)
    {
        if (!isPauseMenuOpen && !isBigInvOpen && !isDeathMenuOpen && !isTaskMenuOpen)
        {
            _lowerInventory.SetActive(false);
            _storyUI.SetActive(false);
            _TaskWindow.SetActive(false);

            _pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            isPauseMenuOpen = true;

            SetOffCursor();
        }
        else if (isPauseMenuOpen)
        {
            _lowerInventory.SetActive(true);
            _storyUI.SetActive(true);
            _settingsMenu.SetActive(false);
            _helpMenu.SetActive(false);
            _pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            isPauseMenuOpen = false;
            _lowerInventory.SetActive(true);

            SetOnCursor();
        }
    }

    void OpenTaskMenuUI(InputAction.CallbackContext context)
    {
        if (isTaskMenuOpen)
        {
            _lowerInventory.SetActive(true);
            _storyUI.SetActive(true);
            Time.timeScale = 1f;
            isTaskMenuOpen = false;

            _TaskWindow.SetActive(false);
        }
        else if (!isPauseMenuOpen && !isDeathMenuOpen && !isBigInvOpen)
        {
            logoText.text = taskManager.TaskLogoText;
            descText.text = taskManager.TaskDescriptionText;
            goalText.text = taskManager.TaskGoalText;
            _lowerInventory.SetActive(false);
            _storyUI.SetActive(false);
            Time.timeScale = 0f;
            isTaskMenuOpen = true;

            _TaskWindow.SetActive(true);
        }
    }

    public void OpenSettings()
    {
        if (AudioVolumes.audioVolumes == null) return;
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

    public void OpenHelpMenu()
    {
        _helpMenu.SetActive(true);
    }

    public void EscapeToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void SetFullScreen(bool isfscreen)
    {
        Screen.fullScreen = isfscreen;
        Debug.Log("Fullscreen mode is now: " + isfscreen);
    }

    private void SetOffCursor()
    {
        _attackCursor.SetActive(false);
        _cursor.SetActive(false);
    }

    private void SetOnCursor()
    {
        _cursor.SetActive(true);
        _attackCursor.SetActive(true);
    }

    private void OnEnable()
    {
        _playerInputActions.Player.OpenBigInventory.performed += OpenBigInventory;
        _playerInputActions.Player.EscapeTo.performed += OpenPauseMenu;
        _playerInputActions.Player.OpenTaskMenu.performed += OpenTaskMenuUI;
        _playerInputActions.Enable();
    }

    private void OnDisable()
    {
        _playerInputActions.Player.OpenBigInventory.performed -= OpenBigInventory;
        _playerInputActions.Player.EscapeTo.performed -= OpenPauseMenu;
        _playerInputActions.Player.OpenTaskMenu.performed -= OpenTaskMenuUI;
        _playerInputActions.Disable();
    }
}

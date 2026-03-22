using UnityEngine;
using UnityEngine.UI;

public class PauseAndHelpWindow : MonoBehaviour
{
    private PlayerInputActions _playerInputActions;

    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _helpMenu;

    [SerializeField] bool setPause;

    [SerializeField] private Toggle fsToggle;

    void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Player.EscapeTo.performed += context => SetPauseMenu();

        fsToggle.isOn = Screen.fullScreen;
    }

    void Start()
    {
        fsToggle.onValueChanged.AddListener(SetFullScreen);
    }

    public void SetPauseMenu()
    {
        setPause = !setPause;
        _pauseMenu.SetActive(setPause);
        if (setPause)
        {
            Time.timeScale = 0f;
        }
        else
        {
            _helpMenu.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public void OpenHelpMenu()
    {
        _helpMenu.SetActive(true);
    }

    public void CloseHelpMenu()
    {
        _helpMenu.SetActive(false);
    }

    public void SetFullScreen(bool isfscreen)
    {
        Screen.fullScreen = isfscreen;
        Debug.Log("Fullscreen mode is now: " + isfscreen);
    }

    private void OnEnable()
    {
        _playerInputActions.Enable();
    }

    private void OnDisable()
    {
        _playerInputActions.Disable();
    }
}

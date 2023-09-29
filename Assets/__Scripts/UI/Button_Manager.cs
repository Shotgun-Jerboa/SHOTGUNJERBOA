using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Button_Manager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject deathMenu;
    [SerializeField] GameObject gameMusic;
    [SerializeField] SettingVars inputActions;

    public static bool GameIsPaused = false;
    private PlayerInput playerInput;
    private InputAction menuOpen;
    private InputAction menuClose;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        inputActions.input.Gameplay.MenuOPEN.performed += ctx => Pause();
        inputActions.input.UI.MenuCLOSE.performed += ctx => Resume();

    }

    private void Update()
    {

    }

    public void Quit_Game()
    {
        Application.Quit();
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        playerInput.actions.FindActionMap("UI").Enable();
        playerInput.actions.FindActionMap("Gameplay").Disable();
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        playerInput.actions.FindActionMap("UI").Disable();
        playerInput.actions.FindActionMap("Gameplay").Enable();
    }

    public void MainMenu(int sceneID)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneID);
    }

    public void Restart(int sceneID)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneID);
        Time.timeScale = 1f;
    }
}

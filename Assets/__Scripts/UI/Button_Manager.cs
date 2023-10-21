using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Button_Manager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject deathMenu;
    [SerializeField] GameObject gameMusic;
    [SerializeField] SettingVars inputActions;
    [SerializeField] PlayerInput playerInput;
    public int gameStartScene;

    public static bool GameIsPaused = false;
    public UnityEngine.UIElements.Button resume;

    [SerializeField] Animator anim;

    private void Awake()
    {

    }



    public void Quit_Game()
    {
        Application.Quit();
    }

    public void MenuOPEN(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (!GameIsPaused)
            {
                Pause();
            }
        }

    }

    public void MenuCLOSE(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (GameIsPaused)
            {
                Resume();
            }
        }
    }

    public void Pause()
    {
        playerInput.SwitchCurrentActionMap("UI");
        anim.SetTrigger("PauseMenu");
        GameIsPaused = true;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void Resume()
    {
        playerInput.SwitchCurrentActionMap("Gameplay");
        anim.SetTrigger("CloseMenu");
        GameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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

    public void StartGame()
    {
        SceneManager.LoadScene(gameStartScene);
    }
}

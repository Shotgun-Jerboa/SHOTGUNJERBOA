using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Button_Manager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject deathMenu;
    [SerializeField] GameObject gameMusic;
    [SerializeField] SettingVars inputActions;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] SettingVars sensitivity;
    public float setVolume;
    public int gameStartScene;
    public Slider sliderVal1;
    public Slider sliderVal2;
    public TMP_Dropdown resolution;
    public TMP_Dropdown screenType;
    public int see;
    public string dropdownName;
    protected int width;
    protected int height;
    protected string test;
    public GameObject canvas;

    public static bool GameIsPaused = false;
    public bool inOptions = false;
    public UnityEngine.UIElements.Button resume;

    [SerializeField] Animator anim;

    private void Awake()
    {

    }

    public void Update()
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
            if (GameIsPaused && !inOptions)
            {
                Resume();
            }
            if (inOptions)
            {
                CloseOptions();
            }
        }
    }

    public void Pause()
    {
        playerInput.SwitchCurrentActionMap("UI");
        Time.timeScale = 0;
        anim.SetTrigger("PauseMenu");
        GameIsPaused = true;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        canvas.SetActive(false);
    }

    public void Resume()
    {
        playerInput.SwitchCurrentActionMap("Gameplay");
        anim.SetTrigger("CloseMenu");
        Time.timeScale = 1;
        GameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        canvas.SetActive(true);
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

    public void ChangeVol()
    {
        setVolume = sliderVal1.value;
        setVolume = setVolume / 100;
        AudioListener.volume = setVolume;
    }

    public void ChangeSensitivity()
    {
        sensitivity.CamSensitivityX = sliderVal2.value * 5;
        sensitivity.CamSensitivityY = sliderVal2.value * 5;
    }

    public void ToOptions()
    {
        inOptions = true;
        anim.SetTrigger("GoToOptions");
    }

    public void CloseOptions()
    {
        inOptions = false;
        anim.SetTrigger("BackToMenu");
    }

    public void ResolutionOptions()
    {
        if (resolution.value == 0)
        {

        }
        else
        {
            //see = int.Parse(resolution.captionText.text);
            dropdownName = resolution.captionText.text;
            string[] splitTitle = dropdownName.Split(new string[] { "x" }, System.StringSplitOptions.None);
            int.TryParse(splitTitle[0], out width);
            int.TryParse(splitTitle[1], out height);
            Screen.SetResolution(width, height, true);
            //test = resolution.captionText;
        }
    }

    public void ScreenType()
    {
        if (screenType.value == 0)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else if (screenType.value == 1)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        else if (screenType.value == 2)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
        else if (screenType.value == 3)
        {
            Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
        }
        else
        {
            Debug.Log("Not a valid screen type");
        }
    }
}

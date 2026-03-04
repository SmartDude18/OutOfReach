using UnityEngine;
using UnityEngine.InputSystem;

public class Pause : MonoBehaviour
{
    //UI Elements
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject confirmationMenu;
    [SerializeField] UIBehavior behavior;

    //Music
    [SerializeField] AudioSource pauseMusic;

    //dialogue
    [SerializeField] AudioSource narrator;
    [SerializeField] AudioClip pauseMocking;
    [SerializeField] AudioClip pauseMocking2;
    [SerializeField] AudioClip quitMocking;
    

    private int pauseCount = 0;
    private bool paused = false;


    private void Start()
    {
        pauseMenu.SetActive(false);
        confirmationMenu.SetActive(false);
        pauseMusic.Stop();
    }

    private void Update()
    {
        if (InputSystem.actions.FindAction("Pause").WasPressedThisFrame() && !paused)
        {
            PauseGame();
            MockPlayer();
            paused = true;
        }
    }
    private void PauseGame()
    {
        //pause game
        behavior.OnPause();
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pauseMenu.SetActive(true);
        if (!pauseMusic.isPlaying)
        {
            pauseMusic.Play();
        }
        pauseCount++;
    }
    private void MockPlayer()
    {
        if (pauseMocking != null)
        {
            if (pauseCount == 1)
            {
                narrator?.PlayOneShot(pauseMocking);
            }
            else
            {
                narrator?.PlayOneShot(pauseMocking2);
            }
        }
    }

    public void OnContinue()
    {
        Debug.Log("Continue pressed");
        pauseMenu.SetActive(false);
        pauseMusic.Stop();
        narrator?.Stop();
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        behavior.OnContinue();
        paused = false;
    }
    public void OnExit()
    {
        confirmationMenu.SetActive(true);
        pauseMenu.SetActive(false);
        narrator?.Stop();

        if(quitMocking != null)
        {
            narrator?.PlayOneShot(quitMocking);
        }
    }
    public void OnConfirm()
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }
    public void OnDeny()
    {
        confirmationMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class Pause : MonoBehaviour
{
    //UI Elements
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject confirmationMenu;

    //Music
    [SerializeField] AudioSource pauseMusic;

    //dialogue
    [SerializeField] AudioSource narrator;
    [SerializeField] AudioClip pauseMocking;
    [SerializeField] AudioClip pauseMocking2;
    [SerializeField] AudioClip quitMocking;

    private int pauseCount = 0;


    private void Start()
    {
        pauseMenu.SetActive(false);
        confirmationMenu.SetActive(false);
        pauseMusic.Stop();
    }

    private void Update()
    {
        if (InputSystem.actions.FindAction("Pause").WasPressedThisFrame())
        {
            PauseGame();
            MockPlayer();
        }
    }
    private void PauseGame()
    {
        //pause game
        Time.timeScale = 0;
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
        pauseMenu.SetActive(false);
        pauseMusic.Stop();
        narrator?.Stop();
        Time.timeScale = 1;
    }
    public void OnExit()
    {
        confirmationMenu.SetActive(true);

        if(quitMocking != null)
        {
            narrator?.PlayOneShot(quitMocking);
        }
    }
    public void OnConfirm()
    {
        Application.Quit();
    }
    public void OnDeny()
    {
        confirmationMenu.SetActive(false);
    }
}

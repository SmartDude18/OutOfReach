using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIBehavior : MonoBehaviour
{
    
    [SerializeField] TMP_Text deathCounterText;
    [SerializeField] TMP_Text timerText;
    [SerializeField] PlayerController Player;

    private float timerS = 0;
    private int timerM = 0;
    public bool isPaused { get; private set; } = false;

    void Start()
    {
        formatTimer();
        deathCounterText.text = Player.playerDeaths.ToString();
    }

    void Update()
    {
        //update timer
        if (!isPaused)
        {
            timerS += Time.deltaTime;
            formatTimer();
        }
    }
    private void formatTimer()
    {
        if (timerS >= 60)
        {
            timerM++;
            timerS = 0;
        }
        timerText.text = formatDigit(timerM) + ":" + formatDigit((int)timerS);
    }
    private string formatDigit(int num)
    {
        if (num < 10)
        {
            return "0" + num.ToString();
        }
        return num.ToString();
    }
    public void OnDeath()
    {
    
        deathCounterText.text = Player.playerDeaths.ToString();
    }

    public void OnPause()
    {
        //pause timer
        isPaused = true;
    }

    public void OnContinue()
    {
        //start timer
        isPaused = false;
    }

    public void ResetGame()
    {
        timerS = 0;
        timerM = 0;
    }
    
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBehavior : MonoBehaviour
{
    
    [SerializeField] TMP_Text deathCounterText;
    [SerializeField] TMP_Text timerText;

    private float timerS = 0;
    private int timerM = 0;
    private int deathCount = 0;
    private bool isPaused = false;

    void Start()
    {
        formatTimer();
        deathCounterText.text = deathCount.ToString();
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
        //increment death counter
        deathCount++;
        deathCounterText.text = deathCount.ToString();
    }

    private void OnPause()
    {
        //pause timer
        isPaused = true;
    }

    private void OnContinue()
    {
        //start timer
        isPaused = false;
    }
}

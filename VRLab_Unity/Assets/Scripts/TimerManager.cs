using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TimerManager : MonoBehaviour
{
    public static TimerManager instance;

    public TextMeshProUGUI firstText;
    public TextMeshProUGUI secondText;
    public TextMeshProUGUI reastart;

    private int bestTimer;
    private bool started;
    private float timer;
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        bestTimer = 19870;

        
    }

    private void Update()
    {
        if (started)
        {
            timer += Time.deltaTime;
            if(bestTimer != 0)
            {
                secondText.text = CalculateTime((int)timer);
            }
            else
            {
                firstText.text = CalculateTime((int)timer);
            }
        }
    }


    public void StartTimer()
    {
        started = true;

        timer = 0;
        bestTimer = PlayerPrefs.GetInt("BestTimer");
        firstText.gameObject.SetActive(true);
        if (bestTimer != 0)
        {
            firstText.text = "Best Score  ";
            firstText.text += CalculateTime(bestTimer);
            firstText.color = Color.blue;
            secondText.gameObject.SetActive(true);
        }
        else
        {
            secondText.gameObject.SetActive(false);
        }
    }

    public void Finish()
    {
        started = false;

        PlayerPrefs.SetInt("BestTimer", (int)timer);
        PlayerPrefs.Save();

        if (bestTimer > 0)
        {
            if(bestTimer < timer)
            {
                secondText.text = "New Best Score  " + secondText.text;
                secondText.color = Color.green;
            }
            else
            {
                secondText.color = Color.red;
            }
        }
        else
        {
            firstText.text = "New best Score" + CalculateTime((int)timer);
        }
        reastart.text = "Press A to restart \n Press B to quit";
    }



    private string CalculateTime(int timer)
    {
        string _string = string.Empty;
        int min = timer  / 60;
        int sec = timer - min * 60;
        if (min < 10)
            _string += min + " : ";
        else
            _string +=  min + " : ";
        if (sec < 10)
            _string += "0" + sec;
        else
            _string += sec;

      
        return _string;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{

    public Text timer;
    private float time = 3 * 60;

    private bool startTimer = false;
    // Start is called before the first frame update
    void Start()
    {
        UpdateTimer();
    }

    // Update is called once per frame
    void Update()
    {
        if (startTimer && time > 0.0f){
            time -= Time.deltaTime;
            UpdateTimer();
        } else if (time < 0.0f){
            timer.text = "Time Up!";
        }
    }

    void UpdateTimer() {

        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        timer.text = minutes.ToString("D2") + ":" + seconds.ToString("D2");

    }

    public void StartTimer() {
        startTimer = !startTimer;
    }
}

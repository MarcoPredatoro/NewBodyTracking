using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{

    private Text timer;
    private float time = 5 * 60;
    public GameObject gameOver;
    public Points points;

    private bool startTimer = false;
    // Start is called before the first frame update
    void Start()
    {
        timer = GetComponentInChildren<Text>();
        UpdateTimer();
    }

    // Update is called once per frame
    void Update()
    {
        if (startTimer && time > 0.0f){
            time -= Time.deltaTime;
            UpdateTimer();
        } else if (startTimer && time < 0.0f){
            EndGame();
            StartTimer(false);
        }
    }

    void UpdateTimer() {

        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        timer.text = minutes.ToString("D2") + ":" + seconds.ToString("D2");

    }

    public void StartTimer(bool value) {
        startTimer = value;
    }

    public void ResetTimer() {
        time = 5 * 60;
        StartTimer(true);
    }

    void EndGame() {
        gameOver.SetActive(true);
        gameOver.GetComponentInChildren<Text>().text = "The Winner is " + (points.getPoints() > points.threshold ? "Marco" : "Polo");
        GameObject.Find("networking").GetComponent<EventManager>().SendGameOver();
    }
}

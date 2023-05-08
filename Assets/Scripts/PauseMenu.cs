using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    public static bool isPaused = false;

    public GameObject pauseMenuUi;
    public GameObject pointsUi;
    public GameObject endScreen;

    void Start() {
        Unpause();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (endScreen.activeInHierarchy)
            {
                endScreen.SetActive(false);
            }
            else if (isPaused) {
                Unpause();
            } else {
                Pause();
            }
        } 
    }

    void Unpause() {
        pauseMenuUi.SetActive(false);
        pointsUi.SetActive(true);
        isPaused = false;
    }
    void Pause() {
        pauseMenuUi.SetActive(true);
        pointsUi.SetActive(false);
        isPaused = true;
    }

    void ResetPoints() {
        
    }

   
}

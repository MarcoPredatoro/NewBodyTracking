using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class RFID : MonoBehaviour
{
    public InputField input;
    public bool activate = true;
    private int[] rfidUsed = {0, 0, 0};
    public int totalNumber = 5;

    void Start()
    {
        // input.Select();
        input.ActivateInputField();
    }
    void Update()
    {
        // real eggs
        if(input.text == "2860601348" && rfidUsed[0] < totalNumber) {
            // GetComponent<main>().losePoints(10);
            GetComponent<EventManager>().IncrementPointsByRFID(20);
            input.text = null;
            // input.Select();
            input.ActivateInputField();
            rfidUsed[0]++;
        } 
        else if(input.text == "2860743172" && rfidUsed[1] < totalNumber) {
            // GetComponent<main>().updatePoints(20);
            GetComponent<EventManager>().IncrementPointsByRFID(20);
            input.text = null;
            // input.Select();
            input.ActivateInputField();
            rfidUsed[1]++;
        } 
        else if(input.text == "1513049860" && rfidUsed[2] < totalNumber) {
            // GetComponent<main>().updatePoints(30);
            GetComponent<EventManager>().IncrementPointsByRFID(20);
            input.text = null;
            // input.Select();
            input.ActivateInputField();
            rfidUsed[2]++;
        }
        else if(input.text == "2860674820" && rfidUsed[1] < totalNumber) {
            // GetComponent<main>().updatePoints(20);
            GetComponent<EventManager>().IncrementPointsByRFID(20);
            input.text = null;
            // input.Select();
            input.ActivateInputField();
            rfidUsed[1]++;
        } 
        // fake eggs
        else if(input.text == "1437055369" && rfidUsed[2] < totalNumber) {
            // GetComponent<main>().updatePoints(30);
            GetComponent<EventManager>().IncrementPointsByRFID(-100);
            input.text = null;
            // input.Select();
            input.ActivateInputField();
            rfidUsed[2]++;
        }
        else if(input.text == "2860599812" && rfidUsed[1] < totalNumber) {
            // GetComponent<main>().updatePoints(20);
            GetComponent<EventManager>().IncrementPointsByRFID(-100);
            input.text = null;
            // input.Select();
            input.ActivateInputField();
            rfidUsed[1]++;
        } 
        else if(input.text == "2860741636" && rfidUsed[2] < totalNumber) {
            // GetComponent<main>().updatePoints(30);
            GetComponent<EventManager>().IncrementPointsByRFID(-100);
            input.text = null;
            // input.Select();
            input.ActivateInputField();
            rfidUsed[2]++;
        }
        else if(input.text == "2860598276" && rfidUsed[2] < totalNumber) {
            // GetComponent<main>().updatePoints(30);
            GetComponent<EventManager>().IncrementPointsByRFID(-100);
            input.text = null;
            // input.Select();
            input.ActivateInputField();
            rfidUsed[2]++;
        }
        if(!input.isFocused && activate){
            input.ActivateInputField();
        }
    }
}


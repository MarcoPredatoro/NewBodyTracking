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
        
        if(input.text == "1437055369" && rfidUsed[0] < totalNumber) {
            // GetComponent<main>().losePoints(10);
            GetComponent<EventManager>().IncrementPointsByRFID(10);
            input.text = null;
            // input.Select();
            input.ActivateInputField();
            rfidUsed[0]++;
        } 
        else if(input.text == "1437198297" && rfidUsed[1] < totalNumber) {
            // GetComponent<main>().updatePoints(20);
            GetComponent<EventManager>().IncrementPointsByRFID(20);
            input.text = null;
            // input.Select();
            input.ActivateInputField();
            rfidUsed[1]++;
        } 
        else if(input.text == "1513049860" && rfidUsed[2] < totalNumber) {
            // GetComponent<main>().updatePoints(30);
            GetComponent<EventManager>().IncrementPointsByRFID(30);
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


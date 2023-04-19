using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;



public class RFID : MonoBehaviour
{
    public InputField input;
    public bool activate = true;
    private int[] rfidUsed = { 0, 0, 0 };
    public int totalNumber = 5;



    Dictionary<string, string> egg = new Dictionary<string, string>()
    {
        {"Real1", "2860760324" },

        {"Real2", "2860758788"},

        {"Real3", "2860770052"},

        {"Real4", "2860741636"},

        {"Real5", "2860599812"},

        {"Real6", "2860773124"},

    };

    Dictionary<string, bool> eggRfid = new Dictionary<string, bool>()
    {
        //Real eggs
        {"2860760324",true},
        {"2860758788",true},
        {"2860770052",true},
        {"2860741636",true},
        {"2860599812",true},
        {"2860773124",true},
        //Rotten eggs
        {"2860598276",false},

        {"2860674820",false},

        {"2860743172",false},

        {"2860771588",false},

    };

    public void UpdateEggStatus(string input)
    {
        string rfidCode = egg[input];
        if (eggRfid.ContainsKey(rfidCode) && eggRfid[rfidCode])
        {
            eggRfid[rfidCode] = false;
        }
    }


    void Start()
    {
        // input.Select();
        input.ActivateInputField();
    }
    void Update()


    {
        if (eggRfid[input.text] == true)
        {
            // GetComponent<main>().losePoints(10);
            GetComponent<EventManager>().IncrementPointsByRFID(20);
            input.text = null;
            // input.Select();
            input.ActivateInputField();
            rfidUsed[0]++;
        }

        if (eggRfid[input.text] == false)
        {
            // GetComponent<main>().losePoints(10);
            GetComponent<EventManager>().IncrementPointsByRFID(-100);
            input.text = null;
            // input.Select();
            input.ActivateInputField();
            rfidUsed[0]++;
        }








        // real eggs
        if (input.text == "2860601348" && rfidUsed[0] < totalNumber)
        {
            // GetComponent<main>().losePoints(10);
            GetComponent<EventManager>().IncrementPointsByRFID(20);
            input.text = null;
            // input.Select();
            input.ActivateInputField();
            rfidUsed[0]++;
        }

        else if (input.text == "2860743172" && rfidUsed[1] < totalNumber)
        {
            // GetComponent<main>().updatePoints(20);
            GetComponent<EventManager>().IncrementPointsByRFID(20);
            input.text = null;
            // input.Select();
            input.ActivateInputField();
            rfidUsed[1]++;
        }
        else if (input.text == "1513049860" && rfidUsed[2] < totalNumber)
        {
            // GetComponent<main>().updatePoints(30);
            GetComponent<EventManager>().IncrementPointsByRFID(20);
            input.text = null;
            // input.Select();
            input.ActivateInputField();
            rfidUsed[2]++;
        }
        else if (input.text == "2860674820" && rfidUsed[1] < totalNumber)
        {
            // GetComponent<main>().updatePoints(20);
            GetComponent<EventManager>().IncrementPointsByRFID(20);
            input.text = null;
            // input.Select();
            input.ActivateInputField();
            rfidUsed[1]++;
        }
        // fake eggs
        else if (input.text == "1437055369" && rfidUsed[2] < totalNumber)
        {
            // GetComponent<main>().updatePoints(30);
            GetComponent<EventManager>().IncrementPointsByRFID(-100);
            input.text = null;
            // input.Select();
            input.ActivateInputField();
            rfidUsed[2]++;
        }
        else if (input.text == "2860599812" && rfidUsed[1] < totalNumber)
        {
            // GetComponent<main>().updatePoints(20);
            GetComponent<EventManager>().IncrementPointsByRFID(-100);
            input.text = null;
            // input.Select();
            input.ActivateInputField();
            rfidUsed[1]++;
        }
        else if (input.text == "2860741636" && rfidUsed[2] < totalNumber)
        {
            // GetComponent<main>().updatePoints(30);
            GetComponent<EventManager>().IncrementPointsByRFID(-100);
            input.text = null;
            // input.Select();
            input.ActivateInputField();
            rfidUsed[2]++;
        }
        else if (input.text == "2860598276" && rfidUsed[2] < totalNumber)
        {
            // GetComponent<main>().updatePoints(30);
            GetComponent<EventManager>().IncrementPointsByRFID(-100);
            input.text = null;
            // input.Select();
            input.ActivateInputField();
            rfidUsed[2]++;
        }
        if (!input.isFocused && activate)
        {
            input.ActivateInputField();
        }
    }
}
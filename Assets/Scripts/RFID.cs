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

    EventManager eventmanager;



    Dictionary<string, string> egg = new Dictionary<string, string>()
    {
        {"Real1", "2860598276"},
        {"Real2", "2860743172"},
        {"Real3", "2860674820"},
        {"Real4", "2860599812"},
        {"Real5", "2860758788"},
        {"Real6", "2860876548"},
        {"Real7", "2860814852"},
        {"Real8", "2860773124"},
        {"Real9", "2860760324"},
        {"Real10", "2860771588"},
        {"Real11", "2860770052"},
        {"Real12", "2860807940"},
        {"Real13", "2860809988"},
        {"Real14", "2860811780"},
        {"Real15", "2860813316"},

    };

    Dictionary<string, bool> eggRfid;
    public void UpdateEggStatus(string input)
    {
        string rfidCode = egg[input];
        if (eggRfid.ContainsKey(rfidCode) && eggRfid[rfidCode])
        {
            eggRfid[rfidCode] = false;
        }
    }

    public void ResetEggs()
    {
        eggRfid = new Dictionary<string, bool>()
    {
        //Real eggs
        {"2860598276", true},
        {"2860743172", true},
        {"2860674820", true},
        {"2860599812", true},
        {"2860758788", true},
        {"2860876548", true},
        {"2860814852", true},
        {"2860773124", true},
        {"2860760324", true},
        {"2860771588", true},
        {"2860770052", true},
        {"2860807940", true},
        {"2860809988", true},
        {"2860811780", true},
        {"2860813316", true},

        //Rotten eggs
        {"2860879620", false},
        {"2860878084", false},
        {"2860741636", false},
        {"2860875012", false},
        {"2860806404", false},
    };
    }


    void Start()
    {
        // input.Select();
        input.ActivateInputField();
        eventmanager = GameObject.Find("networking").GetComponent<EventManager>();
        ResetEggs();
    }
    void Update()
    {
        if (eggRfid.ContainsKey(input.text))
        {
            if (eggRfid[input.text] == true)
            {
                // GetComponent<main>().losePoints(10);
                eventmanager.IncrementPointsByRFID(16);
                input.text = null;
                // input.Select();
                input.ActivateInputField();
                rfidUsed[0]++;
            }
            else
            {
                // GetComponent<main>().losePoints(10);
                eventmanager.IncrementPointsByRFID(-31);
                input.text = null;
                // input.Select();
                input.ActivateInputField();
                rfidUsed[0]++;
            }
        }

        if (!input.isFocused && activate)
        {
            input.ActivateInputField();
        }
    }
}
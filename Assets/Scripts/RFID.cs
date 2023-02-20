using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Photon.Pun;
using ExitGames.Client.Photon;
using System;
using Photon.Realtime;

public class RFID : MonoBehaviourPun
{
    public InputField input;
    public bool activate = true;
    private int[] rfidUsed = {0, 0, 0};
    public int totalNumber = 20;

    // event codes
    private const byte RFID_POINTS_EVENT = 1;
    private const byte MARCO_STAB_EVENT = 2;

    void Start()
    {
        // input.Select();
        input.ActivateInputField();
    }
    void Update()
    {
        
        if(input.text == "1437055369" && rfidUsed[0] < totalNumber) {
            // GetComponent<main>().losePoints(10);
            IncrementPointsByRFID(10);
            input.text = null;
            // input.Select();
            input.ActivateInputField();
            rfidUsed[0]++;
        } 
        else if(input.text == "1437198297" && rfidUsed[1] < totalNumber) {
            // GetComponent<main>().updatePoints(20);
            IncrementPointsByRFID(20);
            input.text = null;
            // input.Select();
            input.ActivateInputField();
            rfidUsed[1]++;
        } 
        else if(input.text == "1513049860" && rfidUsed[2] < totalNumber) {
            // GetComponent<main>().updatePoints(30);
            IncrementPointsByRFID(30);
            input.text = null;
            // input.Select();
            input.ActivateInputField();
            rfidUsed[2]++;
        } 
        if(!input.isFocused && activate){
            input.ActivateInputField();
        }
    }

    private void IncrementPointsByRFID(int numberOfPoints)
    {
        Debug.Log("sent: " + numberOfPoints);
        RaiseEventOptions options = RaiseEventOptions.Default;
        options.Receivers = ReceiverGroup.All;
        PhotonNetwork.RaiseEvent(RFID_POINTS_EVENT, numberOfPoints, options, SendOptions.SendReliable);
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
    }

    private void NetworkingClient_EventReceived(EventData obj)
    {
        if (obj.Code == RFID_POINTS_EVENT)
        {
            int numberOfPoints = (int)obj.CustomData;
            Debug.Log("received: " + obj.CustomData);
            GetComponent<main>().updatePoints(numberOfPoints);
        }
        else if (obj.Code == MARCO_STAB_EVENT)
        {
            //TODO: replace all "losePoints" calls
            GetComponent<main>().losePoints(10);
        }
    }
}


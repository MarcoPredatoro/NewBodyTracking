using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Photon.Pun;
using ExitGames.Client.Photon;
using System;
using Photon.Realtime;

public class EventManager : MonoBehaviour
{
    // event codes
    private const byte RFID_POINTS_EVENT = 1;
    private const byte MARCO_STAB_EVENT = 2;
    private const byte RESET_POINTS_EVENT = 3;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void IncrementPointsByRFID(int numberOfPoints)
    {
        Debug.Log("sent: " + numberOfPoints);
        RaiseEventOptions options = RaiseEventOptions.Default;
        options.Receivers = ReceiverGroup.All;
        PhotonNetwork.RaiseEvent(RFID_POINTS_EVENT, numberOfPoints, options, SendOptions.SendReliable);
    }

    public void SendMarcoCollision()
    {
        Debug.Log("sending collision");
        RaiseEventOptions options = RaiseEventOptions.Default;
        options.Receivers = ReceiverGroup.All;
        PhotonNetwork.RaiseEvent(MARCO_STAB_EVENT, 30, options, SendOptions.SendReliable);
    }

    public void ResetPoints()
    {
        Debug.Log("resetting points");
        RaiseEventOptions options = RaiseEventOptions.Default;
        options.Receivers = ReceiverGroup.All;
        PhotonNetwork.RaiseEvent(RESET_POINTS_EVENT, true, options, SendOptions.SendReliable);
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
            Debug.Log("RFID received: " + obj.CustomData);
            GetComponent<main>().updatePoints(numberOfPoints);
        }
        else if (obj.Code == MARCO_STAB_EVENT)
        {
            int numberOfPoints = (int)obj.CustomData;
            Debug.Log("stab received: " + obj.CustomData);
            GetComponent<main>().updatePoints(-numberOfPoints);
        }
        else if (obj.Code == RESET_POINTS_EVENT)
        {
            GetComponent<main>().points = 0;
            GetComponent<main>().updatePoints(0);
        }
    }
}

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
    private const byte BLIND_EVENT = 4;
    private const byte DECOY_EVENT = 5;
    private const byte EGG_TIMER_EVENT = 6;
    private const byte GAME_COMPLETE_EVENT = 7;
    private const byte GAME_START = 8;
    public Points points;
    private int noHits=0;

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

    public void SendGameOver()
    {
        Debug.Log("sending game over");
        RaiseEventOptions options = RaiseEventOptions.Default;
        options.Receivers = ReceiverGroup.All;
        PhotonNetwork.RaiseEvent(GAME_COMPLETE_EVENT, 30, options, SendOptions.SendReliable);
    }

    public void SendGameStart()
    {
        Debug.Log("sending game start");
        RaiseEventOptions options = RaiseEventOptions.Default;
        options.Receivers = ReceiverGroup.All;
        PhotonNetwork.RaiseEvent(GAME_START, 30, options, SendOptions.SendReliable);
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
            points.updatePoints(numberOfPoints);
        }
        else if (obj.Code == MARCO_STAB_EVENT)
        {
            int numberOfPoints = (int)obj.CustomData;
            Debug.Log("stab received: " + obj.CustomData);
            points.updatePoints(-numberOfPoints);
            Debug.Log(noHits++);
        }
        else if (obj.Code == RESET_POINTS_EVENT)
        {
            points.resetPoints();
        }
        else if (obj.Code == GAME_COMPLETE_EVENT)
        {
            Debug.Log("GAME OVER");
            ResetPoints();
        }
        else if (obj.Code == EGG_TIMER_EVENT)

        {
            // open the box and generate a new sequence?
            string eggCode = (string)obj.CustomData;
            Debug.Log("eggCode recieved: " + obj.CustomData);
            GameObject.Find("Main").GetComponent<RFID>().UpdateEggStatus(eggCode);

        }
    }
}

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
    private const byte SEQUENCE_GENERATED_EVENT = 4;
    private const byte SEQUENCE_COMPLETED_EVENT = 5;
    private const byte GAME_OVER_EVENT = 7;
    public Points points;

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

    //private int sequenceLength = 5;
    private System.Random rnd = new System.Random();
    public void GenerateSequence(int sequenceLength, int numberOfButtons)
    {
        int[] sequence = new int[sequenceLength];
        for (int i = 0; i < sequenceLength; i++)
        {
            sequence[i] = rnd.Next(97, 97 + numberOfButtons);
        }
        Debug.Log("broadcasting sequence: " + sequence[0] + sequence[1] + sequence[2] + sequence[3]);
        RaiseEventOptions options = RaiseEventOptions.Default;
        options.Receivers = ReceiverGroup.All;
        PhotonNetwork.RaiseEvent(SEQUENCE_GENERATED_EVENT, sequence, options, SendOptions.SendReliable);
    }
    public void SequenceCompleted()
    {
        Debug.Log("sequence completed");
        RaiseEventOptions options = RaiseEventOptions.Default;
        options.Receivers = ReceiverGroup.All;
        PhotonNetwork.RaiseEvent(SEQUENCE_COMPLETED_EVENT, true, options, SendOptions.SendReliable);
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
        }
        else if (obj.Code == RESET_POINTS_EVENT)
        {
            points.resetPoints();
        }
        else if (obj.Code == SEQUENCE_GENERATED_EVENT)
        {
            // hand the sequence to the button listener to be listened for
            int[] sequence = (int[])obj.CustomData;
            Debug.Log("received sequence: " + sequence[0] + sequence[1] + sequence[2]);
            GetComponent<ButtonListener>().RecieveSequence(sequence);
        }
        else if (obj.Code == SEQUENCE_COMPLETED_EVENT)
        {
            // open the box and generate a new sequence?
            points.updatePoints(20);
            GenerateSequence(4, 4);
        }
        else if (obj.Code == GAME_OVER_EVENT)
        {
            Debug.Log("GAME OVER");
            ResetPoints();
        }
    }
}

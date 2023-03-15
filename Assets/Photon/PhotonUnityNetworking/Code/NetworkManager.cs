using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;


public class NetworkManager : MonoBehaviourPunCallbacks
{
    // event codes
    private const byte RESET_POINTS_EVENT = 3;

    // Start is called before the first frame update
    void Start()
    {
        ConnecttoServer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ConnecttoServer()
    {
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Connecting to server....");
    }

    public override void OnConnectedToMaster()
    {
        
        //PhotonNetwork.JoinLobby();
        base.OnConnectedToMaster();
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 5;
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;

        PhotonNetwork.JoinOrCreateRoom("Room1", roomOptions, TypedLobby.Default);
        Debug.Log("Connected to server");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        base.OnJoinedRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("New Player entered room");
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log("resetting points");
        RaiseEventOptions options = RaiseEventOptions.Default;
        options.Receivers = ReceiverGroup.All;
        PhotonNetwork.RaiseEvent(RESET_POINTS_EVENT, true, options, SendOptions.SendReliable);

    }

    public override void OnCreatedRoom() {
        Debug.Log("Created Room");
    }

   
}
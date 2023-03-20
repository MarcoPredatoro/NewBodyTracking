using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkPlayerSpawn : MonoBehaviourPunCallbacks
{

  private GameObject spawnedPlayerPrefab;
  // private GameObject spawnedPlayerPrefab2;

  public override void OnJoinedRoom()
  {
    base.OnJoinedRoom();
    spawnedPlayerPrefab = PhotonNetwork.Instantiate("polo-with-bones", transform.position, transform.rotation);
    Debug.Log("Player spawned");
    // spawnedPlayerPrefab2 = PhotonNetwork.Instantiate("Polo2", transform.position, transform.rotation);
    // Debug.Log("Player spawned2");
  }

  public override void OnLeftRoom()
  {
    base.OnLeftRoom();
    PhotonNetwork.Destroy(spawnedPlayerPrefab);
    // PhotonNetwork.Destroy(spawnedPlayerPrefab2);
    Debug.Log("Player destroyed");
  }




}
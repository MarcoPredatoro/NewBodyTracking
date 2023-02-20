using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class NetworkPlayerMapping : MonoBehaviourPun
{
    public string bone;
    public string pointBody;
    private Transform playerHead;
    private PhotonView photonView;
    // public GameObject cubePrefab ;

    //event codes:
    private const byte MARCO_STAB_EVENT = 2;

    // // Start is called before the first frame update

    private void MarcoCollision()
    {
        Debug.Log("sent: ");
        RaiseEventOptions options = RaiseEventOptions.Default;
        options.Receivers = ReceiverGroup.All;
        PhotonNetwork.RaiseEvent(MARCO_STAB_EVENT, 10, options, SendOptions.SendReliable);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Marco(Clone)"){
            // Debug.Log("!!!!!!!!!!!!!!!!!!!!!!");
            //GameObject.Find("Main").GetComponent<main>().losePoints(10);
            MarcoCollision();
        }
        if (collision.gameObject.name == "LeftHand"){
            // Debug.Log("!!!!!!!!!!!!!!!!!!!!!!");
            //GameObject.Find("Main").GetComponent<main>().losePoints(10);
            MarcoCollision();
        }
        if (collision.gameObject.name == "RightHand"){
            // Debug.Log("!!!!!!!!!!!!!!!!!!!!!!");
            //GameObject.Find("Main").GetComponent<main>().losePoints(10);
            MarcoCollision();

        }
    } 
    
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        playerHead = GameObject.Find("Kinect4AzureTracker").GetComponentInChildren<Transform>().Find(pointBody).Find(bone);

    }

    // void OnTriggerEnter (Collider other ){
    //     if(!photonView.IsMine){
    //         return;
    //     }

    //     PhotonView otherPhotonView = other.gameObject.GetComponent<PhotonView>();
    //     if(otherPhotonView != null && otherPhotonView.IsMine){
    //         interactionDetected= true;
    //         photonView.ObservedComponents.Add(this);
    //     }
    // }

    // void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info ){
    //     if (stream.IsWriting){
    //         stream.SendNext(interactionDetected);
    //     }
    //     else{
    //         interactionDetected=(bool)stream.ReceiveNext();
    //         if(interactionDetected){
    //             Vector3 interactionPosition = transform.position;
    //             GameObject cube = PhotonNetwork.Instantiate(cubePrefab.name, interactionPosition, Quaternion.identity);
    //         }
    //     }
    // }
    
    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine) {
            mapSpawnedObject(playerHead);
        }
    }

    void mapSpawnedObject (Transform t) {
        transform.position = t.position;
        transform.rotation = t.rotation;

    }
}

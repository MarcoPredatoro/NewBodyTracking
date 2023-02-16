using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkPlayerMapping : MonoBehaviour
{
    private Transform Kinect4AzureTracker;
    private PhotonView photonView;
    // public GameObject cubePrefab ;

    // // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Marco(Clone)"){
            // Debug.Log("!!!!!!!!!!!!!!!!!!!!!!");
            GameObject.Find("Main").GetComponent<main>().losePoints(10);
        }
    } 
    
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        Kinect4AzureTracker = GameObject.Find("Kinect4AzureTracker").GetComponentInChildren<Transform>().Find("pointBody").Find("pelvis");
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
            mapSpawnedObject(Kinect4AzureTracker);
        }
    }

    void mapSpawnedObject (Transform t) {
        transform.position = t.position;
        transform.rotation = t.rotation;

    }
}

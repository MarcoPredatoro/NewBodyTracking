using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkPlayerMapping : MonoBehaviour
{
    public string bone;
    public string pointBody;
    private Transform playerHead;
    private PhotonView photonView;
    // public GameObject cubePrefab ;

    // // Start is called before the first frame update
    
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        playerHead = GameObject.Find("MergedBodyTracker").GetComponentInChildren<Transform>().Find(pointBody).Find("pointBody").Find(bone);

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

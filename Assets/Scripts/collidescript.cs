using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class collidescript : MonoBehaviour
{
    private PhotonView photonView;
    public GameObject cubePrefab ;

     private void OnCollisionEnter(Collision collision)
    {
         if (collision.gameObject.name == "Polo")
        {
            PhotonNetwork.Instantiate(cubePrefab.name, transform.position, transform.rotation);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
         photonView = GetComponent<PhotonView>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

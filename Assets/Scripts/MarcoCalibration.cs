using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarcoCalibration : MonoBehaviour
{
    public Text syncText;
    public GameObject main;
    private SkeletalTrackingProvider m_skeletalTrackingProvider;
    private int noIterations = 100;
    public GameObject firstPelvis;

    void Start() {
    }


    public void Syncronisation() {
        StartCoroutine(WaitCameraMarcoCalibrate());
    }

    public IEnumerator<WaitForSeconds> WaitCameraMarcoCalibrate() {
        Debug.Log("Starting Marco Calibration");
        float timer = 0.5f;
        bool success = true;
        var marco = GameObject.Find("TestMarco(Clone)");
        var parent = GameObject.Find("MarcoContainer");
        var m_skeletalTrackingProvider = main.GetComponent<main>().GetSkeletalTrackingProvider(0);
        
        // Wait for the skeleton Tracker to be running and marco to have connected to the game
        while ((!m_skeletalTrackingProvider.IsRunning || marco == null) && success){
            yield return new WaitForSeconds(timer);
            timer += 0.25f;
            marco = GameObject.Find("TestMarco(Clone)");

            if(timer > 800){
                success = false;
                break;
            }
        }
        parent.transform.position = new Vector3(0,0,0);

        
        if (success) {
            // Get Skeleton position
            var position = new Vector3(0,0,0);
            for(int i = 0; i < noIterations; i++){
                position += marco.transform.position - firstPelvis.transform.position;
                syncText.text = "" + i;
                yield return new WaitForSeconds(0.01f);
            }
            position /= noIterations;
            parent.transform.position = new Vector3(position.x, position.y+1f, position.z);

            // marco.transform.SetParent(parent.transform);
            Debug.Log("Successful marco calibration");
        } else {
            Debug.Log("Marco Calibration Failed");
        }

    }

}

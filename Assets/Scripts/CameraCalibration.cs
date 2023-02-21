using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraCalibration : MonoBehaviour
{
    private int noIterations = 100;

    public GameObject firstPelvis;
    public GameObject secondPelvis;

    public GameObject main;

    public GameObject container;
    public GameObject tracker;
    public GameObject container1;
    public GameObject tracker1;

    public Text moveText;
    public Text syncText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void MoveCamera0() {
        StartCoroutine(WaitMoveCamera());
    }

    IEnumerator<WaitForSeconds> WaitMoveCamera() {
        Debug.Log("Starting Camera Movement");
        float timer = 0.5f;
        bool success = true;
        var m_skeletalTrackingProvider = main.GetComponent<main>().GetSkeletalTrackingProvider(0);
        
        while (!m_skeletalTrackingProvider.IsRunning){
            yield return new WaitForSeconds(timer);
            timer += 0.25f;
            if(timer > 60){
                success = false;
                break;
            }
        }

        if (success) {
            Debug.Log("Moving Camera");  
            Vector3 translation = new Vector3(0,0,0);
            Vector3 rotation = new UnityEngine.Vector3(0,0,0);
            for (var i = 0; i < noIterations; i++){
                moveText.text = "" + i;
                var bone_0 = firstPelvis.transform.position;
                var rotation_0 = firstPelvis.transform.rotation.eulerAngles;

                translation +=  new Vector3(0f, 1f, 0f) - bone_0;
                rotation += new Vector3(0,0,0) - rotation_0;  

                yield return new WaitForSeconds(0.01f);
            }   
            translation /= noIterations;
            rotation /= noIterations;

            tracker.transform.rotation = Quaternion.Euler(rotation);
            container.transform.position = new Vector3(translation.x, translation.y, translation.z);

            Debug.Log(translation + " " + rotation);
        } else {
            Debug.Log("Camera Calibration failed");
        }
    } 

    public void Syncronisation() {
        StartCoroutine(WaitTwoCameraCalibrate());
    }
    IEnumerator<WaitForSeconds> WaitTwoCameraCalibrate() {
        Debug.Log("Starting Camera Calibration");
        float timer = 0.5f;
        bool success = true;
        var m_skeletalTrackingProvider = main.GetComponent<main>().GetSkeletalTrackingProvider(0);
        var m_skeletalTrackingProvider1 = main.GetComponent<main>().GetSkeletalTrackingProvider(1);
        
        while (!(m_skeletalTrackingProvider.IsRunning && m_skeletalTrackingProvider1.IsRunning)){
            yield return new WaitForSeconds(timer);
            timer += 0.25f;
            if(timer > 60){
                success = false;
                break;
            }
        }

        if (success) {
            Debug.Log("Calibrating Cameras");  
            Vector3 translation = new Vector3(0,0,0);
            Vector3 rotation = new UnityEngine.Vector3(0,0,0);
            for (var i = 0; i < noIterations; i++){
                syncText.text = "" + i;
                var bone_0 = firstPelvis.transform.position;
                var bone_1 = secondPelvis.transform.position;
                var rotation_0 = firstPelvis.transform.rotation.eulerAngles;
                var rotation_1 = secondPelvis.transform.rotation.eulerAngles;

                translation +=  bone_0 - bone_1;
                rotation += rotation_0 - rotation_1;  

                yield return new WaitForSeconds(0.01f);
            }   
            translation /= noIterations;
            rotation /= noIterations;

            tracker1.transform.rotation = Quaternion.Euler(rotation);
            container1.transform.position = new Vector3(translation.x, translation.y, translation.z);

            Debug.Log(translation + " " + rotation);
        } else {
            Debug.Log("Camera Calibration failed");
        }
    } 

    IEnumerator<WaitForSeconds> TestWaitTwoCameraCalibrate() {
        Debug.Log("Starting Camera Calibration");
        float timer = 0.5f;
        bool success = true;
        var m_skeletalTrackingProvider = main.GetComponent<main>().GetSkeletalTrackingProvider(0);
        // var m_skeletalTrackingProvider1 = main.GetComponent<main>().GetSkeletalTrackingProvider(1);
        
        while (!(m_skeletalTrackingProvider.IsRunning)){
            yield return new WaitForSeconds(timer);
            timer += 0.25f;
            if(timer > 60){
                success = false;
                break;
            }
        }

        if (success) {
            Debug.Log("Calibrating Cameras");  
            Vector3 translation = new Vector3(0,0,0);
            Vector3 rotation = new UnityEngine.Vector3(0,0,0);
            for (var i = 0; i < noIterations; i++){
                syncText.text = "" + i;
                var bone_0 = firstPelvis.transform.position;
                var bone_1 = secondPelvis.transform.position;
                var rotation_0 = firstPelvis.transform.rotation.eulerAngles;
                var rotation_1 = secondPelvis.transform.rotation.eulerAngles;

                translation +=  bone_0 - bone_1;
                rotation += rotation_0 - rotation_1;   
                yield return new WaitForSeconds(0.05f);
            }
            translation /= noIterations;
            rotation /= noIterations;

            tracker.transform.rotation = Quaternion.Euler(rotation);
            container.transform.position = new Vector3(translation.x, translation.y, translation.z);
        } else {
            Debug.Log("Camera Calibration failed");
        }
    } 

}


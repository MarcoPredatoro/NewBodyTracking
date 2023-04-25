using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.Azure.Kinect.BodyTracking;

public class CameraCalibration : MonoBehaviour
{
    private int noIterations = 300;

    public GameObject firstPelvis;
    public GameObject secondPelvis;
    public GameObject firstFoot;
    public GameObject secondFoot;

    public GameObject main;

    public GameObject container;
    public GameObject tracker;
    public GameObject container1;
    public GameObject tracker1;

    public Text moveText0;
    public Text moveText1;
    public Text syncText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void MoveCamera0() {
        StartCoroutine(WaitMoveCamera(0, moveText0, firstPelvis, firstFoot, tracker, container));
    }

    public void MoveCamera1() {
        StartCoroutine(WaitMoveCamera(1, moveText1, secondPelvis, secondFoot, tracker1, container1));
    }

    IEnumerator<WaitForSeconds> WaitMoveCamera(int camera, Text moveText, GameObject pelvis, GameObject foot, GameObject tracker, GameObject container) {
        Debug.Log("Starting Camera Movement");
        float timer = 0.5f;
        bool success = true;
        var m_skeletalTrackingProvider = main.GetComponent<main>().GetSkeletalTrackingProvider(camera);
        
        // Wait until the trackers have initialised
        while (!m_skeletalTrackingProvider.IsRunning){
            yield return new WaitForSeconds(timer);
            timer += 0.25f;
            if(timer > 60){
                success = false;
                break;
            }
        }

        if (success) {
            Debug.Log("Rotating Camera");
            // Calculate the angle of the camera using the acceleromiter data
            Vector3 angles = m_skeletalTrackingProvider.AcceleromiterAngles();
            var roll = Mathf.Atan2(angles.y, angles.z) * 180f / 3.1415f + 180f;
            var pitch = Mathf.Asin(angles.x / 9.81f) * 180f / 3.1415f;
            tracker.transform.rotation = Quaternion.Euler(pitch , 0, roll);

            Debug.Log("Translating Camera");  
            Vector3 translation = new Vector3(0,0,0);
            float floor = 0f;
            // Make sure to go through several frames of the game
            for (var i = 0; i < noIterations; i++){
                moveText.text = "" + i;

                // Calculate the bone and foot position in order to calculate the translation (averaged for accuracy)
                translation += - pelvis.transform.position;
                floor += - foot.transform.position.y;
                
                yield return new WaitForSeconds(0.01f);
            }   
            translation /= noIterations;
            floor /= noIterations;

            tracker.transform.position = new Vector3(translation.x, floor, translation.z);
            Debug.Log(roll + " " + pitch);
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
        
        // wait for both cameras to be initialised
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
            // Calculating the rotation between the cameras
            Quaternion rotation = new Quaternion();
            // Make sure to go through several frames of the game
            for (var i = 0; i < noIterations; i++){
                syncText.text = "" + i;
                Quaternion rotationTemp = new Quaternion();
                for (int jointNum = 0; jointNum < (int)JointId.Count; jointNum++)
                {
                    // Calculate the value of the rotations for each set of bones
                    var rotation_0 = tracker.transform.GetChild(0).GetChild(jointNum).rotation;
                    var rotation_1 = tracker1.transform.GetChild(0).GetChild(jointNum).rotation;
                    
                    if(jointNum == 0){
                        // If it is the first bone just set the value (makes sure to converge faster)
                        rotationTemp = rotation_0 * Quaternion.Inverse(rotation_1); 
                    } else {
                        // Average the rotation for each bone
                        var tempR = rotation_0 * Quaternion.Inverse(rotation_1); 
                        rotationTemp = Quaternion.Slerp(rotationTemp, tempR, 0.5f);
                    }
                }
                if(i == 0){
                    rotation = rotationTemp;
                } else {
                    rotation = Quaternion.Slerp(rotation, rotationTemp, 0.5f);
                }

                yield return new WaitForSeconds(0.005f);
            }   
            
            container1.transform.rotation = rotation;
            

            // Calculating the translation Between the Cameras

            Vector3 translation = new Vector3(0,0,0);
            // Make sure to go through several frames of the game
            for (var i = 0; i < noIterations; i++){
                syncText.text = "" + (noIterations + i);

                // Calculate the value of the transforms and rotations for each set of bones
                var bone_0 = firstPelvis.transform.position;
                var bone_1 = secondPelvis.transform.position;

                // Sum them in order to calcualate the average difference (for the translation)
                translation += bone_0 - bone_1;

                yield return new WaitForSeconds(0.005f);
            }   
            translation /= noIterations;
            container1.transform.position = translation;

            Debug.Log(translation + " " + rotation);
            // Turn on merged bodies and turn off normal containers
            GameObject.Find("Main").GetComponent<main>().renderMergedSkeletons = true;
            tracker.SetActive(false);
            tracker1.SetActive(false);
        } else {
            Debug.Log("Camera Calibration failed");
        }
    } 



}


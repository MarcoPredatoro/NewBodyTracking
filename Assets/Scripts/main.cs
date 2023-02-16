using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine.UI;
// using OpenCvSharp;


public class main : MonoBehaviour
{
    // Handler for SkeletalTracking thread.
    public GameObject m_tracker_1;
    public GameObject m_tracker_2;
    private SkeletalTrackingProvider m_skeletalTrackingProvider;
    private SkeletalTrackingProvider m_skeletalTrackingProvider1;

    public GameObject firstPelvis;
    public GameObject secondPelvis;

    private int noIterations = 100;

    public BackgroundData m_lastFrameData = new BackgroundData();

    private int points;

    public Text problemText;
    public UnityEngine.UI.Image problemImage;
    public Material problemMaterial;
    public Material reset;
    public Text pointsText;


    void Start()
    {
        // if (Device.GetInstalledCount() == 1){
        //     m_skeletalTrackingProvider = new SkeletalTrackingProvider(0);
        // } else if (Device.GetInstalledCount() == 2) {
        // MakeCalibration make = new MakeCalibration();
        // UnityEngine.Debug.Log(make.calibrate());
        m_skeletalTrackingProvider = new SkeletalTrackingProvider(0);
        m_skeletalTrackingProvider1 = new SkeletalTrackingProvider(1);
        
        // }

        pointsText.text = "Points: 0";
        points = 0;

        // StartCoroutine(makePoloAppear());

        StartCoroutine(WaitTwoCameraCalibrate());
        StartCoroutine(WaitCameraMarcoCalibrate());
        
    }

    void Update() {
        if (m_skeletalTrackingProvider.IsRunning) {
            if (m_skeletalTrackingProvider.GetCurrentFrameData(ref m_lastFrameData))
            {
                if (m_lastFrameData.NumOfBodies > 0)
                {
                    // m_tracker_1.GetComponent<TrackerHandler>().updateTracker(m_lastFrameData,0);
                    // m_tracker_1.GetComponent<TrackerHandler>().updateTracker(m_lastFrameData,1);
                    var onSameSide = m_tracker_1.GetComponent<TrackerHandler>().updateBothPeople(m_lastFrameData);

                    // Make sure that more than 2 people will be tracked (not just the one !!!!!)

                    if (onSameSide) {
                        problemText.text = "MOVE OVER TO CORRECT SIDES";
                    } else {
                        problemText.text = "";
                    }

                }
            }
        }
        // Multiple camera setup
        if (m_skeletalTrackingProvider1.IsRunning)
        {
            if (m_skeletalTrackingProvider1.GetCurrentFrameData(ref m_lastFrameData))
            {
                if (m_lastFrameData.NumOfBodies > 0)
                {
                    m_tracker_2.GetComponent<TrackerHandler>().updateTracker(m_lastFrameData,0);
                    // m_tracker_2.GetComponent<TrackerHandler>().updateTracker(m_lastFrameData,1);
                    // m_tracker_2.GetComponent<TrackerHandler>().updateTracker(m_lastFrameData,1);
                    // m_tracker2.GetComponent<TrackerHandler>().updateTracker(m_lastFrameData,2);
                }
            }
        }

    }

    void OnApplicationQuit()
    {
        if (m_skeletalTrackingProvider != null)
        {
            m_skeletalTrackingProvider.Dispose();
        }
        if (m_skeletalTrackingProvider1 != null){
            m_skeletalTrackingProvider1.Dispose();
        }
    }

/*

    Camera Calibration

*/
    IEnumerator<WaitForSeconds> WaitTwoCameraCalibrate() {
        Debug.Log("Starting Camera Calibration");
        float timer = 0.5f;
        bool success = true;
        
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
                var bone_0 = firstPelvis.transform.position;
                var bone_1 = secondPelvis.transform.position;
                var rotation_0 = firstPelvis.transform.rotation.eulerAngles;
                var rotation_1 = secondPelvis.transform.rotation.eulerAngles;

                translation += bone_1 - bone_0;
                rotation += rotation_1 - rotation_0;

                
            }
            translation /= noIterations;
            rotation /= noIterations;

            Debug.Log(translation + " " + rotation);
        } else {
            Debug.Log("Camera Calibration failed");
        }
    }

/*
    Marco
*/

    

    IEnumerator<WaitForSeconds> WaitCameraMarcoCalibrate() {
        Debug.Log("Starting Marco Calibration");
        float timer = 0.5f;
        bool success = true;
        var marco = GameObject.Find("TestMarco(Clone)");
        var parent = GameObject.Find("MarcoContainer");
        
        while ((!m_skeletalTrackingProvider.IsRunning || marco == null) && success){
            yield return new WaitForSeconds(timer);
            timer += 0.25f;
            marco = GameObject.Find("TestMarco(Clone)");

            if(timer > 800){
                success = false;
                break;
            }
        }

        

        if (success) {
            // Get Skeleton position
            var position = new Vector3(0,0,0);
            for(int i = 0; i < noIterations; i++){
                position += firstPelvis.transform.position - marco.transform.position;
                yield return new WaitForSeconds(0.05f);
            }
            position /= noIterations;
            parent.transform.position = new Vector3(position.x, parent.transform.position.y, position.z);
            marco.transform.SetParent(parent.transform);
        } else {
            Debug.Log("Marco Calibration Failed");
        }

        Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!");
    }


/*

    Points System

*/

    public int getPoints() {
        return points;
    }
    public void updatePoints(int value) {
        points += value;
        pointsText.text = "Points: " + points.ToString();
    }

    private bool loseTimer = false;
    public void losePoints(int value) {
        if (!loseTimer) {
            loseTimer = true;
            points -= value;
            pointsText.text = "Points: " + points.ToString();
            problemImage.color = new Color(255,0,0);
            problemImage.material = problemMaterial;
            StartCoroutine(turnBacktoWhite());

        }
    }


    IEnumerator<WaitForSeconds> turnBacktoWhite() {
        yield return new WaitForSeconds(5);
        problemImage.material = reset;
        problemImage.color = new Color(255,255,255);
        loseTimer = false;
    }


/*

    Appearing and disapearing polo

*/

    IEnumerator<WaitForSeconds> makePoloAppear() {
        Debug.Log("Starting Make Polo Appear");
        float timer = 0.5f;
        bool success = true;
        var game = GameObject.Find("Polo(Clone)");
        
        while (game == null && success){
            yield return new WaitForSeconds(timer);
            timer += 0.25f;
            game = GameObject.Find("Polo(Clone)");

            if(timer > 400){
                success = false;
                break;
            }


        }

        while(true && success) {
            game.GetComponent<MeshRenderer>().enabled = false;
            yield return new WaitForSeconds(15);
            game.GetComponent<MeshRenderer>().enabled = true;
            yield return new WaitForSeconds(3);

        }

        if (!success) {
            Debug.Log("Polo Disapearing Failed");
        }


    }
}



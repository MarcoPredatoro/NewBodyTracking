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

    public BackgroundData m_lastFrameData = new BackgroundData();

    public int points;

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

        // points = 0;
        pointsText.text = "Points: " + points;

        StartCoroutine(makePoloAppear());
        
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

    public SkeletalTrackingProvider GetSkeletalTrackingProvider(int i){
        if(i == 0 ){
            return m_skeletalTrackingProvider;
        } else if (i == 1) {
            return m_skeletalTrackingProvider1;
        }
        return m_skeletalTrackingProvider;
    }
/*  

    Camera Calibration

*/


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
    public GameObject poloPosition;
    IEnumerator<WaitForSeconds> makePoloAppear() {
        Debug.Log("Starting Make Polo Appear");
        float timer = 0.5f;
        bool success = true;
        var game = GameObject.Find("Person");
        
        while (game == null && success){
            yield return new WaitForSeconds(timer);
            timer += 0.25f;
            game = GameObject.Find("Person");

            if(timer > 400){
                success = false;
                break;
            }
        }

        while(true && success) {
            poloPosition.GetComponent<MeshRenderer>().enabled = false;
            yield return new WaitForSeconds(4);
            poloPosition.transform.position = new Vector3(game.transform.position.x, game.transform.position.y, game.transform.position.z);
            poloPosition.GetComponent<MeshRenderer>().enabled = true;
            yield return new WaitForSeconds(3);

        }

        if (!success) {
            Debug.Log("Polo Disapearing Failed");
        }


    }
}



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

    public BackgroundData m_lastFrameData0 = new BackgroundData();
    public BackgroundData m_lastFrameData1 = new BackgroundData();

    public int points;

    public Text problemText;
    public UnityEngine.UI.Image problemImage;
    public Material problemMaterial;
    public Material reset;
    public Text pointsText;



    void Start()
    {
        m_skeletalTrackingProvider = new SkeletalTrackingProvider(0);
        m_skeletalTrackingProvider1 = new SkeletalTrackingProvider(1);
        

        pointsText.text = "Points: " + points;

        
    }

    void Update() {

        /*

            Loop through all bodies,
            If there x,z are close to each other (will do this by 1.x - 0.x > or < 0.2)
            only render one of the skeletons
            Make sure to keep track of which skeletons have been rendered (using a list or something)

            Render all remaining skeletons

            Check to see if you can render the one cameras skeleton using the other azure tracker!!
        */


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
                    m_tracker_2.GetComponent<TrackerHandler>().updateTracker(m_lastFrameData,1);
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


}



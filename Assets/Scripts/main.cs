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
    private SkeletalTrackingProvider m_skeletalTrackingProvider;
    private SkeletalTrackingProvider m_skeletalTrackingProvider1;

    public MergeBodies mergeBodies;

    public int points;

    public Text problemText;
    public UnityEngine.UI.Image problemImage;
    public Material problemMaterial;
    public Material reset;
    public Text pointsText;

    public bool renderMergedSkeletons = false;



    void Start()
    {
        m_skeletalTrackingProvider = new SkeletalTrackingProvider(0);
        m_skeletalTrackingProvider1 = new SkeletalTrackingProvider(1);
        

        pointsText.text = "Points: " + points;

        
    }

    void Update() {
        if (m_skeletalTrackingProvider.IsRunning && m_skeletalTrackingProvider1.IsRunning) {
            if (renderMergedSkeletons) {
                mergeBodies.renderSkeletons(m_skeletalTrackingProvider, m_skeletalTrackingProvider1 );
            } else {
                mergeBodies.renderBothSkeletons(m_skeletalTrackingProvider, m_skeletalTrackingProvider1 );
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



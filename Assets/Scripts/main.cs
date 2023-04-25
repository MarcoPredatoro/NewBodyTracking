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
    public bool renderMergedSkeletons = false;



    void Start()
    {
        m_skeletalTrackingProvider = new SkeletalTrackingProvider(0);
        m_skeletalTrackingProvider = new SkeletalTrackingProvider(1);
        // StartCoroutine(WaitTwoCameraConnection());
        
    }

    void Update() {
        if (!(m_skeletalTrackingProvider == null || m_skeletalTrackingProvider1 == null)) {
            if (m_skeletalTrackingProvider.IsRunning && m_skeletalTrackingProvider1.IsRunning) {
                if (renderMergedSkeletons) {
                    mergeBodies.renderSkeletons(m_skeletalTrackingProvider, m_skeletalTrackingProvider1 );
                } else {
                    mergeBodies.renderBothSkeletons(m_skeletalTrackingProvider, m_skeletalTrackingProvider1 );
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


    public void StartGame() {
        GameObject.Find("networking").GetComponent<EventManager>().ResetPoints();
        GetComponent<Points>().resetPoints();
        // GameObject.Find("EndScreen").SetActive(false);
        GameObject.Find("Timer").GetComponent<Timer>().ResetTimer();
        GameObject.Find("networking").GetComponent<EventManager>().SendGameStart();

        
    }
    IEnumerator<WaitForSeconds> WaitTwoCameraConnection() {
        Debug.Log("Attempting to Connect to Cameras");
        
        int attempts = 0;
        bool success = true;
        Debug.Log(m_skeletalTrackingProvider.hasFailed);
        yield return new WaitForSeconds(5.0f);
        // wait for both cameras to be initialised
        while ((m_skeletalTrackingProvider == null || m_skeletalTrackingProvider.hasFailed)){
            yield return new WaitForSeconds(3.0f);
            m_skeletalTrackingProvider = new SkeletalTrackingProvider(0);
            attempts++;
            if(attempts > 5){
                success = false;
                break;
            }
        }
        attempts = 0;
        while (m_skeletalTrackingProvider1 == null || m_skeletalTrackingProvider1.hasFailed){
            yield return new WaitForSeconds(3.0f);
            m_skeletalTrackingProvider1 = new SkeletalTrackingProvider(1);
            attempts++;
            if(attempts > 5){
                success = false;
                break;
            }
        }

        if (!success) {
            Debug.Log("Camera Connection Failed !!!!!!!!!!!!!");
        }
    } 


}



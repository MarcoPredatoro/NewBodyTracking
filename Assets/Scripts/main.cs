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
    private const float SYNC_EPSILON = 0.5f;
    public GameObject m_tracker_0;
    public GameObject m_tracker_1;
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


        if (m_skeletalTrackingProvider.IsRunning && m_skeletalTrackingProvider1.IsRunning) {
            if (m_skeletalTrackingProvider.GetCurrentFrameData(ref m_lastFrameData0) && m_skeletalTrackingProvider1.GetCurrentFrameData(ref m_lastFrameData1))
            {
                if (m_lastFrameData0.NumOfBodies > 0 || m_lastFrameData1.NumOfBodies > 0)
                {
                    List<Tuple<Vector3,float>> location0 = m_tracker_0.GetComponent<TrackerHandler>().getLocations(m_lastFrameData0, m_tracker_0.transform);
                    List<Tuple<Vector3,float>> location1 = m_tracker_1.GetComponent<TrackerHandler>().getLocations(m_lastFrameData1, m_tracker_1.transform);
                    
                    List<Tuple<int, int>> bodies = new List<Tuple<int, int>>();
                    List<bool> isUsed = new List<bool>(new bool[location0.Count + location1.Count]);
                    
                    for (int i = 0; i < location0.Count; i++){
                        for (int j = 0; j < location1.Count; j++){
                            // Check how close the bodies are together
                            // Debug.Log(Mathf.Abs(Vector3.Magnitude(location0[i].Item1 - location1[j].Item1)));
                            if(Mathf.Abs(Vector3.Magnitude(location0[i].Item1 - location1[j].Item1)) < SYNC_EPSILON && !isUsed[i] && !isUsed[location0.Count + j]) {
                                // Add to the bodies array the tracker and the body location that is closest to it's respective camera
                                bodies.Add(location0[0].Item2 > location1[0].Item2 ? new Tuple<int, int>(0, i) : new Tuple<int, int>(1, j));

                                isUsed[i] = true;
                                isUsed[location0.Count + j] = true;
                            } 
                        }
                    }
                    for (int i = 0; i < isUsed.Count; i++){
                        // Debug.Log(isUsed[i]);
                        if (!isUsed[i]){
                            bodies.Add(i < location0.Count ? new Tuple<int, int>(0,i) : new Tuple<int, int>(1,i - location0.Count));
                        }
                    }

                    foreach (var b in bodies){
                        Debug.Log(b.Item1 + " " + b.Item2);
                        if(b.Item1 == 0) {
                            m_tracker_0.GetComponent<TrackerHandler>().updateTracker(m_lastFrameData0, b.Item2);
                        } else {
                            m_tracker_1.GetComponent<TrackerHandler>().updateTracker(m_lastFrameData1, b.Item2);
                        }
                    }
                    Debug.Log("------------");

                    // m_tracker_0.GetComponent<TrackerHandler>().updateTracker(m_lastFrameData0, 0);
                    // m_tracker_1.GetComponent<TrackerHandler>().updateTracker(m_lastFrameData1, 0);

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



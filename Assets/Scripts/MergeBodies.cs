using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Microsoft.Azure.Kinect.BodyTracking;

public class MergeBodies : MonoBehaviour
{

    public float SYNC_EPSILON = 1.0f;
    public GameObject m_tracker_0;
    public GameObject m_tracker_1;

    public BackgroundData m_lastFrameData0 = new BackgroundData();
    public BackgroundData m_lastFrameData1 = new BackgroundData();

    public Quaternion Y_180_FLIP = new Quaternion(0.00000f, 0.70711f, 0.00000f, 0.70711f);

    public void renderSkeletons(SkeletalTrackingProvider m_skeletalTrackingProvider, SkeletalTrackingProvider m_skeletalTrackingProvider1) {
        var bodies = getBodies(m_skeletalTrackingProvider, m_skeletalTrackingProvider1);

        for (var i = 0; i < bodies.Count; i++) {
            renderSkeleton(bodies[i].Item1, bodies[i].Item2.GetComponent<TrackerHandler>(), bodies[i].Item2.transform, i);
        }
        
    }

    private List<Tuple<Body, GameObject>> getBodies(SkeletalTrackingProvider m_skeletalTrackingProvider, SkeletalTrackingProvider m_skeletalTrackingProvider1) {
        List<Tuple<Body, GameObject>> returnBodies = new List<Tuple<Body, GameObject>>();
        if (m_skeletalTrackingProvider.GetCurrentFrameData(ref m_lastFrameData0) && m_skeletalTrackingProvider1.GetCurrentFrameData(ref m_lastFrameData1))
        // if the frame processing was successfull
        {
            if (m_lastFrameData0.NumOfBodies > 0 || m_lastFrameData1.NumOfBodies > 0)
            // if either camera has bodies within it
            {
                // Calculate the location of the bodies in world space 
                List<Tuple<Vector3,float>> location0 = m_tracker_0.GetComponent<TrackerHandler>().getLocations(m_lastFrameData0);
                List<Tuple<Vector3,float>> location1 = m_tracker_1.GetComponent<TrackerHandler>().getLocations(m_lastFrameData1);

                // the tuple is (camera, body index)                 
                List<Tuple<int, int>> bodies = new List<Tuple<int, int>>();
                List<float> distances = new List<float>();
                List<bool> isUsed = new List<bool>(new bool[location0.Count + location1.Count]);
                
                for (int i = 0; i < location0.Count; i++){
                    for (int j = 0; j < location1.Count; j++){
                        // Check how close the bodies are together
                        if(GetXZDistance(location0[i].Item1, location1[j].Item1) < SYNC_EPSILON && !isUsed[i] && !isUsed[location0.Count + j]) {
                            // chose the order to render the bodies based on the distance to the main camera
                            var distance = location0[i].Item2;
                            // Add to the bodies array the tracker and the body location that is closest to it's respective camera
                            var body = location0[0].Item2 > location1[0].Item2 ? new Tuple<int, int>(0, i) : new Tuple<int, int>(1, j);
                            
                            addBodyToRender(distance, body, ref distances, ref bodies);

                            isUsed[i] = true;
                            isUsed[location0.Count + j] = true;
                        } 
                    }
                }
                for (int i = 0; i < isUsed.Count; i++){
                    // If the body hasn't been used then add it to the array aswell
                    if (!isUsed[i]){
                        Tuple<int, int> body;
                        float distance; 
                        if (i < location0.Count){
                            body = new Tuple<int, int>(0,i);
                            distance = location0[i].Item2;
                        } else {
                            body = new Tuple<int, int>(1,i - location0.Count);
                            distance = location1[i - location0.Count].Item2;
                        }

                        addBodyToRender(distance, body, ref distances, ref bodies);

                    }

                }

                for (int i = 0; i < (int)Mathf.Min(2, bodies.Count); i++){
                    // Add all the bodies to the return bodies to be rendered
                    if(bodies[i].Item1 == 0) {
                        returnBodies.Add(new Tuple<Body, GameObject>(m_lastFrameData0.Bodies[bodies[i].Item2], m_tracker_0));
                    } else {
                        returnBodies.Add(new Tuple<Body, GameObject>(m_lastFrameData1.Bodies[bodies[i].Item2], m_tracker_1));
                    }
                }
                // Debug.Log(bodies.Count + " " + returnBodies.Count + " " + location0.Count + " "  + location1.Count);
            }
        }
        return returnBodies;
    }
    
    private void renderSkeleton(Body skeleton, TrackerHandler tracker, Transform kinectTransform, int skeletonNumber)
    {
        // Sets the transform of the merged point body to be that of the chosen calibrated camera
        transform.GetChild(skeletonNumber).position = kinectTransform.position;
        transform.GetChild(skeletonNumber).rotation = kinectTransform.rotation;

        // Debug.Log(transform.GetChild(skeletonNumber).GetChild(0).GetChild(0).name);

        for (int jointNum = 0; jointNum < (int)JointId.Count; jointNum++)
        {
            Vector3 jointPos = new Vector3(skeleton.JointPositions3D[jointNum].X, -skeleton.JointPositions3D[jointNum].Y, skeleton.JointPositions3D[jointNum].Z);
            Vector3 offsetPosition = transform.GetChild(skeletonNumber).rotation * jointPos ;
            Vector3 positionInTrackerRootSpace = transform.GetChild(skeletonNumber).position + offsetPosition;
            Quaternion jointRot = Y_180_FLIP * new Quaternion(skeleton.JointRotations[jointNum].X, skeleton.JointRotations[jointNum].Y,
                skeleton.JointRotations[jointNum].Z, skeleton.JointRotations[jointNum].W) * Quaternion.Inverse(tracker.basisJointMap[(JointId)jointNum]);

            // these are absolute body space because each joint has the body root for a parent in the scene graph
            transform.GetChild(skeletonNumber).GetChild(0).GetChild(jointNum).localPosition = jointPos;
            transform.GetChild(skeletonNumber).GetChild(0).GetChild(jointNum).localRotation = jointRot;

            const int boneChildNum = 0;
            if (tracker.parentJointMap[(JointId)jointNum] != JointId.Head && tracker.parentJointMap[(JointId)jointNum] != JointId.Count)
            {
                Vector3 parentTrackerSpacePosition = new Vector3(skeleton.JointPositions3D[(int)tracker.parentJointMap[(JointId)jointNum]].X,
                    -skeleton.JointPositions3D[(int)tracker.parentJointMap[(JointId)jointNum]].Y, skeleton.JointPositions3D[(int)tracker.parentJointMap[(JointId)jointNum]].Z);
                Vector3 boneDirectionTrackerSpace = jointPos - parentTrackerSpacePosition;
                Vector3 boneDirectionWorldSpace = transform.GetChild(skeletonNumber).rotation * boneDirectionTrackerSpace;
                Vector3 boneDirectionLocalSpace = Quaternion.Inverse(transform.GetChild(skeletonNumber).GetChild(0).GetChild(jointNum).rotation) * Vector3.Normalize(boneDirectionWorldSpace);
                transform.GetChild(skeletonNumber).GetChild(0).GetChild(jointNum).GetChild(boneChildNum).localScale = new Vector3(1, 20.0f * 0.5f * boneDirectionWorldSpace.magnitude, 1);
                transform.GetChild(skeletonNumber).GetChild(0).GetChild(jointNum).GetChild(boneChildNum).localRotation = Quaternion.FromToRotation(Vector3.up, boneDirectionLocalSpace);
                transform.GetChild(skeletonNumber).GetChild(0).GetChild(jointNum).GetChild(boneChildNum).position = transform.GetChild(skeletonNumber).GetChild(0).GetChild(jointNum).position - 0.5f * boneDirectionWorldSpace;
            }
            else
            {
                transform.GetChild(skeletonNumber).GetChild(0).GetChild(jointNum).GetChild(boneChildNum).gameObject.SetActive(false);
            }
        }
    }

    public void renderBothSkeletons(SkeletalTrackingProvider m_skeletalTrackingProvider, SkeletalTrackingProvider m_skeletalTrackingProvider1) {
        if (m_skeletalTrackingProvider.GetCurrentFrameData(ref m_lastFrameData0) && m_skeletalTrackingProvider1.GetCurrentFrameData(ref m_lastFrameData1))
        // if the frame processing was successfull
        {
            if (m_lastFrameData0.NumOfBodies > 0 )
            {
                // If the number of bodies for the first camera was greater than 0 render the skeletons
                m_tracker_0.GetComponent<TrackerHandler>().updateTracker(m_lastFrameData0, 0);
                m_tracker_0.GetComponent<TrackerHandler>().updateTracker(m_lastFrameData0, 1);
                m_tracker_0.GetComponent<TrackerHandler>().updateTracker(m_lastFrameData0, 2);

            }
            if (m_lastFrameData1.NumOfBodies > 0) {
                // If the number of bodies for the second camera was greater than 0 render the skeletons
                m_tracker_1.GetComponent<TrackerHandler>().updateTracker(m_lastFrameData1, 0);
                m_tracker_1.GetComponent<TrackerHandler>().updateTracker(m_lastFrameData1, 1);
                m_tracker_1.GetComponent<TrackerHandler>().updateTracker(m_lastFrameData1, 2);
            }
        }
    }

    // Inserts the body to the bodies array based on it's distance to it's closest camera
    private void addBodyToRender(float distance, Tuple<int,int> body, ref List<float> distances, ref List<Tuple<int, int>> bodies ) {
        bool added = false;

        for(int k = 0; k < distances.Count; k++){
            if (distance < distances[k]){
                bodies.Insert(k, body);
                distances.Insert(k, distance);
                added = true;
                break;
            }
        }
        if (!added){
            bodies.Add(body);
            distances.Add(distance);
        }
    }

    // Return the magnitude for the x and z components
    private float GetXZDistance(Vector3 v1, Vector3 v2) {
        Vector3 vector = v1 - v2;
        return Mathf.Abs( Mathf.Sqrt( vector.x * vector.x + vector.z * vector.z ));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Photon.Pun;
using System;

public class NetworkPuppeteeringVR : MonoBehaviourPun, IPunObservable
{
    // we will have to set all of these in Start
    Animator PuppetAnimator;
    //PhotonView pv;
    // here we go
    [SerializeField]
    Dictionary<int, float[]> kinectRotationsMap; //changing JointId to int for serialization AND QUATERNION TO FLOAT[]
    [SerializeField]
    Vector3 hipPosition;

    private static HumanBodyBones MapKinectInt(int joint)
    {
        // https://docs.microsoft.com/en-us/azure/Kinect-dk/body-joints
        switch (joint)
        {
            case 0: return HumanBodyBones.Hips;
            case 1: return HumanBodyBones.Spine;
            case 2: return HumanBodyBones.Chest;
            case 3: return HumanBodyBones.Neck;
            case 26: return HumanBodyBones.Head;
            case 18: return HumanBodyBones.LeftUpperLeg;
            case 19: return HumanBodyBones.LeftLowerLeg;
            case 20: return HumanBodyBones.LeftFoot;
            case 21: return HumanBodyBones.LeftToes;
            case 22: return HumanBodyBones.RightUpperLeg;
            case 23: return HumanBodyBones.RightLowerLeg;
            case 24: return HumanBodyBones.RightFoot;
            case 25: return HumanBodyBones.RightToes;
            case 4: return HumanBodyBones.LeftShoulder;
            case 5: return HumanBodyBones.LeftUpperArm;
            case 6: return HumanBodyBones.LeftLowerArm;
            case 7: return HumanBodyBones.LeftHand;
            case 11: return HumanBodyBones.RightShoulder;
            case 12: return HumanBodyBones.RightUpperArm;
            case 13: return HumanBodyBones.RightLowerArm;
            case 14: return HumanBodyBones.RightHand;
            default: return HumanBodyBones.LastBone;
        }
    }

    private void Start()
    {
        PuppetAnimator = GetComponent<Animator>();
        //pv = GetComponent<PhotonView>();
    }

    private void LateUpdate()
    {
        mapBonesFromPhoton();
    }

    private void mapBonesFromPhoton()
    {
        for (int j = 0; j < 32; j++)
        {
            if (MapKinectInt(j) != HumanBodyBones.LastBone)
            {
                // just uhh
                // set the values that come through the stream i guess?

                // get the transform of the hbb corresponding to the kinect joint in question
                Transform finalJoint = PuppetAnimator.GetBoneTransform(MapKinectInt(j));
                // HERE WE GO BOYS WE'RE DOING THE STUPID-ASS CONVERSION SHIT
                float[] rot = kinectRotationsMap[j];
                Quaternion rotato = new Quaternion(rot[0], rot[1], rot[2], rot[3]);
                finalJoint.rotation = rotato;

                if (j == 0)
                {
                    finalJoint.position = hipPosition;
                }

            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        object[] data = (object[])stream.ReceiveNext();
        kinectRotationsMap = (Dictionary<int, float[]>)data[0];
        // this only works because you cast to an object earlier
        // wait
        // Quaternion -> object casts are valid and exist
        hipPosition = (Vector3)data[1];
        //Debug.Log("received hips: " + hipPosition.ToString());
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using System.Text;
using Photon.Pun;

public class NetworkPuppeteering : MonoBehaviourPun, IPunObservable
{
    // we will have to set all of these in Start
    TrackerHandler KinectDevice;
    Animator PuppetAnimator;
    //public string pointBody; // <--- you'll need this for multiple polos, maybe?
    GameObject RootPosition; // this is usually pelvis
    Transform CharacterRootTransform; // this is usually the transform of the gameobject the script is attached to
    PhotonView pv;
    Dictionary<JointId, Quaternion> absoluteOffsetMap;
    // here we go
    [SerializeField]
    Dictionary<JointId, Quaternion> kinectRotationsMap;

    float OffsetY = 1;
    float OffsetZ = 1;

    private static HumanBodyBones MapKinectJoint(JointId joint)
    {
        // https://docs.microsoft.com/en-us/azure/Kinect-dk/body-joints
        switch (joint)
        {
            case JointId.Pelvis: return HumanBodyBones.Hips;
            case JointId.SpineNavel: return HumanBodyBones.Spine;
            case JointId.SpineChest: return HumanBodyBones.Chest;
            case JointId.Neck: return HumanBodyBones.Neck;
            case JointId.Head: return HumanBodyBones.Head;
            case JointId.HipLeft: return HumanBodyBones.LeftUpperLeg;
            case JointId.KneeLeft: return HumanBodyBones.LeftLowerLeg;
            case JointId.AnkleLeft: return HumanBodyBones.LeftFoot;
            case JointId.FootLeft: return HumanBodyBones.LeftToes;
            case JointId.HipRight: return HumanBodyBones.RightUpperLeg;
            case JointId.KneeRight: return HumanBodyBones.RightLowerLeg;
            case JointId.AnkleRight: return HumanBodyBones.RightFoot;
            case JointId.FootRight: return HumanBodyBones.RightToes;
            case JointId.ClavicleLeft: return HumanBodyBones.LeftShoulder;
            case JointId.ShoulderLeft: return HumanBodyBones.LeftUpperArm;
            case JointId.ElbowLeft: return HumanBodyBones.LeftLowerArm;
            case JointId.WristLeft: return HumanBodyBones.LeftHand;
            case JointId.ClavicleRight: return HumanBodyBones.RightShoulder;
            case JointId.ShoulderRight: return HumanBodyBones.RightUpperArm;
            case JointId.ElbowRight: return HumanBodyBones.RightLowerArm;
            case JointId.WristRight: return HumanBodyBones.RightHand;
            default: return HumanBodyBones.LastBone;
        }
    }
    private void Start()
    {
        // here's the fun part: working out what doesn't need to happen if it's received over the network
        
        PuppetAnimator = GetComponent<Animator>();
        pv = GetComponent<PhotonView>();

        if (pv.IsMine)
        {
            KinectDevice = GameObject.Find("Kinect4AzureTracker").GetComponent<TrackerHandler>();
            // currently hardcoding the pointbody this belongs to
            RootPosition = GameObject.Find("pointBody/pelvis");
            CharacterRootTransform = GetComponent<Transform>();

            // there is definitely a case to be made for increasing the transmission rate from 10Hz, but oh well

            Transform _rootJointTransform = CharacterRootTransform;

            absoluteOffsetMap = new Dictionary<JointId, Quaternion>();
            for (int i = 0; i < (int)JointId.Count; i++)
            {
                HumanBodyBones hbb = MapKinectJoint((JointId)i);
                if (hbb != HumanBodyBones.LastBone)
                {
                    Transform transform = PuppetAnimator.GetBoneTransform(hbb);
                    Quaternion absOffset = GetSkeletonBone(PuppetAnimator, transform.name).rotation;
                    // find the absolute offset for the tpose
                    while (!ReferenceEquals(transform, _rootJointTransform))
                    {
                        transform = transform.parent;
                        absOffset = GetSkeletonBone(PuppetAnimator, transform.name).rotation * absOffset;
                    }
                    absoluteOffsetMap[(JointId)i] = absOffset;
                }
            }
        }
    }

    private static SkeletonBone GetSkeletonBone(Animator animator, string boneName)
    {
        int count = 0;
        StringBuilder cloneName = new StringBuilder(boneName);
        cloneName.Append("(Clone)");
        foreach (SkeletonBone sb in animator.avatar.humanDescription.skeleton)
        {
            if (sb.name == boneName || sb.name == cloneName.ToString())
            {
                return animator.avatar.humanDescription.skeleton[count];
            }
            count++;
        }
        return new SkeletonBone();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (pv.IsMine)
        {
            mapBonesFromKinect();
        }
    }

    private void mapBonesFromKinect()
    {
        Dictionary<JointId, Quaternion> kinectRotations = new Dictionary<JointId, Quaternion>();
        for (int j = 0; j < (int)JointId.Count; j++)
        {
            if (MapKinectJoint((JointId)j) != HumanBodyBones.LastBone && absoluteOffsetMap.ContainsKey((JointId)j))
            {
                // get the absolute offset
                Quaternion absOffset = absoluteOffsetMap[(JointId)j];
                // calculate the new angle
                Quaternion rot = absOffset * Quaternion.Inverse(absOffset) * KinectDevice.absoluteJointRotations[j] * absOffset;
                // add it to the dictionary
                kinectRotations[(JointId)j] = rot;

                // get the transform of the hbb corresponding to the kinect joint in question
                Transform finalJoint = PuppetAnimator.GetBoneTransform(MapKinectJoint((JointId)j));
                // OVERWRITE the rotation of the bone
                finalJoint.rotation = rot;
                if (j == 0)
                {
                    finalJoint.position = CharacterRootTransform.position + new Vector3(RootPosition.transform.localPosition.x, RootPosition.transform.localPosition.y + OffsetY, RootPosition.transform.localPosition.z - OffsetZ);

                    // i'm just gonna
                    // put a photonTransformView on the hip bone
                }

                // update the kinectRotationsMap i guess??
                kinectRotationsMap = kinectRotations;
            }
        }
    }

    private void mapBonesFromPhoton()
    {
        for (int j = 0; j < (int)JointId.Count; j++)
        {
            if (MapKinectJoint((JointId)j) != HumanBodyBones.LastBone && absoluteOffsetMap.ContainsKey((JointId)j))
            {
                // just uhh
                // set the values that come through the stream i guess?

                // get the transform of the hbb corresponding to the kinect joint in question
                Transform finalJoint = PuppetAnimator.GetBoneTransform(MapKinectJoint((JointId)j));
                finalJoint.rotation = kinectRotationsMap[(JointId)j];

                // do i even need to do the position? i assume photonTransformView will deal with this???
                // but only if i put a photonTransformView on the hips
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(kinectRotationsMap);
        }
        else if (stream.IsReading)
        {
            kinectRotationsMap = (Dictionary<JointId, Quaternion>)stream.ReceiveNext();
            mapBonesFromPhoton();
        }
    }

}
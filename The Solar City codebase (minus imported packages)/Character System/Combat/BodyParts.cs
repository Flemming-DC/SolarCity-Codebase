using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyParts : MonoBehaviour
{
    [Header("Used at Runtime")]
    public Transform leftHand;
    public Transform rightHand;

    [Header("Only Used In Editor")]
    public GameObject leftWeapon;
    public GameObject rightWeapon;


    public Transform GetHand(Hand hand)
    {
        if (hand == Hand.right)
            return rightHand;
        else if (hand == Hand.left)
            return leftHand;
        else
            return null;
    }



}

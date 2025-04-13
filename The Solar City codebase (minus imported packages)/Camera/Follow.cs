using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    [SerializeField] bool unParentFromFollowedTransform = true;
    [SerializeField] Transform followedTransform;
    [SerializeField] Transform corpseTransform;

    private void Awake()
    {
        if (unParentFromFollowedTransform)
            transform.SetParent(null);
    }

    void LateUpdate()
    {
        if (followedTransform != null)
            transform.position = followedTransform.position;
    }


}

using System;
using System.Collections;
using UnityEngine;
using MenteBacata.ScivoloCharacterController;

public class CapsuleUpdater : MonoBehaviour
{
    public float bottom { get => characterCapsule.verticalOffset; }
    public float height { get => characterCapsule.height; }
    public float top { get => characterCapsule.verticalOffset + characterCapsule.height; }
    CharacterCapsule characterCapsule;
    CapsuleCollider capsuleCollider;
    float defaultbottom;
    float defaultHeight;

    void Start()
    {
        characterCapsule = GetComponentInParent<CharacterCapsule>();
        capsuleCollider = GetComponentInParent<CapsuleCollider>();
        defaultbottom = characterCapsule.verticalOffset;
        defaultHeight = characterCapsule.height;
        if (capsuleCollider == null)
            Debug.LogWarning($"capsuleCollider is null");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) && transform.parent.name.ToLower().Contains("player"))
            StartCoroutine(LerpCapsuleRadius(0.2f, 2, true));
    }



    public void SetCapsulesByTop(float bottom_, float top)
    {
        float height_ = top - bottom_;
        SetCapsulesByHeight(bottom_, height_);
    }

    public void SetCapsulesByHeight(float bottom_, float height_)
    {
        characterCapsule.height = height_;
        capsuleCollider.height = height_;

        characterCapsule.verticalOffset = bottom_;
        capsuleCollider.center = new Vector3(capsuleCollider.center.x,
                                            bottom_ + 0.5f * height_,
                                            capsuleCollider.center.z);
    }

    public void ResetCapsules()
    {
        SetCapsulesByHeight(defaultbottom, defaultHeight);
    }

    public IEnumerator LerpCapsuleRadius(float targetRadius, float fullDuration, bool reset, float waitTimeBeforeReset = 0)
    {
        float initialRadius = capsuleCollider.radius;
        float radius = initialRadius;
        float t = 0;
        float partialDuration = reset ? 0.5f * (fullDuration - waitTimeBeforeReset) : fullDuration;
        
        while (radius != targetRadius)
        {
            t += Time.deltaTime / partialDuration;
            radius = Mathf.Lerp(initialRadius, targetRadius, t);
            characterCapsule.radius = radius;
            capsuleCollider.radius = radius;
            yield return null;
        }

        if (!reset)
            yield break;
        yield return new WaitForSeconds(waitTimeBeforeReset);

        while (radius != initialRadius)
        {
            t -= Time.deltaTime / partialDuration;
            radius = Mathf.Lerp(initialRadius, targetRadius, t);
            characterCapsule.radius = radius;
            capsuleCollider.radius = radius;
            yield return null;
        }

    }



}

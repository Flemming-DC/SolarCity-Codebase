using System;
using UnityEngine;
using MenteBacata.ScivoloCharacterController;

public class ScivoloMover : MonoBehaviour
{
    [SerializeField] float timeBeforeUngrounded = 0.02f;
    [SerializeField] MeshRenderer groundedIndicator;
    [SerializeField] UpdateTimer updateTimer;

    public Vector3 velocity { get; set; }
    public bool isGroundedWithLag { get; private set; }
    public bool isGrounded { get => groundDetector.DetectGround(out GroundInfo dummy); }
    public event Action<bool, float> OnGroundedChanged; // isGroundedWithLag, position.y

    bool isPlayer { get => PlayerReferencer.player.transform == transform.parent; }
    CharacterMover mover;
    CharacterCapsule characterCapsule;
    GroundDetector groundDetector;
    MoveContact[] moveContacts = CharacterMover.NewMoveContactArray;
    float nextUngroundedTime = -1f;
    bool scivoloMotionEnabled;

    void Start()
    {
        mover = GetComponentInParent<CharacterMover>();
        characterCapsule = GetComponentInParent<CharacterCapsule>();
        groundDetector = GetComponentInParent<GroundDetector>();
        scivoloMotionEnabled = true;

        updateTimer.Start(GetComponentInParent<CharacterReferencer>(), 0.1f, 0.8f);
        Load();

    }

    void OnDestroy() => updateTimer.Stop();

    void Update()
    {
        if (updateTimer.TimeToUpdate() || !isGroundedWithLag)
            UpdateGroundedness();

        if (scivoloMotionEnabled && velocity != Vector3.zero)
            mover.Move(velocity * Time.deltaTime, moveContacts, out int _);
    }


    void UpdateGroundedness()
    {
        bool LastisGroundedWithLag = isGroundedWithLag;
        isGroundedWithLag = DetectGroundWithLag();
        if (LastisGroundedWithLag != isGroundedWithLag)
            OnGroundedChanged?.Invoke(isGroundedWithLag, transform.position.y);

        SetGroundedIndicatorColor(isGroundedWithLag);
        mover.isInWalkMode = isGroundedWithLag;

    }

    bool DetectGroundWithLag()
    {
        bool isCurrentlyGrounded = groundDetector.DetectGround(out GroundInfo groundInfo);

        if (isCurrentlyGrounded)
            if (groundInfo.isOnFloor && velocity.y < 0.1f)
                nextUngroundedTime = Time.time + timeBeforeUngrounded;
            else
                nextUngroundedTime = -1f;

        isGroundedWithLag = Time.time < nextUngroundedTime;
        if (velocity.y > 0)
            isGroundedWithLag = false;

        return isGroundedWithLag;
    }

    void SetGroundedIndicatorColor(bool isGrounded)
    {
        if (groundedIndicator != null)
            groundedIndicator.material.color = isGrounded ? Color.green : Color.blue;
    }

    public void Enabled(bool enabled_)
    {
        characterCapsule.enabled = enabled_;
        mover.enabled = enabled_;
        groundDetector.enabled = enabled_;
        scivoloMotionEnabled = enabled_;
    }

    void Load()
    {
        if (!isPlayer)
            return;
        if (!SaveData.loaded)
            return;
        if (!SaveData.file.KeyExists(SaveData.respawnPosition))
            return;

        Enabled(false);
        PlayerReferencer.player.transform.position = SaveData.file.GetUnityVector3(SaveData.respawnPosition);
        Enabled(true);
    }


}

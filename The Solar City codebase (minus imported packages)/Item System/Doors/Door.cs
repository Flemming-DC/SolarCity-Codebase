using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Door : APathBlocker, Interactable
{
    [SerializeField] float openDuration = 0.5f;
    [SerializeField] float rightOpenAngle = 90f;
    [SerializeField] Transform leftPivot;
    [SerializeField] Transform rightPivot;
    [SerializeField] Item key;
    [SerializeField] Sound DoorSound;
    [SerializeField] bool canOnlyOpenFromBehind;
    [SerializeField] bool canClose = true;
    [SerializeField] bool openAtStartUpRegardlessOfSavedata;

    bool hasKey;
    bool isOpen;
    float leftRotationSpeed;
    float rightRotationSpeed;
    Quaternion leftOpenRotation;
    Quaternion rightOpenRotation;

    void Start()
    {
        if (leftPivot != null  && !leftPivot.IsChildOf(transform))
            Debug.LogWarning($"The leftPivot on {transform.Path()}.Door is not a child of the door. This is probably an error.");
        if (rightPivot != null && !rightPivot.IsChildOf(transform))
            Debug.LogWarning($"The rightPivot on {transform.Path()}.Door is not a child of the door. This is probably an error.");

        if (leftPivot == null)
        {
            var pivotObject = new GameObject("leftPivot");
            leftPivot = pivotObject.transform;
            leftPivot.parent = transform;
        }

        // the signCorrection prevents the door from opening in the opposite direction, when comming from behind
        if (canOnlyOpenFromBehind)
            rightOpenAngle = -rightOpenAngle;
        leftOpenRotation = Quaternion.Euler(0, -rightOpenAngle, 0);
        rightOpenRotation = Quaternion.Euler(0, rightOpenAngle, 0);
        leftRotationSpeed = Quaternion.Angle(leftOpenRotation, Quaternion.identity) / openDuration;
        rightRotationSpeed = Quaternion.Angle(rightOpenRotation, Quaternion.identity) / openDuration;

        DoorSound?.MakeSource(gameObject);
        ItemPickup.onPickup += OnPickup;
        Load();
    }

    void OnDestroy()
    {
        ItemPickup.onPickup -= OnPickup;
    }


    void OnPickup(Item item)
    {
        if (item != key)
            return;
        hasKey = true;
        SaveData.file.Add($"{transform.Path()}.Door.hasKey", hasKey);
    }

    public void Interact()
    {
        bool missingKey = key != null && !hasKey;

        if (missingKey)
            LockedDoorUI.Show("Locked", key.icon);
        else if (IsForward() && canOnlyOpenFromBehind)
            LockedDoorUI.Show("Can't open from this side");
        else
            StartCoroutine(ToggleOpenRoutine(canClose));
    }

    IEnumerator ToggleOpenRoutine(bool enableOnFinish = true)
    {
        enabled = false;
        DoorSound?.Play(gameObject);

        Quaternion leftTargetRotation = TargetRotation(leftOpenRotation);
        Quaternion rightTargetRotation = TargetRotation(rightOpenRotation);

        yield return this.EveryFrame(_ =>
        {
            leftPivot.localRotation = Quaternion.RotateTowards(
                leftPivot.localRotation,
                leftTargetRotation,
                leftRotationSpeed * Time.deltaTime);

            rightPivot.localRotation = Quaternion.RotateTowards(
                rightPivot.localRotation,
                rightTargetRotation,
                rightRotationSpeed * Time.deltaTime);

        }, openDuration);

        isOpen = !isOpen;
        if (enableOnFinish)
            enabled = true;

        if (canClose)
            yield break;
        SaveData.file.Add($"{transform.Path()}.Door.isOpen", isOpen);
    }


    public override void SetPathBlocked(bool block)
    {
        bool OpenAndEnabled = !block;
        if (isOpen != OpenAndEnabled)
            StartCoroutine(ToggleOpenRoutine(OpenAndEnabled));
        else
            enabled = OpenAndEnabled;
    }


    Quaternion TargetRotation(Quaternion openRotation)
    {
        if (isOpen)
            return Quaternion.identity;
        else if (IsForward())
            return openRotation;
        else
            return Quaternion.Inverse(openRotation);
    }

    bool IsForward()
    {
        // the doors are oriented such that transform.right is what you would intuitively regard as forward
        Vector3 displacement = PlayerReferencer.player.transform.position - transform.position;
        return Vector3.Dot(displacement, transform.right) > 0;
    }


    void Load()
    {
        if (SaveData.loaded)
        {
            hasKey = SaveData.file.GetBool($"{transform.Path()}.Door.hasKey");
            isOpen = SaveData.file.GetBool($"{transform.Path()}.Door.isOpen");
            if (openAtStartUpRegardlessOfSavedata)
                isOpen = true;
        }
        leftPivot.localRotation = isOpen ? Quaternion.Inverse(leftOpenRotation) : Quaternion.identity;
        rightPivot.localRotation = isOpen ? Quaternion.Inverse(rightOpenRotation) : Quaternion.identity;

        if (isOpen && !canClose)
            enabled = false;
    }

}

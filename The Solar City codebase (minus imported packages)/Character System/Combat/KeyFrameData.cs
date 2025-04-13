using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class KeyFrameData
{
    public List<Vector3> storedLocalPositions = new List<Vector3>();
    public List<Quaternion> storedLocalRotations = new List<Quaternion>();
    Transform[] bones;

    public KeyFrameData(Transform rig)
    {
        bones = rig.GetComponentsInChildren<Transform>();
        storedLocalPositions.Clear();
        storedLocalRotations.Clear();

        for (int i = 0; i < bones.Length; i++)
        {
            storedLocalPositions.Add(bones[i].localPosition);
            storedLocalRotations.Add(bones[i].localRotation);
        }
    }

    public IEnumerator LerpRoutine(float duration)
    {
        float timeFraction = 0;
        List<Vector3> initialLocalPositions = bones.Select(b => b.localPosition).ToList();
        List<Quaternion> initialLocalRotations = bones.Select(b => b.localRotation).ToList();

        while (timeFraction <= 1)
        {
            timeFraction += Time.deltaTime / duration;
            for (int i = 0; i < bones.Length; i++)
            {
                bones[i].localPosition = Vector3.Lerp(initialLocalPositions[i], storedLocalPositions[i], timeFraction);
                bones[i].localRotation = Quaternion.Lerp(initialLocalRotations[i], storedLocalRotations[i], timeFraction);
            }
            yield return null;
        }
        /*
        for (int i = 0; i < bones.Length; i++)
        {
            Vector3 error = bones[i].localPosition - storedLocalPositions[i];
            if (error != Vector3.zero)
                Debug.LogWarning(error);
        }
        Debug.Log("no further errors detected");
        */
    }

    public void Print()
    {
        Debug.Log("----------- KeyFrameData start ----------");
        bones.Print();
        storedLocalPositions.Print();
        storedLocalRotations.Print();
        Debug.Log("----------- KeyFrameData end ----------");
    }

}

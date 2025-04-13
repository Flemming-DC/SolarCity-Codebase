using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hearing : MonoBehaviour, ISense
{
    [SerializeField] float hearingRadius = 5;
    [SerializeField] float hearingMemory = 2;


    public List<GameObject> detectedObjects { get; private set; } = new();

    void Start()
    {
        Hurt.onHurt += OnSomeoneHurt;
    }

    private void OnDestroy()
    {
        Hurt.onHurt -= OnSomeoneHurt;
    }


    void OnSomeoneHurt(GameObject attacked, GameObject attacker)
    {
        HearIfNear(attacked);
        if (attacker != null)
            HearIfNear(attacker);
    }

    void HearIfNear(GameObject noisyObject)
    {
        float SqrDistance = (noisyObject.transform.position - transform.position).sqrMagnitude;
        if (SqrDistance > hearingRadius * hearingRadius)
            return;
        if (!isActiveAndEnabled)
            return;

        detectedObjects.Add(noisyObject);
        this.Delay(() => detectedObjects.Remove(noisyObject), hearingMemory);
    }

}

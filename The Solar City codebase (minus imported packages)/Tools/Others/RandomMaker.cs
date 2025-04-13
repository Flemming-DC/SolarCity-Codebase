using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomMaker
{

    public static bool Bool(float probability)
    {
        return Random.value < probability;
    }

    public static Vector3 HorizontalDirection()
    {
        float randomAnge = Random.Range(0, 2 * Mathf.PI);
        return new Vector3(Mathf.Cos(randomAnge), 0, Mathf.Sin(randomAnge));
    }

    public static Quaternion HorizontalRotation()
    {
        float randomAnge = Random.Range(0, 2 * Mathf.PI);
        Vector3 randomDirection = new Vector3(Mathf.Cos(randomAnge), 0, Mathf.Sin(randomAnge));
        return Quaternion.LookRotation(randomDirection);
    }

}

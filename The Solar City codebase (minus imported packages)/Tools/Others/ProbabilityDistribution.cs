using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ProbabilityDistribution
{
    float[] cumulativeProbabilities;


    public ProbabilityDistribution(float[] relativeProbabilities)
    {
        InitializeCumulativeProbabilities(relativeProbabilities);
    }

    void InitializeCumulativeProbabilities(float[] relativeProbabilities) 
    {
        float sum = relativeProbabilities.Sum();
        relativeProbabilities = relativeProbabilities.Select(p => p / sum).ToArray();

        cumulativeProbabilities = new float[relativeProbabilities.Length];
        cumulativeProbabilities[0] = relativeProbabilities[0];
        for (int i = 1; i < relativeProbabilities.Length; i++)
            cumulativeProbabilities[i] = cumulativeProbabilities[i - 1] + relativeProbabilities[i];

        float one = cumulativeProbabilities.Last();
        one = Mathf.Round(one * 100f) / 100f;
        if (one != 1)
            Debug.Log($"the last cumulative probability is {cumulativeProbabilities.Last()}." +
                      $"The answer should be 1.");
    }

    public int Draw() 
    {
        float rand = UnityEngine.Random.Range(0f, 1f);
        for (int i = 0; i < cumulativeProbabilities.Length; i++)
        {
            if (rand < cumulativeProbabilities[i])
                return i;
        }

        Debug.LogWarning($"Failed to draw a random element from the probability distribution");
        return -1;
    }


}

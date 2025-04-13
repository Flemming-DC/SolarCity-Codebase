using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSword : MonoBehaviour
{
    [SerializeField] Sound fireSound;
    [SerializeField] GameObject flame;

    List<ParticleSystem> flameParticles;

    void Start()
    {
        if (IsDead())
        {
            flameParticles = new List<ParticleSystem>(flame.GetComponentsInChildren<ParticleSystem>());
            flameParticles.ForEach(p => p.Stop());
        }
        else
        {
            fireSound.MakeSource(gameObject);
            fireSound.Play(gameObject);
        }
    }

    void OnDestroy()
    {
        fireSound?.FadeOut(gameObject, 1);
    }


    bool IsDead()
    {
        Transform ancestor = transform;
        while (ancestor != null)
        {
            ancestor = ancestor.parent;
            if (ancestor.TryGetComponent(out Ragdoll ragdoll))
                return ragdoll.isDead;
            else if (ancestor.TryGetComponent(out CharacterReferencer referencer))
                return referencer.Ragdoll.GetComponent<Ragdoll>().isDead;
        }

        Debug.LogWarning(
            $"Failed to find either Ragdoll or CharacterReferencer in the ancestors of " +
            $"the FireSword {transform.Path()}, which should be impossible.");
        return false;
    }



}

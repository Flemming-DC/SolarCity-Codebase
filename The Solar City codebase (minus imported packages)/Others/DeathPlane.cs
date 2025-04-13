using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DeathPlane : MonoBehaviour
{
    [SerializeField] float delay;

    private void OnTriggerEnter(Collider other)
    {
        var character = GetCharacter(other.transform);
        if (character == null)
            return;

        if (!character.TryGetComponentInCharacter(out ADamagable damagable))
            return;

        if (character.gameObject == PlayerReferencer.player)
            damagable.PlayerPretendToDie();
        else
            this.Delay(() => damagable.TakeDamageDirectly(1000 * 1000), delay);

    }

    public Transform GetCharacter(Transform boneOrCharacter)
    {
        // this function is used to get the character from bones inside the ragdoll. 
        // if the input is a character, then it returns immediately

        if (boneOrCharacter.IsInLayerMask(LayerData.instance.characterMask))
            return boneOrCharacter;

        while (boneOrCharacter != null)
        {
            if (boneOrCharacter.TryGetComponent(out IRagdoll ragdoll))
                return ragdoll.character;
            boneOrCharacter = boneOrCharacter.parent;
        }

        return null;
    }



}

using UnityEngine;

public class PlayerReferencer : MonoBehaviour
{
    public static GameObject player { get; private set; }

    private void Awake()
    {
        if (player == null)
            player = gameObject;
        else
            Debug.LogWarning($"Player has already been instantiated. You cannot have two players");

    }

    private void OnDestroy()
    {
        player = null;
    }


    public static T CharacterComponent<T>() //where T : Component
    {
        return player.GetComponentInCharacter<T>();
    }


}

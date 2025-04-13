using UnityEngine;

public class PermanentDeletion : MonoBehaviour
{
    static bool quiting;

    void Start()
    {
        if (!SaveData.loaded)
            return;
        if (!SaveData.file.GetBool(transform.Path() + ".PermanentDeletion.Deleted"))
            return;
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (quiting)
            return;
        if (!gameObject.scene.isLoaded)
            return;

        SaveData.file.Add(transform.Path() + ".PermanentDeletion.Deleted", true);
    }

    private void OnApplicationQuit()
    {
        quiting = true;
    }

}


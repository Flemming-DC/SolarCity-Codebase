using UnityEngine;
using TigerForge;

public class SaveData : MonoBehaviour
{
    [SerializeField] bool loadOnStartup = true;

    public static EasyFileSave file { get; private set; }
    public static bool loaded { get; private set; }
    public static string respawnPosition = "RespawnPosition";


    private void Awake()
    {
        file = new EasyFileSave();

        if (!loadOnStartup)
            return;
        loaded = file.Load();
        if (!loaded)
            Debug.LogWarning("loading failed");
    }

    private void OnDestroy()
    {
        bool saved = file.Save();
        if (!saved)
            Debug.LogWarning("saving failed");
    }






}

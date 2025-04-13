using UnityEngine;
using UnityEngine.UI;

// initialized late, so that others can receive the event onCheapGraphicsChanged at startup
[DefaultExecutionOrder(10)] 
public class SettingsCanvas : ASettingsCanvas
{
    [SerializeField] Toggle cheapGraphics;
    [SerializeField] Toggle hardMode;
    [SerializeField] Slider cameraSensitivity;
    [SerializeField] GameObject cameraRotatorObject;
    [SerializeField] float maxRotationSpeedMultiplier = 4f;

    IJoyStickCameraRotator cameraRotator;
    ADamagable playerDamagable;

    void Start()
    {
        cameraRotator = cameraRotatorObject.GetComponent<IJoyStickCameraRotator>();
        playerDamagable = PlayerReferencer.CharacterComponent<ADamagable>();

        cheapGraphics.onValueChanged.AddListener(OnCheapGraphicsChanged);
        hardMode.onValueChanged.AddListener(OnHardModeChanged);
        cameraSensitivity.onValueChanged.AddListener(OnCameraSensitivityChanged);

        if (cameraSensitivity.minValue != 0)
            Debug.LogWarning($"The minValue of {cameraSensitivity.name} should be zero");
        if (cameraSensitivity.maxValue != 1)
            Debug.LogWarning($"The maxValue of {cameraSensitivity.name} should be one");
        
        Load();
    }


    public void OnCheapGraphicsChanged(bool useCheapGraphics)
    {
        if (QualitySettings.names.Length != 4)
            Debug.LogWarning("The number of quality settings has been changed. " +
                "You need to update references to their indices.");

        SaveData.file.Add($"{transform.Path()}.SettingsCanvas.cheapGraphics.isOn", useCheapGraphics);
        ASettingsCanvas.useCheapGraphics = useCheapGraphics;
        QualitySettings.SetQualityLevel(useCheapGraphics ? 1 : 3);
        onToggleCheapGraphics?.Invoke(useCheapGraphics);

    }

    public void OnHardModeChanged(bool startHardMode)
    {
        SaveData.file.Add($"{transform.Path()}.SettingsCanvas.hardMode.isOn", startHardMode);
        playerDamagable?.SetMaxHealthFactor(startHardMode ? 1 : 2);
    }

    public void OnCameraSensitivityChanged(float newValue)
    {
        SaveData.file.Add($"{transform.Path()}.SettingsCanvas.cameraSensitivity.value", newValue);
        float multiplier = Mathf.Pow(maxRotationSpeedMultiplier, 2 * newValue - 1);
        cameraRotator?.SetRotationSpeedMultiplier(multiplier);
    }





    void Load()
    {
        if (SaveData.loaded)
        {
            if (SaveData.file.KeyExists($"{transform.Path()}.SettingsCanvas.cheapGraphics.isOn"))
                cheapGraphics.isOn = SaveData.file.GetBool($"{transform.Path()}.SettingsCanvas.cheapGraphics.isOn");
            if (SaveData.file.KeyExists($"{transform.Path()}.SettingsCanvas.hardMode.isOn"))
                hardMode.isOn = SaveData.file.GetBool($"{transform.Path()}.SettingsCanvas.hardMode.isOn");
            if (SaveData.file.KeyExists($"{transform.Path()}.SettingsCanvas.cameraSensitivity.value"))
                cameraSensitivity.value = SaveData.file.GetFloat($"{transform.Path()}.SettingsCanvas.cameraSensitivity.value");
        }
        OnCheapGraphicsChanged(cheapGraphics.isOn);
        OnHardModeChanged(hardMode.isOn);
        OnCameraSensitivityChanged(cameraSensitivity.value);
    }

}

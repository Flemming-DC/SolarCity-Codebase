using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] string volumeParameter = "masterVolume";
    [SerializeField] float maxDecibel = 20;
    [SerializeField] float sensitivity = 30;

    Toggle muteToggle;
    Slider slider;
    float minVolume;
    bool toogleIgnoreCallback;

    private void Awake()
    {
        muteToggle = this.GetComponentInDirectChildren<Toggle>();
        slider = this.GetComponentInDirectChildren<Slider>();

        minVolume = Mathf.Pow(10, -(maxDecibel + 80) / sensitivity);
        slider.onValueChanged.AddListener(OnVolumeChanged);
        muteToggle.onValueChanged.AddListener(OnMuteToggleChanged);

        if (slider.minValue != 0)
            Debug.LogWarning($"The minValue of {slider.name} should be zero");
        if (slider.maxValue != 1)
            Debug.LogWarning($"The maxValue of {slider.name} should be one");
    }

    private void Start()
    {
        // this line cannot be moved into the awake function, since then the mixer doesn't update on startup
        slider.value = PlayerPrefs.GetFloat(volumeParameter, slider.value); 
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(volumeParameter, slider.value);
    }


    void OnVolumeChanged(float volume)
    {
        toogleIgnoreCallback = true;
        muteToggle.isOn = (volume > 0);
        toogleIgnoreCallback = false;
        float decibel = 20 + sensitivity * Mathf.Log10(Mathf.Max(volume, minVolume));
        audioMixer.SetFloat(volumeParameter, decibel);
    }

    private void OnMuteToggleChanged(bool enableSound)
    {
        if (toogleIgnoreCallback)
            return;

        if (enableSound)
            slider.value = PlayerPrefs.GetFloat(volumeParameter, slider.value);
        else
        {
            PlayerPrefs.SetFloat(volumeParameter, slider.value);
            slider.value = 0;
        }
    }



}

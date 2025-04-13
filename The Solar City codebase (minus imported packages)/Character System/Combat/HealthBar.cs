using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using ConditionalField;

public class HealthBar : MonoBehaviour
{
    [SerializeField] bool followCharacter = true;
    [ConditionalField(nameof(followCharacter))] [SerializeField] float verticalOffset;
    [SerializeField] bool showHealthTemporarily = true;
    [ConditionalField(nameof(showHealthTemporarily))] [SerializeField] float visibilityDuration = 3;
    [ConditionalField(nameof(showHealthTemporarily))] [SerializeField] Image background;
    [SerializeField] Image fill;
    [SerializeField] UpdateTimer updateTimer;

    float height;
    Damagable damagable;
    Transform cameraTransform;

    public void SetupHealthBar(Damagable damagable_, float height_)
    {
        damagable = damagable_;
        height = height_;
        updateTimer.Start(damagable.GetComponentInParent<CharacterReferencer>(), 0.1f, 0.4f);
        SetHealthUI(damagable.health / damagable.MaxHealth);
        SetVisibilityIfNeeded(false);
        cameraTransform = Camera.main.transform;
        damagable.OnHealthChanged += OnHealthChanged;

        if (!followCharacter)
            transform.SetParent(null);
    }

    void OnDestroy()
    {
        damagable.OnHealthChanged -= OnHealthChanged;
        updateTimer.Stop();
    }

    void LateUpdate()
    {
        if (!fill.enabled)
            return;
        if (!updateTimer.TimeToUpdate())
            return;
        if (!followCharacter)
            return;

        transform.localPosition = verticalOffset * height * Vector3.up;
        transform.LookAt(transform.position + cameraTransform.rotation * Vector3.forward,
                         cameraTransform.rotation * Vector3.up); // rotate to face player
    }


    void OnHealthChanged(GameObject dummy1, float dummy2, float newHealth)
    {
        SetHealthUI(newHealth / damagable.MaxHealth);
    }

    void SetHealthUI(float healthPct)
    {
        if (healthPct < 0)
            Debug.LogWarning($"HealthBar received a negative health percent.");

        SetVisibilityIfNeeded(true);
        fill.fillAmount = healthPct;
        // evt. change fillamount graduately
        this.Delay(() => SetVisibilityIfNeeded(false), visibilityDuration);
    }


    void SetVisibilityIfNeeded(bool visible)
    {
        if (showHealthTemporarily)
        {
            fill.enabled = visible;
            background.enabled = visible;
        }
    }

}

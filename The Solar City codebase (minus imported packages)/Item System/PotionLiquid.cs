using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionLiquid : MonoBehaviour
{
    [SerializeField] Transform liquid; // evt. make a class that sets every variable for the Potion Liquid shader on this particular gameObject
    [SerializeField] float fillAmount;
    [SerializeField] float MaxWobble = 0.03f;
    [SerializeField] float WobbleSpeed = 1f;
    [SerializeField] float Recovery = 1f;
    [SerializeField] Color liquidColor;
    [SerializeField] Color surfaceColor;
    [SerializeField] Color fresnelColor;

    Renderer rend;
    Vector3 lastPos;
    Vector3 velocity;
    Vector3 lastRot;
    Vector3 angularVelocity;
    float wobbleAmountX;
    float wobbleAmountZ;
    float wobbleAmountToAddX;
    float wobbleAmountToAddZ;
    float pulse;

    void Start()
    {
        rend = liquid.GetComponent<Renderer>();
        rend.material.SetVector("_LiquidColor", liquidColor);
        rend.material.SetVector("_SurfaceColor", surfaceColor);
        rend.material.SetVector("_FresnelColor", fresnelColor);
    }

    private void Update()
    {
        Wobble();
        HandleFillAmount();
    }


    void HandleFillAmount()
    {
        float halfHeight = rend.bounds.size.y;
        rend.material.SetFloat("_Fill", (fillAmount - 0.5f) * halfHeight);
        rend.material.SetVector("_Center", rend.bounds.center);
    }

    void Wobble()
    {
        wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, Time.deltaTime * (Recovery));
        wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, Time.deltaTime * (Recovery));

        pulse = 2 * Mathf.PI * WobbleSpeed;
        wobbleAmountX = wobbleAmountToAddX * Mathf.Sin(pulse * Time.time);
        wobbleAmountZ = wobbleAmountToAddZ * Mathf.Sin(pulse * Time.time);

        rend.material.SetFloat("_WobbleX", wobbleAmountX);
        rend.material.SetFloat("_WobbleZ", wobbleAmountZ);

        velocity = (lastPos - liquid.position) / Time.deltaTime;
        angularVelocity = liquid.rotation.eulerAngles - lastRot;

        wobbleAmountToAddX += Mathf.Clamp((velocity.x + (angularVelocity.z * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
        wobbleAmountToAddZ += Mathf.Clamp((velocity.z + (angularVelocity.x * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);

        lastPos = liquid.position;
        lastRot = liquid.rotation.eulerAngles;
    }


}

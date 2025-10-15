using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class Sugar : MonoBehaviour
{
    // Sugar attributes
    public float maxSugar = 100f;
    public float currentSugar;
    public float decayRate = 5f;

    // UI attributes
    public Slider sugarSlider;
    public Image fillImage;
    public Color fullColor = Color.green;
    public Color midColor = Color.yellow;
    public Color lowColor = Color.red;

    // Damage shader material
    public Material vignetteMaterial;
    public float lowHealthThreshold = 30f;
    public float pulseSpeed = 2f;
    public float minRadius = 0.25f;
    public float maxRadius = 0.5f;

    void Start()
    {
        currentSugar = maxSugar;

        if (sugarSlider != null)
        {
            sugarSlider.maxValue = maxSugar;
            sugarSlider.value = currentSugar;
        }

        if (vignetteMaterial != null)
        {
            vignetteMaterial.SetFloat("_Intensity", 0);
        }
    }

    void Update()
    {
        // keep sugar bar in sync (TBD If needed)
        // UpdateSugarBar();
        UpdateDamageEffect();
    }

    void UpdateDamageEffect()
    {
        if (vignetteMaterial == null) return;

        if (currentSugar <= lowHealthThreshold && currentSugar > 0)
        {
            float pulse = Mathf.Lerp(minRadius, maxRadius, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
            vignetteMaterial.SetFloat("_Vignette_radius", pulse);
        }
        else
        {
            // disable by setting to 1
            vignetteMaterial.SetFloat("_Vignette_radius", 1f);
        }
    }

    void UpdateSugarBar()
    {
        if (sugarSlider != null)
            sugarSlider.value = currentSugar;

        if (fillImage != null)
        {
            UpdateBarColor();
        }

        UpdateDamageEffect();
    }

    void UpdateBarColor()
    {
        float percentage = currentSugar / maxSugar;
        //Debug.Log("percentage " + percentage.ToString());

        // Color transitions:
        Color targetColor;
        if (percentage > 0.5f)
        {
            // green to yellow
            targetColor = Color.Lerp(midColor, fullColor, (percentage - 0.5f) * 2f);
        }
        else
        {
            // yellow to red
            targetColor = Color.Lerp(lowColor, midColor, percentage * 2f);
        }

        fillImage.color = Color.Lerp(fillImage.color, targetColor, Time.deltaTime * 10f);
    }

    public void AddSugar(float amount)
    {
        //Debug.Log("adding sugar " + amount.ToString());
        currentSugar = Math.Min(currentSugar + amount, maxSugar);
        UpdateSugarBar();
    }

    public void DrainSugar(float amount)
    {
        //Debug.Log("subtracting sugar " + amount.ToString());
        currentSugar = Math.Max(currentSugar - amount, 0f);
        UpdateSugarBar();

        if (currentSugar <= 0)
        {
            // disable damage shader on death
            if (vignetteMaterial != null)
            {
                vignetteMaterial.SetFloat("_Vignette_radius", 1f);
            }

            EventBus.Publish(new PlayerDeathEvent());
        }
    }
}

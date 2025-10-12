using System;
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

    void Start()
    {
        currentSugar = maxSugar;

        if (sugarSlider != null)
        {
            sugarSlider.maxValue = maxSugar;
            sugarSlider.value = currentSugar;
        }
    }

    void Update()
    {
        // decay sugar at constant rate
        currentSugar -= decayRate * Time.deltaTime;
        // keep bar in sync
        UpdateUI();
    }

    void UpdateUI()
    {
        if (sugarSlider != null)
            sugarSlider.value = currentSugar;

        if (fillImage != null)
        {
            UpdateBarColor();
        }
    }

    void UpdateBarColor()
    {
        float percentage = currentSugar / maxSugar;
        // Debug.Log("percentage " + percentage.ToString());

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
        Debug.Log("adding sugar " + amount.ToString());
        currentSugar = Math.Min(currentSugar + amount, maxSugar);
        UpdateUI();
    }

    public void DrainSugar(float amount)
    {
        Debug.Log("subtracting sugar " + amount.ToString());
        currentSugar = Math.Max(currentSugar - amount, 0f);
        UpdateUI();
    }
}

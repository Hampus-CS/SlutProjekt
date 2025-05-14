using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Sliders : MonoBehaviour
{
    public Slider manaSlider;
    public Slider healthSlider;

    private Image manaFillImage;
    private Image healthFillImage;
    
    private Coroutine healthFlashCoroutine;
    private bool flashHealthOnDamage = false;

    private float flashSpeed = 4f;
    public float flashPrecent = 0.3f;

    void Start()
    {
        manaFillImage = CustomizeSlider(manaSlider);
        healthFillImage = CustomizeSlider(healthSlider);
    }

    void Update()
    {
        
        if (!flashHealthOnDamage)
        {
            UpdateSliderVisual(healthSlider, healthFillImage, Color.red, Color.green);
        }
        UpdateSliderVisual(manaSlider, manaFillImage, Color.red,new Color(0.529f, 0.808f, 0.980f));
    }

    void UpdateSliderVisual(Slider slider, Image fillImage, Color lowColor, Color fullColor)
    {
        if (slider == null || fillImage == null) return;

        float fillPercent = slider.value / slider.maxValue;

        Color baseColor = fillPercent <= flashPrecent ? lowColor : fullColor;

        if (fillPercent <= flashPrecent)
        {
            // Flash when under flashPrecent (30%)
            float alpha = Mathf.Lerp(0.25f, 1f, Mathf.Abs(Mathf.Sin(Time.time * flashSpeed)));
            baseColor.a = alpha;
        }
        else
        {
            baseColor.a = 1f;
        }

        fillImage.color = baseColor;
    }


    Image CustomizeSlider(Slider slider)
    {
        if (slider == null) return null;

        // Remove handle
        if (slider.handleRect != null)
        {
            Destroy(slider.handleRect.gameObject);
            slider.handleRect = null;
        }

        Transform fillArea = slider.transform.Find("Fill Area");
        Image fillImage = null;

        if (fillArea != null)
        {
            RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();

            // Resize background to match fill area
            Transform backgroundTransform = slider.transform.Find("Background");
            if (backgroundTransform != null)
            {
                RectTransform backgroundRect = backgroundTransform.GetComponent<RectTransform>();

                // Match Fill Area size and alignment
                backgroundRect.anchorMin = fillAreaRect.anchorMin;
                backgroundRect.anchorMax = fillAreaRect.anchorMax;
                backgroundRect.pivot = fillAreaRect.pivot;
                backgroundRect.anchoredPosition = fillAreaRect.anchoredPosition;
                backgroundRect.sizeDelta = fillAreaRect.sizeDelta;

                // Set background color
                Image bgImage = backgroundTransform.GetComponent<Image>();
                if (bgImage != null)
                {
                    bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.5f);
                }
            }

            // Set up fill area
            Transform fill = fillArea.Find("Fill");
            if (fill != null)
            {
                RectTransform fillRect = fill.GetComponent<RectTransform>();
                fillRect.anchorMin = new Vector2(0, 0);
                fillRect.anchorMax = new Vector2(1, 1);
                fillRect.offsetMin = Vector2.zero;
                fillRect.offsetMax = Vector2.zero;

                fillImage = fill.GetComponent<Image>();
            }
        }

        return fillImage;
    }
    
    public void FlashHealthOnDamage()
    {
        flashHealthOnDamage = true;

        if (healthFlashCoroutine != null)
            StopCoroutine(healthFlashCoroutine);

        healthFlashCoroutine = StartCoroutine(HealthDamageFlash());
    }
    
    private IEnumerator HealthDamageFlash()
    {
        float flashDuration = 0.5f;
        float elapsed = 0f;

        while (elapsed < flashDuration)
        {
            float alpha = Mathf.Lerp(0.25f, 1f, Mathf.Abs(Mathf.Sin(Time.time * 20f)));
            Color flashColor = Color.red;
            flashColor.a = alpha;

            healthFillImage.color = flashColor;

            elapsed += Time.deltaTime;
            yield return null;
        }

        flashHealthOnDamage = false;
    }
}
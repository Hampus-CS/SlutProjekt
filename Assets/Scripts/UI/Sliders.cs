using UnityEngine;
using UnityEngine.UI;

public class Sliders : MonoBehaviour
{
    public Slider manaSlider;
    public Slider healthSlider;

    private Image manaFillImage;
    private Image healthFillImage;

    private float flashSpeed = 4f;

    void Start()
    {
        manaFillImage = CustomizeSlider(manaSlider);
        healthFillImage = CustomizeSlider(healthSlider);
    }

    void Update()
    {
        UpdateSliderVisual(healthSlider, healthFillImage, Color.red, Color.green);
        UpdateSliderVisual(manaSlider, manaFillImage, Color.red, Color.blue);
    }

    void UpdateSliderVisual(Slider slider, Image fillImage, Color lowColor, Color fullColor)
    {
        if (slider == null || fillImage == null) return;

        float fillPercent = slider.value / slider.maxValue;
        Color baseColor = Color.Lerp(lowColor, fullColor, fillPercent);

        if (fillPercent <= 0.2f)
        {
            // Flash
            float alpha = Mathf.Abs(Mathf.Sin(Time.time * flashSpeed));
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
}
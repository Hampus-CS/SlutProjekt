using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class CharacterCarouselSelector : MonoBehaviour
{
    public List<Sprite> characterSprites;

    public Image slotLeft;
    public Image slotCenter;
    public Image slotRight;

    private int selectedIndex = 0;

    void Start()
    {
        UpdateSlots(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ScrollLeft();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ScrollRight();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            SelectCurrentCharacter();
        }
    }

    public void ScrollLeft()
    {
        selectedIndex = (selectedIndex - 1 + characterSprites.Count) % characterSprites.Count;
        UpdateSlots(false);
    }

    public void ScrollRight()
    {
        selectedIndex = (selectedIndex + 1) % characterSprites.Count;
        UpdateSlots(false);
    }
    public void SelectCurrentCharacter()
    {
        Sprite selected = characterSprites[selectedIndex];
        Debug.Log("Selected character: " + selected.name);

        // Trigger whatever happens when a character is chosen
    }

    void UpdateSlots(bool instant)
    {
        int count = characterSprites.Count;
        int leftIndex = (selectedIndex - 1 + count) % count;
        int rightIndex = (selectedIndex + 1) % count;

        slotLeft.sprite = characterSprites[leftIndex];
        slotCenter.sprite = characterSprites[selectedIndex];
        slotRight.sprite = characterSprites[rightIndex];

        ApplyStyle(slotLeft, 0.5f, 0.8f, instant);
        ApplyStyle(slotCenter, 1f, 1.2f, instant);
        ApplyStyle(slotRight, 0.5f, 0.8f, instant);
    }

    void ApplyStyle(Image img, float alpha, float scale, bool instant)
    {
        if (img.TryGetComponent<CanvasGroup>(out var group))
        {
            if (!instant)
            {
                group.alpha = 1f;
                group.DOFade(alpha, 0.4f).SetEase(Ease.InOutQuad);
            }
            else
            {
                group.alpha = alpha;
            }
        }

        if (!instant)
        {
            img.rectTransform.localScale = Vector3.one;
        }
        else
        {
            img.rectTransform.localScale = Vector3.one * scale;
            img.rectTransform.DOScale(Vector3.one * scale, 0.4f).SetEase(Ease.OutBack);
        }
    }
}

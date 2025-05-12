using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using DG.Tweening;

public class CharacterDetailCarouselSelector : MonoBehaviour
{
    [Header("UI Slots (Image + CanvasGroup on each)")]
    [SerializeField] private Image portraitSlotImage;
    [SerializeField] private Image basicSlotImage;
    [SerializeField] private Image abilitySlotImage;

    [Header("Scroll Settings")]
    [Tooltip("Seconds between allowed scrolls")]
    [SerializeField] private float scrollCooldown = 0.3f;

    private Sprite[] slotSprites;
    private int selectedIndex;
    private bool canScroll = true;
    
    public event Action<int> OnDetailIndexChanged;

    public void Initialize(Sprite idleSprite, Sprite basicSprite, Sprite abilitySprite)
    {
        slotSprites = new[] { idleSprite, basicSprite, abilitySprite };
        selectedIndex = 0;
        UpdateSlots(instant: true);
        OnDetailIndexChanged?.Invoke(0);
    }
    
    void Update()
    {
        if (!canScroll) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            StartCoroutine(ScrollCooldown(() => ChangeIndex(-1)));
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            StartCoroutine(ScrollCooldown(() => ChangeIndex(+1)));
        else if (Input.GetKeyDown(KeyCode.Space))
            OnDetailIndexChanged?.Invoke(selectedIndex);
    }
    
    public void MoveLeft()  => StartCoroutine(ScrollCooldown(() => ChangeIndex(+1)));
    public void MoveRight() => StartCoroutine(ScrollCooldown(() => ChangeIndex(-1)));
    public void Confirm()   => OnDetailIndexChanged?.Invoke(selectedIndex);

    private IEnumerator ScrollCooldown(Action act)
    {
        canScroll = false;
        act();
        yield return new WaitForSeconds(scrollCooldown);
        canScroll = true;
    }

    private void ChangeIndex(int delta)
    {
        selectedIndex = (selectedIndex + delta + slotSprites.Length) % slotSprites.Length;
        UpdateSlots(instant: false);
        OnDetailIndexChanged?.Invoke(selectedIndex);
    }

    private void UpdateSlots(bool instant)
    {
        ApplySlot(portraitSlotImage, slotSprites[0], selectedIndex == 0, instant);
        ApplySlot(basicSlotImage,    slotSprites[1], selectedIndex == 1, instant);
        ApplySlot(abilitySlotImage,  slotSprites[2], selectedIndex == 2, instant);
    }

    private void ApplySlot(Image img, Sprite sprite, bool isCenter, bool instant)
    {
        img.sprite = sprite;

        float targetAlpha = isCenter ? 1f : 0.5f;
        float targetScale = isCenter ? 1.2f : 0.8f;
        var group = img.GetComponent<CanvasGroup>();

        if (instant)
        {
            group.alpha = targetAlpha;
            img.rectTransform.localScale = Vector3.one * targetScale;
        }
        else
        {
            group.DOFade(targetAlpha, scrollCooldown);
            img.rectTransform.DOScale(Vector3.one * targetScale, scrollCooldown);
        }
    }
}

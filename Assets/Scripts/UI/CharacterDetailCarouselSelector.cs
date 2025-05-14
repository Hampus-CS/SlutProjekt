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

    private Sprite[] sprites;
    private int selectedIndex;
    private bool canScroll = true;
    
    public event Action<int> OnDetailIndexChanged;

    public void Initialize(Sprite idleSprite, Sprite basicSprite, Sprite abilitySprite)
    {
        sprites = new[] { idleSprite, basicSprite, abilitySprite };
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
    public void Confirm() => OnDetailIndexChanged?.Invoke(selectedIndex);

    private IEnumerator ScrollCooldown(Action act)
    {
        canScroll = false;
        act();
        yield return new WaitForSeconds(scrollCooldown);
        canScroll = true;
    }

    private void ChangeIndex(int delta)
    {
        selectedIndex = (selectedIndex + delta + sprites.Length) % sprites.Length;
        UpdateSlots(instant: false);
        OnDetailIndexChanged?.Invoke(selectedIndex);
    }

    private void UpdateSlots(bool instant)
    {
        
        int count = sprites.Length;                 
        int left  = (selectedIndex - 1 + count) % count;
        int right = (selectedIndex + 1) % count;
        
        ApplySlot(portraitSlotImage, sprites[left],   false, instant);
        ApplySlot(basicSlotImage,    sprites[selectedIndex], true, instant);
        ApplySlot(abilitySlotImage,  sprites[right],  false, instant);
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

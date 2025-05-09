using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;

public class CharacterCarouselSelector : MonoBehaviour
{
    [Header("Character Data List")]
    [SerializeField] private List<CharacterData> characters;
    
    [Header("UI References")]
    [SerializeField] private Image slotLeftImage;
    [SerializeField] private Animator slotLeftAnimator;
    [SerializeField] private Image slotCenterImage;
    [SerializeField] private Animator slotCenterAnimator;
    [SerializeField] private Image slotRightImage;
    [SerializeField] private Animator slotRightAnimator;

    [Header("Scroll & Tween Settings")]
    [Tooltip("Seconds between allowed scrolls")]
    public float scrollCooldown = 0.3f;
    [Tooltip("Alpha of non‑selected slots")]
    public float sideAlpha = 0.5f;
    [Tooltip("Scale of non‑selected slots")]
    public float sideScale = 0.8f;
    [Tooltip("Alpha of selected (center) slot")]
    public float centerAlpha = 1f;
    [Tooltip("Scale of selected (center) slot")]
    public float centerScale = 1.2f;
    
    [Header("Generic Slot Controller")]
    [Tooltip("AnimatorController with one state named 'Loop'")]
    [SerializeField] private RuntimeAnimatorController slotBaseController;
    
    [SerializeField] private int selectedIndex = 0;
    private bool canScroll = true;

    // Fired when player confirms (via UI button or Space)
    public event Action<int> OnCharacterChosen;
    
    public void Initialize(List<CharacterData> allCharacters)
    {
        characters = allCharacters;
        selectedIndex = 0;
        UpdateSlots(instant: true);
    }
    
    void Update()
    {
        if (!canScroll) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            StartCoroutine(ScrollCooldown(() => ChangeIndex(-1)));
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            StartCoroutine(ScrollCooldown(() => ChangeIndex(+1)));
        else if (Input.GetKeyDown(KeyCode.Space))
            OnCharacterChosen?.Invoke(selectedIndex);
    }

    // Hook to < and > UI Buttons
    public void MoveLeft()  => ChangeIndex(-1);
    public void MoveRight() => ChangeIndex(+1);

    // Hook this to “Select” UI Button
    public void ConfirmSelection() => OnCharacterChosen?.Invoke(selectedIndex);
    
    private void ChangeIndex(int delta)
    {
        selectedIndex = (selectedIndex + delta + characters.Count) % characters.Count;
        UpdateSlots(instant: false);
    }
    
    private IEnumerator ScrollCooldown(Action change)
    {
        canScroll = false;
        change();
        yield return new WaitForSeconds(scrollCooldown);
        canScroll = true;
    }
    
    void UpdateSlots(bool instant)
    {
        int count = characters.Count;
        int leftIndex = (selectedIndex - 1 + count) % count;
        int rightIndex = (selectedIndex + 1) % count;
        
        ApplySlot(slotLeftImage, slotLeftAnimator, characters[leftIndex].uiIdleClip, characters[leftIndex].fallbackIdleSprite, sideAlpha, sideScale, instant);

        ApplySlot(slotCenterImage, slotCenterAnimator, characters[selectedIndex].uiIdleClip, characters[selectedIndex].fallbackIdleSprite,centerAlpha, centerScale, instant);

        ApplySlot(slotRightImage, slotRightAnimator, characters[rightIndex].uiIdleClip, characters[rightIndex].fallbackIdleSprite, sideAlpha, sideScale, instant);
    }

    void ApplySlot(Image img, Animator animator, AnimationClip clip, Sprite fallback, float targetAlpha, float targetScale, bool instant)
    {
    
        // assign animation or fallback sprite
        if (clip != null)
        {
            var overrideCtrl = new AnimatorOverrideController(slotBaseController);
            overrideCtrl["Loop"] = clip;
            animator.runtimeAnimatorController = overrideCtrl;
        }
        else
        {
            img.sprite = fallback;
        }
        
        // fade & scale via CanvasGroup + RectTransform
        var group = img.GetComponent<CanvasGroup>();
        if (instant)
        {
            group.alpha = targetAlpha;
            img.rectTransform.localScale = Vector3.one * targetScale;
        }
        else
        {
            group.DOFade(targetAlpha, scrollCooldown).SetEase(Ease.InOutQuad);
            img.rectTransform.DOScale(Vector3.one * targetScale, scrollCooldown).SetEase(Ease.OutBack);
        }
        
    }
}

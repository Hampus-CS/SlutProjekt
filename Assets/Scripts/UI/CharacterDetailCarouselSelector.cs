using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using DG.Tweening;

public class CharacterDetailCarouselSelector : MonoBehaviour
{
    [Header("UI Slots (Image, Animator, CanvasGroup on each)")]
    [SerializeField] private Image    portraitSlotImage;
    [SerializeField] private Animator portraitSlotAnimator;
    [SerializeField] private Image    basicSlotImage;
    [SerializeField] private Animator basicSlotAnimator;
    [SerializeField] private Image    abilitySlotImage;
    [SerializeField] private Animator abilitySlotAnimator;

    [Header("Scroll Settings")]
    [Tooltip("Seconds between allowed scrolls")]
    [SerializeField] private float scrollCooldown = 0.3f;

    [Header("Generic Slot Controller")]
    [SerializeField] private RuntimeAnimatorController slotBaseController;
    
    private AnimationClip[] clips;
    private Sprite[] fallbackSprites;
    private int selectedIndex;
    private bool canScroll = true;
    
    /// <summary>
    /// Fired whenever the center‑slot index changes (0=Portrait, 1=Basic, 2=Ability)
    /// </summary>
    public event Action<int> OnDetailIndexChanged;

    /// <summary>
    /// Initialize with the three animator‑controllers and three fallback sprites.
    /// Call this from CharacterDetailDisplay.SetCharacter(...).
    /// </summary>
    public void Initialize(
        AnimationClip idleClip,   Sprite idleFallback,
        AnimationClip basicClip,  Sprite basicFallback,
        AnimationClip abilityClip,Sprite abilityFallback)
    {
        clips = new[] { idleClip, basicClip, abilityClip };
        fallbackSprites = new[]  { idleFallback, basicFallback, abilityFallback };
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
        selectedIndex = (selectedIndex + delta + 3) % 3;
        UpdateSlots(instant: false);
        OnDetailIndexChanged?.Invoke(selectedIndex);
    }

    private void UpdateSlots(bool instant)
    {
        int count = clips.Length;
        int left  = (selectedIndex - 1 + count) % count;
        int right = (selectedIndex + 1)        % count;
        
        ApplySlot(portraitSlotImage, portraitSlotAnimator, clips[left], fallbackSprites[left],  false, instant);
        ApplySlot(basicSlotImage,    basicSlotAnimator,    clips[selectedIndex], fallbackSprites[selectedIndex], true,  instant);
        ApplySlot(abilitySlotImage,  abilitySlotAnimator,  clips[right], fallbackSprites[right], false, instant);
    }

    private void ApplySlot(Image img, Animator animator, AnimationClip clip,Sprite fallback, bool isCenter, bool instant)
    {
        // assign animation or fallback
        if (clip != null)
        {
            var overrideCtrl = new AnimatorOverrideController(slotBaseController);
            overrideCtrl["Loop"] = clip;
            animator.runtimeAnimatorController = overrideCtrl;
        }
        else
        {
            // stop animating so the Image.sprite shows
            animator.runtimeAnimatorController = null;
            animator.enabled = false;
            img.sprite = fallback;
        }
        // style: center vs side
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

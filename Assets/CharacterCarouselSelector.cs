using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class CharacterCarouselSelector : MonoBehaviour
{
    [Header("Character Data List")]
    public List<CharacterData> characters;

    [Header("UI References")]
    public Image slotLeft;
    public Image slotCenter;
    public Image slotRight;

    [Header("Panels to Toggle")]
    public GameObject characterCarouselPanel;
    public GameObject characterDetailPanelRoot;

    private int selectedIndex = 0;

    void Start()
    {
        if (characters == null || characters.Count == 0)
        {
            Debug.LogError("No characters assigned to the carousel!");
            return;
        }
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
        selectedIndex = (selectedIndex - 1 + characters.Count) % characters.Count;
        UpdateSlots(false);
    }

    public void ScrollRight()
    {
        selectedIndex = (selectedIndex + 1) % characters.Count;
        UpdateSlots(false);
    }

    public void SelectCurrentCharacter()
    {
        if (characters == null || characters.Count == 0) return;

        CharacterData selected = characters[selectedIndex];
        CharacterDetailContext.SelectedCharacter = selected;

        Debug.Log("Selected character: " + selected.displayName + " (ID: " + selected.id + ")");

        PanelTransition.SwapPanels(characterCarouselPanel, characterDetailPanelRoot);
    }

    void UpdateSlots(bool instant)
    {
        int count = characters.Count;
        int leftIndex = (selectedIndex - 1 + count) % count;
        int rightIndex = (selectedIndex + 1) % count;

        slotLeft.sprite = characters[leftIndex].idleSprite;
        slotCenter.sprite = characters[selectedIndex].idleSprite;
        slotRight.sprite = characters[rightIndex].idleSprite;

        ApplyStyle(slotLeft, 0.5f, 0.8f, instant);
        ApplyStyle(slotCenter, 1f, 1.2f, instant);
        ApplyStyle(slotRight, 0.5f, 0.8f, instant);
    }

    void ApplyStyle(Image img, float alpha, float scale, bool instant)
    {
        if (img.TryGetComponent<CanvasGroup>(out var group))
        {
            if (instant)
            {
                group.alpha = 1f;
            }
            else
            {
                group.alpha = 0f;
                group.DOFade(1f, 0.4f).SetEase(Ease.InOutQuad);
            }

            if (instant)
            {
                group.alpha = alpha;
            }
            else
            {
                group.DOFade(alpha, 0.4f).SetEase(Ease.InOutQuad);
            }
        }

        if (!instant)
        {
            img.rectTransform.localScale = Vector3.one;
        }

        if (instant)
        {
            img.rectTransform.localScale = Vector3.one * scale;
        }
        else
        {
            img.rectTransform.DOScale(Vector3.one * scale, 0.4f).SetEase(Ease.OutBack);
        }
    }
}

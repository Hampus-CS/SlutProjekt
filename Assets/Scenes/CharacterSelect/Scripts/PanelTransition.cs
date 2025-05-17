using UnityEngine;
using DG.Tweening;

public static class PanelTransition
{
    public static void FadeIn(GameObject panel, float duration = 0.4f)
    {
        if (panel == null) return;

        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            Debug.LogError($"No CanvasGroup on {panel.name}!");
            return;
        }

        panel.SetActive(true);
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        cg.DOFade(1f, duration).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            cg.interactable = true;
            cg.blocksRaycasts = true;
        });
    }

    public static void FadeOut(GameObject panel, float duration = 0.4f)
    {
        if (panel == null) return;

        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            Debug.LogError($"No CanvasGroup on {panel.name}!");
            return;
        }

        cg.interactable = false;
        cg.blocksRaycasts = false;

        cg.DOFade(0f, duration).SetEase(Ease.InOutQuad)
          .OnComplete(() => panel.SetActive(false));
    }

    public static void SwapPanels(GameObject from, GameObject to, float duration = 0.4f)
    {
        FadeOut(from, duration);
        FadeIn(to, duration);
    }
}

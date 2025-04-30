using UnityEngine;

public class BackToCharacterSelect : MonoBehaviour
{
    [Header("Panels to Toggle")]
    public GameObject characterCarouselPanel;
    public GameObject characterDetailPanelRoot;

    public void OnBackButtonPressed()
    {
        PanelTransition.SwapPanels(characterDetailPanelRoot, characterCarouselPanel);
    }
}

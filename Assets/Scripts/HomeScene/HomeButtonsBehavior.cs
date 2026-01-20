using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HomeButtonsBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

    public void OnPointerClick(PointerEventData eventData) {
        if (!this.GetComponent<Button>().interactable) return;
        switch (this.gameObject.name) {
            case "PlayButton": HomeUIManager.instance.OnPlay(); break;
            case "StoryButton": HomeUIManager.instance.OnStory(); break;
            case "PowerUpsButton": HomeUIManager.instance.OnPowerUps(); break;
            case "ScoresButton": HomeUIManager.instance.OnScores(); break;
        }
    }
    public void OnPointerEnter(PointerEventData eventData) {
        if (!this.GetComponent<Button>().interactable) return;
        OnButtonEnter();
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (!this.GetComponent<Button>().interactable) return;
        OnButtonExit();
    }
    private void OnButtonEnter() => this.transform.parent.localRotation = Quaternion.Euler(0, 0, 0); // Rotation back to 0
    private void OnButtonExit() => this.transform.parent.localRotation = Quaternion.Euler(0, 0, -1); // Rotation to -1
}

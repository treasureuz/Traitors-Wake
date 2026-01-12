using UnityEngine;
using UnityEngine.EventSystems;

public class HomeButtonsBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

    public void OnPointerClick(PointerEventData eventData) {
        switch (this.gameObject.name) {
            case "PlayButton": HomeUIManager.instance.OnPlay(); break;
            case "StoryButton": HomeUIManager.instance.OnStory(); break;
            case "PowerUpsButton": HomeUIManager.instance.OnPowerUps(); break;
        }
    }
    public void OnPointerEnter(PointerEventData eventData) => OnButtonEnter();
    public void OnPointerExit(PointerEventData eventData) => OnButtonExit();
    
    private void OnButtonEnter() => this.transform.parent.localRotation = Quaternion.Euler(0, 0, 0); // Rotation back to 0
    private void OnButtonExit() => this.transform.parent.localRotation = Quaternion.Euler(0, 0, -1); // Rotation to -1
}

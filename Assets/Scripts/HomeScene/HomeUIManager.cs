using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeUIManager : MonoBehaviour {
    [SerializeField] private Button _playButton;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _controlsText;
    [SerializeField] private float _typewriterDelay = 0.55f;

    private const string titleText = "TRAITOR'S WAKE";
    private const string controlsText = "W(up), A(left), S(down), D(right) — (Arrow Keys)";
    private const int gameSceneIndex = 1;
    
    void Start() {
        this._titleText.text = "";
        StartCoroutine(StartTitleAnimation());
        this._controlsText.text = "<u>Movement Controls</u>: " + controlsText; // Set controls text
    }
    
    private IEnumerator StartTitleAnimation() {
        var index = 0;
        while (index < titleText.Length) {
            yield return new WaitForSeconds(this._typewriterDelay);
            this._titleText.text += titleText[index++];
        }
        this._titleText.text = titleText;
    }
    
    // The title text is already set, but the characters start invisible and gradually become visible.
    // *Based on length and typewriterDelay*
    // private IEnumerator StartTitleAnimation() {
    //     this._title.text = titleText;
    //     this._title.maxVisibleCharacters = 0;
    //
    //     var elapsed = 0f;
    //     var duration = titleText.Length * this._typewriterDelay;
    //
    //     while (elapsed < duration) {
    //         elapsed += Time.deltaTime;
    //         this._title.maxVisibleCharacters = Mathf.FloorToInt(Mathf.Lerp(0, titleText.Length, elapsed / duration));
    //         yield return null;
    //     }
    //     this._title.maxVisibleCharacters = titleText.Length;
    // }

    public void OnPlay() {
        EventSystem.current.SetSelectedGameObject(null); // removes "selectedButtonColor"
        SceneManager.LoadScene(gameSceneIndex);
    }
    public void OnPlayEnter() => this._playButton.transform.parent.Rotate(0, 0, 1); // Rotation back to 0
    public void OnPlayExit() => this._playButton.transform.parent.Rotate(0, 0, -1); // Rotation to -1
    
}

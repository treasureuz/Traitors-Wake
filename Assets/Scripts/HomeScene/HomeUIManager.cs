using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeUIManager : MonoBehaviour {
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _storyButton;
    [SerializeField] private Button _powerUpsButton;
    [SerializeField] private List<GameObject> _homePanels;
    [SerializeField] private GameObject _storyUI;
    [SerializeField] private GameObject _powerUpsUI;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _controlsText;
    [SerializeField] private float _typewriterDelay = 0.55f;
    
    private const string titleText = "TRAITOR'S WAKE";
    private const string controlsText = "W, A, S, D (*Arrow keys apply*)";
    private const string gameSceneName = "GameScene";
    
    void Start() {
        this._titleText.text = "";
        StartCoroutine(StartTitleAnimation());
        this._homePanels = new List<GameObject> { this._storyUI, this._powerUpsUI };
        this._controlsText.text = "<u>Controls</u>: " + controlsText; // Set controls text
    }
    
    private IEnumerator StartTitleAnimation() {
        var index = 0;
        while (index < titleText.Length) {
            yield return new WaitForSeconds(this._typewriterDelay);
            this._titleText.text += titleText[index++];
        }
        this._titleText.text = titleText;
    }

    public void OnPlay() {
        EventSystem.current.SetSelectedGameObject(null); // Removes "selectedButtonColor"
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnStory() {
        EventSystem.current.SetSelectedGameObject(null); // Removes "selectedButtonColor"
        DisableAllPanelsExcept(this._storyUI); // Shows storyUI
    }
    
    public void OnPowerUps() {
        EventSystem.current.SetSelectedGameObject(null); // Removes "selectedButtonColor"
        DisableAllPanelsExcept(this._powerUpsUI); // Shows powerUpsUI
    }

    private void DisableAllPanelsExcept(GameObject objToShow) {
        foreach (GameObject obj in this._homePanels) {
            if (obj == objToShow) objToShow.SetActive(true); 
            else obj.SetActive(false);
        }
    }
    
    public void OnPlayEnter() => this._playButton.transform.parent.Rotate(0, 0, 1); // Rotation back to 0
    public void OnPlayExit() => this._playButton.transform.parent.Rotate(0, 0, -1); // Rotation to -1
    public void OnStoryEnter() => this._storyButton.transform.parent.Rotate(0, 0, 1); // Rotation back to 0
    public void OnStoryExit() => this._storyButton.transform.parent.Rotate(0, 0, -1); // Rotation to -1
    public void OnPowerUpEnter() => this._powerUpsButton.transform.parent.Rotate(0, 0, 1); // Rotation back to 0
    public void OnPowerUpExit() => this._powerUpsButton.transform.parent.Rotate(0, 0, -1); // Rotation to -1
    
}

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DiffSelectUIManager : MonoBehaviour {
    [SerializeField] private Button _easyGoButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private TextMeshProUGUI _levelsText;

    private const string levelsText = "Levels";

    void Start() {
        this._easyGoButton.onClick.AddListener(() => OnGo(GameManager.Difficulty.Easy));
        DisplayLevelText();
    }

    private void OnGo(GameManager.Difficulty difficulty) {
        EventSystem.current.SetSelectedGameObject(null); // Removes "selectedButtonColor"
        GameManager.instance.SetDifficulty(difficulty);
        SceneManager.LoadScene("GameScene");
    }

    public void OnBack() {
        EventSystem.current.SetSelectedGameObject(null); // Removes "selectedButtonColor"
        SceneManager.LoadScene("HomeScene");
    }
    
    private void DisplayLevelText() {
        this._levelsText.text =
            $"{levelsText}: {LevelManager.instance.GetCurrentLevelByDiff() - 1}/{LevelManager.instance.GetMaxLevelByDiff()}";
    }
}

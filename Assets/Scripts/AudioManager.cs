using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour {
    [SerializeField] private AudioSource _gameSource;
    [SerializeField] private AudioSource _mainSource;

    public static AudioManager instance;

    void Awake() {
        if (instance != null) {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        // If scene is GameScene, play game bg noise
        if (scene.name == "GameScene") this._gameSource.Play();
        else this._gameSource.Stop();
    }

    public void PlayClip(AudioClip clip) {
        this._mainSource.PlayOneShot(clip);
    }

    public void PlayRandomClip(AudioClip[] clips) {
        var randomClipIndex = Random.Range(0, clips.Length);
        this._mainSource.PlayOneShot(clips[randomClipIndex]);
    }

    public void PlayRandomClip(AudioSource source, AudioClip[] clips) {
        var randomClipIndex = Random.Range(0, clips.Length);
        source.PlayOneShot(clips[randomClipIndex]);
    }
}
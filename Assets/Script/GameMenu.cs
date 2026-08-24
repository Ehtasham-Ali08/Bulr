using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    [Header("Scenes")]
    public string gameSceneName = "MainScene";

    [Header("Menu")]
    public GameObject continueButton;

    public AudioSource bgMusic;
    private void Start()
    {
        // Make sure the game is not paused
        Time.timeScale = 1f;
        //audio player
        bgMusic.Play();

        // Continue is hidden when entering the menu normally
        continueButton.SetActive(false);
    }

    // =========================
    // START NEW GAME
    // =========================

    public void StartGame()
    {
        Time.timeScale = 1f;

        Debug.Log("Starting New Game...");

        bgMusic.Stop();

        SceneManager.LoadScene(gameSceneName);
    }

    // =========================
    // CONTINUE
    // =========================

    public void ContinueGame()
    {
        Time.timeScale = 1f;

        Debug.Log("Continuing Game...");

        SceneManager.LoadScene(gameSceneName);
    }

    // =========================
    // EXIT GAME
    // =========================

    public void ExitGame()
    {
        Debug.Log("Exiting Game...");

        bgMusic.Stop();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
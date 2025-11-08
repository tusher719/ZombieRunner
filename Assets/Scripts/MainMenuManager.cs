using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject settingsPanel;

    void Start()
    {
        // Make sure settings panel is hidden at start
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    // Called by Play button
    public void PlayGame()
    {
        SceneManager.LoadScene("Sandbox");
    }

    // Called by Settings button
    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    // Called by Back button in Settings
    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    // Called by Exit button
    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}

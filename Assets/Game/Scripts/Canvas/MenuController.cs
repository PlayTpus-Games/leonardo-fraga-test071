using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private IntData _highscore;
    [SerializeField] private IntData _level;
    
    public void Restart()
    {
        LevelManager.instance.RestartLevel();
    }

    public void NewGame()
    {
        _highscore.SetValue(0);
        _level.SetValue(0);
        SaveLoadController.instance.Save();
        SceneManager.LoadScene(0);
        Restart();
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        return;
        #endif
#pragma warning disable CS0162 // Unreachable code detected
        Application.Quit();
#pragma warning restore CS0162 // Unreachable code detected
    }
}

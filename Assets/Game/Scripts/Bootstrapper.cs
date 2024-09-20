using UnityEngine;
using UnityEngine.SceneManagement;

public static class PerformBoostrap
{
    private const string SCENE_NAME = "Bootstrapper";
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute()
    {
        for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
        {
            if (SceneManager.GetSceneAt(sceneIndex).name == SCENE_NAME)
                return;
        }
        
        SceneManager.LoadScene(SCENE_NAME, LoadSceneMode.Additive);
    }
}

public class Bootstrapper : MonoBehaviour
{
    public static Bootstrapper Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
    }
}

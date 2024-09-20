using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    private int _sceneIndex;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _sceneIndex = 1;
        Load(_sceneIndex);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            StartCoroutine(ReloadLevelScene());
        }
    }

    private IEnumerator ReloadLevelScene()
    {
        GameplayInitializer.Instance.Unload();
        yield return null;
        yield return new WaitForEndOfFrame();
        
        AsyncOperation operation = SceneManager.UnloadSceneAsync(_sceneIndex);
        yield return new WaitUntil(() => operation.isDone);
        yield return null;
        yield return new WaitForEndOfFrame();
        
        Load(_sceneIndex);
    }

    private void Load(int sceneIndex)
    {
        if (!SceneManager.GetSceneByBuildIndex(sceneIndex).isLoaded)
            SceneManager.LoadScene(sceneIndex, LoadSceneMode.Additive);
    }
}

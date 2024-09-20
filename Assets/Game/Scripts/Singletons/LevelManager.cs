using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] private IntData _levelData;
    [SerializeField] private IntData _highScoreData;
    
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
#if UNITY_EDITOR
        int sceneCount = SceneManager.sceneCount;
        for (int sceneIndex = 0; sceneIndex < sceneCount; sceneIndex++)
        {
            Scene scene = SceneManager.GetSceneAt(sceneIndex);
            if (scene.isLoaded && scene.name.Contains("Level_"))
                _sceneIndex = scene.buildIndex;
        }

        if (_sceneIndex <= 0)
            _sceneIndex = _levelData.Value;
#else
        _sceneIndex = _levelData.Value;
#endif
        
        Load(_sceneIndex);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
            ReloadLevel();
    }
    
    public void ReloadLevel() => StartCoroutine(ReloadLevelScene());
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

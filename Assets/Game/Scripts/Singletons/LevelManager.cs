using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] private GameEventData _onVictoryEvent;
    [SerializeField] private IntData _levelData;
    [SerializeField] private IntData _highScoreData;
    
    private int _sceneIndex;

    private const int FIRST_LEVEL_SCENE_INDEX = 1;
    
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
        _onVictoryEvent.Subscribe(IncreaseLevel);
        
#if UNITY_EDITOR
        _sceneIndex = GetCurrentLoadedLevel();
        if (_sceneIndex <= 0)
            _sceneIndex = _levelData.Value;
#else
        _sceneIndex = _levelData.Value;
#endif
        
        Load(_sceneIndex);
    }

    private void OnDestroy() => _onVictoryEvent.Unsubscribe(IncreaseLevel);

    private int GetCurrentLoadedLevel()
    {
        int sceneCount = SceneManager.sceneCount;
        for (int sceneIndex = 0; sceneIndex < sceneCount; sceneIndex++)
        {
            Scene scene = SceneManager.GetSceneAt(sceneIndex);
            if (scene.isLoaded && scene.name.Contains("Level_"))
                return scene.buildIndex;
        }

        return -1;
    }
    
    private void IncreaseLevel()
    {
        int currentSceneIndex = GetCurrentLoadedLevel();
        if (currentSceneIndex >= _sceneIndex)
        {
            _sceneIndex++;
            if (_sceneIndex >= SceneManager.sceneCountInBuildSettings)
                _sceneIndex = FIRST_LEVEL_SCENE_INDEX;
        }

        _levelData.SetValue(_sceneIndex);
        SaveLoadManager.Instance.Save();

        StartCoroutine(LoadScene(currentSceneIndex, _sceneIndex));
    }
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
            ReloadLevel();
    }
    
    public void ReloadLevel() => StartCoroutine(LoadScene(_sceneIndex, _sceneIndex));
    private IEnumerator LoadScene(int currentSceneIndex, int newSceneIndex)
    {
        GameplayInitializer.Instance.Unload();
        
        yield return null;
        yield return new WaitForEndOfFrame();
        
        AsyncOperation operationLoad = SceneManager.LoadSceneAsync(newSceneIndex, LoadSceneMode.Additive);
        operationLoad.allowSceneActivation = false;
        
        SceneManager.UnloadSceneAsync(currentSceneIndex);
        operationLoad.allowSceneActivation = true;
    }

    private void Load(int sceneIndex)
    {
        if (!SceneManager.GetSceneByBuildIndex(sceneIndex).isLoaded)
            SceneManager.LoadScene(sceneIndex, LoadSceneMode.Additive);
    }
}

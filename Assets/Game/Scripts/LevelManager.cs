using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    
    [SerializeField] private IntData _currentLevel;
    [SerializeField] private LevelData[] _levelSequence;

    private int levelIndex;
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        CardMatchingController.instance.Subscribe_OnVictory(NextLevel);
    }

    private void OnDisable()
    {
        CardMatchingController.instance.Unsubscribe_OnVictory(NextLevel);
    }

    private void NextLevel()
    {
        levelIndex++;
        if (levelIndex >= _levelSequence.Length)
            levelIndex = 0;
        
        _currentLevel.SetValue(levelIndex);
        CardGrid.instance.SetNewLevel(_levelSequence[levelIndex]);
    }

    public void RestartLevel()
    {
        levelIndex = SaveLoadController.instance.Level;
        CardGrid.instance.SetNewLevel(_levelSequence[levelIndex]);
        ScoreManager.instance.SetValues();
    }
    
    private void Start()
    {
        levelIndex = SaveLoadController.instance.Level;
        _currentLevel.SetValue(levelIndex);
        CardGrid.instance.SetNewLevel(_levelSequence[levelIndex]);
    }

    public void NewGame()
    {
        levelIndex = 0;
        CardGrid.instance.SetNewLevel(_levelSequence[levelIndex]);
    }
}

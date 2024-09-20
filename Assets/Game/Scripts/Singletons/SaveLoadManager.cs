using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance;

    [SerializeField] private IntData _levelData;
    [SerializeField] private IntData _highScoreData;

    private static int _level;
    private static int _highScore;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoLoad()
    {
        _level = PlayerPrefs.GetInt(SaveLoadParameters.LEVEL, 1);
        _highScore = PlayerPrefs.GetInt(SaveLoadParameters.HIGHSCORE, 0);
    }
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
        
        _levelData.SetValue(_level);
        _highScoreData.SetValue(_highScore);
    }
    
    public void Save()
    {
        PlayerPrefs.SetInt(SaveLoadParameters.LEVEL, _levelData.Value);
        PlayerPrefs.SetInt(SaveLoadParameters.HIGHSCORE, _highScoreData.Value);
    }
    
    public void NewSave()
    {
        _levelData.SetValue(1);
        _highScoreData.SetValue(0);
        
        PlayerPrefs.SetInt(SaveLoadParameters.LEVEL, _levelData.Value);
        PlayerPrefs.SetInt(SaveLoadParameters.HIGHSCORE, _highScoreData.Value);
    }
}

public static class SaveLoadParameters
{
    public static readonly string LEVEL = "lv";
    public static readonly string HIGHSCORE = "hs";
}
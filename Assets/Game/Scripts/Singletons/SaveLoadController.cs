using UnityEngine;

public class SaveLoadController : MonoBehaviour
{
    public static SaveLoadController instance;

    [SerializeField] private IntData _levelData;
    [SerializeField] private IntData _highscoreData;
    
    private int lastSavedIndex;
    private int highscore;

    public int Level => _levelData.value;
    public int Highscore => _highscoreData.value;
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
        
        lastSavedIndex = PlayerPrefs.GetInt(SaveLoadParameters.LAST_LEVEL_PLAYED, 0);
        highscore = PlayerPrefs.GetInt(SaveLoadParameters.HIGHSCORE, 0);
    }

    public void Save()
    {
        PlayerPrefs.SetInt(SaveLoadParameters.LAST_LEVEL_PLAYED, _levelData.value);
        PlayerPrefs.SetInt(SaveLoadParameters.HIGHSCORE, _highscoreData.value);
        lastSavedIndex = _levelData.value;
        highscore = _highscoreData.value;
    }
}

public static class SaveLoadParameters
{
    public const string LAST_LEVEL_PLAYED = "llp";
    public const string HIGHSCORE = "hs";
}
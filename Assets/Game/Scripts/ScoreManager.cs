using System.Collections;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private static ScoreManager instance;
    
    [SerializeField] private IntData _highScore;
    [SerializeField] private IntData _score;
    [SerializeField] private IntData _combo;
    [Space(15)]
    [SerializeField] private int _pointsPerMatch;
    [Tooltip("If the multiplier is 0.2 and the combo is at 2, the score will be multiplied by 1.2. If combo is 3, by 1.4")]
    [SerializeField] private float _multiplierIncreasePerCombo;

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
        CardMatchingController.instance.Subscribe_OnMatch(Match);
        CardMatchingController.instance.Subscribe_OnMismatch(Mismatch);
        CardMatchingController.instance.Subscribe_OnVictory(Victory);
    }
    private void OnDisable()
    {
        CardMatchingController.instance.Unsubscribe_OnMatch(Match);
        CardMatchingController.instance.Unsubscribe_OnMismatch(Mismatch);
        CardMatchingController.instance.Unsubscribe_OnVictory(Victory);
    }
    
    private void Start() => SetValues();
    private void SetValues()
    {
        _combo.SetValue(0);
        _score.SetValue(0);
        _highScore.SetValue(SaveLoadController.instance.Highscore);
    }
    private void Match()
    {
        _combo.AddValue(1);
        if (_combo.value < 2)
            _score.AddValue(_pointsPerMatch);
        else
        {
            float multiplier = _multiplierIncreasePerCombo * (_combo.value - 1) + 1;
            _score.AddValue((int)(_pointsPerMatch * multiplier));
        }
    }
    private void Mismatch() => _combo.SetValue(0);
    private void Victory()
    {
        _highScore.AddValue(_score.value);
        _combo.SetValue(0);
        _score.SetValue(0);
    }
}

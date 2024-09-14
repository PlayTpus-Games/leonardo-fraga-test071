using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    private static ScoreDisplay instance;
    
    [SerializeField] private IntData _highscoreData;
    [SerializeField] private IntData _scoreData;
    [SerializeField] private IntData _comboData;
    [SerializeField] private IntData _levelData;
    [Space(15)]
    [SerializeField] private TextMeshProUGUI _level;
    [SerializeField] private TextMeshProUGUI _highscore;
    [SerializeField] private TextMeshProUGUI _score;
    [SerializeField] private TextMeshProUGUI _combo;
    [Space(15), Header("Pop Animation")]
    [SerializeField] private float _duration;
    [SerializeField] private float _scaleMultiplier;
    [SerializeField] private AnimationCurve _curve;
    [Tooltip("Will update the text value when the animation reaches this value. 0 = at the beginning, 1 = at the end.")]
    [SerializeField, Range(0f, 1f)] private float _changeTextAt;

    private Coroutine _coCombo;
    private Coroutine _coScore;

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
        _scoreData.Subscribe_OnValueChanged(UpdateScore);
        _comboData.Subscribe_OnValueChanged(UpdateCombo);   
        _highscoreData.Subscribe_OnValueChanged(UpdateHighscore);   
        _levelData.Subscribe_OnValueChanged(UpdateLevel);
    }

    private void OnDisable()
    {
        _scoreData.Unsubscribe_OnValueChanged(UpdateScore);
        _comboData.Unsubscribe_OnValueChanged(UpdateCombo);
        _highscoreData.Unsubscribe_OnValueChanged(UpdateHighscore);   
        _levelData.Unsubscribe_OnValueChanged(UpdateLevel);
    }

    private void UpdateCombo(int combo)
    {
        if (_coCombo != null)
            StopCoroutine(_coCombo);
        
        _coCombo = StartCoroutine(PopUpAnimation(_combo, combo, _changeTextAt, _duration, _scaleMultiplier, _curve));
    }

    private void UpdateScore(int score)
    {
        if (_coScore != null)
            StopCoroutine(_coScore);
        
        _coScore = StartCoroutine(PopUpAnimation(_score, score, _changeTextAt, _duration, _scaleMultiplier, _curve));
    }

    private void UpdateHighscore(int highscore)
    {
        StartCoroutine(PopUpAnimation(_highscore, highscore, _changeTextAt, _duration, _scaleMultiplier, _curve));
    }
    
    private void UpdateLevel(int level)
    {
        StartCoroutine(PopUpAnimation(_level, level+1, _changeTextAt, _duration, _scaleMultiplier, _curve));
    }
    
    private IEnumerator PopUpAnimation(TextMeshProUGUI text, int newValue, float changeTextAt, float duration, float scaleMultiplier, AnimationCurve curve)
    {
        float elapsedTime = 0f;
        Transform trans = text.transform;
        trans.localScale = Vector3.one;
        
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            if (t >= changeTextAt)
                text.SetText($"{newValue}");

            trans.localScale = Vector3.Lerp(Vector3.one, Vector3.one * scaleMultiplier, curve.Evaluate(t));
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        trans.localScale = Vector3.one;
    }
}

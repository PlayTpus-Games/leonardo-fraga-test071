using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private IntData _highScore;
    [SerializeField] private IntData _score;
    [SerializeField] private IntData _combo;
    [Space(15)]
    [SerializeField] private int _pointsPerMatch;
    [Tooltip("If the multiplier is 0.2 and the combo is at 2, the score will be multiplied by 1.2. If combo is 3, by 1.4")]
    [SerializeField] private float _multiplierIncreasePerCombo;
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += NewLevelLoaded;
        SceneManager.sceneUnloaded += LevelUnloaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= NewLevelLoaded;
        SceneManager.sceneUnloaded -= LevelUnloaded;
    }

    private void Start() => StartCoroutine(DelayedStart());

    private void NewLevelLoaded(Scene scene, LoadSceneMode loadMode) => StartCoroutine(DelayedStart());
    private IEnumerator DelayedStart()
    {
        yield return null;
        CardMatchingController.instance.Subscribe_OnMatch(Match);
        CardMatchingController.instance.Subscribe_OnMismatch(Mismatch);
        CardMatchingController.instance.Subscribe_OnVictory(Victory);

        _combo.SetValue(0);
        _score.SetValue(0);
    }
    private void LevelUnloaded(Scene scene)
    {
        CardMatchingController.instance.Unsubscribe_OnMatch(Match);
        CardMatchingController.instance.Unsubscribe_OnMismatch(Mismatch);
        CardMatchingController.instance.Unsubscribe_OnVictory(Victory);
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
    }
}

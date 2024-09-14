using UnityEngine;

[CreateAssetMenu(fileName = "card-flip-data", menuName = "Game/Card Flip Data")]
public class CardFlipData : ScriptableObject
{
    [SerializeField] private float _duration;
    [SerializeField] private float _delay;
    [SerializeField] private bool _randomDelay;
    [SerializeField] private Vector2 _delayBetweenXY;
    [SerializeField] private float _scaleMultiplier;
    [SerializeField] private AnimationCurve _upDownCurve;
    [SerializeField] private AnimationCurve _rotateCurve;
    [SerializeField] private AnimationCurve _scaleCurve;
    
    public float Duration => _duration;
    public float Delay => _delay;
    public bool RandomDelay => _randomDelay;
    public Vector2 DelayBetweenXY => _delayBetweenXY;
    public float ScaleMultiplier => _scaleMultiplier;
    public AnimationCurve UpDownCurve => _upDownCurve;
    public AnimationCurve RotateCurve => _rotateCurve;
    public AnimationCurve ScaleCurve => _scaleCurve;
}

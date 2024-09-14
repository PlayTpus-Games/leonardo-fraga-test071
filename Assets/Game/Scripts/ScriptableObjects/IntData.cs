using System;
using UnityEngine;

[CreateAssetMenu(fileName = "int-data", menuName = "Game/ScriptableObject Variables/int")]
public class IntData : ScriptableObject
{
    [SerializeField] private int _value;
    [SerializeField] private int _minValue;
    [SerializeField] private int _maxValue = 9999;

    private Action<int> OnValueChanged;
    public void Subscribe_OnValueChanged(Action<int> action) => OnValueChanged += action;
    public void Unsubscribe_OnValueChanged(Action<int> action) => OnValueChanged -= action;
    
    public int value => _value;

    public void SetValue(int newValue, bool fireEvent = true)
    {
        _value = newValue;
        Clamp();
        
        if (fireEvent)
            OnValueChanged?.Invoke(_value);
    }

    public void AddValue(int newValue, bool fireEvent = true)
    {
        if (newValue == 0)
            return;
        
        _value += newValue;
        Clamp();
        
        if (fireEvent)
            OnValueChanged?.Invoke(_value);
    }

    private void OnValidate() => Clamp();
    private void Clamp() => _value = Mathf.Clamp(_value, _minValue, _maxValue);
}

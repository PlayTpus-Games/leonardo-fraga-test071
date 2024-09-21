using System;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "game-event-default", menuName = "Game/Game Event Data")]
public class GameEventData : ScriptableObject
{
    #pragma warning disable
    [ShowInInspector, ReadOnly, TextArea(2, 4)]
    private readonly string warning = "Best used with a single raise point to prevent problems while debugging.";
    #pragma warning restore
    
    private Action OnEvent;

    public void Subscribe(Action action) => OnEvent += action;
    public void Unsubscribe(Action action) => OnEvent -= action;

    /// <summary>
    /// Raises the event. The 'name' must be passed as parameter for debug tracking purposes only.
    /// </summary>
    /// <param name="caller">Name of the caller script. Used for debug tracking purposes only.</param>
    public void Raise(string caller) => OnEvent?.Invoke();
}

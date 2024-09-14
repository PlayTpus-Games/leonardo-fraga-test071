using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlipCardsAtBeginning : MonoBehaviour
{
    [SerializeField] private float _initialDelay;
    [SerializeField] private float _intervalBetweenCards;

    private CardFlipper _cardFlipper;
    private CardSpawner _cardSpawner;
    private Card[] cards;
    
    private Action OnAllCardsFlipped;
    private Action OnCardsUnflipped;
    public void Subscribe_OnAllCardsFlipper(Action action) => OnAllCardsFlipped += action;
    public void Unsubscribe_OnAllCardsFlipper(Action action) => OnAllCardsFlipped -= action;

    public void Subscribe_OnCardsUnflipped(Action action) => OnCardsUnflipped += action;
    public void Unsubscribe_OnCardsUnflipped(Action action) => OnCardsUnflipped -= action;
    
    private void Awake()
    {
        _cardFlipper = GetComponent<CardFlipper>();
        _cardSpawner = GetComponent<CardSpawner>();
    }

    private void OnEnable() => SceneManager.sceneUnloaded += StopCoroutines;
    private void OnDisable() => SceneManager.sceneUnloaded -= StopCoroutines;
    private void StopCoroutines(Scene arg0) => StopAllCoroutines();
    
    public void FlipCards()
    {
        OnCardsUnflipped?.Invoke();
        cards = _cardSpawner.Cards.ToArray();
        StartCoroutine(FlipCardsCoroutine());
        StartCoroutine(RaiseOnAllCardsFlipped());
    }

    private IEnumerator FlipCardsCoroutine()
    {
        yield return new WaitForSeconds(_initialDelay);
        foreach (Card card in cards)
        {
            _cardFlipper.FlipCard(card, CardFlipper.FlipType.GameStart);
            SoundManager.instance.Play_CardFlip(0.5f, true);
            
            if (_intervalBetweenCards > 0f)
                yield return new WaitForSeconds(_intervalBetweenCards);
        }
    }

    private IEnumerator RaiseOnAllCardsFlipped()
    {
        yield return new WaitForSeconds(_initialDelay + (cards.Length + 1) * _intervalBetweenCards + 0.5f);
        OnAllCardsFlipped?.Invoke();
    }
}

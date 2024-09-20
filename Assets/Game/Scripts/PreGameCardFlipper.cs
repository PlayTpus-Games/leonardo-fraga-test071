using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CardFlipper))]
public class PreGameCardFlipper : MonoBehaviour, IUnloadable
{
    [SerializeField] private float _hideCardsAfterDelay;
    [SerializeField] private float _intervalBetweenCards;
    
    private CardFlipper _flipper;
    private HashSet<int> _remainingCards;

    public Action<HashSet<int>> OnCardsInitialized;

    private void Awake() => _flipper = GetComponent<CardFlipper>();
    
    public void InitializeGameplay(CardHolder cardHolder)
    {
        CreateRemainingCardsHashSet(cardHolder);
        StartCoroutine(HideCardsAndRaiseEvent(cardHolder));
    }
    private void CreateRemainingCardsHashSet(CardHolder cardHolder)
    {
        _remainingCards = new HashSet<int>(cardHolder.Cards.Count / 2);
        foreach (Card card in cardHolder.Cards)
            _remainingCards.Add(card.Index);
    }
    private IEnumerator HideCardsAndRaiseEvent(CardHolder cardHolder)
    {
        yield return HideCards(cardHolder);
        yield return new WaitForSeconds(0.5f);
        OnCardsInitialized?.Invoke(_remainingCards);
    }
    private IEnumerator HideCards(CardHolder cardHolder)
    {
        yield return new WaitForSeconds(_hideCardsAfterDelay);
        foreach (Card card in cardHolder.Cards)
        {
            _flipper.FlipCard(card, CardFlipper.FlipType.InitialFlip);
            SoundManager.instance.Play_CardFlip(0.5f, true);
            
            if (_intervalBetweenCards > 0f)
                yield return new WaitForSeconds(_intervalBetweenCards);
        }
    }

    public void Unload()
    {
        StopAllCoroutines();
        _remainingCards?.Clear();
    }
}

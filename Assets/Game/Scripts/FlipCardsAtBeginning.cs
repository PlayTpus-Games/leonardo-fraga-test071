using System.Collections;
using UnityEngine;

public class FlipCardsAtBeginning : MonoBehaviour
{
    [SerializeField] private float _initialDelay;
    [SerializeField] private float _intervalBetweenCards;

    private CardFlipper _cardFlipper;
    private SpawnCards _spawnCards;
    private Card[] cards;

    private void Awake()
    {
        _cardFlipper = GetComponent<CardFlipper>();
        _spawnCards = GetComponent<SpawnCards>();
        cards = _spawnCards.Cards.ToArray();
    }

    private void Start() => StartCoroutine(FlipCards());
    private IEnumerator FlipCards()
    {
        yield return new WaitForSeconds(_initialDelay);
        foreach (Card card in cards)
        {
            _cardFlipper.FlipCard(card, CardFlipper.FlipType.GameStart);
            if (_intervalBetweenCards > 0f)
                yield return new WaitForSeconds(_intervalBetweenCards);
        }
    }
}

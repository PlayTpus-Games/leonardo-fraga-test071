using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class CardFlipper : MonoBehaviour, IUnloadable
{
    public enum FlipType { InitialFlip, GameplayFlip }

    [SerializeField] private CardFlipData _initialFlipData;
    [SerializeField] private CardFlipData _gameplayFlipData;

    //private bool _reloading;
    
    private const float EULER_Z_LOOKING_DOWN = -180f;

    public void FlipCard(Card card, FlipType flipType)
    {
        if (card.IsFlipping)
            return;

        Coroutine coroutine = StartCoroutine(FlipCardCoroutine(card, flipType));
        bool hiding = Math.Abs(card.transform.eulerAngles.z) < 0.1f;
        card.SetIsFlipping(coroutine, hiding);
        SoundManager.instance.Play_CardFlip();
    }
    private IEnumerator FlipCardCoroutine(Card card, FlipType flipType)
    {
        //if (!card)
        //    Debug.Log("");
        
        Transform cardTransform = card.transform;
        CardFlipData data = flipType is FlipType.InitialFlip ? _initialFlipData : _gameplayFlipData;

        float delay = data.RandomDelay ? Random.Range(data.DelayBetweenXY.x, data.DelayBetweenXY.y) : data.Delay;
        if (delay > 0f)
            yield return new WaitForSeconds(delay);
        
        float targetY = cardTransform.position.y + (cardTransform.lossyScale.x * 0.5f) + 0.2f;
        
        Vector3 initialPos = cardTransform.position;
        Vector3 finalPos = initialPos + new Vector3(0f, targetY, 0f);
        
        Quaternion initialRot = cardTransform.rotation;
        Quaternion finalRot = Quaternion.Euler(0f, 0f, initialRot.eulerAngles.z == 0f ? EULER_Z_LOOKING_DOWN : 0f);

        Vector3 initialScale = cardTransform.localScale;
        Vector3 multipliedScale = new Vector3(initialScale.x * data.ScaleMultiplier, initialScale.y, initialScale.z * data.ScaleMultiplier);
        
        float elapsedTime = 0;
        while (elapsedTime < data.Duration)// && !_reloading)
        {
            float t = elapsedTime / data.Duration;
            
            cardTransform.position = Vector3.Lerp(initialPos, finalPos, data.UpDownCurve.Evaluate(t));
            cardTransform.rotation = Quaternion.Slerp(initialRot, finalRot, data.RotateCurve.Evaluate(t));
            cardTransform.localScale = Vector3.Lerp(initialScale, multipliedScale, data.ScaleCurve.Evaluate(t));
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cardTransform.position = initialPos;
        cardTransform.rotation = finalRot;
        cardTransform.localScale = initialScale;
    }

    public void Unload()
    {
        //_reloading = true;
        StopAllCoroutines();
        //StartCoroutine(ReloadingForAFrame());
    }

    // private IEnumerator ReloadingForAFrame()
    // {
    //     yield return null;
    //     _reloading = false;
    // }
}

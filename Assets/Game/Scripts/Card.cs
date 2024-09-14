using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider)), ExecuteInEditMode]
public class Card : MonoBehaviour
{
    [SerializeField] private BoxCollider _col;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private int _index;
    public int Index => _index;

    private Quaternion prevRotation;
    private bool _flipping;
    public bool IsFlipping => _flipping;
    
    public void SetCard((Sprite sprite, int index) spriteIndex)
    {
        _spriteRenderer.sprite = spriteIndex.sprite;
        _index = spriteIndex.index;
    }

    public void SetIsFlipping(Coroutine flipCoroutine) => StartCoroutine(SetIsFlippingCoroutine(flipCoroutine));
    private IEnumerator SetIsFlippingCoroutine(Coroutine flipCoroutine)
    {
        _flipping = true;
        yield return flipCoroutine;
        _flipping = false;
    }
    
    private void Update()
    {
        if (Application.isPlaying)
            return;

        if (prevRotation != transform.rotation)
            return;

        float yPos = _col.bounds.extents.y / (transform.localScale.y == 0 ? 0.001f : transform.localScale.y) + 0.01f;
        _spriteRenderer.transform.localPosition = new Vector3(0f, yPos, 0f);
        prevRotation = transform.rotation;
    }
}

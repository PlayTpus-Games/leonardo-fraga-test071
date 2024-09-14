using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider)), ExecuteInEditMode]
public class Card : MonoBehaviour
{
    [SerializeField] private BoxCollider _col;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [Tooltip("Index used for matching cards. Do NOT alter this value")]
    [SerializeField] private int _index;
    public int Index => _index;

    private Quaternion prevRotation;
    private bool _flipping;
    public bool IsFlipping => _flipping;
    private bool _revealed;
    public bool IsRevealed => _revealed;
    
    public void SetCard((Sprite sprite, int index) spriteIndex)
    {
        _spriteRenderer.sprite = spriteIndex.sprite;
        _index = spriteIndex.index;
    }

    public void SetIsFlipping(Coroutine flipCoroutine, bool hiding) => StartCoroutine(SetIsFlippingCoroutine(flipCoroutine, hiding));
    private IEnumerator SetIsFlippingCoroutine(Coroutine flipCoroutine, bool hiding)
    {
        if (!hiding)
            _revealed = true;
        
        _flipping = true;
        yield return flipCoroutine;
        _flipping = false;

        if (hiding)
            _revealed = false;
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

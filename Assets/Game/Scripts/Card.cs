using UnityEngine;

[RequireComponent(typeof(BoxCollider)), ExecuteInEditMode]
public class Card : MonoBehaviour
{
    [SerializeField] private BoxCollider _col;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private int _index;
    public int Index => _index;
    
    public void SetCard((Sprite sprite, int index) spriteIndex)
    {
        _spriteRenderer.sprite = spriteIndex.sprite;
        _index = spriteIndex.index;
    }
    public void SetCard(Sprite sprite, int index)
    {
        _spriteRenderer.sprite = sprite;
        _index = index;
    }
    
    private void Update()
    {
        float yPos = _col.bounds.extents.y / (transform.localScale.y == 0 ? 0.001f : transform.localScale.y) + 0.01f;
        _spriteRenderer.transform.localPosition = new Vector3(0f, yPos, 0f);
    }
}

using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Card : MonoBehaviour
{
    private BoxCollider col;
    private SpriteRenderer spriteRenderer;
    private bool _autoScale;
    
    private void Awake()
    {
        Init();
    }

    private bool Init()
    {
        col ??= GetComponent<BoxCollider>();
        spriteRenderer ??= GetComponentInChildren<SpriteRenderer>();
        return true;
    }

    private void ScaleSpriteToCard()
    {
        Bounds bounds = col.bounds;
        spriteRenderer.transform.localScale = new Vector3(bounds.extents.x, bounds.extents.z, 0f) * 0.25f + Vector3.forward;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Init())
            return;

        float yPos = col.bounds.extents.y / (transform.localScale.y == 0 ? 0.001f : transform.localScale.y) + 0.01f;
        spriteRenderer.transform.localPosition = new Vector3(0f, yPos, 0f);
    }
#endif
}

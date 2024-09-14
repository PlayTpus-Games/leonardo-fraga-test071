using UnityEngine;

public class CardSelection : MonoBehaviour
{
    [SerializeField] private LayerMask _cardLayerMask;

    private CardFlipper _flipper;
    private Camera _camera;

    private void Awake()
    {
        _flipper = GetComponent<CardFlipper>();
        _camera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, 30f, _cardLayerMask))
            {
                Card card = hitInfo.transform.GetComponent<Card>();
                _flipper.FlipCard(card, CardFlipper.FlipType.CardSelected);
            }
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class Card : MonoBehaviour
{
    [SerializeField] private string value;

    private static Card activeCard;

    private Collider2D cardCollider;
    private Camera mainCamera;
    private Vector3 originalPosition;
    private Vector3 pointerOffset;
    private bool isDragging;
    private OnDropCard currentDropArea;

    public string Value => value;

    private void Awake()
    {
        originalPosition = transform.position;
        cardCollider = GetComponent<Collider2D>();
        if (cardCollider == null)
        {
            Debug.LogError("Card requires a Collider2D to support dragging.", this);
        }

        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Card dragging requires a camera tagged MainCamera.", this);
        }
    }

    private void Update()
    {
        if (Mouse.current == null || cardCollider == null || mainCamera == null)
        {
            return;
        }

        Vector2 screenPosition = Mouse.current.position.ReadValue();
        Vector3 pointerPosition = GetMousePositionInWorldSpace(screenPosition);
        if (Mouse.current.leftButton.wasPressedThisFrame &&
            activeCard == null &&
            cardCollider.OverlapPoint(pointerPosition))
        {
            if (currentDropArea != null)
            {
                currentDropArea.ClearCard(this);
                currentDropArea = null;
            }

            activeCard = this;
            pointerOffset = transform.position - pointerPosition;
            isDragging = true;
        }

        if (!isDragging)
        {
            return;
        }

        if (Mouse.current.leftButton.isPressed)
        {
            transform.position = pointerPosition + pointerOffset;
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            CompleteDrop(pointerPosition);
        }
    }

    private void OnDisable()
    {
        if (currentDropArea != null)
        {
            currentDropArea.ClearCard(this);
            currentDropArea = null;
        }

        if (activeCard == this)
        {
            activeCard = null;
            isDragging = false;
        }
    }

    private void CompleteDrop(Vector3 pointerPosition)
    {
        isDragging = false;
        activeCard = null;

        Collider2D[] hitColliders = Physics2D.OverlapPointAll(pointerPosition);
        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider == cardCollider)
            {
                continue;
            }

            ICardDropArea cardDropArea = hitCollider.GetComponentInParent<ICardDropArea>();
            if (cardDropArea != null && cardDropArea.OnCardDrop(this))
            {
                return;
            }
        }

        transform.position = originalPosition;
    }

    private Vector3 GetMousePositionInWorldSpace(Vector2 screenPosition)
    {
        Vector3 screenPositionWithDepth = screenPosition;
        screenPositionWithDepth.z = mainCamera.WorldToScreenPoint(transform.position).z;
        return mainCamera.ScreenToWorldPoint(screenPositionWithDepth);
    }

    internal void SetDropArea(OnDropCard dropArea)
    {
        currentDropArea = dropArea;
    }

    internal void ReturnToOriginalPosition()
    {
        if (currentDropArea != null)
        {
            currentDropArea.ClearCard(this);
            currentDropArea = null;
        }

        transform.position = originalPosition;
    }
}

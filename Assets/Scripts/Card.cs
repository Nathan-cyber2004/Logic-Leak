using UnityEngine;
using UnityEngine.InputSystem;

public class Card : MonoBehaviour
{
    [SerializeField] private string value;
    private static Card activeCard;
    private Collider2D cardCollider;
    private Camera mainCamera;
    private Vector3 originalPosition, pointerOffset;
    private bool isDragging;
    private OnDropCard currentDropArea;
    public string Value => value;

    private void Awake()
    {
        originalPosition = transform.position;
        cardCollider = GetComponent<Collider2D>();
        mainCamera = Camera.main;
        if (cardCollider == null) Debug.LogError("Card requires a Collider2D to support dragging.", this);
        if (mainCamera == null) Debug.LogError("Card dragging requires a camera tagged MainCamera.", this);
    }
    private void Update()
    {
        if (Mouse.current == null || cardCollider == null || mainCamera == null) return;
        Vector2 screenPosition = Mouse.current.position.ReadValue();
        Vector3 pointer = GetMousePositionInWorldSpace(screenPosition);
        if (Mouse.current.leftButton.wasPressedThisFrame && activeCard == null && cardCollider.OverlapPoint(pointer))
        {
            if (currentDropArea != null) { currentDropArea.ClearCard(this); currentDropArea = null; }
            activeCard = this;
            pointerOffset = transform.position - pointer;
            isDragging = true;
        }
        if (!isDragging) return;
        if (Mouse.current.leftButton.isPressed) transform.position = pointer + pointerOffset;
        if (Mouse.current.leftButton.wasReleasedThisFrame) CompleteDrop(pointer);
    }
    private void OnDisable()
    {
        if (currentDropArea != null) { currentDropArea.ClearCard(this); currentDropArea = null; }
        if (activeCard == this) { activeCard = null; isDragging = false; }
    }
    private void CompleteDrop(Vector3 pointer)
    {
        isDragging = false;
        activeCard = null;
        foreach (Collider2D hit in Physics2D.OverlapPointAll(pointer))
        {
            if (hit == cardCollider) continue;
            ICardDropArea area = hit.GetComponentInParent<ICardDropArea>();
            if (area != null && area.OnCardDrop(this)) return;
        }
        transform.position = originalPosition;
    }
    private Vector3 GetMousePositionInWorldSpace(Vector2 screenPosition)
    {
        Vector3 point = screenPosition;
        point.z = mainCamera.WorldToScreenPoint(transform.position).z;
        return mainCamera.ScreenToWorldPoint(point);
    }
    internal void SetDropArea(OnDropCard area) { currentDropArea = area; }
    internal void ReturnToOriginalPosition()
    {
        if (currentDropArea != null) { currentDropArea.ClearCard(this); currentDropArea = null; }
        transform.position = originalPosition;
    }
}

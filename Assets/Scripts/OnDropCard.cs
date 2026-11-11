using UnityEngine;

public class OnDropCard : MonoBehaviour, ICardDropArea
{
    [SerializeField] private string requiredValue;
    [SerializeField] private bool acceptAnyCard;

    public string AssignedValue { get; private set; }
    private Card assignedCard;

    public bool OnCardDrop(Card card)
    {
        if (card == null)
        {
            return false;
        }

        if (acceptAnyCard)
        {
            AssignCard(card);
            return true;
        }

        if (string.IsNullOrWhiteSpace(requiredValue))
        {
            Debug.LogWarning("Set the required Python value on this drop area.", this);
            return false;
        }

        if (card.Value != requiredValue)
        {
            return false;
        }

        AssignCard(card);
        return true;
    }

    internal void ClearCard(Card card)
    {
        if (assignedCard != card)
        {
            return;
        }

        assignedCard = null;
        AssignedValue = null;
    }

    private void AssignCard(Card card)
    {
        if (assignedCard != null && assignedCard != card)
        {
            assignedCard.ReturnToOriginalPosition();
        }

        assignedCard = card;
        AssignedValue = card.Value;
        card.SetDropArea(this);
        card.transform.position = transform.position;
    }
}

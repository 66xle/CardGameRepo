namespace events
{
    public class CardDrag : CardEvent
    {
        public CardDrag(CardWrapper card, Card evt = null) : base(card, evt)
        {
        }
    }
}

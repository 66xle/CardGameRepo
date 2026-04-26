namespace events
{
    public class CardClick : CardEvent
    {
        public CardClick(CardWrapper card, Card evt = null) : base(card, evt)
        {
        }
    }
}

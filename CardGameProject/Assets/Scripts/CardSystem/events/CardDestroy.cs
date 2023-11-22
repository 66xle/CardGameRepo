namespace events {
    public class CardDestroy : CardEvent {
        public CardDestroy(CardWrapper card, Card evt = null) : base(card, evt) {
        }
    }
}

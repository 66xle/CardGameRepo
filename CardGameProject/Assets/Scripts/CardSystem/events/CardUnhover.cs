namespace events {
    public class CardUnhover : CardEvent {
        public CardUnhover(CardWrapper card, Card evt = null) : base(card, evt) {
        }
    }
}

namespace events {

    public class CardPlayed : CardEvent {
        public CardPlayed(CardWrapper card, Card evt = null) : base(card, evt) {
        }
    }
}

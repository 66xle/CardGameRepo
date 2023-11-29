namespace events {
    
    public class CardEvent {
        public readonly CardWrapper card;
        public readonly Card cardObj;

        public CardEvent(CardWrapper card, Card cardObj)
        {
            this.card = card;
            this.cardObj = cardObj; 
        }
    }
}

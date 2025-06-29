using System;
using events;
using UnityEngine;
using UnityEngine.Events;

namespace config {
    [Serializable]
    public class EventsConfig {
        [SerializeField]
        public UnityEvent<CardPlayed, Card, string> OnCardPlayed;

        [SerializeField]
        public UnityEvent<CardClick> OnCardClick;

        [SerializeField]
        public UnityEvent<CardDrag> OnCardDrag;

        [SerializeField]
        public UnityEvent<CardHover> OnCardHover;
        
        [SerializeField]
        public UnityEvent<CardUnhover> OnCardUnhover;
        
        [SerializeField]
        public UnityEvent<CardDestroy> OnCardDestroy;
    }
}

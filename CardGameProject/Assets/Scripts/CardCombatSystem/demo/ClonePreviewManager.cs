using System.Collections.Generic;
using System.Linq.Expressions;
using events;
using MyBox;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace demo {
    /**
     * Offers a preview of a card when hovering over it by cloning the original card and placing it on top of the original.
     * Allows setting the global preview position and scale.
     */
    public class ClonePreviewManager : MonoBehaviour, CardPreviewManager {

        [SerializeField]
        [MustBeAssigned] private InputManager InputManager;

        [SerializeField]
        private float verticalPosition;
        
        [SerializeField]
        private float previewScale = 1f;
        
        [SerializeField]
        private int previewSortingOrder = 1;

        private Dictionary<CardWrapper, Transform> previews = new Dictionary<CardWrapper, Transform>();

        [HideInInspector] public CardWrapper currentCard;
        bool IsCardClicked = false;

        private void Update()
        {
            if (InputManager.LeftClickInputDown && !IsCardClicked && currentCard != null)
            {
                CardPreviewEnd();
            }

            if (IsCardClicked)
                IsCardClicked = false;
        }


        public void OnCardClick(CardClick cardClick)
        {
            if (Time.timeScale == 1)
            {
                if (currentCard != null)
                    CardPreviewEnd();

                IsCardClicked = true;

                currentCard = cardClick.card;
                OnCardPreviewStarted(currentCard);

                currentCard.IsPreviewActive = true;
                currentCard.gameObject.GetComponent<CanvasGroup>().alpha = 0f;
            }
        }

        public void OnCardDrag(CardDrag cardDrag)
        {
            CardPreviewEnd();
        }

        private void CardPreviewEnd()
        {
            OnCardPreviewEnded(currentCard);
            currentCard.IsPreviewActive = false;
            currentCard.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
            currentCard = null;
        }

        public void OnCardPreviewStarted(CardWrapper card) {
            if (!previews.ContainsKey(card)) {
                CreateCloneForCard(card);
            }

            var preview = previews[card];
            preview.gameObject.SetActive(true);

            RectTransform previewPos = preview.GetComponent<RectTransform>();
            RectTransform cardPos = card.GetComponent<RectTransform>();
            previewPos.localPosition = new Vector3(previewPos.localPosition.x, verticalPosition, previewPos.localPosition.z);
            previewPos.position = new Vector3(card.transform.position.x, previewPos.transform.position.y, card.transform.position.z);
        }

        private void CreateCloneForCard(CardWrapper card) {
            var clone = Instantiate(card.gameObject, transform);
            clone.transform.position = card.transform.position;
            clone.transform.localScale = Vector3.one * previewScale;
            clone.transform.rotation = Quaternion.identity;
            var cloneCanvas = clone.GetComponent<Canvas>();
            cloneCanvas.sortingOrder = previewSortingOrder;
            StripCloneComponents(clone);
            previews.Add(card, clone.transform);
        }

        private static void StripCloneComponents(GameObject clone) {
            var cloneWrapper = clone.GetComponent<CardWrapper>();
            if (cloneWrapper != null) {
                Destroy(cloneWrapper);
            }

            var cloneRaycaster = clone.GetComponent<GraphicRaycaster>();
            if (cloneRaycaster != null) {
                Destroy(cloneRaycaster);
            }
        }

        public void OnCardPreviewEnded(CardWrapper card) {
            previews[card].gameObject.SetActive(false);
        }
    }
}

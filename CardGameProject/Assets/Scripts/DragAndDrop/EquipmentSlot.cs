using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject card = eventData.pointerDrag;
        DraggableItem draggableItem = card.GetComponent<DraggableItem>();

        if (transform.childCount > 0)
        {
            transform.GetChild(0).SetParent(draggableItem.parentAfterDrag);
        }

        
        draggableItem.parentAfterDrag = transform;
    }
}

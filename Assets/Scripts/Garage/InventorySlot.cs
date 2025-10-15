using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    // Inventory Slot within the Inventory UI

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            // The item that we are currently dragging ONTO ourselves
            DraggableItem draggedItem = eventData.pointerDrag.GetComponent<DraggableItem>();
            if (draggedItem != null) 
            {
                // store parent of dragging item before we try to swap
                DraggableItem draggableComponent = draggedItem.GetComponent<DraggableItem>();
                Transform draggedOriginalParent = draggableComponent.originalParent;

                // check if WE currently have an item ON us
                DraggableItem existingItem = null;
                foreach (Transform child in transform)
                { 
                    DraggableItem childItem = child.GetComponent<DraggableItem>();
                    if (childItem != null)
                    {
                        existingItem = childItem;
                        break;
                    }
                }

                draggedItem.transform.SetParent(transform);
                draggedItem.transform.localPosition = Vector3.zero;

                // if the slot has item, swap them, move to dragged items old slot
                if (existingItem != null)
                {
                    DraggableItem existingDraggable = existingItem.GetComponent<DraggableItem>();
                    existingItem.transform.SetParent(draggedOriginalParent);
                    existingItem.transform.localPosition = Vector3.zero;
                }
            }
        }
    }
}
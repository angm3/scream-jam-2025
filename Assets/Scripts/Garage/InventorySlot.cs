using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    // Inventory Slot within the Inventory UI
    public string slotType = "any"; // "any", "candy", "blueprints", "items", "bike_tire_upgrade", "bike_weapon", "bike_item"
    public string slotId = ""; // if set, only accepts items with this specific id (like "batwing")

    public TMP_Text countText;
    public GameObject displaySprite;

    private int count = 0;

    void UpdateCountText()
    {
        if (countText != null)
        {
            countText.text = $"{count}/1";
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            // The item that we are currently dragging ONTO ourselves
            DraggableItem draggedItem = eventData.pointerDrag.GetComponent<DraggableItem>();
            if (draggedItem != null) 
            {
                // Logic for validating slot types
                CollectibleUIItem draggedUIItem = draggedItem.GetComponent<CollectibleUIItem>();
                if (draggedUIItem == null) return;

                // check based on slot type
                if (slotType != "any" && draggedUIItem.collectibleType != slotType)
                {
                    return;
                }

                // check based on slot id
                if (!string.IsNullOrEmpty(slotId) && draggedUIItem.collectibleId != slotId)
                {
                    return;
                }

                if (displaySprite != null)
                { 
                    // We dragged onto a special inventory slot 
                    draggedItem.gameObject.SetActive(false);
                    displaySprite.SetActive(true);
                    count++;
                    UpdateCountText();
                }

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
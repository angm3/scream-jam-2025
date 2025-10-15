using UnityEngine;
using UnityEngine.EventSystems;

public class StashSlot : MonoBehaviour, IDropHandler
{
    // Stash Slot within the Stash UI

    public void OnDrop(PointerEventData eventData)
    {
        // Check whether the draggable item is able to be dropped into this slot
        if (eventData.pointerDrag != null)
        {
            DraggableItem item = eventData.pointerDrag.GetComponent<DraggableItem>();
            if (item != null && transform.childCount == 0)
            {
                item.transform.SetParent(transform);
                item.transform.localPosition = Vector3.zero;
            }
        }
    }
}
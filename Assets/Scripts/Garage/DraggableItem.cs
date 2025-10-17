using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform originalParent;
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        
        AudioManager.Instance.PlaySFX("drag_start",  0.5f, 0.3f);
        originalParent = transform.parent;
        transform.SetParent(canvas.transform);
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        // re-enable visuals in case they were hidden
        CollectibleUIItem item = GetComponent<CollectibleUIItem>();
        if (item != null)
        {
            if (item.itemSpriteImage != null)
                item.itemSpriteImage.enabled = true;
            if (item.iconImage != null)
                item.iconImage.enabled = true;
            if (item.stackText != null && item.stackCount > 1)
                item.stackText.gameObject.SetActive(true);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySFX("drag_end", 0.5f, 0.3f);
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // If still parented to canvas, no valid drop occurred
        if (transform.parent == canvas.transform)
        {
            transform.SetParent(originalParent);
            transform.localPosition = Vector3.zero;
        }
    }
}
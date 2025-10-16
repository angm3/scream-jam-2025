using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public TMP_Text candyCounterText;

    void Start()
    {
        UpdateCandyCounter();
    }

    private void OnEnable()
    {
        EventBus.Subscribe<InventoryChangedEvent>(OnInventoryChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<InventoryChangedEvent>(OnInventoryChanged);
    }

    void OnInventoryChanged(InventoryChangedEvent e)
    {
        //Debug.Log("Inventory change event fired");
        UpdateCandyCounter();
    }

    public void UpdateCandyCounter()
    {
        //Debug.Log("Updating candy counter UI");
        Inventory inv = GameManager.Instance.currentPlayerInventory;
        if (inv != null && candyCounterText != null)
        {
            candyCounterText.text = $"Candy: {inv.candyCount}";
        }
    }
}
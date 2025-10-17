using TMPro;
using UnityEngine;
using System.Collections;

public class PlayerUI : MonoBehaviour
{
    public TMP_Text candyCounterText;

    void Awake()
    {
        // Ensure this object persists across scenes
        candyCounterText = GameObject.Find("CandyCountText").GetComponent<TMP_Text>();
    }

    void Start()
    {
        //candyCounterText = GameObject.Find("CandyCountText").GetComponent<TMP_Text>();

        GetReferenceToText();
        if (candyCounterText == null)
        {
            Debug.Log("CandyCounter: Could not find CandyCounter text component");
        }
        else
        {
            Debug.Log("CandyCounter: Found CandyCounter text component");
        }

        
        UpdateCandyCounter();

    }

    private void OnEnable()
    {
        EventBus.Subscribe<InventoryChangedEvent>(OnInventoryChanged);
        GetReferenceToText();
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

    public void GetReferenceToText()
    {
        candyCounterText = GameObject.Find("CandyCountText").GetComponent<TMP_Text>();
    }
    

    public void UpdateCandyCounter()
    {
        Debug.Log("Updating candy counter UI");
        Inventory inv = GameManager.Instance.currentPlayerInventory;
        
        GetReferenceToText();
        
        if (inv != null && candyCounterText != null)
        {
            candyCounterText.text = $"Candy: {inv.candyCount}";
        }
    }
}
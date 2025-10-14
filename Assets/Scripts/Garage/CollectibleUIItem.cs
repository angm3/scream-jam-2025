using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectibleUIItem : MonoBehaviour
{
    public string collectibleType; // "candy", "blueprint", "potion_ingredient"
    public string collectibleId;
    public int stackCount = 1;

    public Image iconImage;
    public TMP_Text stackText;

    public void SetData(string type, string id, int count = 1)
    {
        collectibleType = type;
        collectibleId = id;
        stackCount = count;

        if (stackText != null)
        {
            stackText.gameObject.SetActive(count > 1);
            stackText.text = count.ToString();
        }
    }
}
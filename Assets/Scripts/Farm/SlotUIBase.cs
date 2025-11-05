using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotUIBase : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI quantityText;
    [HideInInspector] public int slotIndex; // index trong vùng của nó (inventory/hotbar)

    public void Show(ItemStack s)
    {
        if (s.IsEmpty)
        {
            icon.enabled = false;
            quantityText.text = "";
            return;
        }
        icon.enabled = true;
        icon.sprite = s.item.icon;
        icon.preserveAspect = true;
        quantityText.text = s.quantity > 1 ? s.quantity.ToString() : "";
    }
}

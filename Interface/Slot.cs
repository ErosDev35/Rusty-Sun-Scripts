using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Slot : MonoBehaviour
{
    public Item slotItem;
    public string item_type = "any";
    public GameObject itemNumberDisplay;
    public Slot slotToSync = null;
    public void OnClick()
    {
        GameObject.Find("Bag").GetComponent<Inventory>().SlotClick(GetComponent<Slot>());
    }
    public void OnHover()
    {
         GameObject.Find("Bag").GetComponent<Inventory>().hoveredSlot = this.transform.GetComponent<Slot>();
    }
    public void OnHoverExit()
    {
        GameObject bag = GameObject.Find("Bag");
        if(bag == null || bag.GetComponent<Inventory>() == null) return;
        else bag.GetComponent<Inventory>().hoveredSlot = null;
    }
    void Update()
    {
        if(transform.childCount > 0)
        this.transform.GetChild(0).GetComponent<Image>().sprite = (slotItem != null)? slotItem.inventoryImage : null;

        if(itemNumberDisplay != null)
        {
           itemNumberDisplay.SetActive(slotItem != null && slotItem.itemNumber >= 2);
           itemNumberDisplay.GetComponent<TextMeshProUGUI>().text = (slotItem != null && slotItem.itemNumber >= 2)? slotItem.itemNumber.ToString() : "";
        } 
    }
    
}

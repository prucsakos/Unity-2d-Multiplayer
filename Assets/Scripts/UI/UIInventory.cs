using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public class UIInventory : MonoBehaviour
{
    private Inventory inventory;
    private Transform itemSlotContainer;
    private Transform itemSlotTemplate;
    private PlayerController player;

    private void Awake()
    {
        itemSlotContainer = transform.Find("InventorySlotsContainerGrid");
        itemSlotTemplate = transform.Find("ItemSlotTemplate");
        
    }

    public void SetPlayer(PlayerController p)
    {
        this.player = p;
    }
    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;

        inventory.ItemListChanged += OnItemListChanged;
        RefreshInventoryItems();
    }

    private void OnItemListChanged(object sender, EventArgs e)
    {
        RefreshInventoryItems();
    }

    private void RefreshInventoryItems()
    {
        //make it empty
        foreach (Transform child in itemSlotContainer)
        {
            Destroy(child.gameObject);
        }


        foreach (Item item in inventory.getItemList())
        {
            RectTransform itemSRT = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
            itemSRT.gameObject.SetActive(true);
            Image im = itemSRT.Find("Image").GetComponent<Image>();
            im.sprite = item.GetSprite();
            TextMeshProUGUI UItext = itemSRT.Find("amountText").GetComponent<TextMeshProUGUI>();
            if(item.amount > 1)
            {
                UItext.SetText(item.amount.ToString());
            } else
            {
                UItext.SetText("");
            }

            ClickHandler ch = itemSRT.GetComponent<ClickHandler>();
            ch.OnLeftClickAction = () =>
            {
                // Drop Item
                inventory.RemoveItem(item);
                ItemWorld.DropItem(player.transform.position, player.mouseDir, item);
            };
            ch.OnRightClickAction = () =>
            {
                // Use Item
                inventory.UseItem(item);
                Debug.Log("Used");
            };
        }
    }
}

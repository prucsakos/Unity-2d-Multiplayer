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
    private Transform inventoryHelmetSlot;
    private Transform inventoryArmorSlot;
    private Transform inventoryGunSlot;
    private PlayerController player;

    private void Awake()
    {
        itemSlotContainer = transform.Find("InventorySlotsContainerGrid");
        itemSlotTemplate = transform.Find("ItemSlotTemplate");
        inventoryHelmetSlot = transform.Find("CharacterSlotsBackground/Head");
        inventoryArmorSlot = transform.Find("CharacterSlotsBackground/Body");
        inventoryGunSlot = transform.Find("CharacterSlotsBackground/Weapon");
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
        Image helmIm = inventoryHelmetSlot.Find("Image").GetComponent<Image>();
        Image armIm = inventoryArmorSlot.Find("Image").GetComponent<Image>();
        Image gunIm = inventoryGunSlot.Find("Image").GetComponent<Image>();
        // CHECK HELMET SLOT
        Item helm = inventory.getHelmet();
        if(helm == null)
        {
            helmIm.sprite = ItemAssets.Instance.emptyHeadSlot;
        } else
        {
            helmIm.sprite = helm.GetSprite();
        }
        // CHECK ARMOR SLOT
        Item arm = inventory.getArmor();
        if (arm == null)
        {
            armIm.sprite = ItemAssets.Instance.emptyBodySlot;
        }
        else
        {
            armIm.sprite = arm.GetSprite();
        }
        // CHECK GUN SLOT
        Item gun = inventory.getWeapon();
        if (gun == null)
        {
            gunIm.sprite = ItemAssets.Instance.emptyGunSlot;
            Debug.Log("EMPTYGUNSLOT");
        }
        else
        {
            gunIm.sprite = gun.GetSprite();
            Debug.Log("GUNSLOTFOUND");
        }

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

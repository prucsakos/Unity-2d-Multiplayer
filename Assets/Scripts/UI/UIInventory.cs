using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.EventSystems;

public class UIInventory : MonoBehaviour
{
    private Inventory inventory;
    private Transform itemSlotContainer;
    private Transform itemSlotTemplate;
    private Transform itemSlotContainerBackground;
    private Transform inventoryHelmetSlot;
    private Transform inventoryArmorSlot;
    private Transform inventoryGunSlot;
    private PlayerController player;

    private void Awake()
    {
        itemSlotContainer = transform.Find("InventorySlotsContainerGrid");
        itemSlotContainerBackground = transform.Find("InventoryBackground");
        itemSlotContainerBackground.GetComponent<DropHandler>().doAction = DropOnInventory;

        itemSlotTemplate = transform.Find("ItemSlotTemplate");

        inventoryHelmetSlot = transform.Find("CharacterSlotsBackground/Head");
        inventoryHelmetSlot.GetComponent<DropHandler>().doAction = DropOnHelmet;
        inventoryArmorSlot = transform.Find("CharacterSlotsBackground/Body");
        inventoryArmorSlot.GetComponent<DropHandler>().doAction = DropOnArmor;
        inventoryGunSlot = transform.Find("CharacterSlotsBackground/Weapon");
        inventoryGunSlot.GetComponent<DropHandler>().doAction = DropOnWeapon;

    }

    public void SetPlayer(PlayerController p)
    {
        this.player = p;
    }
    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;

        inventory.ItemListChanged += OnItemListChanged;

        inventoryHelmetSlot.GetComponent<ClickHandler>().setInventory(inventory);
        inventoryHelmetSlot.GetComponent<ClickHandler>().OnRightClickAction = () =>
        {
            inventory.UnEquipItem(inventory.getHelmet());
        };
        inventoryArmorSlot.GetComponent<ClickHandler>().setInventory(inventory);
        inventoryArmorSlot.GetComponent<ClickHandler>().OnRightClickAction = () =>
        {
            inventory.UnEquipItem(inventory.getArmor());
        };
        inventoryGunSlot.GetComponent<ClickHandler>().setInventory(inventory);
        inventoryGunSlot.GetComponent<ClickHandler>().OnRightClickAction = () =>
        {
            inventory.UnEquipItem(inventory.getWeapon());
        };

        RefreshInventoryItems();
    }

    private void OnItemListChanged(object sender, EventArgs e)
    {
        RefreshInventoryItems();
    }

    private void RefreshInventoryItems()
    {
        // TODO, ITEMHOLDER
        Image helmIm = inventoryHelmetSlot.Find("Image").GetComponent<Image>();
        ItemHolder helmIH = inventoryHelmetSlot.GetComponent<ItemHolder>();
        Image armIm = inventoryArmorSlot.Find("Image").GetComponent<Image>();
        ItemHolder armIH = inventoryArmorSlot.GetComponent<ItemHolder>();
        Image gunIm = inventoryGunSlot.Find("Image").GetComponent<Image>();
        ItemHolder gunIH = inventoryGunSlot.GetComponent<ItemHolder>();
        // CHECK HELMET SLOT
        Item helm = inventory.getHelmet();
        if(helm == null)
        {
            helmIm.sprite = ItemAssets.Instance.emptyHeadSlot;
        } else
        {
            helmIm.sprite = helm.GetSprite();
            helmIH.setItem(helm, ItemHolder.State.OnCharacter);
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
            armIH.setItem(arm, ItemHolder.State.OnCharacter);
        }
        // CHECK GUN SLOT
        Item gun = inventory.getWeapon();
        if (gun == null)
        {
            gunIm.sprite = ItemAssets.Instance.emptyGunSlot;
        }
        else
        {
            gunIm.sprite = gun.GetSprite();
            gunIH.setItem(gun, ItemHolder.State.OnCharacter);
        }

        // INVENTORY REFRESH
        foreach (Transform child in itemSlotContainer)
        {
            Destroy(child.gameObject);
        }


        foreach (Item item in inventory.getItemList())
        {
            RectTransform itemSRT = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
            itemSRT.gameObject.SetActive(true);
            itemSRT.GetComponent<ItemHolder>().setItem(item, ItemHolder.State.InInventory);
            Image im = itemSRT.Find("Image").GetComponent<Image>();
            im.sprite = item.GetSprite();
            TextMeshProUGUI UItext = itemSRT.Find("amountText").GetComponent<TextMeshProUGUI>();
            UItext.alpha = 1f;
            if(item.amount > 1)
            {
                UItext.SetText(item.amount.ToString());
            } else
            {
                UItext.SetText("");
            }

            ClickHandler ch = itemSRT.GetComponent<ClickHandler>();
            ch.setInventory(inventory, item);
            ch.OnLeftClickAction = () =>
            {
                // Drop Item
                inventory.RemoveItem(item, true);
                // ItemWorld.DropItem(player.transform.position, player.mouseDir, item);
                player.DropItem(player.transform.position, player.mouseDir, item, true);
            };
            ch.OnRightClickAction = () =>
            {
                // Use Item
                inventory.UseItem(item);
                Debug.Log("Used");
            };
        }
    }

    // DropEvents
    public void DropOnInventory(PointerEventData ped)
    {
        Debug.Log("INVENTORY DROP LOG");
        ped.pointerDrag.TryGetComponent<ItemHolder>(out ItemHolder ih);
        if (ih)
        {
            if (!ih.isSet) return;
            if(ih.state == ItemHolder.State.OnCharacter)
            {
                inventory.UnEquipItem(ih.item);
            }
        }
    }
    public void DropOnHelmet(PointerEventData ped)
    {
        ped.pointerDrag.TryGetComponent<ItemHolder>(out ItemHolder ih);
        if (ih)
        {
            if (!ih.isSet) return;
            if (ih.state == ItemHolder.State.InInventory && ih.item.itemType == ItemType.Head)
            {
                inventory.EquipItem(ih.item);
            }
        }
    }
    public void DropOnArmor(PointerEventData ped)
    {
        ped.pointerDrag.TryGetComponent<ItemHolder>(out ItemHolder ih);
        if (ih)
        {
            if (!ih.isSet) return;
            if (ih.state == ItemHolder.State.InInventory && ih.item.itemType == ItemType.Body)
            {
                inventory.EquipItem(ih.item);
            }
        }
    }
    public void DropOnWeapon(PointerEventData ped)
    {
        ped.pointerDrag.TryGetComponent<ItemHolder>(out ItemHolder ih);
        if (ih)
        {
            if (!ih.isSet) return;
            if (ih.state == ItemHolder.State.InInventory && (ih.item.itemType == ItemType.Pistol || ih.item.itemType == ItemType.AR || ih.item.itemType == ItemType.RocketLauncher))
            {
                inventory.EquipItem(ih.item);
            }
        }
    }

}



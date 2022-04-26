using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Inventory
{

    public event EventHandler ItemListChanged;


    private List<Item> itemList;
    private Action<Item> useItemAction;

    //Character Item Slots
    private Item Helmet;
    private Item Armor;
    private Item Weapon;

    public Inventory(Action<Item> useItemAction)
    {
        itemList = new List<Item>();

        this.useItemAction = useItemAction;
        AddItem(new Item { itemType = Item.ItemType.AR, amount = 1 });
        AddItem(new Item { itemType = Item.ItemType.HealthPotion, amount = 5 });

        Debug.Log("Init inventory");
    }
    public void AddItem(Item item)
    {
        if(item.IsStackable())
        {
            bool alreadyInInventory = false;
            foreach (var inventoryItem in itemList)
            {
                if(item.itemType == inventoryItem.itemType)
                {
                    inventoryItem.amount += item.amount;
                    alreadyInInventory = true;
                }
            }
            if (!alreadyInInventory)
            {
                itemList.Add(item); 
            }
        } else
        {
        itemList.Add(item);
        }
        ItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void RemoveItem(Item item)
    {
        if (item.IsStackable())
        {
            Item itemInInventory = null;
            foreach (Item inventoryItem in itemList)
            {
                if (item.itemType == inventoryItem.itemType)
                {
                    inventoryItem.amount -= 1;
                    itemInInventory = inventoryItem;
                }
            }
            if (itemInInventory != null && itemInInventory.amount <= 0)
            {
                itemList.Remove(item);
            }
        }
        else
        {
            itemList.Remove(item);
        }
        ItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void UseItem(Item item)
    {
        useItemAction(item);
    }
    public List<Item> getItemList()
    {
        return itemList;
    }
    public Item getHelmet()
    {
        return Helmet;
    }
    public Item getArmor()
    {
        return Armor;
    }
    public Item getWeapon()
    {
        return Weapon;
    }
    public void EquipItem(Item item)
    {
        switch (item.itemType)
        {
            case Item.ItemType.Pistol:
            case Item.ItemType.AR:
            case Item.ItemType.RocketLauncher:
                if(Weapon == null)
                {
                    Weapon = item;

                } else
                {
                    Item tmp = Weapon;
                    Weapon = item;
                    AddItem(tmp);
                }
                RemoveItem(item);
                ItemListChanged?.Invoke(this, EventArgs.Empty);
                break;
            case Item.ItemType.Head:
                if (Helmet == null)
                {
                    Helmet = item;

                }
                else
                {
                    Item tmp = Helmet;
                    Helmet = item;
                    AddItem(tmp);
                }
                RemoveItem(item);
                ItemListChanged?.Invoke(this, EventArgs.Empty);
                break;
            case Item.ItemType.Body:
                if (Armor == null)
                {
                    Armor = item;

                }
                else
                {
                    Item tmp = Armor;
                    Armor = item;
                    AddItem(tmp);
                }
                RemoveItem(item);
                ItemListChanged?.Invoke(this, EventArgs.Empty);
                break;
            default:
                return;
        }
    }
}

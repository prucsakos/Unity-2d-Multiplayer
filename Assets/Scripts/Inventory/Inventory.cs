using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Inventory
{

    public event EventHandler ItemListChanged;

    private List<Item> itemList;
    private Action<Item> useItemAction;
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
}

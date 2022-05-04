using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Inventory
{
    public static int GetArmorFromNetStruct(ItemStructNetcode helm, ItemStructNetcode arm)
    {
        int res = 0;
        if (helm.isSet) res += new Item(helm).ShieldDamage;
        if (arm.isSet) res += new Item(arm).ShieldDamage;
        Debug.Log($"Inventory static: def: {res}");
        return res;
    }

    public event EventHandler ItemListChanged;
    public event EventHandler ClothesChanged;
    public event EventHandler XpChanged;
    public event EventHandler LevelUp;


    private List<Item> itemList;
    private Action<Item> useItemAction;

    // Character Global Base Stats
    private static float BaseMovementSpeed = 1f;

    // Character Stat Modifiers
    private static float MovementSpeedModifier = 0.1f;

    // Character Stats
    public float MovementSpeed;


    // Character Item Slots
    private Item Helmet;
    private Item Armor;
    private Item Weapon;

    // Experience point
    private static int[] XpNeededForLevelUps =  { 10, 15, 20, 30, 40, 50, 75, 100, 150, 200, 300, 500, 1000 };
    private int xp = 0;
    private int level = 0;

    public Inventory(Action<Item> useItemAction)
    {
        itemList = new List<Item>();
        this.useItemAction = useItemAction;

        LevelUp += OnLevelUp;

        MovementSpeed = BaseMovementSpeed + level * MovementSpeedModifier;

        AddItem(new Item(ItemType.Pistol, ItemTier.Common, 1));
        AddItem(new Item(ItemType.AR, ItemTier.Legendary, 1));
        AddItem(new Item(ItemType.RocketLauncher, ItemTier.Legendary, 1));
        AddItem(new Item(ItemType.Head, ItemTier.Common, 1));
        AddItem(new Item(ItemType.Body, ItemTier.Good, 1));
    }
    public void ResetInventory()
    {
        Helmet = null;
        Armor = null;
        Weapon = null;
        itemList = new List<Item>();
        AddItem(new Item(ItemType.Pistol, ItemTier.Common, 1));
    }

    // Character Stats
    public int GetArmor()
    {
        int res = 0;
        if (Helmet != null) res += Helmet.ShieldDamage;
        if (Armor != null) res += Armor.ShieldDamage;
        return res;
    }
    public ItemWeaponStats GetWeaponStats()
    {
        if (Weapon == null) return null;
        return Weapon.WeaponStats;
    }

    // XP
    private void OnLevelUp(object sender, EventArgs e)
    {
        MovementSpeed = BaseMovementSpeed + level * MovementSpeedModifier;
    }

    public int GetXp()
    {
        return xp;
    }
    public int GetLevel()
    {
        return level;
    }
    public int GetMaxXp()
    {
        if (level < XpNeededForLevelUps.Length)
        {
            return XpNeededForLevelUps[level];
        } else
        {
            return 0;
        }
    }
    public void ReceiveXp(int amount)
    {
        xp += amount;
        while (level < XpNeededForLevelUps.Length && xp >= XpNeededForLevelUps[level])
        {
            xp -= XpNeededForLevelUps[level];
            level++;
            LevelUp?.Invoke(this, EventArgs.Empty);
        }
        XpChanged?.Invoke(this, EventArgs.Empty);
    }
    // INVENTORY
    public void AddItem(Item item)
    {
        if(item.itemType == ItemType.Xp)
        {
            ReceiveXp(item.amount);
            return;
        }
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
    /// <summary>
    /// 
    /// </summary>
    /// <param name="onlyOne"> Only matters if the item is Stackable.</param>
    public void RemoveItem(Item item, bool onlyOne = true)
    {
        if (item.IsStackable())
        {
            Item itemInInventory = null;
            foreach (Item inventoryItem in itemList)
            {
                if (item.itemType == inventoryItem.itemType)
                {
                    if (onlyOne)
                    {
                        inventoryItem.amount -= 1;
                    } else
                    {
                        inventoryItem.amount -= item.amount;
                    }
                    itemInInventory = inventoryItem;
                }
            }
            if (itemInInventory != null && itemInInventory.amount <= 0)
            {
                itemList.Remove(itemInInventory);
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
    // CHARACTER SLOTS
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
            case ItemType.Pistol:
            case ItemType.AR:
            case ItemType.RocketLauncher:
                if(Weapon == null)
                {
                    Weapon = item.clone();

                } else
                {
                    Item tmp = Weapon;
                    Weapon = item.clone();
                    AddItem(tmp);
                }
                RemoveItem(item);
                ItemListChanged?.Invoke(this, EventArgs.Empty);
                ClothesChanged?.Invoke(this, EventArgs.Empty);
                break;
            case ItemType.Head:
                if (Helmet == null)
                {
                    Helmet = item.clone();

                }
                else
                {
                    Item tmp = Helmet;
                    Helmet = item.clone();
                    AddItem(tmp);
                }
                RemoveItem(item);
                ItemListChanged?.Invoke(this, EventArgs.Empty);
                ClothesChanged?.Invoke(this, EventArgs.Empty);
                break;
            case ItemType.Body:
                if (Armor == null)
                {
                    Armor = item.clone();

                }
                else
                {
                    Item tmp = Armor;
                    Armor = item.clone();
                    AddItem(tmp);
                }
                RemoveItem(item);
                ItemListChanged?.Invoke(this, EventArgs.Empty);
                ClothesChanged?.Invoke(this, EventArgs.Empty);
                break;
            default:
                return;
        }
    }
    public void UnEquipItem(Item item)
    {
        if (item == null) return;
        switch (item.itemType)
        {
            case ItemType.Pistol:
            case ItemType.AR:
            case ItemType.RocketLauncher:
                if (Weapon == null)
                {
                    return;

                }
                else
                {
                    Item tmp = Weapon;
                    Weapon = null;
                    AddItem(tmp);
                }
                ItemListChanged?.Invoke(this, EventArgs.Empty);
                ClothesChanged?.Invoke(this, EventArgs.Empty);
                break;
            case ItemType.Head:
                if (Helmet == null)
                {
                    return;

                }
                else
                {
                    Item tmp = Helmet;
                    Helmet = null;
                    AddItem(tmp);
                }
                ItemListChanged?.Invoke(this, EventArgs.Empty);
                ClothesChanged?.Invoke(this, EventArgs.Empty);
                break;
            case ItemType.Body:
                if (Armor == null)
                {
                    return;

                }
                else
                {
                    Item tmp = Armor;
                    Armor = null;
                    AddItem(tmp);
                }
                ItemListChanged?.Invoke(this, EventArgs.Empty);
                ClothesChanged?.Invoke(this, EventArgs.Empty);
                break;
            default:
                return;
        }
    }
}

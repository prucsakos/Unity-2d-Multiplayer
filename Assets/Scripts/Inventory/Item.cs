using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public enum ItemType 
{
    Pistol,
    AR,
    RocketLauncher,
    Head,
    Body,
    HealthPotion,
    Coin,
    Xp
}
public enum ItemTier
{
    Common,
    Good,
    Rare,
    Legendary
}
public class ItemWeaponStats
{
    public float TimeBetweenAttacks;
    public float BulletVelocity;
    public float BulletRange;
    public int WeaponDamage;
    public ItemWeaponStats clone()
    {
        return new ItemWeaponStats { BulletVelocity = this.BulletVelocity, TimeBetweenAttacks = this.TimeBetweenAttacks, WeaponDamage = this.WeaponDamage, BulletRange=this.BulletRange };
    }
    public override string ToString()
    {
        return $"TimeBetweenAttacks: {TimeBetweenAttacks}, BulletVelocity: {BulletVelocity}, BulletRange: {BulletRange}, WeaponDamage: {WeaponDamage}";
    }
}

public class Item 
{
    public static Item GetRandomEquipment()
    {
        float tierRoll = UnityEngine.Random.Range(0f, 1f);
        float typeRoll = UnityEngine.Random.Range(0f, 1f);
        ItemTier itier;
        ItemType itype;
        int ind = 0;
        while(tierRoll > TierChances[ind])
        {
            ind++;
        }
        itier = TierNames[ind];
        ind = 0;
        while (typeRoll > TypeChances[ind])
        {
            ind++;
        }
        itype = TypeNames[ind];

        return new Item(itype, itier, 1);
    }
    // Chances
    public static float[] TierChances = new float[] { 0.4f, 0.7f, 0.9f, 1f };
    public static ItemTier[] TierNames = new ItemTier[] { ItemTier.Common, ItemTier.Good, ItemTier.Rare, ItemTier.Legendary };

    public static float[] TypeChances = new float[] { 0.25f, 0.35f, 0.5f ,0.75f, 1f };
    public static ItemType[] TypeNames = new ItemType[] { ItemType.Pistol, ItemType.AR, ItemType.RocketLauncher, ItemType.Head, ItemType.Body  };

    public static float ChanceForNpcItemDrop = 0.3f;
    public static float ChanceForNpcHealthPotion = 0.5f;

    // Static stats
    public static Dictionary<ItemTier, int> TierToShieldDmg = new Dictionary<ItemTier, int>() { {ItemTier.Common, 1 }, { ItemTier.Good, 2 }, { ItemTier.Rare, 3 }, { ItemTier.Legendary, 4 }, };
    public static Dictionary<ItemTier, ItemWeaponStats> TierToPistolStats = new Dictionary<ItemTier, ItemWeaponStats>()
    {
        {ItemTier.Common, new ItemWeaponStats { WeaponDamage = 2, BulletVelocity = 2f, TimeBetweenAttacks = 1f, BulletRange=0.5f } },
        {ItemTier.Good, new ItemWeaponStats { WeaponDamage = 3, BulletVelocity = 2.2f, TimeBetweenAttacks = 1f, BulletRange=0.6f } },
        {ItemTier.Rare, new ItemWeaponStats { WeaponDamage = 4, BulletVelocity = 2.4f, TimeBetweenAttacks = 0.9f, BulletRange=0.7f } },
        {ItemTier.Legendary, new ItemWeaponStats { WeaponDamage = 5, BulletVelocity = 2.6f, TimeBetweenAttacks = 0.8f, BulletRange=0.8f } }
    };
    public static Dictionary<ItemTier, ItemWeaponStats> TierToArStats = new Dictionary<ItemTier, ItemWeaponStats>()
    {
        {ItemTier.Common, new ItemWeaponStats { WeaponDamage = 3, BulletVelocity = 4f, TimeBetweenAttacks = 0.3f, BulletRange=1f } },
        {ItemTier.Good, new ItemWeaponStats { WeaponDamage = 4, BulletVelocity = 4.2f, TimeBetweenAttacks = 0.25f, BulletRange=1.1f } },
        {ItemTier.Rare, new ItemWeaponStats { WeaponDamage = 5, BulletVelocity = 4.4f, TimeBetweenAttacks = 0.2f, BulletRange=1.2f } },
        {ItemTier.Legendary, new ItemWeaponStats { WeaponDamage = 6, BulletVelocity = 4.6f, TimeBetweenAttacks = 0.1f, BulletRange=1.3f } }
    };
    public static Dictionary<ItemTier, ItemWeaponStats> TierToRocketStats = new Dictionary<ItemTier, ItemWeaponStats>()
    {
        {ItemTier.Common, new ItemWeaponStats { WeaponDamage = 10, BulletVelocity = 1f, TimeBetweenAttacks = 3f, BulletRange=5f } },
        {ItemTier.Good, new ItemWeaponStats { WeaponDamage = 15, BulletVelocity = 1.2f, TimeBetweenAttacks = 2.8f, BulletRange=6f } },
        {ItemTier.Rare, new ItemWeaponStats { WeaponDamage = 20, BulletVelocity = 1.4f, TimeBetweenAttacks = 2.6f, BulletRange=7f } },
        {ItemTier.Legendary, new ItemWeaponStats { WeaponDamage = 25, BulletVelocity = 1.6f, TimeBetweenAttacks = 2.4f, BulletRange=8f } }
    };

    // Core
    public ItemType itemType;
    public ItemTier itemTier;
    public int amount;

    // Weapon
    public ItemWeaponStats WeaponStats;

    // Body/Helmet
    public int ShieldDamage;
    /*
    public Item()
    {
    }
    */
    public Item(ItemType itype, ItemTier itier, int am)
    {
        this.itemType = itype;
        this.itemTier = itier;
        this.amount = am;
        FetchStaticStats();
    }
    public Item(ItemStructNetcode isn)
    {
        this.itemType = isn.itemType;
        this.itemTier = isn.itemTier;
        this.amount = isn.amount;
        FetchStaticStats();
    }
    public Color GetColor()
    {
        switch (itemTier)
        {
            case ItemTier.Common:
                return new Color(192, 192, 192);
            case ItemTier.Good:
                return new Color(156, 219, 67);
            case ItemTier.Rare:
                return new Color(188, 74, 155);
            case ItemTier.Legendary:
                return new Color(255, 252, 64);
            default:
                return new Color();
        }
    }
    public Sprite GetIcon()
    {
        switch (itemType)
        {
            case ItemType.Pistol:
                if (itemTier == ItemTier.Common) return ItemAssets.Instance.CommonPistol;
                if (itemTier == ItemTier.Good) return ItemAssets.Instance.GoodPistol;
                if (itemTier == ItemTier.Rare) return ItemAssets.Instance.RarePistol;
                if (itemTier == ItemTier.Legendary) return ItemAssets.Instance.LegendaryPistol;
                return null;
            case ItemType.AR:
                if (itemTier == ItemTier.Common) return ItemAssets.Instance.CommonAR;
                if (itemTier == ItemTier.Good) return ItemAssets.Instance.GoodAR;
                if (itemTier == ItemTier.Rare) return ItemAssets.Instance.RareAR;
                if (itemTier == ItemTier.Legendary) return ItemAssets.Instance.LegendaryAR;
                return null;
            case ItemType.RocketLauncher:
                if (itemTier == ItemTier.Common) return ItemAssets.Instance.CommonRocketLauncher;
                if (itemTier == ItemTier.Good) return ItemAssets.Instance.GoodRocketLauncher;
                if (itemTier == ItemTier.Rare) return ItemAssets.Instance.RareRocketLauncher;
                if (itemTier == ItemTier.Legendary) return ItemAssets.Instance.LegendaryRocketLauncher;
                return null;
            case ItemType.Head:
                if (itemTier == ItemTier.Common) return ItemAssets.Instance.CommonHelmetIcon;
                if (itemTier == ItemTier.Good) return ItemAssets.Instance.GoodHelmetIcon;
                if (itemTier == ItemTier.Rare) return ItemAssets.Instance.RareHelmetIcon;
                if (itemTier == ItemTier.Legendary) return ItemAssets.Instance.LegendaryHelmetIcon;
                return null;
            case ItemType.Body:
                if (itemTier == ItemTier.Common) return ItemAssets.Instance.CommonArmorIcon;
                if (itemTier == ItemTier.Good) return ItemAssets.Instance.GoodArmorIcon;
                if (itemTier == ItemTier.Rare) return ItemAssets.Instance.RareArmorIcon;
                if (itemTier == ItemTier.Legendary) return ItemAssets.Instance.LegendaryArmorIcon;
                return null;
            case ItemType.HealthPotion:
                return ItemAssets.Instance.healthPotionSprite;
            case ItemType.Coin:
                return ItemAssets.Instance.coinSprite;
            case ItemType.Xp:
                return ItemAssets.Instance.xpSprite;
            default:
                return null;
        }
    }
    public Sprite GetSprite()
    {
        switch(itemType)
        {
            case ItemType.Pistol:    return GetPistolSprite();
            case ItemType.AR:    return GetARSprite();
            case ItemType.RocketLauncher:    return GetRocketLauncherSprite();
            case ItemType.Head:    return GetHelmetSprite();
            case ItemType.Body:   return GetArmorSprite();


            case ItemType.HealthPotion:    return ItemAssets.Instance.healthPotionSprite;
            case ItemType.Coin:   return ItemAssets.Instance.coinSprite;
            case ItemType.Xp: return ItemAssets.Instance.xpSprite;
            default: return ItemAssets.Instance.notFound;
        }
    }
    private Sprite GetPistolSprite()
    {
        switch (itemTier)
        {
            case ItemTier.Common:
                return ItemAssets.Instance.CommonPistol;
            case ItemTier.Good:
                return ItemAssets.Instance.GoodPistol;
            case ItemTier.Rare:
                return ItemAssets.Instance.RarePistol;
            case ItemTier.Legendary:
                return ItemAssets.Instance.LegendaryPistol;
            default:
                return ItemAssets.Instance.EmptySprite;
        }
    }
    private Sprite GetARSprite()
    {
        switch (itemTier)
        {
            case ItemTier.Common:
                return ItemAssets.Instance.CommonAR;
            case ItemTier.Good:
                return ItemAssets.Instance.GoodAR;
            case ItemTier.Rare:
                return ItemAssets.Instance.RareAR;
            case ItemTier.Legendary:
                return ItemAssets.Instance.LegendaryAR;
            default:
                return ItemAssets.Instance.EmptySprite;
        }
    }
    private Sprite GetRocketLauncherSprite()
    {
        switch (itemTier)
        {
            case ItemTier.Common:
                return ItemAssets.Instance.CommonRocketLauncher;
            case ItemTier.Good:
                return ItemAssets.Instance.GoodRocketLauncher;
            case ItemTier.Rare:
                return ItemAssets.Instance.RareRocketLauncher;
            case ItemTier.Legendary:
                return ItemAssets.Instance.LegendaryRocketLauncher;
            default:
                return ItemAssets.Instance.EmptySprite;
        }
    }
    private Sprite GetHelmetSprite()
    {
        switch (itemTier)
        {
            case ItemTier.Common:
                return ItemAssets.Instance.CommonHelmet;
            case ItemTier.Good:
                return ItemAssets.Instance.GoodHelmet;
            case ItemTier.Rare:
                return ItemAssets.Instance.RareHelmet;
            case ItemTier.Legendary:
                return ItemAssets.Instance.LegendaryHelmet;
            default:
                return ItemAssets.Instance.EmptySprite;
        }
    }
    private Sprite GetArmorSprite()
    {
        switch (itemTier)
        {
            case ItemTier.Common:
                return ItemAssets.Instance.CommonArmor;
            case ItemTier.Good:
                return ItemAssets.Instance.GoodArmor;
            case ItemTier.Rare:
                return ItemAssets.Instance.RareArmor;
            case ItemTier.Legendary:
                return ItemAssets.Instance.LegendaryArmor;
            default:
                return ItemAssets.Instance.EmptySprite;
        }
    }

    public bool IsStackable()
    {
        switch (itemType)
        {
            default:
            case ItemType.Coin:
            case ItemType.HealthPotion:
            case ItemType.Xp:
                return true;
            case ItemType.Pistol:
            case ItemType.AR:
            case ItemType.RocketLauncher:
            case ItemType.Body:
            case ItemType.Head:
                return false;
        }
    }

    public Item clone()
    {
        return new Item(itemType, itemTier, amount);
    }

    private void FetchStaticStats()
    {
        switch (itemType)
        {
            case ItemType.Pistol:
                if (TierToPistolStats.TryGetValue(itemTier, out ItemWeaponStats PistolStats))
                {
                    WeaponStats = PistolStats.clone();
                }
                break;
            case ItemType.AR:
                if (TierToArStats.TryGetValue(itemTier, out ItemWeaponStats ARStats))
                {
                    WeaponStats = ARStats.clone();
                }
                break;
            case ItemType.RocketLauncher:
                if (TierToRocketStats.TryGetValue(itemTier, out ItemWeaponStats RocketStats))
                {
                    WeaponStats = RocketStats.clone();
                }
                break;
            case ItemType.Head:
                if (TierToShieldDmg.TryGetValue(itemTier, out int headarm))
                {
                    ShieldDamage = headarm;
                }
                break;
            case ItemType.Body:
                if (TierToShieldDmg.TryGetValue(itemTier, out int armor))
                {
                    ShieldDamage = armor;
                }
                break;
            case ItemType.HealthPotion:
                break;
            case ItemType.Coin:
                break;
            case ItemType.Xp:
                break;
            default:
                break;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ItemAssets : MonoBehaviour
{
    public static ItemAssets Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    public Transform PlayerPrefab;
    public Transform pfItemWorld;

    public Sprite pistolSprite;
    public Sprite arSprite;
    public Sprite rocketLauncherSprite;
    public Sprite headSprite;
    public Sprite bodySprite;
    public Sprite healthPotionSprite;
    public Sprite coinSprite;
    public Sprite xpSprite;
    public Sprite notFound;

    public Sprite EmptyChest;
    public Sprite ClosedChest;

    public Sprite emptyGunSlot;
    public Sprite emptyHeadSlot;
    public Sprite emptyBodySlot;

    // Map Tiles
    public Transform Spawn;
    public Transform Room1;
    public Transform BlockingGate;

    public Sprite Character_0;

    // Final Item Sprites
    public Sprite CommonHelmet;
    public Sprite CommonArmor;
    public Sprite CommonPistol;
    public Sprite CommonAR;
    public Sprite CommonRocketLauncher;

    public Sprite GoodHelmet;
    public Sprite GoodArmor;
    public Sprite GoodPistol;
    public Sprite GoodAR;
    public Sprite GoodRocketLauncher;

    public Sprite RareHelmet; 
    public Sprite RareArmor;
    public Sprite RarePistol;
    public Sprite RareAR;
    public Sprite RareRocketLauncher;

    public Sprite LegendaryHelmet;
    public Sprite LegendaryArmor;
    public Sprite LegendaryPistol;
    public Sprite LegendaryAR;
    public Sprite LegendaryRocketLauncher;

    public Sprite EmptySprite;

}

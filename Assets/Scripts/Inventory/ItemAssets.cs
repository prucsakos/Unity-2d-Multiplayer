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
        SimpleRoomList = new Transform[] { Room1, Room2, Room3 };
        SimpleEnemySprites = new Sprite[] { BossSprite, Enemy1, Enemy2, Enemy3, Enemy4, Enemy5, Enemy6, Enemy7, Enemy8 };
        
    }
    public Transform PlayerPrefab;
    public Transform pfItemWorld;
    public Transform NavMesh2dObj;

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
    public Transform EmptyMap;
    public Transform Spawn;
    public Transform Room1;
    public Transform Room2;
    public Transform Room3;
    public Transform BossRoom;
    public Transform[] SimpleRoomList;
    public Transform BlockingGate;

    public Sprite EmptySprite;

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

    // Item Icon Sprites
    public Sprite CommonHelmetIcon;
    public Sprite GoodHelmetIcon;
    public Sprite RareHelmetIcon;
    public Sprite LegendaryHelmetIcon;
    public Sprite CommonArmorIcon;
    public Sprite GoodArmorIcon;
    public Sprite RareArmorIcon;
    public Sprite LegendaryArmorIcon;

    // depracated
    public Sprite SimplePistolIcon;
    public Sprite SimpleArIcon;
    public Sprite SimpleRocketlauncherIcon;


    // Simple Enemies and Boss
    public Sprite Enemy1;
    public Sprite Enemy2;
    public Sprite Enemy3;
    public Sprite Enemy4;
    public Sprite Enemy5;
    public Sprite Enemy6;
    public Sprite Enemy7;
    public Sprite Enemy8;
    public Sprite[] SimpleEnemySprites;

    public Sprite BossSprite;


}

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

}

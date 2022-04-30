using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAssets : MonoBehaviour
{
    public static ItemAssets Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

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

    public Sprite emptyGunSlot;
    public Sprite emptyHeadSlot;
    public Sprite emptyBodySlot;
}

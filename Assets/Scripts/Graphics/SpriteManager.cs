using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    private SpriteRenderer CharacterSprite = new SpriteRenderer();
    private SpriteRenderer HelmetSprite = new SpriteRenderer();
    private SpriteRenderer ArmorSprite = new SpriteRenderer();
    private SpriteRenderer WeaponSprite = new SpriteRenderer();

    public void Awake()
    {
        CharacterSprite = transform.Find("Character").GetComponent<SpriteRenderer>();
        HelmetSprite = transform.Find("HelmetSprite").GetComponent<SpriteRenderer>();
        ArmorSprite = transform.Find("ArmorSprite").GetComponent<SpriteRenderer>();
        WeaponSprite = transform.Find("WeaponSprite").GetComponent<SpriteRenderer>();
        CharacterSprite.sprite = ItemAssets.Instance.Character_0;
    }
    
    public void SetHelmet(ItemStructNetcode isn)
    {
        if (!isn.isSet)
        {
            HelmetSprite.color = new Color(0, 0, 0, 0);
            return;
        }
        Item i = new Item(isn);
        HelmetSprite.sprite = i.GetSprite();
        HelmetSprite.color = i.GetColor();
    }
    public void SetArmor(ItemStructNetcode isn)
    {
        if (!isn.isSet)
        {
            ArmorSprite.sprite = null;
            ArmorSprite.color = new Color(0, 0, 0, 0);
            return;
        }
        Item i = new Item(isn);
        ArmorSprite.sprite = i.GetSprite();
        ArmorSprite.color = i.GetColor();
    }
    public void SetWeapon(ItemStructNetcode isn)
    {
        if (!isn.isSet)
        {
            WeaponSprite.sprite = null;
            WeaponSprite.color = new Color(0, 0, 0, 0);
            return;
        }
        Item i = new Item(isn);
        WeaponSprite.sprite = new Item(isn).GetSprite();
        WeaponSprite.color = i.GetColor();
    }
}

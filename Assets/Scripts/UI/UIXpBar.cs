using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class UIXpBar : NetworkBehaviour
{
    public Slider slider;
    public Image fill;
    public TextMeshPro text;
    private Inventory inv;

    private bool isReady = false;

    private void init()
    {
        fill = transform.Find("Fill").GetComponent<Image>();
        slider = GetComponent<Slider>();
        text = transform.Find("XpTextBackground/XpText").GetComponent<TextMeshPro>();
        isReady = true;
    }
    private void Start()
    {
        if (!IsLocalPlayer)
        {
            gameObject.SetActive(false);
            return;
        }
        if (!isReady)
        {
            init();
        }

    }
    public void setXpBar(Inventory inv)
    {
        if (!IsLocalPlayer) return;
        if (!isReady)
        {
            init();
        }
        this.inv = inv;
        slider.maxValue = inv.GetMaxXp();
        slider.value = inv.GetXp();
        text.text = LevelText();
        inv.LevelUp += OnLevelUp;
        inv.XpChanged += OnXpChanged;
    }

    private void OnXpChanged(object sender, System.EventArgs e)
    {
        slider.value = inv.GetXp();
    }

    private void OnLevelUp(object sender, System.EventArgs e)
    {
        slider.maxValue = inv.GetMaxXp();
        slider.value = inv.GetXp();
        text.text = LevelText();
    }
    private string LevelText()
    {
        //return "lvl " + inv.GetLevel().ToString();
        return inv.GetLevel().ToString();
    }
}

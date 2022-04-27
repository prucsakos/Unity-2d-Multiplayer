using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIHealthBar : MonoBehaviour
{
    public Gradient gradient;
    public Image fill;
    public Slider slider;
    private Damageable damage;

    private bool isReady = false;

    private void init()
    {
        fill = transform.Find("Fill").GetComponent<Image>();
        slider = GetComponent<Slider>();
        isReady = true;
    }
    private void Start()
    {
        if(!isReady)
        {
            init();
        }

    }
    public void setDamagable(Damageable damage)
    {
        if(!isReady)
        {
            init();
        }
        this.damage = damage;
        slider.maxValue = damage.GetMaxHP();
        slider.value = damage.GetHP();
        damage.HpChanged += OnDmgChanged;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    private void OnDmgChanged(object sender, EventArgs e)
    {
        slider.value = damage.GetHP();
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}

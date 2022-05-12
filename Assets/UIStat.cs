using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStat : MonoBehaviour
{
    public GameManager gm;
    public Text currentLevelText;
    public Text bestLevelText;

    private void Awake()
    {
        currentLevelText.text = GetLevel();
        bestLevelText.text = "";
        gm.NetLevel.OnValueChanged += LevelChanged;
    }

    private void LevelChanged(int previousValue, int newValue)
    {
        currentLevelText.text = GetLevel();
        if(newValue == 0)
        {
            bestLevelText.text = $"Best: {previousValue}";
        }
    }

    private string GetLevel()
    {
        return $"Level: {gm.NetLevel.Value}";
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    public enum State
    {
        OnCharacter,
        InInventory
    }


    public Item item;
    public State state;
    public bool isSet = false;

    public void setItem(Item item, State s)
    {
        this.item = item;
        isSet = true;
        state = s;
    }
}

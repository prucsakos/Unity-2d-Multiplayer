using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClickHandler : MonoBehaviour, IPointerClickHandler
{

    public Action OnLeftClickAction;
    public Action OnRightClickAction;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (OnLeftClickAction != null) {
                OnLeftClickAction();
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (OnRightClickAction != null) {
                OnRightClickAction();
            } 
        }
    }
}
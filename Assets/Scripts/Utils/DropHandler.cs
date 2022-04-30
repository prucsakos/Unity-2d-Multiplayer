using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropHandler : MonoBehaviour, IDropHandler
{
    public Action<PointerEventData> doAction;
    public void OnDrop(PointerEventData eventData)
    {
        eventData.pointerDrag.GetComponent<ClickHandler>().OnEndDrag(null);
        doAction(eventData);
    }
}

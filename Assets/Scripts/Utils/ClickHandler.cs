using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClickHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{

    public Action OnLeftClickAction;
    public Action OnRightClickAction;

    private Item item;

    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform imRect;
    [SerializeField] private RectTransform activeRect;
    // private CanvasGroup cg;
    private Inventory inventory;

    private bool isDraging;

    private void Update()
    {
        
    }
    private void Awake()
    {
        // cg = GetComponent<CanvasGroup>();
    }
    public void setInventory(Inventory inv, Item it)
    {
        inventory = inv;
        item = it;
    }
    public void setInventory(Inventory inv)
    {
        inventory = inv;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {

        isDraging = true;
        Sprite sprite = item!=null ? item.GetSprite() : GetComponent<ItemHolder>().item.GetSprite();
        activeRect.TryGetComponent<Image>(out Image im);
        if (im)
        {
            im.sprite = sprite;
        } else
        {
            im = activeRect.gameObject.AddComponent<Image>();
            im.sprite = sprite;
        }
        activeRect.transform.position = eventData.position;
        Debug.Log("BeginDrag");
    }

    public void OnDrag(PointerEventData eventData)
    {
        activeRect.anchoredPosition += eventData.delta / canvas.scaleFactor;
        Debug.Log("Drag");
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        isDraging = false;
        activeRect.TryGetComponent<Image>(out Image im);
        if (im) Destroy(im);
        Debug.Log("OnEndDrag");
    }



    public void OnPointerClick(PointerEventData eventData)
    {
        if (isDraging)
        {
            OnEndDrag(null);
            return;
        }
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

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
    }
}
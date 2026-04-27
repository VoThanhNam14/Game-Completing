using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    Transform FormerPlace;
    CanvasGroup canvasGroup;
    private InventoryController inventoryController;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        inventoryController = InventoryController.Instance;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        FormerPlace = transform.parent;
        Canvas parentCanvas = GetComponentInParent<Canvas>();
    
        if (parentCanvas != null)
        {
            transform.SetParent(parentCanvas.transform);
        }
        else
        {
            transform.SetParent(transform.root);
        }

        transform.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        Slot dropSlot = eventData.pointerEnter?.GetComponent<Slot>();
        if(dropSlot == null)
        {
            GameObject item = eventData.pointerEnter;
            if(item != null)
            {
                dropSlot = item.GetComponentInParent<Slot>();
            }
        }
        Slot FormerSlot = FormerPlace.GetComponent<Slot>();

        if(dropSlot != null)
        {
            if(dropSlot.currentItem != null)
            {
                Item draggedItem = GetComponent<Item>();
                Item targetItem = dropSlot.currentItem.GetComponent<Item>();
                if (dropSlot == FormerSlot)
                {
                    transform.SetParent(FormerPlace);
                    GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    return; 
                }
                if(draggedItem.ID == targetItem.ID)
                {
                    targetItem.AddToStack(draggedItem.quantity);
                    FormerSlot.currentItem = null;
                    Destroy(gameObject);
                    return;
                }
                else
                {
                    dropSlot.currentItem.transform.SetParent(FormerSlot.transform);
                    FormerSlot.currentItem = dropSlot.currentItem;
                    dropSlot.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                }
            }
            else
            {
                FormerSlot.currentItem = null;
            }
            transform.SetParent(dropSlot.transform);
            dropSlot.currentItem = gameObject;
        }
        else
        {
            transform.SetParent(FormerPlace);
        }
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        InventoryController.Instance.NotifyInventoryChanged();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
        // Kiểm tra nếu người chơi đang giữ phím Shift (trái hoặc phải)
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                MoveToInventory();
            }
        }
        else if(eventData.button == PointerEventData.InputButton.Right)
        {
            SplitStack();
        }
    }
    private void SplitStack()
    {
        Item item = GetComponent<Item>();
        if (item == null || item.quantity <= 1) return;
        int splitAmount = item.quantity/2;
        if(splitAmount <= 0) return;

        Transform currentPanel = transform.parent.parent; 
        bool foundSlot = false;
        foreach (Transform slotTransform in currentPanel)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem == null)
            {
                item.RemoveFromStack(splitAmount); 

                GameObject createdItem = item.CloneItem(splitAmount);
                
                slot.currentItem = createdItem;
                createdItem.transform.SetParent(slot.transform);
                createdItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                foundSlot = true;

                InventoryController.Instance.NotifyInventoryChanged();
                break;
            }
        }
        if (!foundSlot)
        {
            Debug.Log("Không còn ô trống trong bảng này!");
        }
    }
    private void MoveToInventory()
    {
        if (inventoryController == null) return;

        // 1. Nếu đang ở sẵn trong Inventory rồi thì không làm gì cả
        if (transform.parent.parent == inventoryController.inventoryPanel.transform)
            return;

        Item currentItem = GetComponent<Item>();
        Slot formerSlot = transform.parent.GetComponent<Slot>();

        // --- BƯỚC 1: THỬ GỘP (STACKING) ---
        foreach (Transform slotTransform in inventoryController.inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem != null)
            {
                Item targetItem = slot.currentItem.GetComponent<Item>();
                if (targetItem.ID == currentItem.ID)
                {
                    targetItem.AddToStack(currentItem.quantity);
                    if (formerSlot != null) formerSlot.currentItem = null;
                    Destroy(gameObject);

                    InventoryController.Instance.NotifyInventoryChanged();
                    return; // Xong việc, thoát hàm
                }
            }
        }

        // --- BƯỚC 2: TÌM Ô TRỐNG (NẾU KHÔNG GỘP ĐƯỢC) ---
        foreach (Transform slotTransform in inventoryController.inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem == null)
            {
                if (formerSlot != null) formerSlot.currentItem = null;

                slot.currentItem = gameObject;
                transform.SetParent(slot.transform);
                GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                InventoryController.Instance.NotifyInventoryChanged();
                return;
            }
        }

        Debug.Log("Inventory đã đầy!");
    }

    
}

using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler {
    public bool IsFilled {
        get {
            return transform.childCount > 0 || Content != null;
        }
    }

    public Item Content { get; set; }

    //public Item Content {
    //    get {
    //        if (!IsFilled) {
    //            return null;
    //        }
    //        return transform.GetChild(0).GetComponent<Item>();
    //    }
    //}

    public void Add(Item item) {
        item.transform.SetParent(transform, false);
        item.transform.position = transform.position;
        item.transform.DOScaleX(transform.localScale.x, .25f);
        item.transform.DOScaleY(transform.localScale.y, .25f);

        OnAddContent(item);
    }

    public virtual void OnDrop(PointerEventData eventData) {
        var rect = transform as RectTransform;
        var selectedObject = eventData.pointerDrag;

        if (IsFilled && transform.GetChild(0).transform == selectedObject.transform) {
            // Draggable is already in this slot
            return;
        }

        if (IsFilled) {
            Debug.Log("Slot is already filled with another item.");
            return;
        }

        if (RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition)) {
            Add(selectedObject.GetComponent<Item>());
        } else {
            selectedObject.GetComponent<Draggable>().MoveBackToParent();
        }
    }

    public virtual void OnAddContent(Draggable draggable) {
        Content = draggable.GetComponent<Item>();
        draggable.CurrentSlot = this;
    }

    public virtual void OnRemoveContent(Draggable draggable) {
        Content = null;
    }

    public virtual void Detach(Draggable draggable) {
        draggable.ParentToContainer();
        draggable.transform.SetAsLastSibling();

        OnRemoveContent(draggable);
    }
}

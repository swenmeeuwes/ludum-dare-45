using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler {
    public bool IsFilled {
        get {
            return transform.childCount > 0 || Content != null;
        }
    }

    public Item Content { get; private set; }

    public void Add(Item item) {
        item.transform.SetParent(transform);
        item.transform.position = transform.position;
        item.transform.DOScaleX(transform.localScale.x, .25f);
        item.transform.DOScaleY(transform.localScale.y, .25f);

        OnAddContent(item);
    }

    public virtual void OnDrop(PointerEventData eventData) {
        var rect = transform as RectTransform;
        var selectedObject = eventData.pointerDrag;

        if (IsFilled) {
            return;
        }

        if (RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition)) {
            Add(selectedObject.GetComponent<Item>());
        }
    }

    public virtual void OnAddContent(Draggable draggable) {
        Content = draggable.GetComponent<Item>();
    }

    public virtual void OnRemoveContent(Draggable draggable) {
        Content = null;
    }

    public virtual void Detach(Draggable draggable) {
        draggable.SetParentToOriginalParent();

        OnRemoveContent(draggable);
    }
}

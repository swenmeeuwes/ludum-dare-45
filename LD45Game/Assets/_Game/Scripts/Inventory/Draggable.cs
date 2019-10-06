using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    private CanvasGroup _canvasGroup;

    private bool _wasOverDropTargetPreviously = false;
    private Tween _currentTween;

    private Transform _originalParent;

    private void Start() {
        _originalParent = transform.parent;
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetParentToOriginalParent() {
        transform.SetParent(_originalParent);
    }

    public void OnBeginDrag(PointerEventData eventData) {
        var parentSlot = transform.parent.GetComponent<Slot>();
        if (parentSlot != null) {
            parentSlot.Detach(this);
        }

        _canvasGroup.blocksRaycasts = false;

        transform.DOScale(1f, .15f);
    }

    public void OnDrag(PointerEventData eventData) {
        transform.position = Input.mousePosition;

        var slot = eventData.hovered.FirstOrDefault(g => g.GetComponent<Slot>() != null);
        var hoveringAboveDropTarget = slot != null;

        if (!_wasOverDropTargetPreviously && hoveringAboveDropTarget) {
            _currentTween = transform.DOScale(1.1f, .45f).SetLoops(-1, LoopType.Yoyo);
        } else if (_wasOverDropTargetPreviously && !hoveringAboveDropTarget) {
            _currentTween.Kill();
            _currentTween = null;
            transform.DOScale(1f, .15f);
        }

        _wasOverDropTargetPreviously = hoveringAboveDropTarget;
    }

    public void OnEndDrag(PointerEventData eventData) {
        _canvasGroup.blocksRaycasts = true;

        if (_currentTween != null) {
            _currentTween.Kill();
            _currentTween = null;
            transform.DOScale(1f, .15f);
        }
    }
}

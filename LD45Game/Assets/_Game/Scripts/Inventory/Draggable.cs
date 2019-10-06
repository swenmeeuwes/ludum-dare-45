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

    public Slot CurrentSlot { get; set; }

    private Transform _container;

    private void Start() {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetContainer(Transform container) {
        _container = container;
    }

    public void ParentToContainer() {
        transform.SetParent(_container);
    }

    public void MoveBackToParent() {
        transform.SetParent(CurrentSlot.transform, false);
        transform.position = transform.parent.position;
        //transform.DOMove(transform.parent.position, .35f);
    }

    public void OnBeginDrag(PointerEventData eventData) {
        var parentSlot = transform.parent.GetComponent<Slot>();
        if (parentSlot != null) {
            parentSlot.Detach(this);
        }

        _canvasGroup.blocksRaycasts = false;
        _wasOverDropTargetPreviously = false;

        transform.DOScale(1f, .15f);
    }

    public void OnDrag(PointerEventData eventData) {
        transform.position = Input.mousePosition;

        var slot = eventData.hovered.FirstOrDefault(g => g.GetComponent<Slot>() != null);
        var hoveringAboveDropTarget = slot != null;

        if (!_wasOverDropTargetPreviously && hoveringAboveDropTarget) {
            _currentTween = transform.DOScale(1.2f, .45f).SetLoops(-1, LoopType.Yoyo);
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

        MoveBackToParent();
    }
}

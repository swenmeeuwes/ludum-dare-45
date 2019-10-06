using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SellSlot : Slot
{
    [SerializeField] private Image _mirrorImage;

    public override void OnAddContent(Draggable draggable) {
        base.OnAddContent(draggable);

        var imageOfContent = Content.GetComponent<Image>();
        if (imageOfContent == null) {
            return;
        }

        if (IsFilled) {
            _mirrorImage.sprite = imageOfContent.sprite;
            _mirrorImage.DOFade(1, .35f);
        } else {
            _mirrorImage.DOFade(0, .35f);
        }
    }

    public override void OnRemoveContent(Draggable draggable) {
        base.OnRemoveContent(draggable);

        _mirrorImage.DOFade(0, .35f);
    }
}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watch : MonoBehaviour {
    [SerializeField] private RectTransform _content;
    [SerializeField] private CanvasGroup _canvasGroup;

    [SerializeField] private float _startAngle;
    [SerializeField] private float _endAngle;
    [SerializeField] private float _availableTime;

    public CanvasGroup CanvasGroup { get { return _canvasGroup; } }

    private void Start() {
        _content.rotation = Quaternion.AngleAxis(_startAngle, Vector3.forward);
        _content.DORotate(new Vector3(0, 0, _endAngle), _availableTime, RotateMode.FastBeyond360);
    }
}
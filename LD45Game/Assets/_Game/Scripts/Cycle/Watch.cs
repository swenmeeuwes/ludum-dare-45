﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watch : MonoBehaviour {
    [SerializeField] private RectTransform _content;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private CanvasGroup _nightFader;

    [SerializeField] private float _startAngle;
    [SerializeField] private float _endAngle;
    [SerializeField] private float _nightFaderOpacityTarget = 200f;

    public CanvasGroup CanvasGroup { get { return _canvasGroup; } }

    private void Start() {
        Reset();
    }

    private void Reset() {
        _nightFader.alpha = 0;
        _content.rotation = Quaternion.AngleAxis(_startAngle, Vector3.forward);
    }

    public void Run(float availableTimeInSeconds) {
        Reset();
        
        _content.DORotate(new Vector3(0, 0, _endAngle), availableTimeInSeconds, RotateMode.FastBeyond360);
        _nightFader.DOFade(_nightFaderOpacityTarget / 255f, availableTimeInSeconds * 0.66f).SetDelay(availableTimeInSeconds * 0.33f);
    }
}
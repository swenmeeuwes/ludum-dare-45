﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Npc : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _thoughCloud;
    [SerializeField] private CanvasGroup _thoughCloudCanvasGroup;
    [SerializeField] private Image _thoughCloudImage;

    private NpcModel _model;
    public NpcModel Model {
        get { return _model; }
        set {
            _model = value;

            var wantedItemSprite = ItemFactory.Instance.GetSpriteFor(_model.WantedItem);
            _thoughCloudImage.sprite = wantedItemSprite;
        }
    }

    public Transform ShopWaypoint { get; set; }
    public Transform LeaveWaypoint { get; set; }

    private void Start() {
        _thoughCloudCanvasGroup.alpha = 0;
        _thoughCloud.DOFade(0, .0001f);
    }

    public void Activate() {
        if (Model.NpcType == Type.Buying) {
            ShowThoughtCloud(1f);
        }

        DOTween.Sequence()
            .Append(transform.DOMove(ShopWaypoint.position, 5f))
            .AppendCallback(() => {
                BarterController.Instance.ShowSellDialog(_model.WantedItem);
            });
    }

    private void ShowThoughtCloud(float delay) {
        _thoughCloud.DOFade(1, .45f).SetDelay(delay);
        _thoughCloudCanvasGroup.DOFade(1, .45f).SetDelay(delay);
    }

    private void Leave() {
        DOTween.Sequence()
            .Append(transform.DOMove(LeaveWaypoint.position, 2.5f))
            .AppendCallback(() => { Destroy(gameObject); });
    }

    public enum Type {
        Buying = 0,
        Selling = 1,
        Wandering = 2
    }
}

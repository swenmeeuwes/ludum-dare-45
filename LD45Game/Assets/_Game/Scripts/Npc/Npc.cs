using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Npc : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _thoughCloud;
    [SerializeField] private CanvasGroup _thoughCloudCanvasGroup;
    [SerializeField] private Image _thoughCloudImage;
    [SerializeField] private SpriteRenderer _holdingItem;

    private NpcModel _model;
    public NpcModel Model {
        get { return _model; }
        set {
            _model = value;

            var wantedItemSprite = ItemFactory.Instance.GetSpriteFor(_model.Item);
            _thoughCloudImage.sprite = wantedItemSprite;
        }
    }

    public Transform ShopWaypoint { get; set; }
    public Transform LeaveWaypoint { get; set; }

    private void Start() {
        _thoughCloudCanvasGroup.alpha = 0;
        _thoughCloud.DOFade(0, 0);
        _holdingItem.DOFade(0, 0);
    }

    public void Activate() {
        if (Model.NpcType == Type.Buying) {
            ShowThoughtCloud(1f);

            DOTween.Sequence()
                .Append(transform.DOMove(ShopWaypoint.position, 5f))
                .AppendCallback(() => {
                    if (!Inventory.Instance.HasItemForSale(_model.Item)) {
                        HideThoughtCloud();
                        Leave();
                        return;
                    }
                    BarterController.Instance.ShowSellDialog(this);
                });
        } else if (Model.NpcType == Type.Selling) {
            ShowHoldingItem(_model.Item, .3f);
            DOTween.Sequence()
                .Append(transform.DOMove(ShopWaypoint.position, 5f))
                .AppendCallback(() => {
                    if (!Inventory.Instance.HasSpace) {
                        Leave();
                        return;
                    }
                    BarterController.Instance.ShowBuyDialog(this);
                });
        }
    }

    public void FinishTrade() {
        HideThoughtCloud();
        HideHoldingItem();
        Leave();
    }

    private void ShowThoughtCloud(float delay = 0f) {
        _thoughCloud.DOFade(1, .45f).SetDelay(delay);
        _thoughCloudCanvasGroup.DOFade(1, .45f).SetDelay(delay);
    }

    private void HideThoughtCloud(float delay = 0f) {
        _thoughCloud.DOFade(0, .45f).SetDelay(delay);
        _thoughCloudCanvasGroup.DOFade(0, .45f).SetDelay(delay);
    }

    private void ShowHoldingItem(ItemData.Type itemType, float delay = 0f) {
        var itemData = ItemFactory.Instance.GetDataFor(itemType);
        ShowHoldingItem(itemData, delay);
    }

    private void ShowHoldingItem(ItemData itemData, float delay = 0f) {
        _holdingItem.sprite = itemData.sprite;
        _holdingItem.DOFade(1, .45f).SetDelay(delay);
    }

    private void HideHoldingItem(float delay = 0f) {
        _holdingItem.DOFade(0, .45f).SetDelay(delay);
    }

    private void Leave() {
        GameController.Instance.OnNpcLeave(this);

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

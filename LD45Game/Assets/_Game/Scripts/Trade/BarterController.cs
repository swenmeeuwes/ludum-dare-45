using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarterController : MonoBehaviour
{
    [SerializeField] private CanvasGroup _panelCanvasGroup;
    [SerializeField] private TMP_InputField _amountInputField;
    [SerializeField] private Button _increaseAmountButton;
    [SerializeField] private Button _decreaseAmountButton;
    [SerializeField] private Button _barterButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Image _itemImage;
    [SerializeField] private Image _npcImage;
    [SerializeField] private TMP_Text _npcTextField;
    [SerializeField] private TMP_Text _actionTextField;

    [SerializeField] private string[] _sellingDialogs;
    [SerializeField] private string[] _tooExpensiveDialogs;
    [SerializeField] private string[] _wayTooExpensiveDialogs;
    [SerializeField] private string[] _tooManyOfferIterationsDialogs;
    [SerializeField] private string[] _soldDialogs;
    [SerializeField] private string _itemNameInterpolator = "%item%";
    [SerializeField] private string _priceInterpolator = "%price%";
    [SerializeField] private string _newPriceInterpolator = "%newprice%";

    public static BarterController Instance { get; private set; }

    public Action CurrentAction { get; private set; }
    public Npc CurrentNpc { get; private set; }
    public NpcModel CurrentNpcModel { get; private set; }
    public ItemData CurrentItemData { get; private set; }
    public int CurrentOfferIteration { get; private set; }
    public int CurrentAcceptAmountByNpc { get; private set; }
    public int? FirstOffer { get; private set; }

    private bool _userCanInput;
    public bool UserCanInput {
        get { return _userCanInput; }
        set {
            _userCanInput = value;
            UpdateButtonStates();
        }
    }

    public int? CurrentOfferAmount {
        get {
            try {
                return int.Parse(_amountInputField.text);
            } catch (FormatException e) {
                return null;
            }
        }
        set {
            _amountInputField.text = value.ToString();
        }
    }

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        _panelCanvasGroup.alpha = 0;
        CurrentOfferAmount = 0;

        _panelCanvasGroup.gameObject.SetActive(false);
    }
    public void OnInputAmountValueChange() {
        if (CurrentOfferAmount < 0) {
            CurrentOfferAmount = 0;
        }

        if (FirstOffer.HasValue && CurrentOfferAmount > FirstOffer) {
            CurrentOfferAmount = FirstOffer;
        }

        UpdateButtonStates();
    }

    public void HandleIncreaseAmountButton() {
        if (CurrentOfferAmount.HasValue) {
            CurrentOfferAmount++;
        }
    }

    public void HandleDecreaseAmountButton() {
        if (CurrentOfferAmount.HasValue) {
            CurrentOfferAmount--;
        }
    }

    public void HandleBarterButton() {
        if (CurrentOfferIteration == 0) {
            FirstOffer = CurrentOfferAmount.Value;
        }

        CurrentOfferIteration++;

        if (CurrentAction == Action.Selling) {
            if (CurrentOfferAmount > CurrentNpcModel.AmountThresholdForLeaving) {
                // Too cheap/ expensive -> leaving instantly
                _npcTextField.text = InterpolateText(_wayTooExpensiveDialogs[UnityEngine.Random.Range(0, _wayTooExpensiveDialogs.Length)]);
                UserCanInput = false;

                // FinishDialog(3f); // Disabled automatic popup closing for now, user should press the 'X' button (gives the users time to read)
                return;
            }

            if (CurrentOfferIteration > CurrentNpcModel.AmountOfOffers) {
                // Too much offer iterations
                _npcTextField.text = InterpolateText(_tooManyOfferIterationsDialogs[UnityEngine.Random.Range(0, _tooManyOfferIterationsDialogs.Length)]);
                UserCanInput = false;

                return;
            }

            if (CurrentOfferAmount <= CurrentAcceptAmountByNpc) {
                // We have a deal!
                _npcTextField.text = InterpolateText(_soldDialogs[UnityEngine.Random.Range(0, _soldDialogs.Length)]);

                var sellSuccess = Inventory.Instance.SellItem(CurrentItemData.type);
                if (sellSuccess) {
                    GameController.Instance.Money += CurrentOfferAmount.Value;
                } else {
                    _npcTextField.text = InterpolateText("Where did the %item% go? You cannot just simply remove items from your display that you are selling!");
                }

                UserCanInput = false;

                return;
            }

            // Else we negotiate
            var deltaNegotiation = CurrentOfferAmount.Value - CurrentAcceptAmountByNpc;
            if (deltaNegotiation > CurrentNpcModel.MaxOfferIncrement) {
                deltaNegotiation = CurrentNpcModel.MaxOfferIncrement;
            } else {
                deltaNegotiation /= 2;
            }

            CurrentAcceptAmountByNpc += deltaNegotiation;
            CurrentOfferAmount = CurrentAcceptAmountByNpc;

            _npcTextField.text = InterpolateText(_tooExpensiveDialogs[UnityEngine.Random.Range(0, _tooExpensiveDialogs.Length)]);
        }
    }

    public void ShowSellDialog(Npc npc) {
        _panelCanvasGroup.gameObject.SetActive(true);
        FirstOffer = null;

        CurrentOfferIteration = 0; // todo: refactor to reset method along with setting currentnpc to null

        CurrentAction = Action.Selling;
        CurrentNpc = npc;
        CurrentNpcModel = npc.Model;

        CurrentAcceptAmountByNpc = CurrentNpcModel.InitialOfferAmount;
        CurrentOfferAmount = CurrentNpcModel.InitialOfferAmount;

        CurrentItemData = ItemFactory.Instance.GetDataFor(CurrentNpcModel.WantedItem);
        CurrentOfferAmount = CurrentNpcModel.InitialOfferAmount;

        _actionTextField.text = "Selling";
        _itemImage.sprite = CurrentItemData.sprite;

        var npcText = InterpolateText(_sellingDialogs[UnityEngine.Random.Range(0, _sellingDialogs.Length)]);

        _npcTextField.text = npcText;

        _amountInputField.Select();

        _panelCanvasGroup.DOFade(1, .35f);

        UserCanInput = true;
    }

    public void FinishDialog(float delay = 0f) {
        _panelCanvasGroup.DOFade(0, .45f).SetDelay(delay);

        DOTween.Sequence()
            .SetDelay(delay + .45f + .55f)
            .OnComplete(() => {
                CurrentNpc.FinishTrade();
            });
    }

    private string InterpolateText(string text) {
        return text
            .Replace(_itemNameInterpolator, CurrentItemData.displayName)
            .Replace(_priceInterpolator, CurrentOfferAmount.ToString());
    }

    private void UpdateButtonStates() {
        if (!UserCanInput) {
            _barterButton.interactable = false;
            _decreaseAmountButton.interactable = false;
            _increaseAmountButton.interactable = false;
            _amountInputField.interactable = false;

            return;
        }

        _barterButton.interactable = CurrentOfferAmount.HasValue;
        _decreaseAmountButton.interactable = CurrentOfferAmount.Value > 0;
        _increaseAmountButton.interactable = FirstOffer.HasValue ? CurrentOfferAmount.Value < FirstOffer : CurrentOfferAmount.Value < 99999;
        _amountInputField.interactable = true;
    }

    public enum Action {
        Selling = 0,
        Buying = 1
    }
}

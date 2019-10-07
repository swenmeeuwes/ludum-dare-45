using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarterController : MonoBehaviour {
    [SerializeField] private CanvasGroup _panelCanvasGroup;
    [SerializeField] private TMP_InputField _amountInputField;
    [SerializeField] private Button _increaseAmountButton;
    [SerializeField] private Button _decreaseAmountButton;
    [SerializeField] private Button _barterButton;
    [SerializeField] private TMP_Text _barterButtonTextField;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Image _itemImage;
    [SerializeField] private Image _npcImage;
    [SerializeField] private TMP_Text _npcTextField;
    [SerializeField] private TMP_Text _actionTextField;

    [SerializeField] private string[] _buyingDialogs;
    [SerializeField] private string[] _tooCheapDialogs;
    [SerializeField] private string[] _wayTooCheapDialogs;
    [SerializeField] private string[] _boughtDialogs;

    [SerializeField] private string[] _sellingDialogs;
    [SerializeField] private string[] _tooExpensiveDialogs;
    [SerializeField] private string[] _wayTooExpensiveDialogs;
    [SerializeField] private string[] _soldDialogs;

    [SerializeField] private string[] _tooManyOfferIterationsDialogs;

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
    public int? PreviousOffer { get; private set; }

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

        UpdateButtonStates();
    }

    public void OnInputAmountValueChangeEnd() {
        if (CurrentOfferAmount < 0) {
            CurrentOfferAmount = 0;
        }

        if (CurrentOfferAmount > GameController.Instance.Money) {
            CurrentOfferAmount = GameController.Instance.Money;
        }

        if (CurrentAction == Action.Selling) {
            if (PreviousOffer.HasValue && CurrentOfferAmount > PreviousOffer) {
                CurrentOfferAmount = PreviousOffer;
            }
        } else if (CurrentAction == Action.Buying) {
            if (PreviousOffer.HasValue && CurrentOfferAmount < PreviousOffer) {
                CurrentOfferAmount = PreviousOffer;
            }
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
        Debug.LogFormat("Offered {0} coins", CurrentOfferAmount.Value);

        PreviousOffer = CurrentOfferAmount.Value;

        CurrentOfferIteration++;

        if (CurrentAction == Action.Selling) {
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

            // Else we negotiate
            var deltaNegotiation = CurrentOfferAmount.Value - CurrentAcceptAmountByNpc;

            // Small chance to let the npc take bigger steps
            // TODO: should we hook this chance to the NpcModel?
            if (UnityEngine.Random.value > .1f) {
                // Proceed the "normal way" of bartering
                if (deltaNegotiation > CurrentNpcModel.MaxOfferIncrement) {
                    deltaNegotiation = CurrentNpcModel.MaxOfferIncrement;
                } else {
                    deltaNegotiation /= 2;
                }
            } else {
                Debug.Log("Random event: NPC takes bigger barter step!");
            }

            CurrentAcceptAmountByNpc += deltaNegotiation;
            CurrentOfferAmount = CurrentAcceptAmountByNpc;

            _npcTextField.text = InterpolateText(_tooExpensiveDialogs[UnityEngine.Random.Range(0, _tooExpensiveDialogs.Length)]);
        } else if (CurrentAction == Action.Buying) {
            if (CurrentOfferAmount >= CurrentAcceptAmountByNpc) {
                // We have a deal!
                _npcTextField.text = InterpolateText(_boughtDialogs[UnityEngine.Random.Range(0, _boughtDialogs.Length)]);

                var buySuccess = Inventory.Instance.AddItem(CurrentItemData);
                if (buySuccess) {
                    GameController.Instance.Money -= CurrentOfferAmount.Value;
                } else {
                    _npcTextField.text = InterpolateText("You do not have space for %item%? Okay...");
                }

                UserCanInput = false;

                return;
            }

            if (CurrentOfferAmount < CurrentNpcModel.AmountThresholdForLeaving) {
                // Too cheap -> leaving instantly
                _npcTextField.text = InterpolateText(_wayTooCheapDialogs[UnityEngine.Random.Range(0, _wayTooCheapDialogs.Length)]);
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

            // Else we negotiate
            var deltaNegotiation = CurrentAcceptAmountByNpc - CurrentOfferAmount.Value;
            if (deltaNegotiation > CurrentNpcModel.MaxOfferIncrement) {
                deltaNegotiation = CurrentNpcModel.MaxOfferIncrement;
            } else {
                deltaNegotiation /= 2;
            }

            CurrentAcceptAmountByNpc -= deltaNegotiation;
            CurrentOfferAmount = CurrentAcceptAmountByNpc;

            _npcTextField.text = InterpolateText(_tooCheapDialogs[UnityEngine.Random.Range(0, _tooCheapDialogs.Length)]);
        }
    }

    public void ShowSellDialog(Npc npc) {
        PrepareDialog();

        CurrentAction = Action.Selling;
        CurrentNpc = npc;
        CurrentNpcModel = npc.Model;

        CurrentAcceptAmountByNpc = CurrentNpcModel.InitialOfferAmount;
        CurrentOfferAmount = CurrentNpcModel.InitialOfferAmount;

        CurrentItemData = ItemFactory.Instance.GetDataFor(CurrentNpcModel.Item);
        CurrentOfferAmount = CurrentNpcModel.InitialOfferAmount;

        _actionTextField.text = "Selling";
        _itemImage.sprite = CurrentItemData.sprite;

        var npcText = InterpolateText(_sellingDialogs[UnityEngine.Random.Range(0, _sellingDialogs.Length)]);

        _npcTextField.text = npcText;

        _amountInputField.Select();

        _panelCanvasGroup.DOFade(1, .35f);

        UserCanInput = true;
    }

    public void ShowBuyDialog(Npc npc) {
        PrepareDialog();

        CurrentAction = Action.Buying;
        CurrentNpc = npc;
        CurrentNpcModel = npc.Model;

        CurrentAcceptAmountByNpc = CurrentNpcModel.InitialOfferAmount;
        CurrentOfferAmount = CurrentNpcModel.InitialOfferAmount;

        CurrentItemData = ItemFactory.Instance.GetDataFor(CurrentNpcModel.Item);
        CurrentOfferAmount = CurrentNpcModel.InitialOfferAmount;

        _actionTextField.text = "Buying";
        _itemImage.sprite = CurrentItemData.sprite;

        var npcText = InterpolateText(_buyingDialogs[UnityEngine.Random.Range(0, _buyingDialogs.Length)]);

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

    private void PrepareDialog() {
        _panelCanvasGroup.gameObject.SetActive(true);
        PreviousOffer = null;

        CurrentOfferIteration = 0;
    }

    private string InterpolateText(string text) {
        return text
            .Replace(_itemNameInterpolator, CurrentItemData.displayName)
            .Replace(_priceInterpolator, CurrentOfferAmount.ToString());
    }

    private void UpdateButtonStates() {
        _barterButtonTextField.text = CurrentOfferAmount == CurrentAcceptAmountByNpc ?
            (CurrentAction == Action.Selling ? "Sell" : "Buy") : "Barter";

        if (!UserCanInput) {
            _barterButton.interactable = false;
            _decreaseAmountButton.interactable = false;
            _increaseAmountButton.interactable = false;
            _amountInputField.interactable = false;

            return;
        }

        _barterButton.interactable = CurrentOfferAmount.HasValue;
        _amountInputField.interactable = true;

        Debug.LogFormat("Current offer amount: {0}\nPrevious offer: {1}\nAccepted amount: {2}", CurrentOfferAmount, PreviousOffer, CurrentAcceptAmountByNpc);
        
        if (CurrentAction == Action.Selling) {
            _decreaseAmountButton.interactable = CurrentOfferAmount.HasValue ? CurrentOfferAmount.Value > CurrentAcceptAmountByNpc
                : (CurrentOfferAmount.HasValue ? CurrentOfferAmount.Value > 0 : false);
            _increaseAmountButton.interactable = PreviousOffer.HasValue && CurrentOfferAmount.HasValue ? CurrentOfferAmount.Value < PreviousOffer
                : (CurrentOfferAmount.HasValue ? CurrentOfferAmount.Value < 99999 : false);
        } else if (CurrentAction == Action.Buying) {
            _decreaseAmountButton.interactable = PreviousOffer.HasValue && CurrentOfferAmount.HasValue ? CurrentOfferAmount.Value > PreviousOffer 
                : (CurrentOfferAmount.HasValue ? CurrentOfferAmount.Value > 0 : false);
            _increaseAmountButton.interactable = CurrentOfferAmount.HasValue ? CurrentOfferAmount.Value < CurrentAcceptAmountByNpc
                : (CurrentOfferAmount.HasValue ? CurrentOfferAmount.Value < 9999 : false);
        }
    }

    public enum Action {
        Selling = 0,
        Buying = 1
    }
}

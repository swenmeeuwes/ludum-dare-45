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
    [SerializeField] private Image _itemImage;
    [SerializeField] private Image _npcImage;
    [SerializeField] private TMP_Text _npcTextField;
    [SerializeField] private TMP_Text _actionTextField;

    public static BarterController Instance { get; private set; }

    public int? CurrentAmount {
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
        CurrentAmount = 0;
    }

    public void OnInputAmountValueChange() {
        if (!CurrentAmount.HasValue) {
            _barterButton.interactable = false;
            return;
        }

        if (CurrentAmount < 0) {
            CurrentAmount = 0;
        }

        _decreaseAmountButton.interactable = CurrentAmount.Value > 0;
    }

    public void HandleIncreaseAmountButton() {
        if (CurrentAmount.HasValue) {
            CurrentAmount++;
        }
    }

    public void HandleDecreaseAmountButton() {
        if (CurrentAmount.HasValue) {
            CurrentAmount--;
        }
    }

    public void HandleBarterButton() {

    }

    public void ShowSellDialog(ItemData.Type itemType) {
        ShowSellDialog(ItemFactory.Instance.GetDataFor(itemType));
    }

    public void ShowSellDialog(ItemData itemData) {
        _actionTextField.text = "Selling";
        _itemImage.sprite = itemData.sprite;

        _amountInputField.Select();

        _panelCanvasGroup.DOFade(1, .35f);
    }
}

using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndOfDayPanelController : MonoBehaviour {
    [SerializeField] private CanvasGroup _overlayCanvasGroup;
    [SerializeField] private CanvasGroup _panelCanvasGroup;

    [SerializeField] private CanvasGroup _boughtCanvasGroup;
    [SerializeField] private CanvasGroup _soldCanvasGroup;
    [SerializeField] private CanvasGroup _earnedCanvasGroup;
    [SerializeField] private CanvasGroup _buttonsCanvasGroup;

    [SerializeField] private TMP_Text _headerTextField;
    [SerializeField] private TMP_Text _itemBoughtTextField;
    [SerializeField] private TMP_Text _itemsSoldTextField;
    [SerializeField] private TMP_Text _earnedTextField;

    [SerializeField] private Button _buyHomeButton;
    [SerializeField] private TMP_Text _buyHomeButtonTextField;

    public static EndOfDayPanelController Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        _overlayCanvasGroup.alpha = 0;
        _panelCanvasGroup.alpha = 0;

        _overlayCanvasGroup.gameObject.SetActive(false);
        _panelCanvasGroup.gameObject.SetActive(false);
    }

    public void Show(int itemsBought, int itemsSold, int earned, int day) {
        _overlayCanvasGroup.gameObject.SetActive(true);
        _panelCanvasGroup.gameObject.SetActive(true);

        _boughtCanvasGroup.alpha = 0;
        _soldCanvasGroup.alpha = 0;
        _earnedCanvasGroup.alpha = 0;

        _headerTextField.text = "End of day " + day.ToString();
        _itemBoughtTextField.text = itemsBought.ToString();
        _itemsSoldTextField.text = itemsSold.ToString();
        _earnedTextField.text = earned.ToString();

        _buyHomeButtonTextField.text = "Buy house for " + GameController.Instance.MoneyGoal;
        _buyHomeButton.interactable = GameController.Instance.Money >= GameController.Instance.MoneyGoal;

        DOTween.Sequence()
            .Append(_overlayCanvasGroup.DOFade(1, .25f))
            .Append(_panelCanvasGroup.DOFade(1, .45f))
            .Append(_boughtCanvasGroup.DOFade(1, .45f))
            .Append(_soldCanvasGroup.DOFade(1, .45f))
            .Append(_earnedCanvasGroup.DOFade(1, .45f))
            .Append(_buttonsCanvasGroup.DOFade(1, .45f));
    }

    public void HandleContinueButton() {
        DOTween.Sequence()
            .AppendCallback(() => {
                GameController.Instance.StartNextDay();
            })
            .Append(_panelCanvasGroup.DOFade(0, .45f))
            .Append(_overlayCanvasGroup.DOFade(0, .25f))
            .AppendCallback(() => {
                _overlayCanvasGroup.gameObject.SetActive(false);
                _panelCanvasGroup.gameObject.SetActive(false);
            });
    }

    public void HandleBuyHomeButton() {
        OutroController.Instance.Show();
    }
}
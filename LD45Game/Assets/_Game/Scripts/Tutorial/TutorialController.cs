using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TutorialController : MonoBehaviour {
    [SerializeField] private CanvasGroup _fader;
    [SerializeField] private CanvasGroup _speechBalloon;
    [SerializeField] private TMP_Text _speechBalloonTextField;
    [SerializeField] private RectTransform _cat;
    [SerializeField] private MoveSlightlyToMouse _catEyes;

    [SerializeField] private Watch _watch;
    [SerializeField] private Inventory _inventory;
    [SerializeField] private CanvasGroup _moneyDisplay;
    [SerializeField] private CanvasGroup _storageSlotsDisplay;
    [SerializeField] private CanvasGroup _sellSlotsDisplay;

    [SerializeField] private ItemData _itemAddedByTutorial;
    public ItemData ItemAddedByTutorial { get { return _itemAddedByTutorial; } }

    private bool _moneyWasAwarded = false;
    private bool _tutorialItemWasAwarded = false;

    private Vector3 _originalCatPosition;

    public bool IsActive { get; set; }

    private void Start() {
        Setup();
    }

    public void Play() {
        StartCoroutine(TutorialSequence());
    }

    private void Setup() {
        _fader.alpha = 0;
        _speechBalloon.alpha = 0;
        _originalCatPosition = _cat.transform.position;
        _catEyes.enabled = false;

        _watch.CanvasGroup.alpha = 0;
        _moneyDisplay.alpha = 0;
        _storageSlotsDisplay.alpha = 0;
        _sellSlotsDisplay.alpha = 0;

        _cat.transform.position += new Vector3(0, -250, 0);
    }

    public void Skip() {
        _fader.DOFade(0, .25f);
        _speechBalloon.DOFade(0, .25f);
        _originalCatPosition = _cat.transform.position;
        _catEyes.enabled = false;

        _watch.CanvasGroup.DOFade(1, .35f);
        _moneyDisplay.DOFade(1, .35f);
        _storageSlotsDisplay.DOFade(1, .35f);
        _sellSlotsDisplay.DOFade(1, .35f);

        _cat.transform.position = _originalCatPosition + new Vector3(0, -250, 0);

        if (!_tutorialItemWasAwarded) {
            var tutorialItem = ItemFactory.Instance.CreateFromData(_itemAddedByTutorial);
            _inventory.AddItem(tutorialItem);
            _tutorialItemWasAwarded = true;
        }

        if (!_moneyWasAwarded) {
            GameController.Instance.Money++;
            _moneyWasAwarded = true;
        }

        StopAllCoroutines();
        IsActive = false;
    }

    private IEnumerator TutorialSequence() {
        IsActive = true;

        _fader.DOFade(1, .45f);
        yield return new WaitForSeconds(.45f);

        _cat.DOMove(_originalCatPosition, .45f, true);
        yield return new WaitForSeconds(.45f);

        _catEyes.enabled = true;
        _catEyes.UpdateOriginalPosition(_cat.transform.position);

        _speechBalloonTextField.text = "Oh hello, nice to find you here homan!";
        _speechBalloon.DOFade(1, .45f);
        yield return new WaitForSeconds(.45f + 3f);

        _speechBalloon.DOFade(0, .45f);
        yield return new WaitForSeconds(.45f);

        _speechBalloonTextField.text = "Strong homan help me, yes?";
        _speechBalloon.DOFade(1, .45f);
        yield return new WaitForSeconds(.45f + 3f);

        _speechBalloon.DOFade(0, .45f);
        yield return new WaitForSeconds(.45f);

        _speechBalloonTextField.text = "We trade goods together!";
        _speechBalloon.DOFade(1, .45f);
        yield return new WaitForSeconds(.45f + 3f);

        _speechBalloon.DOFade(0, .45f);
        yield return new WaitForSeconds(.45f);

        // WATCH

        _speechBalloonTextField.text = "We can trade as long as it is day.";
        _speechBalloon.DOFade(1, .45f);
        yield return new WaitForSeconds(.45f + 3f);

        _speechBalloon.DOFade(0, .45f);
        yield return new WaitForSeconds(.45f);

        _speechBalloonTextField.text = "Let me show you watch, very useful for time.";
        _speechBalloon.DOFade(1, .45f);
        yield return new WaitForSeconds(.45f + .5f);

        _watch.CanvasGroup.DOFade(1, .45f);
        _watch.transform.DOPunchScale(Vector3.one * .1f, .45f);

        yield return new WaitForSeconds(1.8f);

        _speechBalloon.DOFade(0, .45f);
        yield return new WaitForSeconds(.45f);

        _cat.DOShakeAnchorPos(.55f, 15, 20);
        _speechBalloonTextField.text = "WHAT?! Dirty paws you say?";
        _speechBalloon.DOFade(1, .45f);
        yield return new WaitForSeconds(.45f + 3f);

        _speechBalloon.DOFade(0, .45f);
        yield return new WaitForSeconds(.45f);

        _speechBalloonTextField.text = "Those aren't mine!";
        _speechBalloon.DOFade(1, .45f);
        yield return new WaitForSeconds(.45f + 3f);

        _speechBalloon.DOFade(0, .45f);
        yield return new WaitForSeconds(.45f);

        _speechBalloonTextField.text = "When the moon is at the top we'll take a nap.";
        _speechBalloon.DOFade(1, .45f);
        yield return new WaitForSeconds(.45f + 5f);

        _speechBalloon.DOFade(0, .45f);
        yield return new WaitForSeconds(.45f);

        // END OF WATCH

        // START OF MONEY

        _speechBalloonTextField.text = "In the top left corner we keep track of the shinies.";
        _speechBalloon.DOFade(1, .45f);
        yield return new WaitForSeconds(.45f + 4f);

        _speechBalloon.DOFade(0, .45f);
        yield return new WaitForSeconds(.45f);

        _moneyDisplay.DOFade(1, .45f);
        _moneyDisplay.transform.DOPunchScale(Vector3.one * .1f, .45f);

        yield return new WaitForSeconds(2f);

        _speechBalloonTextField.text = "I'll add the shiny that the kind trader homan gave us.";
        _speechBalloon.DOFade(1, .45f);
        yield return new WaitForSeconds(.45f + 4f);

        GameController.Instance.Money++;
        _moneyWasAwarded = true;

        _speechBalloon.DOFade(0, .45f);
        yield return new WaitForSeconds(.45f);

        yield return new WaitForSeconds(2f);

        // END OF MONEY

        // START OF INVENTORY

        _speechBalloonTextField.text = "We'll need a place to store our items.";
        _speechBalloon.DOFade(1, .45f);
        yield return new WaitForSeconds(.45f + 4f);

        _speechBalloon.DOFade(0, .45f);
        yield return new WaitForSeconds(.45f);

        _speechBalloonTextField.text = "BOOM, storage.";
        _speechBalloon.DOFade(1, .45f);
        _storageSlotsDisplay.DOFade(1, .45f);
        _storageSlotsDisplay.transform.DOPunchScale(Vector3.one * .1f, .45f);
        yield return new WaitForSeconds(.45f + 4f);

        _speechBalloon.DOFade(0, .45f);
        yield return new WaitForSeconds(.45f);


        _speechBalloonTextField.text = "This storage holds items that are not on display.";
        _speechBalloon.DOFade(1, .45f);
        yield return new WaitForSeconds(.45f + 4f);

        _speechBalloon.DOFade(0, .45f);
        yield return new WaitForSeconds(.45f);

        _speechBalloonTextField.text = "We can use this to stockpile items that might increase in price, as these items are not for sale";
        _speechBalloon.DOFade(1, .45f);
        yield return new WaitForSeconds(.45f + 7f);

        _speechBalloon.DOFade(0, .45f);
        yield return new WaitForSeconds(.45f);

        _speechBalloonTextField.text = "I've found this item that was left behind by some travelers.";
        _speechBalloon.DOFade(1, .45f);
        yield return new WaitForSeconds(.45f + 5f);

        _speechBalloon.DOFade(0, .45f);
        yield return new WaitForSeconds(.45f);

        var tutorialItem = ItemFactory.Instance.CreateFromData(_itemAddedByTutorial);
        _inventory.AddItem(tutorialItem);
        _tutorialItemWasAwarded = true;

        yield return new WaitForSeconds(2f);

        _speechBalloonTextField.text = "Can you see the item in our storage?";
        _speechBalloon.DOFade(1, .45f);
        yield return new WaitForSeconds(.45f + 5f);

        _speechBalloon.DOFade(0, .45f);
        yield return new WaitForSeconds(.45f);

        // SELL

        _speechBalloonTextField.text = "Above our storage are the items that are on display and will be sold.";
        _speechBalloon.DOFade(1, .45f);
        _sellSlotsDisplay.DOFade(1, .45f);
        _sellSlotsDisplay.transform.DOPunchScale(Vector3.one * .1f, .45f);
        yield return new WaitForSeconds(.45f + 4f);

        _speechBalloon.DOFade(0, .45f);
        yield return new WaitForSeconds(.45f);

        _speechBalloonTextField.text = "Other homans that walk by might be interested in these items.";
        _speechBalloon.DOFade(1, .45f);
        yield return new WaitForSeconds(.45f + 3f);

        _speechBalloon.DOFade(0, .45f);
        yield return new WaitForSeconds(.45f);

        _speechBalloonTextField.text = "Interested homans will barter for the item.";
        _speechBalloon.DOFade(1, .45f);
        yield return new WaitForSeconds(.45f + 3f);

        _speechBalloon.DOFade(0, .45f);
        yield return new WaitForSeconds(.45f);

        _speechBalloonTextField.text = "If we come to an agreement the item will be sold.";
        _speechBalloon.DOFade(1, .45f);
        yield return new WaitForSeconds(.45f + 3f);

        _speechBalloon.DOFade(0, .45f);
        yield return new WaitForSeconds(.45f);

        _speechBalloonTextField.text = "Try displaying the item in our storage.";
        _speechBalloon.DOFade(1, .45f);
        yield return new WaitForSeconds(.45f + 3f);

        _speechBalloon.DOFade(0, .45f);
        yield return new WaitForSeconds(.45f);

        _speechBalloonTextField.text = "List the item for sale by dragging the item from the storage to display slot.";
        _speechBalloon.DOFade(1, .45f);
        yield return new WaitUntil(() => _inventory.ItemsOnDisplay.Count() > 0);

        _speechBalloon.DOFade(0, .45f);
        yield return new WaitForSeconds(.45f);
        

        _speechBalloonTextField.text = "Great work!";
        _speechBalloon.DOFade(1, .45f);
        yield return new WaitForSeconds(.45f + 3f);

        _speechBalloon.DOFade(0, .45f);
        yield return new WaitForSeconds(.45f);

        // END OF INVENTORY

        _speechBalloonTextField.text = "Homan, understand everything?";
        _speechBalloon.DOFade(1, .45f);
        yield return new WaitForSeconds(.45f + 3f);

        _speechBalloon.DOFade(0, .45f);
        yield return new WaitForSeconds(.45f);

        _speechBalloonTextField.text = "Great! Homan is such a quick learner.";
        _speechBalloon.DOFade(1, .45f);
        yield return new WaitForSeconds(.45f + 3f);

        _speechBalloon.DOFade(0, .45f);
        yield return new WaitForSeconds(.45f);

        _cat.DOShakeAnchorPos(.55f, 15, 20);
        _speechBalloonTextField.text = "HOMAN, here comes our first client!";
        _speechBalloon.DOFade(1, .45f);
        yield return new WaitForSeconds(.45f + 3f);

        _speechBalloon.DOFade(0, .45f);
        yield return new WaitForSeconds(.45f);

        yield return new WaitForSeconds(1f);

        _catEyes.enabled = false;
        _cat.DOMove(_cat.transform.position + new Vector3(0, -250, 0), .45f);

        yield return new WaitForSeconds(.55f);

        _fader.DOFade(0, .35f);

        IsActive = false;
    }
}
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour {
    [SerializeField] private Watch _watch;
    [SerializeField] private TutorialController _tutorialController;
    [SerializeField] private TMP_Text _moneyTextField;
    [SerializeField] private int _availableTimePerDayInSeconds;

    public static GameController Instance { get; private set; }

    private int _money = 0;
    public int Money {
        get { return _money; }
        set {
            _money = value;
            _moneyTextField.text = _money.ToString("00000");
        }
    }

    public int ItemsSoldToday { get; set; }
    public int ItemsBoughtToday { get; set; }
    public int MoneyAtStartOfDay { get; set; }

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        StartCoroutine(TutorialSequence());
    }

    private void Update() {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (_tutorialController.IsActive) {
                _tutorialController.Skip();
            }
        }
#endif
    }

    private IEnumerator TutorialSequence() {
        _tutorialController.Play();

        yield return new WaitUntil(() => !_tutorialController.IsActive);

        yield return new WaitForSeconds(1.5f);

        StartDay(true);
    }

    private void StartDay(bool firstDayAfterTutorial = false) {
        _watch.Begin(_availableTimePerDayInSeconds);
        MoneyAtStartOfDay = Money;

        if (firstDayAfterTutorial) {
            // Always spawn a npc that wants the tutorial item
            var tutorialItemData = _tutorialController.ItemAddedByTutorial;
            var maxOfferAmount = (int)(tutorialItemData.worth * Random.Range(1.3f, 1.4f));

            var npc = NpcSpawner.Instance.Spawn(new NpcModel {
                NpcType = Npc.Type.Buying,
                Item = tutorialItemData.type,
                AmountOfOffers = maxOfferAmount > tutorialItemData.worth * 1.2f ? 6 : Random.Range(2, 5),
                AmountThresholdForLeaving = (int)(tutorialItemData.worth * 2f),
                InitialOfferAmount = (int)(maxOfferAmount * Random.Range(.4f, .9f)),
                MaxOfferAmount = maxOfferAmount
            }, false);

            Debug.LogFormat("Tutorial npc:\nWilling to go for: {0}\nInitial offer: {1}\nLeaves at: {2}\nMax increment: {3}", npc.Model.MaxOfferAmount, npc.Model.InitialOfferAmount, npc.Model.AmountThresholdForLeaving, npc.Model.MaxOfferIncrement);

            DOTween.Sequence()
                    .SetDelay(.85f)
                    .OnComplete(() => { npc.Activate(); });
        } else {
            var npc = SpawnNpc();
            DOTween.Sequence()
                    .SetDelay(.85f)
                    .OnComplete(() => { npc.Activate(); });
        }
    }

    public void StartNextDay() {
        StartDay();
    }

    public void OnNpcLeave(Npc npcThatLeft) {
        if (_watch.DayIsOver) {
            Debug.Log("Day is over!");
            EndDay();
            return;
        }

        var newNpc = SpawnNpc();
        var showHint = Random.value > .5f;
        if (showHint) {
            DOTween.Sequence()
                .SetDelay(.85f)
                .OnComplete(() => {
                    var showUsefullTip = Random.value > .3f;
                    if (showUsefullTip) {
                        var itemData = ItemFactory.Instance.GetDataFor(newNpc.Model.Item);
                        var tips = new List<string>();

                        if (newNpc.Model.NpcType == Npc.Type.Buying) {
                            // NPC IS BUYING FROM PLAYER
                            // Starts low
                            if (newNpc.Model.InitialOfferAmount < itemData.worth * 0.7f) tips.Add("Oh I know this homan! This homan always starts low when haggling.");

                            // Is willing to pay more that item's max worth
                            if (newNpc.Model.MaxOfferAmount > itemData.worth * 1.2f) tips.Add("That homan looks like he really wants something. He might pay over what the item is worth.");

                            // Will not iterate as much
                            if (newNpc.Model.MaxOfferAmount <= 3) tips.Add("Hmm, that human looks a little bit irritated.");
                        } else if (newNpc.Model.NpcType == Npc.Type.Selling) {
                            // NPC IS SELLING TO PLAYER
                            //if (newNpc.Model.)
                            // TODO
                        }

                        if (tips.Count > 0) {
                            HintController.Instance.ShowHint(tips[Random.Range(0, tips.Count)]);
                        } else {
                            // No usefull tip to show, show a tip about an item
                            var randomItemData = ItemFactory.Instance.GetRandomItemData();
                            var itemTips = new List<string>();
                            itemTips.Add(string.Format("A {0} generally costs around the {1} coins.", randomItemData.displayName, randomItemData.worth));
                            itemTips.Add(string.Format("The worth of a {0} is generally around the {1} coins.", randomItemData.displayName, randomItemData.worth));

                            HintController.Instance.ShowHint(itemTips[Random.Range(0, itemTips.Count)]);

                        }
                    } else {
                        HintController.Instance.ShowFiller();
                    }
                });
        }

        DOTween.Sequence()
               .SetDelay(showHint ? 2.5f : 0)
               .OnComplete(() => {
                   newNpc.Activate();
               });
    }

    private void EndDay() {
        EndOfDayPanelController.Instance.Show(ItemsBoughtToday, ItemsSoldToday, Money - MoneyAtStartOfDay);

        ItemsBoughtToday = 0;
        ItemsSoldToday = 0;
    }

    private Npc SpawnNpc(bool instantlyActivate = false) {
        Npc npc;
        ItemData itemData;
        if (Inventory.Instance.FilledSlots > 0 && Random.value > .6f) {
            // Buying from player
            itemData = GetRandomItemInInventoryOrRandom();
            var maxOfferAmount = (int)(itemData.worth * Random.Range(.8f, 1.4f));
            npc = NpcSpawner.Instance.Spawn(new NpcModel {
                NpcType = Npc.Type.Buying,
                Item = itemData.type,
                AmountOfOffers = maxOfferAmount > itemData.worth * 1.2f ? 6 : Random.Range(2, 5),
                AmountThresholdForLeaving = (int)(itemData.worth * 1.6f),
                InitialOfferAmount = (int)(maxOfferAmount * Random.Range(.4f, .9f)),
                MaxOfferAmount = maxOfferAmount
            }, instantlyActivate);
        } else {
            // Selling to player
            itemData = GetRandomItemToBuy();
            var lowestOfferAmount = (int)(itemData.worth * Random.Range(.5f, 1.5f));
            npc = NpcSpawner.Instance.Spawn(new NpcModel {
                NpcType = Npc.Type.Selling,
                Item = itemData.type,
                AmountOfOffers = Random.Range(3, 6),
                AmountThresholdForLeaving = (int)(itemData.worth * .5f),
                InitialOfferAmount = (int)(lowestOfferAmount + itemData.worth * Random.Range(0.2f, .9f)),
                MaxOfferAmount = lowestOfferAmount
            }, instantlyActivate);
        }

        Debug.LogFormat("Npc:\nWilling to go for: {0}\nInitial offer: {1}\nLeaves at: {2}\nMax increment: {3}\nItem worth: {4}", npc.Model.MaxOfferAmount, npc.Model.InitialOfferAmount, npc.Model.AmountThresholdForLeaving, npc.Model.MaxOfferIncrement, itemData.worth);

        return npc;
    }

    private ItemData GetRandomItemToBuy() {
        var playerMoney = GameController.Instance.Money;
        var allItemDatas = ItemFactory.Instance.GetAllItemDatas();
        var eligibleItems = allItemDatas.Where(i => i.worth < playerMoney).ToArray();

        if (eligibleItems.Length == 0) {
            // Try random of 3 cheapest
            var threeCheapestItems = allItemDatas.OrderBy(d => d.worth).Take(3).ToArray();
            return threeCheapestItems[Random.Range(0, threeCheapestItems.Length)];
        }

        return eligibleItems[Random.Range(0, eligibleItems.Length)];
    }

    private ItemData GetRandomItemInInventoryOrRandom() {
        var itemsForSale = Inventory.Instance.GetItemsThatAreForSale().ToList();
        if (itemsForSale.Count == 0) {
            return ItemFactory.Instance.GetRandomItemData();
        }

        return ItemFactory.Instance.GetDataFor(itemsForSale[Random.Range(0, itemsForSale.Count)].Type);
    }
}
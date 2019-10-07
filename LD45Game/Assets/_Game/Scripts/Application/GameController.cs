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

    [SerializeField] private TMP_Text _currentDayTextField;

    private int _currentDay;
    public int CurrentDay {
        get { return _currentDay; }
        set {
            _currentDay = value;
            _currentDayTextField.text = "Day " + _currentDay;
        }
    }

    public int ItemsSoldToday { get; set; }
    public int ItemsBoughtToday { get; set; }
    public ItemData.Type? TodaysSpecialItemType { get; set; }
    public int MoneyAtStartOfDay { get; set; }

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        CurrentDay = 1;

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

        Npc npc;
        if (firstDayAfterTutorial) {
            // Always spawn a npc that wants the tutorial item
            var tutorialItemData = _tutorialController.ItemAddedByTutorial;
            var maxOfferAmount = (int)(tutorialItemData.worth * Random.Range(1.3f, 1.4f));

            npc = NpcSpawner.Instance.Spawn(new NpcModel {
                NpcType = Npc.Type.Buying,
                Item = tutorialItemData.type,
                AmountOfOffers = maxOfferAmount > tutorialItemData.worth * 1.2f ? 6 : Random.Range(2, 5),
                AmountThresholdForLeaving = (int)(tutorialItemData.worth * 2f),
                InitialOfferAmount = (int)(maxOfferAmount * Random.Range(.4f, .9f)),
                MaxOfferAmount = maxOfferAmount
            }, false);

            Debug.LogFormat("Tutorial npc:\nWilling to go for: {0}\nInitial offer: {1}\nLeaves at: {2}\nMax increment: {3}", npc.Model.MaxOfferAmount, npc.Model.InitialOfferAmount, npc.Model.AmountThresholdForLeaving, npc.Model.MaxOfferIncrement);
        } else {
            npc = SpawnNpc();
        }

        DOTween.Sequence()
                .SetDelay(TodaysSpecialItemType.HasValue ? 5f : .85f)
                .OnComplete(() => { npc.Activate(); });

        if (TodaysSpecialItemType.HasValue) {
            var todaysSpecialItemDisplayName = ItemFactory.Instance.GetDataFor(TodaysSpecialItemType.Value).displayName;
            var specialItemAnnouncements = new string[] {
                string.Format("I heard a lot of people are wanting {0} today.", todaysSpecialItemDisplayName),
                string.Format("Purrr, {0} is really scarce. They will probably sell for a lot more today.", todaysSpecialItemDisplayName),
                string.Format("Wow, {0} is totally a hot item today!", todaysSpecialItemDisplayName),
                string.Format("Everyone wants a {0} today!", todaysSpecialItemDisplayName),
            };
            HintController.Instance.ShowHint(specialItemAnnouncements[Random.Range(0, specialItemAnnouncements.Length)]);
        }
    }

    public void StartNextDay() {
        CurrentDay++;

        TodaysSpecialItemType = GetItemTypeThatIsWorthMoreToday();
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
        EndOfDayPanelController.Instance.Show(ItemsBoughtToday, ItemsSoldToday, Money - MoneyAtStartOfDay, CurrentDay);

        ItemsBoughtToday = 0;
        ItemsSoldToday = 0;
    }

    private Npc SpawnNpc(bool instantlyActivate = false) {
        Npc npc;
        ItemData itemData;
        if (Inventory.Instance.FilledSlots > 0 && Random.value > .6f) {
            // Buying from player
            itemData = GetRandomItemInInventoryOrRandom();
            var worth = itemData.worth;
            if (itemData.type == TodaysSpecialItemType) {
                worth *= 2;
            }

            var maxOfferAmount = (int)(worth * Random.Range(.8f, 1.4f));
            npc = NpcSpawner.Instance.Spawn(new NpcModel {
                NpcType = Npc.Type.Buying,
                Item = itemData.type,
                AmountOfOffers = maxOfferAmount > worth * 1.2f ? 6 : Random.Range(2, 5),
                AmountThresholdForLeaving = (int)(worth * 1.6f),
                InitialOfferAmount = (int)(maxOfferAmount * Random.Range(.4f, .9f)),
                MaxOfferAmount = maxOfferAmount
            }, instantlyActivate);
        } else {
            // Selling to player
            itemData = GetRandomItemToBuy();
            var worth = itemData.worth;
            if (itemData.type == TodaysSpecialItemType) {
                worth *= 2;
            }

            var lowestOfferAmount = (int)(worth * Random.Range(.5f, 1.5f));
            npc = NpcSpawner.Instance.Spawn(new NpcModel {
                NpcType = Npc.Type.Selling,
                Item = itemData.type,
                AmountOfOffers = Random.Range(3, 6),
                AmountThresholdForLeaving = (int)(worth * .5f),
                InitialOfferAmount = (int)(lowestOfferAmount + worth * Random.Range(0.2f, .9f)),
                MaxOfferAmount = lowestOfferAmount
            }, instantlyActivate);
        }

        Debug.LogFormat("Npc:\nWilling to go for: {0}\nInitial offer: {1}\nLeaves at: {2}\nMax increment: {3}\nItem worth: {4}", npc.Model.MaxOfferAmount, npc.Model.InitialOfferAmount, npc.Model.AmountThresholdForLeaving, npc.Model.MaxOfferIncrement, itemData.worth);

        return npc;
    }

    private ItemData.Type GetItemTypeThatIsWorthMoreToday() {
        var affordableItems = GetAffordableItems().ToArray();
        var itemsInInventory = Inventory.Instance.ItemsInInventory.ToArray();

        ItemData.Type todaysSpecialItem;
        if (itemsInInventory.Length > 0 && Random.value > 0.5f) {
            // something in inventory
            todaysSpecialItem = itemsInInventory[Random.Range(0, itemsInInventory.Length)].Type;
        } else if (affordableItems.Length > 0) {
            // something affordable
            todaysSpecialItem = affordableItems[Random.Range(0, affordableItems.Length)].type;
        } else {
            // totally random
            var allItemDatas = ItemFactory.Instance.GetAllItemDatas().ToArray();
            todaysSpecialItem = allItemDatas[Random.Range(0, allItemDatas.Length)].type;
        }

        return todaysSpecialItem;
    }

    private IEnumerable<ItemData> GetAffordableItems() {
        var playerMoney = GameController.Instance.Money;
        var allItemDatas = ItemFactory.Instance.GetAllItemDatas();
        return allItemDatas.Where(i => i.worth < playerMoney).ToArray();
    }

    private ItemData GetRandomItemToBuy() {
        var eligibleItems = GetAffordableItems().ToArray();

        if (eligibleItems.Length == 0) {
            // Try random of 3 cheapest
            var allItemDatas = ItemFactory.Instance.GetAllItemDatas();
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
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

        if (firstDayAfterTutorial) {
            // Always spawn a npc that wants the tutorial item
            var tutorialItemData = _tutorialController.ItemAddedByTutorial;
            var maxOfferAmount = tutorialItemData.maxWorth + (int)Random.Range(-tutorialItemData.maxWorth * .2f, tutorialItemData.maxWorth * .2f);

            var npc = NpcSpawner.Instance.Spawn(new NpcModel {
                NpcType = Npc.Type.Buying,
                Item = tutorialItemData.type,
                AmountOfOffers = Random.Range(3, 5),
                AmountThresholdForLeaving = (int)(maxOfferAmount * 1.1f),
                InitialOfferAmount = tutorialItemData.minWorth + (int)Random.Range(-tutorialItemData.minWorth * .2f, tutorialItemData.minWorth * .2f),
                MaxOfferAmount = maxOfferAmount
            }, false);
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

    public void OnNpcLeave(Npc npcThatLeft) {
        if (_watch.DayIsOver) {
            Debug.Log("Day is over!");
            return;
        }

        var newNpc = SpawnNpc();
        var showHint = Random.value > .4f;
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
                            if (newNpc.Model.InitialOfferAmount < itemData.minWorth) tips.Add("Oh I know this homan! This homan always starts really low when haggling.");

                            // Is willing to pay more that item's max worth
                            if (newNpc.Model.MaxOfferAmount > itemData.maxWorth) tips.Add("That homan looks like he really wants something. He might pay over what the item is worth.");

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
                            itemTips.Add(string.Format("The minimum worth of a {0} is generally around the {1} coins.", randomItemData.displayName, randomItemData.minWorth));
                            itemTips.Add(string.Format("The maximum worth of a {0} is generally around the {1} coins.", randomItemData.displayName, randomItemData.maxWorth));

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

    private Npc SpawnNpc(bool instantlyActivate = false) {
        Npc npc;
        if (Random.value > .6f) {
            // Buying from player
            var itemData = GetRandomItemInInventoryOrRandom();
            var maxOfferAmount = itemData.maxWorth + (int)Random.Range(-itemData.maxWorth * .2f, itemData.maxWorth * .2f);
            npc = NpcSpawner.Instance.Spawn(new NpcModel {
                NpcType = Npc.Type.Buying,
                Item = itemData.type,
                AmountOfOffers = Random.Range(2, 5),
                AmountThresholdForLeaving = (int)(maxOfferAmount * 1.1f),
                InitialOfferAmount = itemData.minWorth + (int)Random.Range(-itemData.minWorth * .2f, itemData.minWorth * .2f),
                MaxOfferAmount = maxOfferAmount
            }, instantlyActivate);
        } else {
            // Selling to player
            var itemData = GetRandomItemToBuy();
            var lowestOfferAmount = itemData.minWorth + (int)Random.Range(-itemData.minWorth * .2f, itemData.minWorth * .2f);
            npc = NpcSpawner.Instance.Spawn(new NpcModel {
                NpcType = Npc.Type.Selling,
                Item = itemData.type,
                AmountOfOffers = Random.Range(2, 5),
                AmountThresholdForLeaving = (int)(lowestOfferAmount * .8f),
                InitialOfferAmount = (int)(itemData.maxWorth * Random.Range(.4f, 1.2f)),
                MaxOfferAmount = lowestOfferAmount
            }, instantlyActivate);
        }

        return npc;
    }

    private ItemData GetRandomItemToBuy() {
        var playerMoney = GameController.Instance.Money;
        var allItemDatas = ItemFactory.Instance.GetAllItemDatas();
        var eligibleItems = allItemDatas.Where(i => i.minWorth < playerMoney).ToArray();

        if (eligibleItems.Length == 0) {
            // Try random of 3 cheapest
            var threeCheapestItems = allItemDatas.OrderBy(d => d.minWorth).Take(3).ToArray();
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
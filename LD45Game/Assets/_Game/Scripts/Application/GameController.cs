using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
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

        StartDay();
    }

    private void StartDay() {
        _watch.Begin(_availableTimePerDayInSeconds);

        var npc = SpawnNpc();
        DOTween.Sequence()
                .SetDelay(.85f)
                .OnComplete(() => { npc.Activate(); });
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
                        var itemData = ItemFactory.Instance.GetDataFor(newNpc.Model.WantedItem);

                        var tips = new List<string>();

                        // Starts low
                        if (newNpc.Model.InitialOfferAmount < itemData.minWorth) tips.Add("Oh I know this homan! This homan always starts really low when haggling.");

                        // Is willing to pay more that item's max worth
                        if (newNpc.Model.MaxOfferAmount > itemData.maxWorth) tips.Add("That homan looks like he really wants something. He might pay over what the item is worth.");

                        // Will not iterate as much
                        if (newNpc.Model.MaxOfferAmount <= 3) tips.Add("Hmm, that human looks a little bit irritated.");

                        if (tips.Count > 0) {
                            HintController.Instance.ShowHint(tips[Random.Range(0, tips.Count)]);
                        } else {
                            // No usefull tip to show, show a filler instead
                            HintController.Instance.ShowFiller();
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

    private Npc SpawnNpc() {
        var swordItemData = ItemFactory.Instance.GetDataFor(ItemData.Type.Sword);

        var maxOfferAmount = swordItemData.maxWorth + (int)Random.Range(-swordItemData.maxWorth * .2f, swordItemData.maxWorth * .2f);

        return NpcSpawner.Instance.Spawn(new NpcModel {
            NpcType = Npc.Type.Buying,
            WantedItem = ItemData.Type.Sword,
            AmountOfOffers = Random.Range(2, 5),
            AmountThresholdForLeaving = (int)(maxOfferAmount * 1.1f),
            InitialOfferAmount = swordItemData.minWorth + (int)Random.Range(-swordItemData.minWorth * .2f, swordItemData.minWorth * .2f),
            MaxOfferAmount = maxOfferAmount
        }, false);
    }
}
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

        SpawnNpc();
    }

    public void OnNpcLeave(Npc npc) {
        if (_watch.DayIsOver) {
            Debug.Log("Day is over!");
            return;
        }

        var showHint = Random.value > .6f;
        if (showHint) {
            DOTween.Sequence()
               .SetDelay(.85f)
               .OnComplete(() => {
                   // todo: decide if filler or actual tip
                   HintController.Instance.ShowFiller();
               });
        }

        DOTween.Sequence()
               .SetDelay(showHint ? 2.5f : 0)
               .OnComplete(() => {
                   SpawnNpc();
               });
    }

    private void SpawnNpc() {
        NpcSpawner.Instance.Spawn(new NpcModel {
            NpcType = Npc.Type.Buying,
            WantedItem = ItemData.Type.Sword,
            AmountOfOffers = 3,
            AmountThresholdForLeaving = 80,
            InitialOfferAmount = 20,
            MaxOfferAmount = 40
        });
    }
}
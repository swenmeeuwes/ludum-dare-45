﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour {
    [SerializeField] private Watch _watch;
    [SerializeField] private TutorialController _tutorialController;
    [SerializeField] private TMP_Text _moneyTextField;

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
        _tutorialController.Play();
    }
}
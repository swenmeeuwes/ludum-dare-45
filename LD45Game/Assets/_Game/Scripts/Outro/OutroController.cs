using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OutroController : MonoBehaviour {
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TMP_Text _daysTextField;

    public static OutroController Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        _canvasGroup.gameObject.SetActive(false);
        _canvasGroup.alpha = 0;
    }

    public void Show() {
        _canvasGroup.gameObject.SetActive(true);
        _daysTextField.text = string.Format("You were able to afford a house in {0} days!", GameController.Instance.CurrentDay);
        _canvasGroup.DOFade(1, 1.25f);
    }
}

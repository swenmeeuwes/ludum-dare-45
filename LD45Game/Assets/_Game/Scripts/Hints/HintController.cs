using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HintController : MonoBehaviour
{
    [SerializeField] private RectTransform _cat;
    [SerializeField] private MoveSlightlyToMouse _catEyes;
    [SerializeField] private CanvasGroup _speechBalloon;
    [SerializeField] private TMP_Text _speechBalloonTextField;

    [SerializeField] private string[] _fillerDialogs;

    public static HintController Instance { get; private set; }

    private Vector3 _originalCatPosition;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        _originalCatPosition = _cat.transform.position;
        _catEyes.enabled = false;
        _speechBalloon.alpha = 0;

        _cat.transform.position += new Vector3(0, -250, 0);
    }

    public void ShowFiller() {
        ShowHint(_fillerDialogs[Random.Range(0, _fillerDialogs.Length)]);
    }

    public void ShowHint(string hintText) {
        StartCoroutine(ShowHintSequence(hintText));
    }

    public IEnumerator ShowHintSequence(string hintText) {
        _cat.DOMove(_originalCatPosition, .45f, true);
        yield return new WaitForSeconds(.45f);

        _catEyes.enabled = true;
        _catEyes.UpdateOriginalPosition(_cat.transform.position);

        _speechBalloonTextField.text = hintText;
        _speechBalloon.DOFade(1, .45f);

        yield return new WaitForSeconds(3.5f);

        _speechBalloon.DOFade(0, .35f);
        yield return new WaitForSeconds(.45f);

        _catEyes.enabled = false;
        _cat.DOMove(_cat.transform.position + new Vector3(0, -250, 0), .45f);
    }
}

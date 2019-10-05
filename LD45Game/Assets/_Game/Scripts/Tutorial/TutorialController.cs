using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialController : MonoBehaviour {
    [SerializeField] private CanvasGroup _fader;
    [SerializeField] private CanvasGroup _speechBalloon;
    [SerializeField] private TMP_Text _speechBalloonTextField;
    [SerializeField] private RectTransform _cat;
    [SerializeField] private MoveSlightlyToMouse _catEyes;

    [SerializeField] private Watch _watch;

    private Vector3 _originalCatPosition;

    private void Start() {
        _fader.alpha = 0;
        _speechBalloon.alpha = 0;
        _originalCatPosition = _cat.transform.position;
        _catEyes.enabled = false;

        _watch.CanvasGroup.alpha = 0;

        _cat.transform.position += new Vector3(0, -150, 0);

        StartCoroutine(TutorialSequence());
    }

    private IEnumerator TutorialSequence() {
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


        _speechBalloonTextField.text = "When the moon is at the top we'll take a nap.";
        _speechBalloon.DOFade(1, .45f);
        yield return new WaitForSeconds(.45f + 3f);

        _speechBalloon.DOFade(0, .45f);
        yield return new WaitForSeconds(.45f);
    }
}
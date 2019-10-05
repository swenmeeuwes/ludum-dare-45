using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroductionController : MonoBehaviour {
    [SerializeField] private Paw _paw;
    [SerializeField] private TMP_Text _storyTextField;
    [SerializeField] private TMP_Text _instructionTextField;
    [SerializeField] private Rigidbody2D _coin;
    [SerializeField] private CanvasGroup _fader;

    private void Start() {
        _paw.CanvasGroup.alpha = 0;
        _coin.gravityScale = 0;
        _fader.alpha = 0;

        StartCoroutine(RunIntro());
    }

    private IEnumerator RunIntro() {
        _storyTextField.alpha = 0;
        _instructionTextField.alpha = 0;

        _storyTextField.text = "This is the story of a stray cat";
        _storyTextField.DOFade(1, .45f);
        yield return new WaitForSeconds(.45f);
        _instructionTextField.DOFade(1, .15f);
        yield return new WaitUntil(() => Input.anyKeyDown);
        _instructionTextField.DOFade(0, .15f);

        DOTween.Sequence()
            .Append(_storyTextField.DOFade(0, .25f))
            .AppendInterval(.45f)
            .AppendCallback(() => { _storyTextField.text = "A cat without a house, food, never a single pet"; })
            .Append(_storyTextField.DOFade(1, .45f));
        yield return new WaitForSeconds(.65f);
        _instructionTextField.DOFade(1, .15f);
        yield return new WaitUntil(() => Input.anyKeyDown);
        _instructionTextField.DOFade(0, .15f);

        DOTween.Sequence()
            .Append(_storyTextField.DOFade(0, .25f))
            .AppendInterval(.45f)
            .AppendCallback(() => { _storyTextField.text = "But this was about to change"; })
            .Append(_storyTextField.DOFade(1, .45f));
        yield return new WaitForSeconds(.65f);
        _instructionTextField.DOFade(1, .15f);
        yield return new WaitUntil(() => Input.anyKeyDown);
        _instructionTextField.DOFade(0, .15f);

        DOTween.Sequence()
            .Append(_storyTextField.DOFade(0, .25f))
            .AppendInterval(.45f)
            .AppendCallback(() => { _storyTextField.text = "Let's see what this cat is about to arrange"; })
            .Append(_storyTextField.DOFade(1, .45f));
        yield return new WaitForSeconds(.65f);
        _instructionTextField.DOFade(1, .15f);
        yield return new WaitUntil(() => Input.anyKeyDown);
        _instructionTextField.DOFade(0, .15f);

        DOTween.Sequence()
            .Append(_storyTextField.DOFade(0, .25f))
            .AppendInterval(.45f)
            .AppendCallback(() => { _storyTextField.text = "The cat put up a sad face"; })
            .Append(_storyTextField.DOFade(1, .45f));
        yield return new WaitForSeconds(.65f);
        _instructionTextField.DOFade(1, .15f);
        yield return new WaitUntil(() => Input.anyKeyDown);
        _instructionTextField.DOFade(0, .15f);

        DOTween.Sequence()
            .Append(_storyTextField.DOFade(0, .25f))
            .AppendInterval(.45f)
            .AppendCallback(() => { _storyTextField.text = "\"Sir, madame, can you help me find a place?\""; })
            .Append(_storyTextField.DOFade(1, .45f));
        yield return new WaitForSeconds(.65f);
        _instructionTextField.DOFade(1, .15f);
        yield return new WaitUntil(() => Input.anyKeyDown);
        _instructionTextField.DOFade(0, .15f);

        DOTween.Sequence()
            .Append(_storyTextField.DOFade(0, .25f))
            .AppendInterval(.45f)
            .AppendCallback(() => { _storyTextField.text = "(Wave you paw to attract attention)"; })
            .Append(_storyTextField.DOFade(1, .45f));
        yield return new WaitForSeconds(.55f);

        _paw.CanvasGroup.DOFade(1, .65f);

        yield return new WaitForSeconds(5f);

        _paw.CanvasGroup.DOFade(0, .45f);

        yield return new WaitForSeconds(.55f);

        DOTween.Sequence()
            .Append(_storyTextField.DOFade(0, .25f))
            .AppendInterval(.45f)
            .AppendCallback(() => { _storyTextField.text = "In front of the cat stood a travelling merchant wearing a large coat"; })
            .Append(_storyTextField.DOFade(1, .45f));
        yield return new WaitForSeconds(.65f);
        _instructionTextField.DOFade(1, .15f);
        yield return new WaitUntil(() => Input.anyKeyDown);
        _instructionTextField.DOFade(0, .15f);

        DOTween.Sequence()
            .Append(_storyTextField.DOFade(0, .25f))
            .AppendInterval(.45f)
            .AppendCallback(() => { _storyTextField.text = "The merchant began clearing his throat"; })
            .Append(_storyTextField.DOFade(1, .45f));
        yield return new WaitForSeconds(.65f);
        _instructionTextField.DOFade(1, .15f);
        yield return new WaitUntil(() => Input.anyKeyDown);
        _instructionTextField.DOFade(0, .15f);

        DOTween.Sequence()
            .Append(_storyTextField.DOFade(0, .25f))
            .AppendInterval(.45f)
            .AppendCallback(() => { _storyTextField.text = "\"I cannot give you a home\""; })
            .Append(_storyTextField.DOFade(1, .45f));
        yield return new WaitForSeconds(.65f);
        _instructionTextField.DOFade(1, .15f);
        yield return new WaitUntil(() => Input.anyKeyDown);
        _instructionTextField.DOFade(0, .15f);

        DOTween.Sequence()
            .Append(_storyTextField.DOFade(0, .25f))
            .AppendInterval(.45f)
            .AppendCallback(() => { _storyTextField.text = "\"For I only roam\""; })
            .Append(_storyTextField.DOFade(1, .45f));
        yield return new WaitForSeconds(.65f);
        _instructionTextField.DOFade(1, .15f);
        yield return new WaitUntil(() => Input.anyKeyDown);
        _instructionTextField.DOFade(0, .15f);

        DOTween.Sequence()
            .Append(_storyTextField.DOFade(0, .25f))
            .AppendInterval(.45f)
            .AppendCallback(() => { _storyTextField.text = "\"Take this coin and pick up the art of trade\""; })
            .Append(_storyTextField.DOFade(1, .45f));
        yield return new WaitForSeconds(.65f);
        _instructionTextField.DOFade(1, .15f);
        yield return new WaitUntil(() => Input.anyKeyDown);
        _instructionTextField.DOFade(0, .15f);

        DOTween.Sequence()
            .Append(_storyTextField.DOFade(0, .25f))
            .AppendInterval(.45f)
            .AppendCallback(() => { _storyTextField.text = "\"Start trading, don't be afraid.\""; })
            .Append(_storyTextField.DOFade(1, .45f));
        yield return new WaitForSeconds(.65f);
        _instructionTextField.DOFade(1, .15f);
        yield return new WaitUntil(() => Input.anyKeyDown);
        _instructionTextField.DOFade(0, .15f);

        _paw.CanvasGroup.DOFade(1, .65f);
        yield return new WaitForSeconds(.25f);

        _coin.gravityScale = 2;
        _coin.AddTorque(45000f);

        yield return new WaitForSeconds(9f);

        _fader.DOFade(1, 2.45f);

        var loadSceneOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
        loadSceneOperation.allowSceneActivation = false;

        yield return new WaitForSeconds(2.45f);

        loadSceneOperation.allowSceneActivation = true;
    }
}

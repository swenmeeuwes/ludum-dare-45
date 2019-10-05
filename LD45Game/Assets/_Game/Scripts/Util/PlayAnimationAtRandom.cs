using System.Collections;
using UnityEngine;

public class PlayAnimationAtRandom : MonoBehaviour {
    [SerializeField] private Animator _animator;
    [SerializeField] private string _animationTriggerVar;
    [SerializeField] private Vector2 _intervalRange;

    private void Start() {
        StartCoroutine(AnimationSequnece());
    }

    private IEnumerator AnimationSequnece() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(_intervalRange.x, _intervalRange.y));

            _animator.SetTrigger(_animationTriggerVar);
        }
    }
}

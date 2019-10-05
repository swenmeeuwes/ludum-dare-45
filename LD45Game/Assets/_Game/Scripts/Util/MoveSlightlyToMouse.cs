using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSlightlyToMouse : MonoBehaviour {
    [SerializeField] private float _maxDisplacement;
    [SerializeField] private float _displacementModifier;

    private Vector3 _originalPosition;

    private void Start() {
        _originalPosition = transform.position;
    }

    private void Update() {
        var vectorToMouse = Input.mousePosition - transform.position;
        transform.position = _originalPosition + Vector3.ClampMagnitude(vectorToMouse * _displacementModifier, _maxDisplacement);
    }

    public void UpdateOriginalPosition(Vector3 newPos) {
        _originalPosition = newPos;
    }
}

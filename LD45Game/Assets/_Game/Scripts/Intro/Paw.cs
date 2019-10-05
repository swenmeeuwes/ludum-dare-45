using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image), typeof(CanvasGroup))]
public class Paw : MonoBehaviour {
    [SerializeField] private CanvasGroup _canvasGroup;
    public CanvasGroup CanvasGroup { get { return _canvasGroup; } }


    private void Update() {
        LookAtMouse();
    }

    private void LookAtMouse() {
        var direction = Input.mousePosition - transform.position;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }
}
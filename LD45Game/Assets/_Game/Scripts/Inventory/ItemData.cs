using UnityEngine;

[CreateAssetMenu(menuName = "Item", fileName = "Item")]
public class ItemData : ScriptableObject {
    public Type type;
    public string displayName;
    public Sprite sprite;
    public int minWorth;
    public int maxWorth;

    public enum Type {
        Sword = 0,
    }
}
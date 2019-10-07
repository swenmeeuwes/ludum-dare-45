using UnityEngine;

[CreateAssetMenu(menuName = "Item", fileName = "Item")]
public class ItemData : ScriptableObject {
    public Type type;
    public string displayName;
    public Sprite sprite;
    public int worth;

    //public int minWorth;
    //public int maxWorth;

    public enum Type {
        Sword = 0,
        Wheat = 1,
        GoldenNecklaceWithBlueStones = 2,
        Egg = 3,
        Shovel = 4,
        BallOfWool = 5,
        Apple = 6,
        Hatchet = 7,
        WoodenLog = 8,
    }
}
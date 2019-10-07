using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemFactory : MonoBehaviour
{
    [SerializeField] private Transform _itemContainer;
    [SerializeField] private Item _itemPrototype;
    [SerializeField] private ItemMap[] _itemMapping;

    public static ItemFactory Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

    public Item Create(ItemData.Type itemType) {
        var itemData = _itemMapping.FirstOrDefault(m => m.type == itemType).data;
        if (itemData == null) {
            throw new ArgumentOutOfRangeException("Item of type " + itemType + " is not mapped in ItemFactory");
        }

        return CreateFromData(itemData);
    }

    public Item CreateFromData(ItemData data) {
        var item = Instantiate(_itemPrototype);
        item.SetContainer(_itemContainer);

        item.Image.sprite = data.sprite;

        item.Type = data.type;
        item.DisplayName = data.displayName;
        item.Worth = data.worth;

        return item;
    }

    public IEnumerable<ItemData> GetAllItemDatas() {
        return _itemMapping.Select(m => m.data);
    }

    public ItemData GetDataFor(ItemData.Type itemType) {
        var itemMap = _itemMapping.FirstOrDefault(m => m.type == itemType);
        if (itemMap == null) {
            return null;
        }

        return itemMap.data;
    }

    public Sprite GetSpriteFor(ItemData.Type itemType) {
        var itemMap = _itemMapping.FirstOrDefault(m => m.type == itemType);
        if (itemMap == null) {
            return null;
        }

        return itemMap.data.sprite;
    }

    public ItemData GetRandomItemData() {
        if (_itemMapping.Length == 0) {
            return null;
        }

        return _itemMapping[UnityEngine.Random.Range(0, _itemMapping.Length)].data;
    }

    [Serializable]
    public class ItemMap {
        public ItemData.Type type;
        public ItemData data;
    }
}

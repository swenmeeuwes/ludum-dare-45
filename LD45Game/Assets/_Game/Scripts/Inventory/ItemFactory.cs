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
        item.MinWorth = data.minWorth;
        item.MaxWorth = data.maxWorth;

        return item;
    }

    public class ItemMap {
        public ItemData.Type type;
        public ItemData data;
    }
}

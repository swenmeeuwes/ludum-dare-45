using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int _startStorageSlots;
    [SerializeField] private Slot _storageSlotPrefab;
    [SerializeField] private Transform _storageSlotsContainer;

    [SerializeField] private List<SellSlot> _sellSlots = new List<SellSlot>(); // not dynamic at the moment
    private List<Slot> _storageSlots = new List<Slot>();

    public static Inventory Instance { get; set; }

    public bool HasSpace {
        get {
            return _sellSlots.Any(s => !s.IsFilled) || _storageSlots.Any(s => !s.IsFilled);
        }
    }

    public IEnumerable<Item> ItemsOnDisplay {
        get { return _sellSlots.Where(s => s.IsFilled).Select(s => s.Content); }
    }

    public IEnumerable<Item> ItemsInInventory {
        get {
            var onSale = _sellSlots.Where(s => s.IsFilled).Select(s => s.Content);
            var inStorage = _storageSlots.Where(s => s.IsFilled).Select(s => s.Content);
            return onSale.Concat(inStorage);
        }
    }

    public int FilledSlots {
        get {
            return _sellSlots.Count(s => s.IsFilled) + _storageSlots.Count(s => s.IsFilled);
        }
    }

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        Initialize();
    }

    private void Initialize() {
        for (var i = 0; i < _startStorageSlots; i++) {
            var storageSlot = Instantiate(_storageSlotPrefab, _storageSlotsContainer);
            _storageSlots.Add(storageSlot);
        }
    }

    public bool AddItem(ItemData data) {
        return AddItem(ItemFactory.Instance.CreateFromData(data));
    }

    public bool AddItem(Item item) {
        if (!HasSpace) {
            Debug.LogWarning("Tried adding an item to a full inventory.");
        }

        var firstFreeSellSlot = _sellSlots.FirstOrDefault(s => !s.IsFilled);
        if (firstFreeSellSlot != null) {
            firstFreeSellSlot.Add(item);
            return true;
        }

        var firstFreeStorageSlot = _storageSlots.FirstOrDefault(s => !s.IsFilled);
        if (firstFreeStorageSlot != null) {
            firstFreeStorageSlot.Add(item);
            return true;
        } 

        return false;
    }

    public bool AddItemToStorage(Item item) {
        if (_storageSlots.All(s => s.IsFilled)) {
            Debug.LogWarning("Tried adding an item to a full storage.");
        }

        var firstFreeStorageSlot = _storageSlots.FirstOrDefault(s => !s.IsFilled);
        if (firstFreeStorageSlot != null) {
            firstFreeStorageSlot.Add(item);
            return true;
        }

        return false;
    }

    public IEnumerable<Item> GetItemsThatAreForSale() {
        return _sellSlots.Where(s => s.Content != null).Select(s => s.Content);
    }

    public bool HasItemForSale(ItemData.Type itemType) {
        return _sellSlots.Any(s => {
            if (s.Content == null) {
                return false;
            }
            return s.Content.Type == itemType;
        });
    }

    public bool SellItem(ItemData.Type itemType) {
        var slotContainingItem = _sellSlots.FirstOrDefault(s => {
            if (s.Content == null) {
                return false;
            }
            return s.Content.Type == itemType;
        });
        if (slotContainingItem == null) {
            Debug.LogWarningFormat("Cannot sell item of type '{0}' because it is not in the sell slots.", itemType.ToString());
            return false;
        }

        slotContainingItem.Clear();
        return true;
    }
}

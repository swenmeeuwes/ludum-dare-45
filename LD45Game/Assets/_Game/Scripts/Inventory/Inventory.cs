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

    public bool HasSpace {
        get {
            return _sellSlots.Any(s => !s.IsFilled) || _storageSlots.Any(s => !s.IsFilled);
        }
    }

    public IEnumerable<Item> ItemsOnDisplay {
        get { return _sellSlots.Where(s => s.IsFilled).Select(s => s.Content); }
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

    public bool AddItem(Item item) {
        if (!HasSpace) {
            Debug.LogWarning("Tried adding an item to a full inventory.");
        }

        // Prefer putting it in storage, but if there is no space put it in selling gallery
        var firstFreeStorageSlot = _storageSlots.FirstOrDefault(s => !s.IsFilled);
        if (firstFreeStorageSlot != null) {
            firstFreeStorageSlot.Add(item);
            return true;
        } 

        var firstFreeSellSlot = _sellSlots.FirstOrDefault(s => !s.IsFilled);
        if (firstFreeSellSlot != null) {
            firstFreeSellSlot.Add(item);
            return true;
        }

        return false;
    }
}

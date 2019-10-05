using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StoryTeller : MonoBehaviour {
    [Header("References")]
    [SerializeField] private TMP_Text _storyTextField;

    [Header("Settings")]
    [SerializeField] private StoryTellerItem[] _items;

    private int _currentItemIndex = 0;

    public bool HasNextItem { get { return _currentItemIndex + 1 < _items.Length; } }
    public StoryTellerItem NextItem {
        get {
            if (_currentItemIndex + 1 >= _items.Length) {
                return null;
            }
            return _items[_currentItemIndex++];
        }
    }

    [Serializable]
    public class StoryTellerItem {
        public ItemType type;
        public string text;

        public enum ItemType {
            Text,
            Action
        }
    }
}

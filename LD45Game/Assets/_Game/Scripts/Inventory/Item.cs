using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : Draggable {
    [SerializeField] private Image _image;
    public Image Image { get { return _image; } }

    public ItemData.Type Type { get; set; }
    public string DisplayName { get; set; }
    public int MinWorth { get; set; }
    public int MaxWorth { get; set; }
}

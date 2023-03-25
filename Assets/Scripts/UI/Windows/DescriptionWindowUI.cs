using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionWindowUI  : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Text _name;
    [SerializeField] private Text _description;

    public void SetDescript(InventoryItem inventoryItem)
    {
        _name.text = inventoryItem.itemData.title;

        _description.text = inventoryItem.itemData.description;
    }

    public RectTransform GetRectTransform() => _rectTransform;
}

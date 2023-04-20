using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOnstreet : MonoBehaviour, ICanUse
{
    [SerializeField] private ItemData _itemData;
    [SerializeField] private Collider _collider;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Outline _outline;

    public int Amount = 0;

    private void Start() 
    {
        GameManager.Instance.RegistrateUnit(this);
    }

    public Collider GetCollider() => _collider;
    public Rigidbody GetRigidbody() => _rigidbody;
    public Outline GetOutline() => _outline;

    public void ShowOutline(bool value)
    {
        _outline.enabled = value;
    }

    public ItemData GetItemData() => _itemData;

    public void TakeItem(Chest chest)
    {       
        chest.AddItemToChest(new InventoryItemInfo(_itemData, Amount));

        GameManager.Instance.RemoveUsableObject(gameObject);

        Destroy(this.gameObject);
    }

    public void Use(AbstractBehavior applicant)
    {
        ShowOutline(false);
        TakeItem(applicant.Chest);
    }

    private void Dest()
    {
        Destroy(this.gameObject);
    }
}

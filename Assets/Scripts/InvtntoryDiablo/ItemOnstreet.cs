using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOnstreet : MonoBehaviour, ICanUse
{
    [SerializeField] private ItemData itemData;

    public int Amount = 0;

    private Outline outline;

    private void Start() 
    {
        GameManager.Instance.RegistrateUnit(this);

        outline = GetComponent<Outline>();    
    }

    public void ShowOutline(bool value)
    {
        outline.enabled = value;
    }

    public ItemData GetItemData() => itemData;

    public void TakeItem(Chest chest)
    {       
        chest.AddItemToChest(new InventoryItemInfo(itemData, Amount));

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

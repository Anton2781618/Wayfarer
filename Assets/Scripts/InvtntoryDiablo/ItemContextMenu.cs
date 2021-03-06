using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Класс представляет контекстное меню
public class ItemContextMenu : MonoBehaviour
{
    public InventoryItem targetItem;
    private Transform player;

    private void Start() 
    {
        player = FindObjectOfType<PLayerController>().transform;
    }

    public void SetTargetItem(InventoryItem value)
    {
        targetItem = value;
    }

    public void ShowInfo()
    {
        
    }

    public void DropItem()
    {
        Instantiate(targetItem.itemData.prefab, 
        new Vector3(player.position.x + 1, player.position.y + 1, player.position.z), Quaternion.identity);
        
        Destroy(targetItem.gameObject);
        
        GameManager.singleton.SwithContextMenu(false);
        GameManager.singleton.SwithInfoItem(false);
    }

    public void UseItem()
    {
        targetItem.Use();
    }
}

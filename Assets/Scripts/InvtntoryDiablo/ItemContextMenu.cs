using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Класс представляет контекстное меню
public class ItemContextMenu : MonoBehaviour
{
    private InventoryItem _targetItem;
    private PLayerController _player;

    private void Start() 
    {
        _player = GameManager.Instance.pLayerController;
    }

    public void UseItem() => _targetItem.Use(_player);

    public void SetTargetItem(InventoryItem item)
    {
        _targetItem = item;
    }

    public void DropItem()
    {
        Instantiate(_targetItem.itemData.prefab, 
            new Vector3(_player.transform.position.x + 1, _player.transform.position.y + 1, _player.transform.position.z),
            Quaternion.identity);
        
        _targetItem.DestructSelf();
        
        GameManager.Instance.SwithContextMenu(false);

        GameManager.Instance.OpenDescriptInfo(false);
    }
}

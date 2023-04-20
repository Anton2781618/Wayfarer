using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventoryWindowUI : MonoBehaviour
{
    [SerializeField] private InventoryController _inventoryController;
    [SerializeField] private List<ItemGrid> _playerGrids; 
    [SerializeField] private ItemGrid _playerChest; 
    [SerializeField] public Text moneyText; 
    [SerializeField] private Image _imageMoney;

    public InventoryController GetInventoryController() => _inventoryController;
    public List<ItemGrid> GetPlayerInventoryGrids() => _playerGrids;
    public ItemGrid GetPlayerInventoryChest() => _playerChest;

    // блокировать картинку денег
    public void BlockMoneyImage(bool value) => _imageMoney.gameObject.SetActive(!value);

    public void NotEnoughMoneyAnimation()
    {
        StartCoroutine(MoneyAnimation());
    }

    private IEnumerator MoneyAnimation()
    {
        moneyText.color = Color.red;

        yield return new WaitForSeconds(0.2f);
        
        moneyText.color = Color.black;    
        
        yield return new WaitForSeconds(0.2f);
        
        moneyText.color = Color.red;
        
        yield return new WaitForSeconds(0.2f);
        
        moneyText.color = Color.black;
    }
}

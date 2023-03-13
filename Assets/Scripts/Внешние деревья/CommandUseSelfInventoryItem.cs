using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// команда использовать предмет из своего инвинтаря по типу предмета
namespace BehaviorDesigner.Runtime.Tasks.Tutorials
{
    [TaskCategory("Tutorial")]

    public class CommandUseSelfInventoryItem : Action
    {
        [SerializeField] private ItemData.ItemType itemType;
        
        private AbstractBehavior mySelf;

        public override void OnStart()
        {
            mySelf = GetComponent<AbstractBehavior>();
        }


        public override TaskStatus OnUpdate()
        {
            UseSelfInventoryItem();

            return TaskStatus.Success;
        }

        private void UseSelfInventoryItem()
        {
            InventoryItemInfo item =  mySelf.chest.GetInventoryForItemType(itemType);
            
            item.Use(mySelf);

            mySelf.chest.RemoveAtChestGrid(item);
        }
    }
}
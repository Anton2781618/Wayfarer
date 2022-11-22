using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Tutorials
{
    [TaskCategory("Tutorial")]

    public class CommandUseSelfInventoryItem : Action
    {
        [SerializeField] private ItemData.ItemType itemType;
        
        private AbstractBehavior unit;

        public override void OnStart()
        {
            unit = GetComponent<AbstractBehavior>();
        }


        public override TaskStatus OnUpdate()
        {
            UseSelfInventoryItem();

            return TaskStatus.Success;
        }

        private void UseSelfInventoryItem()
        {
            InventoryItemInfo item =  unit.chest.GetInventoryForItemType(itemType);
            
            item.Use(unit);

            unit.chest.RemoveAtChestGrid(item);
        }
    }
}
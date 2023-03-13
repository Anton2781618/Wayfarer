using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// команда взять предмет из инвинтаря таргета по представлению
namespace BehaviorDesigner.Runtime.Tasks.Tutorials
{
    [TaskCategory("Tutorial")]

    public class CommandTakeItemFromTarget : Action
    {
        [SerializeField] private ItemData itemData;
        [SerializeField] private SharedGameObject target;
        private AbstractBehavior mySelf;

        public override void OnStart()
        {
            mySelf = GetComponent<AbstractBehavior>();
        }

        public override TaskStatus OnUpdate()
        {
            TakeInventoryItemFromTarget();

            return TaskStatus.Success;
        }

        private void TakeInventoryItemFromTarget()
        {
            Chest targetChest = target.Value.transform.GetComponent<Chest>();

            InventoryItemInfo item = targetChest.GetInventoryItem(itemData);

            targetChest.RemoveAtChestGrid(item);
            
            mySelf.chest.AddItemToChest(item);
        }
    }
}

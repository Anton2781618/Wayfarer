using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//проверить инвентарь на предмет по представлению
namespace BehaviorDesigner.Runtime.Tasks.Tutorials
{
    [TaskCategory("Tutorial")]
    public class CommandCheckSelfInventoryForItem : Conditional
    {
        // [SerializeField] private ItemData.ItemType itemType;
        [SerializeField] private ItemData itemData;

        private AbstractBehavior unit;

        public override void OnStart()
        {
            unit = GetComponent<AbstractBehavior>();
        }


        public override TaskStatus OnUpdate()
        {
            if(CheckSelfInventoryForItem())
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }

        //проверить свой инвентарб по типу
        private void CheckSelfInventoryForItemType()
        {
            // unit.chest.CheckInventoryForItemsType(itemType);
        }

        private bool CheckSelfInventoryForItem()
        {
            return unit.chest.CheckInventoryForItems(itemData);
        }
    }
}
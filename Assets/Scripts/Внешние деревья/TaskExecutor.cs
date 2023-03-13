using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//шататный класс для всякого
namespace BehaviorDesigner.Runtime.Tasks.Tutorials
{
    [TaskCategory("Tutorial")]
    public class TaskExecutor : Action
    {
        [SerializeField] private Mod mod;

        private UIDialogueTransfer dialogueTransfer;

        private bool isActive = true;
        private enum Mod
        {
            Отключить_Диалог_и_вернуть_успех,
            Вернуть_статус_неактивный,
            Вернуть_статус_провал,
            Сработать_один_раз,
        }
    
        public override void OnStart()
        {
            if(!dialogueTransfer) dialogueTransfer = GameManager.singleton.GetDialogWindow();            
        }

        public override TaskStatus OnUpdate()
        {
            if(mod == Mod.Отключить_Диалог_и_вернуть_успех)
            {
                dialogueTransfer.ShowDialogWindow(false);

                GameManager.singleton.BlockPlayerControl(false); 
            
                return TaskStatus.Success;
            }
            else
            if(mod == Mod.Вернуть_статус_неактивный)
            {
                return TaskStatus.Inactive;
            }
            else
            if(mod == Mod.Вернуть_статус_провал)
            {
                return TaskStatus.Failure;
            }
            else
            if(mod == Mod.Сработать_один_раз)
            {
                if(isActive)
                {
                    isActive = false;
                    
                    return TaskStatus.Success;
                } 

                return TaskStatus.Failure;
            }

            return TaskStatus.Failure;
        }
    }

}

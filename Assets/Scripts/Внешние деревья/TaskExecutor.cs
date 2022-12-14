using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Tutorials
{
    [TaskCategory("Tutorial")]
    public class TaskExecutor : Action
    {
        [SerializeField] private Mod mod;

        private UIDialogueTransfer dialogueTransfer;
        private enum Mod
        {
            Отключить_Диалог,
        }
    
        public override void OnStart()
        {
            if(!dialogueTransfer) dialogueTransfer = GameManager.singleton.GetDialogWindow();

            if(mod == Mod.Отключить_Диалог)
            {
                dialogueTransfer.ShowDialogWindow(false);

                GameManager.singleton.BlockPlayerControl(false); 
            }
        }
    }

}

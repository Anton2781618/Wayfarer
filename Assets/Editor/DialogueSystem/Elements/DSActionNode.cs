using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using DS.ScriptableObjects;

namespace DS.Elements
{
    using System;
    using Data.Save;
    using DS.Enumerations;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEditor.UIElements;
    using Utilities;
    using Windows;

    public class DSActionNode : DSNode
    {       
        private delegate string Operation();
        VisualElement ContainerForTransformation = new VisualElement();
        
        
        public override void Initialize(string nodeName, DSGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, dsGraphView, position);

            DialogueType = DSDialogueType.Action;

            DSChoiceSaveData choiceData = new DSChoiceSaveData()
            {
                Text = "New Choice"
            };

            Choices.Add(choiceData);
        }

        public override void Draw()
        {
            Operation operation = (Operation)System.Delegate.CreateDelegate(typeof(Operation), this, Action.ToString());
            operation.Invoke();

            if(choisenColor == new Color(0,0,0,0)) choisenColor = new Color( 253/255.0f, 228/255.0f, 139/255.0f);

            base.Draw();

            /* MAIN CONTAINER */
            

            // Button addChoiceButton = DSElementUtility.CreateButton("Add Choice", () =>
            // {
            //     DSChoiceSaveData choiceData = new DSChoiceSaveData()
            //     {
            //         Text = "New Choice"
            //     };
                
            //     Choices.Add(choiceData);

            //     Port choicePort = CreateChoicePort(choiceData);

            //     outputContainer.Add(choicePort);
            // });

            // addChoiceButton.AddToClassList("ds-node__button");
            
            // mainContainer.Insert(1, addChoiceButton);

            /* OUTPUT CONTAINER */
            
            foreach (DSChoiceSaveData choice in Choices)
            {
                Port choicePort = CreateChoicePort(choice, false);

                outputContainer.Add(choicePort);
            }

            /* EXTENSION CONTAINER */

            VisualElement customDataContainer = new VisualElement();
            Foldout actionChoicesContainer = DSElementUtility.CreateFoldout(Action.ToString(), true);

            Foldout findСategory = DSElementUtility.CreateFoldout("Найти", true);
            Foldout AttackСategory = DSElementUtility.CreateFoldout("Атака", true);
            Foldout MoveСategory = DSElementUtility.CreateFoldout("Двигаться", true);
            Foldout inventoryСategory = DSElementUtility.CreateFoldout("Инвентарь", true);
            Foldout actionsСategory = DSElementUtility.CreateFoldout("Действия", true);

            findСategory.Add(DSElementUtility.CreateButton("Найти таргет(патруль)", ()=> { actionChoicesContainer.text = CommandFindTheTarget();}));
            findСategory.Add(DSElementUtility.CreateButton("Найти таргет(стоять)", ()=> { actionChoicesContainer.text = CommandHoldPositionFindTheTarget();}));
            AttackСategory.Add(DSElementUtility.CreateButton("Атакавать таргет", ()=> { actionChoicesContainer.text = CommandAttackTheTarget();}));

            MoveСategory.Add(DSElementUtility.CreateButton("Двигаться к таргету", ()=> {actionChoicesContainer.text = CommandMoveToTarget();}));
            MoveСategory.Add(DSElementUtility.CreateButton("Двигаться к корординатам", ()=> {actionChoicesContainer.text = CommandMoveToCoordinates();}));
            MoveСategory.Add(DSElementUtility.CreateButton("Двигаться к на работу", ()=> {actionChoicesContainer.text = CommandMoveToWork();}));

            inventoryСategory.Add(DSElementUtility.CreateButton("Проверить инвентарь на предмет (тагрет)", ()=> {actionChoicesContainer.text = CommandCheckTargetInventoryForItem();}));
            inventoryСategory.Add(DSElementUtility.CreateButton("Проверить инвентарь на предмет (свой)", ()=> {actionChoicesContainer.text = CommandCheckSelfInventoryForItem();}));
            inventoryСategory.Add(DSElementUtility.CreateButton("Забрать из инвентаря таргета предмет", ()=> {actionChoicesContainer.text = CommandTakeItemFromTarget();}));
            inventoryСategory.Add(DSElementUtility.CreateButton("Использовать предмет из своего инвентаря (по типу)", ()=> {actionChoicesContainer.text = CommandUseSelfInventoryItem();}));
            inventoryСategory.Add(DSElementUtility.CreateButton("Забрать деньги у таргета", ()=> {actionChoicesContainer.text = CommandPlayerGiveMoney();}));

            actionsСategory.Add(DSElementUtility.CreateButton("Начать торговлю с таргетом", ()=> {actionChoicesContainer.text = CommandTrading();}));
            actionsСategory.Add(DSElementUtility.CreateButton("Начать диалог с таргетом", ()=> {actionChoicesContainer.text = CommandStartDialogue();}));
            actionsСategory.Add(DSElementUtility.CreateButton("Приступить к работе", ()=> {actionChoicesContainer.text = CommandGetToWork();}));
            actionsСategory.Add(DSElementUtility.CreateButton("Выспаться", ()=> {actionChoicesContainer.text = CommandSleep();}));
            actionsСategory.Add(DSElementUtility.CreateButton("Поднять таргет (предмет)", ()=> {actionChoicesContainer.text = CommandPickUpItem();}));
            
            actionChoicesContainer.Add(findСategory);
            actionChoicesContainer.Add(AttackСategory);
            actionChoicesContainer.Add(MoveСategory);
            actionChoicesContainer.Add(inventoryСategory);
            actionChoicesContainer.Add(actionsСategory);
            
            customDataContainer.Add(actionChoicesContainer);
            extensionContainer.Add(ContainerForTransformation);
            extensionContainer.Add(customDataContainer);
            

            // После добавления пользовательских элементов в extensionContainer вызовите этот метод для того,
            // чтобы они стали видимыми.
            RefreshExpandedState();
        }        

        private void UpdateStyle(Color value)
        {
            mainContainer.style.backgroundColor = value;

            choisenColor = value;
        }

        private Port CreateChoicePort(object userData, bool needDeletButton = true)
        {
            //создаем контейнер с портом
            Port choicePort = this.CreatePort();

            choicePort.userData = userData;

            DSChoiceSaveData choiceData = (DSChoiceSaveData) userData;

            if(needDeletButton)
            {
                Button deleteChoiceButton = DSElementUtility.CreateButton("X", () =>
                {
                    if (Choices.Count == 1)
                    {
                        return;
                    }

                    if (choicePort.connected)
                    {
                        graphView.DeleteElements(choicePort.connections);
                    }

                    Choices.Remove(choiceData);

                    graphView.RemoveElement(choicePort);
                });

                deleteChoiceButton.AddToClassList("ds-node__button");

                choicePort.Add(deleteChoiceButton);
            }

            TextField choiceTextField = DSElementUtility.CreateTextField(choiceData.Text, null, callback =>
            {
                choiceData.Text = callback.newValue;
            });

            choiceTextField.AddClasses(
                "ds-node__text-field",
                "ds-node__text-field__hidden",
                "ds-node__choice-text-field"
            );

            choicePort.Add(choiceTextField);

            return choicePort;
        }

        public override void ResetStyle()
        {
            mainContainer.style.backgroundColor = choisenColor;
        }

        private Port lastPort;
        private void AddPort()
        {
            DSChoiceSaveData choiceData = new DSChoiceSaveData()
            {
                Text = "New Choice"
            };
            
            Choices.Add(choiceData);

            lastPort = CreateChoicePort(choiceData, false);

            outputContainer.Add(lastPort);
        }

        private void DeleteLastPort()
        {            
            if (lastPort.connected)
            {
                graphView.DeleteElements(lastPort.connections);
            }

            Choices.RemoveAt(Choices.Count - 1);

            graphView.RemoveElement(lastPort);
        }


        private void InsertPorts(int value)
        {
            Choices.Clear();
            outputContainer.Clear();

            DSChoiceSaveData choiceData = new DSChoiceSaveData()
            {
                Text = "Новый выбор"
            };

            for (int i = 0; i < value; i++)
            {
                Choices.Add(choiceData);    
            }
            
            foreach (DSChoiceSaveData choice in Choices)
            {
                Port choicePort = CreateChoicePort(choice, false);

                outputContainer.Add(choicePort);
            }
        }


        
        private string CommandAttackTheTarget()
        {
            Action = DSAction.CommandAttackTheTarget;
            
            ContainerForTransformation.Clear();
            
            return "Атакавать игрока";
        }
        private string CommandFindTheTarget()
        {
            Action = DSAction.CommandFindTheTarget;
            
            ContainerForTransformation.Clear();

            ContainerForTransformation.Add(DSElementUtility.CreateLayerMaskField(modelDate.targetMask, x => modelDate.targetMask = (int)x.newValue));
            
            return "Найти таргет(патруль)";
        }
        private string CommandHoldPositionFindTheTarget()
        {
            Action = DSAction.CommandHoldPositionFindTheTarget;
            
            ContainerForTransformation.Clear();

            ContainerForTransformation.Add(DSElementUtility.CreateLayerMaskField(modelDate.targetMask, x => modelDate.targetMask = (int)x.newValue));
            
            return "Найти таргет(стоять)";
        }
        private string CommandMoveToTarget()
        {
            Action = DSAction.CommandMoveToTarget;
            
            ContainerForTransformation.Clear();
            
            return "Двигаться к таргету";
        }
        private string CommandCheckTargetInventoryForItem()
        {
            Action = DSAction.CommandCheckTargetInventoryForItem;
            
            ContainerForTransformation.Clear();
            
            ContainerForTransformation.Add(DSElementUtility.CreateObjectField(modelDate.itemData, x => modelDate.itemData = (ItemData)x.newValue));           

            return "Проверить инвентарь на предмет (тагрет)";
        }
        private string CommandCheckSelfInventoryForItem()
        {
            Action = DSAction.CommandCheckSelfInventoryForItem;
            
            ContainerForTransformation.Clear();
            
            ContainerForTransformation.Add(DSElementUtility.CreateObjectField(modelDate.itemData, x => modelDate.itemData = (ItemData)x.newValue));  

            Debug.Log(Choices.Count);

            if(Choices.Count < 2)
            {
                AddPort();
            }

            return "Проверить свой инвентарь на предмет (свой)";
        }
        private string CommandTakeItemFromTarget()
        {
            Action = DSAction.CommandTakeItemFromTarget;
            
            ContainerForTransformation.Clear();
            
            ContainerForTransformation.Add(DSElementUtility.CreateObjectField(modelDate.itemData, x => modelDate.itemData = (ItemData)x.newValue)); 

            if(Choices.Count > 1)
            {
                DeleteLastPort();          
            }

            return "Забрать из инвентаря таргета предмет";
        }
        private string CommandUseSelfInventoryItem()
        {
            Action = DSAction.CommandUseSelfInventoryItem;
            
            ContainerForTransformation.Clear();
            
            ContainerForTransformation.Add(DSElementUtility.CreateObjectField(modelDate.itemData, x => modelDate.itemData = (ItemData)x.newValue));

            return "Использовать предмет из своего инвентаря (по типу)";
        }
        private string CommandTrading()
        {
            Action = DSAction.CommandTrading;
            
            ContainerForTransformation.Clear();
            
            return "Начать торговлю";
        }
        private string NotAction()
        {
            ContainerForTransformation.Clear();

            return "Нет действий";
        }

        private string CommandStartDialogue()
        {
            Action = DSAction.CommandStartDialogue;
            
            ContainerForTransformation.Clear();

            ContainerForTransformation.Add(DSElementUtility.CreateObjectFieldDSDialogueSO(modelDate.dialogue, x => modelDate.dialogue = (DSDialogueContainerSO)x.newValue));
            
            
            return "Начать диалог";
        }

        private string CommandGetToWork()
        {
            Action = DSAction.CommandGetToWork;
            
            ContainerForTransformation.Clear();
            
            return "Приступить к работе";
        }

        private string CommandSleep()
        {
            Action = DSAction.CommandSleep;
            
            ContainerForTransformation.Clear();
            
            return "Выспаться";
        }
        
        private string CommandPlayerGiveMoney()
        {
            Action = DSAction.CommandPlayerGiveMoney;
            
            ContainerForTransformation.Clear();

            ContainerForTransformation.Add(DSElementUtility.CreateFloatField(modelDate.number, null, collBack => modelDate.number = collBack.newValue));
            
            return "Забрать деньги у таргета";
        }
        private string CommandPickUpItem()
        {
            Action = DSAction.CommandPickUpItem;
            
            ContainerForTransformation.Clear();
            
            return "Поднять таргет (предмет)";
        }
        private string CommandMoveToCoordinates()
        {
            Action = DSAction.CommandMoveToCoordinates;
            
            ContainerForTransformation.Clear();

            ContainerForTransformation.Add(DSElementUtility.CreateFloatField(modelDate.pos.x, null, collBack => modelDate.pos.x = collBack.newValue));
            ContainerForTransformation.Add(DSElementUtility.CreateFloatField(modelDate.pos.y, null, collBack => modelDate.pos.y = collBack.newValue));
            ContainerForTransformation.Add(DSElementUtility.CreateFloatField(modelDate.pos.z, null, collBack => modelDate.pos.z = collBack.newValue));
            
            return "Двигаться к корординатам";
        }
        private string CommandMoveToWork()
        {
            Action = DSAction.CommandMoveToWork;
            
            ContainerForTransformation.Clear();
            
            return "Двигаться к на работу";
        }
    }
}


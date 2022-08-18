using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


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

            if(choisenColor == new Color(0,0,0,0)) choisenColor = new Color( 205/255.0f, 149/255.0f, 117/255.0f);
            base.Draw();

            /* MAIN CONTAINER */
            

            Button addChoiceButton = DSElementUtility.CreateButton("Add Choice", () =>
            {
                DSChoiceSaveData choiceData = new DSChoiceSaveData()
                {
                    Text = "New Choice"
                };
                
                Choices.Add(choiceData);

                Port choicePort = CreateChoicePort(choiceData);

                outputContainer.Add(choicePort);
            });

            addChoiceButton.AddToClassList("ds-node__button");
            
            mainContainer.Insert(1, addChoiceButton);

            /* OUTPUT CONTAINER */
            
            foreach (DSChoiceSaveData choice in Choices)
            {
                Port choicePort = CreateChoicePort(choice);

                outputContainer.Add(choicePort);
            }

            /* EXTENSION CONTAINER */

            VisualElement customDataContainer = new VisualElement();
            

            Foldout actionTextFoldout = DSElementUtility.CreateFoldout(Action.ToString(), true);
            
            Button[] buttons =
            {                
                DSElementUtility.CreateButton("Атакавать игрока", ()=> { actionTextFoldout.text = CommandAttackTheTarget();}),
                DSElementUtility.CreateButton("Двигаться к цели", ()=> {actionTextFoldout.text = CommandMoveToTarget();}),
                DSElementUtility.CreateButton("Двигаться к корординатам", ()=> {actionTextFoldout.text = CommandMoveToCoordinates();}),
                
                DSElementUtility.CreateButton("Проверить инвентарь на предмет", ()=> {actionTextFoldout.text = CheckInventoryForItem();}),
                
                DSElementUtility.CreateButton("Начать торговлю", ()=> {actionTextFoldout.text = CommandTrading();}),
                DSElementUtility.CreateButton("Начать диалог", ()=> {actionTextFoldout.text = CommandStartDialogue();}),
                
                DSElementUtility.CreateButton("Дать денег", ()=> {actionTextFoldout.text = CommandPlayerGiveMoney();}),
                
                DSElementUtility.CreateButton("Нет действий", ()=> {actionTextFoldout.text = NotAction();}),                
                DSElementUtility.CreateButton("Выйти из диалога", ()=> {actionTextFoldout.text = ExitTheDialog(); }),
                
            };

            foreach (var action in buttons)
            {
                actionTextFoldout.Add(action);
            }
            
            customDataContainer.Add(actionTextFoldout);
            // ContainerForTransformation.Add(addChoiceButtondd);
            
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

        private Port CreateChoicePort(object userData)
        {
            //создаем контейнер с портом
            Port choicePort = this.CreatePort();

            choicePort.userData = userData;

            DSChoiceSaveData choiceData = (DSChoiceSaveData) userData;

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
            choicePort.Add(deleteChoiceButton);

            return choicePort;
        }

        public override void ResetStyle()
        {
            mainContainer.style.backgroundColor = choisenColor;
        }
        
        
        private string NotAction()
        {
            ContainerForTransformation.Clear();
            return "Нет действий";
        }
        private string CommandAttackTheTarget()
        {
            Action = DSAction.CommandAttackTheTarget;
            ContainerForTransformation.Clear();
            return "Атакавать игрока";
        }
        private string CommandMoveToTarget()
        {
            Action = DSAction.CommandMoveToTarget;
            ContainerForTransformation.Clear();
            return "Двигаться к цели";
        }
        private string CheckInventoryForItem()
        {
            Action = DSAction.CheckInventoryForItem;
            
            ContainerForTransformation.Clear();
            
            ContainerForTransformation.Add(DSElementUtility.CreateObjectField(modelDate.itemData, x => modelDate.itemData = (ItemData)x.newValue));
            return "Проверить инвентарь на предмет";
        }
        private string CommandTrading()
        {
            Action = DSAction.CommandTrading;
            
            ContainerForTransformation.Clear();
            
            return "Начать торговлю";
        }

        private string CommandStartDialogue()
        {
            Action = DSAction.CommandStartDialogue;
            
            ContainerForTransformation.Clear();
            
            return "Начать диалог";
        }
        private string CommandPlayerGiveMoney()
        {
            Action = DSAction.CommandPlayerGiveMoney;
            
            ContainerForTransformation.Clear();

            ContainerForTransformation.Add(DSElementUtility.CreateFloatField(modelDate.number, null, collBack => modelDate.number = collBack.newValue));
            
            return "Дать денег";
        }
        private string ExitTheDialog()
        {
            Action = DSAction.ExitTheDialog;
            
            ContainerForTransformation.Clear();
            
            UpdateStyle(Color.yellow);
            
            return "Выйти из диалога";
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
    }
}


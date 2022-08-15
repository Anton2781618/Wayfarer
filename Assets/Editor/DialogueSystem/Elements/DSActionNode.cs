using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace DS.Elements
{
    using Data.Save;
    using DS.Enumerations;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEditor.UIElements;
    using Utilities;
    using Windows;

    
    public class DSActionNode : DSNode
    {       
        
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
            if(choisenColor == new Color(0,0,0,0)) choisenColor = Color.green;
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
            ObjectField addChoiceButtondd = DSElementUtility.CreateObjectField(modelDate.itemData, x => modelDate.itemData = (ItemData)x.newValue);

            mainContainer.Insert(1, addChoiceButtondd);
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
                DSElementUtility.CreateButton("Атакавать игрока", ()=> {actionTextFoldout.text = "Атакавать игрока"; Action = DSAction.CommandAttackTheTarget;}),
                DSElementUtility.CreateButton("Убегать", ()=> {actionTextFoldout.text = "Убегать"; Action = DSAction.CommandRetreat;}),
                DSElementUtility.CreateButton("Двигаться к цели", ()=> {actionTextFoldout.text = "двигаться к цели"; Action = DSAction.CommandMoveToTarget;}),
                
                DSElementUtility.CreateButton("Проверить инвентарь на предмет", ()=> 
                {
                    actionTextFoldout.text = "Проверить инвентарь на предмет"; 
                    Action = DSAction.CheckInventoryForItem; 

                    addChoiceButtondd.objectType = typeof(ItemData);
                }),
                DSElementUtility.CreateButton("Проверка информации", ()=> {actionTextFoldout.text = "Проверка информации"; Action = DSAction.CheckingAvailabilityInformation;}),
                DSElementUtility.CreateButton("Начать торговлю", ()=> {actionTextFoldout.text = Action.ToString(); Action = DSAction.CommandTrading;}),
                DSElementUtility.CreateButton("Начать диалог", ()=> {actionTextFoldout.text = "Начать диалог"; Action = DSAction.CommandStartDialogue;}),
                DSElementUtility.CreateButton("Дать денег", ()=> {actionTextFoldout.text = "Дать денег"; Action = DSAction.CommandPlayerGiveMoney;}),
                
                DSElementUtility.CreateButton("Нет действий", ()=> {actionTextFoldout.text = "Нет действий"; Action = DSAction.NotAction; Debug.Log(modelDate);}),
                
                DSElementUtility.CreateButton("Выйти из диалога", ()=> {actionTextFoldout.text = "Выйти из диалога"; Action = DSAction.ExitTheDialog; UpdateStyle(Color.yellow);}),
                
            };

            foreach (var action in buttons)
            {
                actionTextFoldout.Add(action);
            }
            
            customDataContainer.Add(actionTextFoldout);

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
    }
}


using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using DS.ScriptableObjects;

namespace DS.Utilities
{
    using DS.Windows;
    using Elements;
    using UnityEditor.UIElements;

    public static class DSElementUtility
    {
        public static Button CreateButton(string text, Action onClick = null)
        {
            Button button = new Button(onClick)
            {
                text = text
            };

            return button;
        }

        public static Foldout CreateFoldout(string title, bool collapsed = false)
        {
            Foldout foldout = new Foldout()
            {
                text = title,
                value = !collapsed
            };

            return foldout;
        }

        public static Port CreatePort(this DSNode node, string portName = "", Orientation orientation = Orientation.Horizontal, Direction direction = Direction.Output, Port.Capacity capacity = Port.Capacity.Single)
        {
            Port port = node.InstantiatePort(orientation, direction, capacity, typeof(bool));
            
            port.portName = portName;
            
            return port;
        }        

        public static TextField CreateTextField(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            TextField textField = new TextField()
            {
                value = value,
                label = label
            };

            if (onValueChanged != null)
            {
                textField.RegisterValueChangedCallback(onValueChanged);
            }

            return textField;
        }
        

        public static TextField CreateTextArea(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            TextField textArea = CreateTextField(value, label, onValueChanged);

            textArea.multiline = true;

            return textArea;
        }
        public static ObjectField CreateObjectField(UnityEngine.Object value = null, EventCallback<ChangeEvent<UnityEngine.Object>> onValueChanged = null)
        {
            ObjectField objectField = new ObjectField()
            {
                value = value
            };

            objectField.objectType = typeof(ItemData);

            if (onValueChanged != null)
            {
                objectField.RegisterValueChangedCallback(onValueChanged);
            }

            return objectField;
        }
        public static ObjectField CreateObjectFieldDSDialogueSO(UnityEngine.Object value = null, EventCallback<ChangeEvent<UnityEngine.Object>> onValueChanged = null)
        {
            ObjectField objectField = new ObjectField()
            {
                value = value
            };

            objectField.objectType = typeof(DSDialogueContainerSO);

            if (onValueChanged != null)
            {
                objectField.RegisterValueChangedCallback(onValueChanged);
            }

            return objectField;
        }

        public static FloatField CreateFloatField(float value = 0, string label = null, EventCallback<ChangeEvent<float>> onValueChanged = null)
        {
            FloatField floatField = new FloatField()
            {
                value = value,
                label = label
            };

            if (onValueChanged != null)
            {
                floatField.RegisterValueChangedCallback(onValueChanged);
            }

            return floatField;
        }
        public static LayerMaskField CreateLayerMaskField(int value = 0, EventCallback<ChangeEvent<int>> onValueChanged = null)
        {
            LayerMaskField floatField = new LayerMaskField()
            {
                value = value,
            };

            if (onValueChanged != null)
            {
                floatField.RegisterValueChangedCallback(onValueChanged);
            }

            return floatField;
        }
        
        public static EnumFlagsField CreateItemTypeFlagsField(Enum value, EventCallback<ChangeEvent<Enum>> onValueChanged = null)
        {
            EnumFlagsField floatField = new EnumFlagsField(value);

            if (onValueChanged != null)
            {
                floatField.RegisterValueChangedCallback(onValueChanged);
            }

            return floatField;
        }
        public static EnumField CreateItemTypeField(Enum value, EventCallback<ChangeEvent<Enum>> onValueChanged = null)
        {
            EnumField floatField = new EnumField(value);

            if (onValueChanged != null)
            {
                floatField.RegisterValueChangedCallback(onValueChanged);
            }

            return floatField;
        }
    }
}
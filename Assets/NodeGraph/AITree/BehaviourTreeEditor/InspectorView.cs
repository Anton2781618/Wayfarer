using UnityEngine.UIElements;
using UnityEditor;

//класс является представлением левой части редактора
public class InspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits>{} 
    
    private Editor editor;
    public InspectorView() 
    {

    }

    //метод заполняет левую часть редактора
    internal void UpdateSelection(AINodeView nodeView)
    {
        Clear();

        UnityEngine.Object.DestroyImmediate(editor);

        editor = Editor.CreateEditor(nodeView.node);
        
        IMGUIContainer container = new IMGUIContainer(()=> { editor.OnInspectorGUI(); });
        
        Add(container);
    }
}

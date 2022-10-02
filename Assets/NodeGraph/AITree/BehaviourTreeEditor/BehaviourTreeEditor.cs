using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

//класс является инициализатором окна для графов
public class BehaviourTreeEditor : EditorWindow
{
    private BehaviourTreeView treeView;
    private InspectorView InspectorView;

    [MenuItem("Window/DS/AITree")]
    public static void OpenWindow()
    {
        BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviourTreeEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;       

        // Instantiate UXML
        var labelFromUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/NodeGraph/AITree/BehaviourTreeEditor/BehaviourTreeEditor.uxml");
        labelFromUXML.CloneTree(root);

        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/NodeGraph/AITree/BehaviourTreeEditor/BehaviourTreeEditor.uss");
        root.styleSheets.Add(styleSheet); 

        treeView = root.Q<BehaviourTreeView>();
        InspectorView = root.Q<InspectorView>();
    }

    //это зарезервированный метод, запускается при выборе любого объекта в инспектаре
    private void OnSelectionChange()
    {
        BehaviourTree tree = Selection.activeObject as BehaviourTree;

        if(tree)
        {
            treeView.PopulateView(tree);
        }
    }
}

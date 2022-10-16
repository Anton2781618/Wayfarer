using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Callbacks;

//класс является инициализатором окна для графов
public class BehaviourTreeEditor : EditorWindow
{
    private BehaviourTreeView treeView;
    private InspectorView InspectorView;
    private IMGUIContainer blackboardView;
    private SerializedObject treeObject;
    private SerializedProperty blackboardProperty;

    [MenuItem("Window/DS/Редактор ИИ")]
    public static void OpenWindow()
    {
        BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviourTreeEditor");
    }

    [OnOpenAsset]
    public static bool OnOpenAsset(int instance, int line)
    {
        if(Selection.activeObject is BehaviourTree)
        {
            OpenWindow();
            return true;
        }

        return false;
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;       

        // Instantiate UXML
        var labelFromUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/NodeGraph/AITree/BehaviourTreeEditor/BehaviourTreeEditor.uxml");
        labelFromUXML.CloneTree(root);

        // тут мы подключаетм файл стилей 
        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/NodeGraph/AITree/BehaviourTreeEditor/BehaviourTreeEditor.uss");
        root.styleSheets.Add(styleSheet); 

        treeView = root.Q<BehaviourTreeView>();

        InspectorView = root.Q<InspectorView>();

        blackboardView = root.Q<IMGUIContainer>();

        blackboardView.onGUIHandler = () => 
        {
            treeObject.Update();

            EditorGUILayout.PropertyField(blackboardProperty);

            treeObject.ApplyModifiedProperties();
        };

        treeView.OnNodeSelected = OnNodeSelectionChanged;

        OnSelectionChange();
    }

    private void OnEnable() 
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }


    private void OnDisable() 
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange obj)
    {
        switch (obj)
        {
            case PlayModeStateChange.EnteredEditMode:
            OnSelectionChange();
            
            break;

            case PlayModeStateChange.ExitingEditMode:
            break;
            
            case PlayModeStateChange.EnteredPlayMode:
            OnSelectionChange();
            
            break;
            
            case PlayModeStateChange.ExitingPlayMode:
            break;
        }
    }

    //это зарезервированный метод, запускается при выборе любого объекта в инспектаре
    private void OnSelectionChange()
    {
        
        BehaviourTree tree = Selection.activeObject as BehaviourTree;
        
        if(!tree)
        {
            if(Selection.activeGameObject)
            {
                BehaviourTreeRunner runner = Selection.activeGameObject.GetComponent<BehaviourTreeRunner>();

                if(runner)
                {
                    tree = runner.tree;
                }
            }
        }

        if(Application.isPlaying)
        {
            if(tree)
            {
                treeView.PopulateView(tree);
            }
        }
        else
        {
            if(tree && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
            {
                treeView.PopulateView(tree);
            }
        }
        if(tree != null)
        {
            treeObject = new SerializedObject(tree);

            //FindProperty с помощью этоо метода можно получать доступ к полю сериализованного объекта
            blackboardProperty = treeObject.FindProperty("blackboard");
        }
    }

    private void OnNodeSelectionChanged(AINodeView node)
    {
        InspectorView.UpdateSelection(node);
    }

    //этот метод переопределенный, он обновляет инспектор в теле этого метода мы обновляем внешность нодов 
    private void OnInspectorUpdate() 
    {
        treeView?.UpdateNodeStates();
    }
}

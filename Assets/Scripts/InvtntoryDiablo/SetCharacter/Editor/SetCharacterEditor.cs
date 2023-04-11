using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CartoonHeroes
{
   
    [CustomEditor(typeof(Chest))] /* [CanEditMultipleObjects] */
    public class SetCharacterEditor : Editor
    {
        const int defaultSpace = 8;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Chest chest = (Chest)target;
            
            // serializedObject.Update();

            GUILayout.Space(defaultSpace);

            chest.clothes.characterRoot = EditorGUILayout.ObjectField("Character Root", chest.clothes.characterRoot, typeof(Transform), true) as Transform;

            GUILayout.Space(defaultSpace);

            DrawDetails(chest);
                
        }

        private void DrawDetails(Chest chest)
        {
            if (chest.clothes.items != null && chest.clothes.items.Count > 0)
            {
                GUILayout.Space(defaultSpace);

                
                // EditorGUILayout.BeginVertical("Box");
                for(int n = 0; n < chest.clothes.items.Count; n++)
                {
                    EditorGUILayout.BeginHorizontal("Box");

                    if (!chest.clothes.HasItem(n))
                    {
                        chest.clothes.items[n].name = "название слота";
                        
                        GUILayout.Label(chest.clothes.items[n].name);

                        if (GUILayout.Button("Надеть"))
                        {
                            GameObject addedObj = chest.clothes.AddItem(n);
                            // Undo.RegisterCreatedObjectUndo(addedObj, "Added Item");
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Снять"))
                        {
                            List<GameObject> removedObjs = chest.clothes.GetRemoveObjList(n);
                            
                            for(int m = 0; m < removedObjs.Count; m ++)
                            {
                                if(removedObjs[m] != null)
                                {
                                    // Undo.DestroyObjectImmediate(removedObjs[m]); <- глючит капец
                                    DestroyImmediate(removedObjs[m]);
                                }
                            }
                        }
                    }

                    if(chest.clothes.items[n] != null)
                    {
                        chest.clothes.items[n].prefab = 
                        EditorGUILayout.ObjectField(chest.clothes.items[n].prefab, typeof(GameObject), true) as GameObject;
                    }
                    
                    EditorGUILayout.BeginHorizontal("Box");
                    
                    if (GUILayout.Button("X"))
                    {
                        chest.clothes.items.RemoveAt(n);

                        break;
                    }
                    
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.EndHorizontal();
                }

                GUILayout.Space(defaultSpace);
            }
        }

        // serializedObject.ApplyModifiedProperties();
    }
}



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
        public bool _showBckgrounds = false;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Chest chest = (Chest)target;

            
            // GUILayout.Space(defaultSpace);

            // chest.clothes.characterRoot = EditorGUILayout.ObjectField("Character Root", chest.clothes.characterRoot, typeof(Transform), true) as Transform;

            // GUILayout.Space(defaultSpace);


            EditorGUILayout.BeginHorizontal("Box");
            
            _showBckgrounds = EditorGUILayout.Foldout(_showBckgrounds, "Одежда", true);

            EditorGUILayout.EndHorizontal();



            if(_showBckgrounds)
            {
                EditorGUILayout.BeginVertical("Box");
                
                if (GUILayout.Button("Пересоздать слоты"))
                {
                    if(chest.Clothes != null && chest.Clothes.characterRoot != null)
                    {
                        for(int n = 0; n < chest.Clothes.items.Count; n++)
                        {
                            List<GameObject> removedObjs = chest.Clothes.GetRemoveObjList(n);
                                        
                            for(int m = 0; m < removedObjs.Count; m ++)
                            {
                                if(removedObjs[m] != null)
                                {
                                    DestroyImmediate(removedObjs[m]);
                                }
                            }
                        }

                        chest.Clothes.items.Clear();
                    }
                    
                    CreateCloches(chest);
                }
                EditorGUILayout.EndVertical();
             
                DrawDetails(chest);
            }
        }

        private void DrawDetails(Chest chest)
        {
            if (chest.Clothes != null && chest.Clothes.items != null && chest.Clothes.items.Count > 0)
            {
                // GUILayout.Space(defaultSpace);

                
                // EditorGUILayout.BeginVertical("Box");
                for(int n = 0; n < chest.Clothes.items.Count; n++)
                {
                    EditorGUILayout.BeginHorizontal("Box");

                    GUILayout.Label(chest.Clothes.items[n].ItemType.ToString(), GUILayout.Width(100));
                    
                    if (!chest.Clothes.HasItem(n))
                    {

                        if (GUILayout.Button("Надеть"))
                        {
                            GameObject addedObj = chest.Clothes.AddItem(n);
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Снять"))
                        {
                            List<GameObject> removedObjs = chest.Clothes.GetRemoveObjList(n);
                            
                            for(int m = 0; m < removedObjs.Count; m ++)
                            {
                                if(removedObjs[m] != null)
                                {
                                    DestroyImmediate(removedObjs[m]);
                                }
                            }
                        }
                    }

                    if(chest.Clothes.items[n] != null)
                    {
                        chest.Clothes.items[n].prefab = 
                        EditorGUILayout.ObjectField(chest.Clothes.items[n].prefab, typeof(GameObject), true) as GameObject;
                    }
                    
                    // if (GUILayout.Button("X"))
                    // {
                    //     chest.clothes.items.RemoveAt(n);

                    //     break;
                    // }
                    
                    EditorGUILayout.EndHorizontal();
                }

                GUILayout.Space(defaultSpace);
            }
        }

        private void CreateCloches(Chest chest)
        {
            chest.Clothes = new SetCharacter(chest.transform);

            chest.Clothes.items.Add(new SetCharacter.Item(ItemData.ItemType.Шлем));

            chest.Clothes.items.Add(new SetCharacter.Item(ItemData.ItemType.Броня));

            chest.Clothes.items.Add(new SetCharacter.Item(ItemData.ItemType.Ремень));

            chest.Clothes.items.Add(new SetCharacter.Item(ItemData.ItemType.Штаны));

            chest.Clothes.items.Add(new SetCharacter.Item(ItemData.ItemType.Сапоги));

            chest.Clothes.items.Add(new SetCharacter.Item(ItemData.ItemType.Оружие));

            chest.Clothes.items.Add(new SetCharacter.Item(ItemData.ItemType.Щит));

            chest.Clothes.items.Add(new SetCharacter.Item(ItemData.ItemType.Кольцо));

            chest.Clothes.items.Add(new SetCharacter.Item(ItemData.ItemType.Кольцо));

            chest.Clothes.items.Add(new SetCharacter.Item(ItemData.ItemType.Наплечники));

            chest.Clothes.items.Add(new SetCharacter.Item(ItemData.ItemType.Ожерелье));
        }
    }
}



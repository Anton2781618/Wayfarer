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
                            chest.Clothes.AddItem(n);
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Снять"))
                        {
                            chest.Clothes.RemoveItem(n);
                            // List<GameObject> removedObjs = chest.Clothes.GetRemoveObjList(n);
                            
                            // for(int m = 0; m < removedObjs.Count; m ++)
                            // {
                            //     if(removedObjs[m] != null)
                            //     {
                            //         DestroyImmediate(removedObjs[m]);
                            //     }
                            // }
                        }
                    }

                    if(chest.Clothes.items[n] != null)
                    {
                        chest.Clothes.items[n].Prefab = 
                        EditorGUILayout.ObjectField(chest.Clothes.items[n].Prefab, typeof(ItemOnstreet), true) as ItemOnstreet;
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
            chest.Clothes = new SetCharacter(chest.transform, chest.transform.GetComponent<Animator>());

            chest.Clothes.items.Add(new SetCharacter.Item(ItemData.ItemType.Шлем, false, HumanBodyBones.Head));

            chest.Clothes.items.Add(new SetCharacter.Item(ItemData.ItemType.Броня, true));

            chest.Clothes.items.Add(new SetCharacter.Item(ItemData.ItemType.Ремень, true));

            chest.Clothes.items.Add(new SetCharacter.Item(ItemData.ItemType.Штаны, true));

            chest.Clothes.items.Add(new SetCharacter.Item(ItemData.ItemType.Сапоги, true));

            chest.Clothes.items.Add(new SetCharacter.Item(ItemData.ItemType.Оружие, false, HumanBodyBones.RightHand));

            chest.Clothes.items.Add(new SetCharacter.Item(ItemData.ItemType.Щит, false, HumanBodyBones.LeftHand));

            chest.Clothes.items.Add(new SetCharacter.Item(ItemData.ItemType.Кольцо, false, HumanBodyBones.RightHand));

            chest.Clothes.items.Add(new SetCharacter.Item(ItemData.ItemType.Кольцо, false, HumanBodyBones.LeftHand));

            chest.Clothes.items.Add(new SetCharacter.Item(ItemData.ItemType.Наплечники, true));

            chest.Clothes.items.Add(new SetCharacter.Item(ItemData.ItemType.Ожерелье, false, HumanBodyBones.Neck));
        }
    }
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CartoonHeroes
{
    [System.Serializable]
    public class SetCharacter
    {
        public SetCharacter(Transform myTransform)
        {
            characterRoot = myTransform;
        }
        
        public Transform characterRoot;
        public List<Item> items = new List<Item>();
        const string namePrefix = "Set Character_";
        const string hideString = "(Hide)";
        public GameObject disabledGraySkeleton;
        
        [System.Serializable]
        public class Item
        {
            public Item( ItemData.ItemType itemType)
            {
                ItemType = itemType;
            }
            public string name;
            public ItemData.ItemType ItemType;
            public GameObject prefab;
        }

        public GameObject AddItem(int itemSlot)
        {
            Item item = items[itemSlot];
            
            GameObject itemInstance = GameObject.Instantiate(item.prefab);
            
            // itemInstance.GetComponent<Collider>().enabled = false;
            Object.DestroyImmediate(itemInstance.GetComponent<Collider>());
            
            // DestroyImmediate(itemInstance.GetComponent<Outline>());

            Object.DestroyImmediate(itemInstance.GetComponent<ItemOnstreet>());

            Object.DestroyImmediate(itemInstance.GetComponent<Rigidbody>());

            itemInstance.name = itemInstance.name.Substring(0, itemInstance.name.Length - "(Clone)".Length);

            RemoveAnimator(itemInstance);
            
            ParentObjectAndBones(itemInstance);

            SetGraySkeletonVisibility(!VisibleItems());

            return itemInstance;
        }

        public void RemoveItem(int itemSlot)
        {
            List<GameObject> removedObjs = GetRemoveObjList(itemSlot);
                            
            for(int m = 0; m < removedObjs.Count; m ++)
            {
                if(removedObjs[m] != null)
                {
                    Object.DestroyImmediate(removedObjs[m]);
                }
            }
        }
        
        public bool VisibleItems()
        {
            for(int n = 0; n < items.Count; n++)
            {
                if(HasItem(n))
                {
                    return true;
                }
            }

            return false;
        }

        public void SetGraySkeletonVisibility(bool set)
        {
            if (!set)
            {
                Transform[] allCharacterChildren = GetAllCharacterChildren();
                
                for (int i = 0; i < allCharacterChildren.Length; i++)
                {
                    if (allCharacterChildren[i].name.Contains(hideString))
                    {
                        disabledGraySkeleton = allCharacterChildren[i].gameObject;
                        allCharacterChildren[i].gameObject.SetActive(false);
                        break;
                    }
                }
             }
            else {
                if (disabledGraySkeleton != null)
                {
                    disabledGraySkeleton.SetActive(true);
                }
            }

        }

        public bool HasItem(int itemSlot)
        {
            if (items[itemSlot].prefab != null)
            {

                Transform root = GetRoot();
                Transform prefab = items[itemSlot].prefab.transform;
                for (int i = 0; i < root.childCount; i++)
                {
                    Transform child = root.GetChild(i); 
                    if (child.name.Contains(prefab.name) && child.name.Contains(namePrefix))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        //удочерить кости одежды к костям персонажа 
        public void ParentObjectAndBones(GameObject itemInstance)
        {
            Transform[] allCharacterChildren = GetAllCharacterChildren();
            Transform[] allItemChildren = itemInstance.GetComponentsInChildren<Transform>();
            
            itemInstance.transform.position = characterRoot.transform.position;
            itemInstance.transform.parent = characterRoot.transform;

            string[] allItemChildren_NewNames= new string[allItemChildren.Length];

            for(int i = 0; i < allItemChildren.Length; i++)
            {
                //Совпадающие и родительские кости
                for (int n = 0; n < allCharacterChildren.Length; n++)
                {
                    if(allItemChildren[i].name == allCharacterChildren[n].name)
                    {
                        MatchTransform(allItemChildren[i], allCharacterChildren[n]);
                        allItemChildren[i].parent = allCharacterChildren[n];
                    }
                }

                //Rename
                allItemChildren_NewNames[i] = allItemChildren[i].name;

                if (!allItemChildren[i].name.Contains(namePrefix))
                {
                    allItemChildren_NewNames[i] = namePrefix + allItemChildren[i].name;
                }

                if (!allItemChildren[i].name.Contains(itemInstance.name))
                {
                    allItemChildren_NewNames[i] += "_" + itemInstance.name;
                }
            }

            for(int i = 0; i < allItemChildren.Length; i++)
            {
                allItemChildren[i].name = allItemChildren_NewNames[i];
            }
        }

        public Transform GetRoot()
        {
            return characterRoot;
        }

        public Transform[] GetAllCharacterChildren()
        {
            Transform root = GetRoot();
            Transform[] allCharacterChildren = root.GetComponentsInChildren<Transform>();

            /*List<Transform> allCharacterChildren_List = new List<Transform>();
            
            for(int i = 0; i < allCharacterChildren.Length; i++){
                if(allCharacterChildren[i].GetComponent<SkinnedMeshRenderer>() != null || allCharacterChildren[i].GetComponent<Animator>() != null)
                {
                    continue;
                }
                allCharacterChildren_List.Add(allCharacterChildren[i]);
            }

            allCharacterChildren = allCharacterChildren_List.ToArray();*/

            return allCharacterChildren;
        }

        public bool BelongsToItem(Transform obj, int itemSlot)
        {
            if(obj == null || items[itemSlot].prefab == null)
            {
                return false;
            }
            return (obj.name.Contains(namePrefix) && obj.name.Contains(items[itemSlot].prefab.name));
        }

        //удаляет аниматор с одежды 
        public void RemoveAnimator(GameObject item)
        {
            Animator animator = item.GetComponent<Animator>();
            if(animator != null)
            {
                Object.DestroyImmediate(animator);
            }
        }

        public void MatchTransform(Transform obj, Transform target)
        {
            obj.position = target.position;
            obj.rotation = target.rotation;
        }

        public List<GameObject> GetRemoveObjList(int itemSlot)
        {
            Transform[] allChildren = GetAllCharacterChildren();

            List<GameObject> removeList = new List<GameObject>();
            
            for (int i = 0; i < allChildren.Length; i++)
            {
                if(BelongsToItem(allChildren[i], itemSlot))
                {
                    //DestroyImmediate(allChildren[i].gameObject);
                    removeList.Add(allChildren[i].gameObject);
                }
            }

            SetGraySkeletonVisibility(!VisibleItems());
            return removeList;
        }
    }
}


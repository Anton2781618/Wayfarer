using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CartoonHeroes
{
    [System.Serializable]
    public class SetCharacter
    {
        public SetCharacter(Transform myTransform, Animator anim)
        {
            characterRoot = myTransform;

            animator = anim;
        }
        
        public Transform characterRoot;
        public Animator animator;
        public List<Item> items = new List<Item>();
        const string namePrefix = "Set Character_";
        const string hideString = "(Hide)";
        public GameObject disabledGraySkeleton;
        
        [System.Serializable]
        public class Item
        {
            public Item(ItemData.ItemType itemType, bool useRig, HumanBodyBones humanBody = HumanBodyBones.Head)
            {
                ItemType = itemType;

                UseRig = useRig;

                humanBodyBone = humanBody;
            }

            public ItemData.ItemType ItemType;
            public ItemOnstreet Prefab;
            public bool UseRig;//использует ли итем скелет 
            public HumanBodyBones humanBodyBone; 
        }

        public void AddItem(int itemSlot)
        {
            Item item = items[itemSlot];

            if(item.Prefab == null)
            {
                Debug.Log("Слот пуст!");

                return;
            }
                
            ItemOnstreet itemInstance = GameObject.Instantiate(item.Prefab);
            
            GameObject.DestroyImmediate(itemInstance.GetCollider());

            GameObject.DestroyImmediate(itemInstance.GetRigidbody());
            
            GameObject.DestroyImmediate(itemInstance.GetOutline());

            // GameObject.DestroyImmediate(itemInstance.GetComponent<ItemOnstreet>());

            itemInstance.name = itemInstance.name.Substring(0, itemInstance.name.Length - "(Clone)".Length);
            
            if(item.UseRig)
            {
                ParentObjectAndBones(itemInstance);

                SetGraySkeletonVisibility(!VisibleItems());
            }
            else
            {
                SetSetItemNotUseRig(item, itemInstance);
            }
        }

        private void SetSetItemNotUseRig(Item item, ItemOnstreet itemInstance)
        {
            itemInstance.transform.SetParent(animator.GetBoneTransform(item.humanBodyBone)); 
            
            itemInstance.transform.localPosition = Vector3.zero;

            itemInstance.transform.localRotation = Quaternion.identity;
        }

        private void RemoveItedNotUseRig(Item itemInstance)
        {
            GameObject.DestroyImmediate(GetChildOnSkelet(itemInstance));
        }

        public void RemoveItem(int itemSlot)
        {
            Item item = items[itemSlot];

            if(item.UseRig)
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
            else
            {
                RemoveItedNotUseRig(item);
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
            else 
            {
                if (disabledGraySkeleton != null)
                {
                    disabledGraySkeleton.SetActive(true);
                }
            }

        }

        private GameObject GetChildOnSkelet(Item item)
        {
            Transform bone = animator.GetBoneTransform(item.humanBodyBone);

            foreach (Transform child in bone)
            {
                if (child.name == item.Prefab.transform.name)
                {
                    return child.gameObject;
                }
            }

            return null;
        }

        public bool HasItem(int itemSlot)
        {
            Item item = items[itemSlot];

            if(item.Prefab == null)
            {
                return false;
            } 

            if (item.UseRig && item.Prefab != null)
            {
                Transform root = GetRoot();

                for (int i = 0; i < root.childCount; i++)
                {
                    Transform child = root.GetChild(i); 

                    if (child.name.Contains(item.Prefab.transform.name) && child.name.Contains(namePrefix))
                    {
                        return true;
                    }
                }
            }
            else
            if(!item.UseRig)
            {
                Transform bone = animator.GetBoneTransform(item.humanBodyBone);

                foreach (Transform child in bone)
                {
                    if (child.name == item.Prefab.transform.name)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        //удочерить кости одежды к костям персонажа 
        public void ParentObjectAndBones(ItemOnstreet itemInstance)
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
            if(obj == null || items[itemSlot].Prefab == null)
            {
                return false;
            }
            return (obj.name.Contains(namePrefix) && obj.name.Contains(items[itemSlot].Prefab.name));
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


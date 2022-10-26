using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder 
{

    // Это контейнер blackboard, общий для всех узлов.
    // используется для хранения временных данных, к которым нескольким узлам требуется доступ на чтение и запись.
    
    [System.Serializable]
    public class Blackboard 
    {
        public Vector3 moveToPosition;
        public List<SolutionInfo> solutions;
        public GameObject a;
        public ModelDate modelDate;
    }
}
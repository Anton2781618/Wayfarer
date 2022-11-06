using UnityEngine;
using UnityEngine.Events;

namespace IncludedTypes
{
    public class EventActivator : MonoBehaviour
    {
        [SerializeField] private UnityEvent unityEvent;

        [SerializeField] private bool onHover = false;

        public void OnHover(bool isOver)
        {
            if(!isOver && onHover) Execute();
        }

        public void Execute()
        {
            unityEvent?.Invoke();
        }
    }
}



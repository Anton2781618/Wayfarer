﻿namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("The sequence task is similar to an \"and\" operation. It will return failure as soon as one of its child tasks return failure. " +
                     "If a child task returns success then it will sequentially run the next task. If all child tasks return success then it will return success.")]
    [TaskIcon("{SkinColor}SequenceIcon.png")]
    public class Sequence : Composite
    {
        // The index of the child that is currently running or is about to run.
        private int currentChildIndex = 0;
        // The task status of the last child ran.
        private TaskStatus executionStatus = TaskStatus.Inactive;

        public override int CurrentChildIndex()
        {
            return currentChildIndex;
        }

        public override bool CanExecute()
        {
            // We can continue to execuate as long as we have children that haven't been executed and no child has returned failure.
            // Мы можем продолжать выполнение до тех пор, пока у нас есть дочерние элементы, которые не были выполнены, и ни один дочерний элемент не вернул ошибку.
            return currentChildIndex < children.Count && executionStatus != TaskStatus.Failure;
        }

        public override void OnChildExecuted(TaskStatus childStatus)
        {
            // Increase the child index and update the execution status after a child has finished running.
            // Увеличьте дочерний индекс и обновите статус выполнения после завершения дочернего элемента.
            currentChildIndex++;
            executionStatus = childStatus;
        }

        public override void OnConditionalAbort(int childIndex)
        {
            // Set the current child index to the index that caused the abort
            // Установите для текущего дочернего индекса значение индекса, вызвавшего прерывание
            currentChildIndex = childIndex;
            executionStatus = TaskStatus.Inactive;
        }

        public override void OnEnd()
        {
            // All of the children have run. Reset the variables back to their starting values.
            // Все дети убежали. Верните переменным их начальные значения.
            executionStatus = TaskStatus.Inactive;
            currentChildIndex = 0;
        }
    }
}
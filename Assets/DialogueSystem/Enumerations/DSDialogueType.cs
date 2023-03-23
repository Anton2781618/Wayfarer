

namespace DS.Enumerations
{
    public enum DSDialogueType
    {
        SingleChoice,
        MultipleChoice,
        Action
    }

    public enum UnitAtribut
    {
        Здоровье,
        Мана
    }     
    public enum UnitOperation
    {
        Прибавить,
        Вычисть
    }
    public enum ObjectOperation
    {
        Уничножить,
        Выключить,
        Включить,
        Использовать,
    }
    public enum CurrentAnimation
    {
        Украсть,
    }

    //енумератор действий 
    public enum DSAction 
    {        
        CommandAttackTheTarget,
        CommandRetreat,
        CommandCheckTargetInventoryForItem,
        CommandTakeItemFromTarget,
        CheckingAvailabilityInformation,
        NotAction,
        ExitTheDialog,
        CommandTrading,
        CommandPlayerGiveMoney,
        CommandPickUpItem,
        CommandMoveToTarget,
        CommandStandStill,
        CommandStartDialogue,
        CommandMoveToCoordinates,
        CommandFindTheTarget,
        CommandHoldPositionFindTheTarget,
        CommandUseSelfInventoryItem,
        CommandCheckSelfInventoryForItemType,
        CommandCheckSelfInventoryForItem,
        CommandGetToWork,
        CommandSleep,
        CommandMoveToWork,
        CommandPerformOperationWithAttribute,
        CommandTaskToGroup,
        CommandAddItemToTargetInventory,
        CommandObjectOperation,
        CommandTakeDecision,
        CommandPlayAnimation,
        CommandFaceToTarget,
    }
}
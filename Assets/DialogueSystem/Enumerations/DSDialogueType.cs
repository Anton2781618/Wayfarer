

namespace DS.Enumerations
{
    public enum DSDialogueType
    {
        SingleChoice,
        MultipleChoice,
        Action
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
    }
}
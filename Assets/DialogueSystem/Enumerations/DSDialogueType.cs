

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
        CheckingAvailabilityInformation,
        NotAction,
        ExitTheDialog,
        CommandTrading,
        CommandPlayerGiveMoney,
        CommandMoveToTarget,
        CommandStandStill,
        CommandStartDialogue,
        CommandMoveToCoordinates,
        CommandFindTheTarget,
    }
}
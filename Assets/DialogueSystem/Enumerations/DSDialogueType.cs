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
        CheckInventoryForItem,
        CheckingAvailabilityInformation,
        NotAction,
        ExitTheDialog,
        CommandTrading,
        CommandPlayerGiveMoney,
        CommandMoveToTarget,
        CommandStandStill,
        CommandStartDialogue,

    }

}
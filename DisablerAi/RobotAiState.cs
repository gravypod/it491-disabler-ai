namespace DisablerAi
{
    public enum RobotAiState
    {
        Start, // done
        Inactive, // done
        Alert, // done
        AlertCallHeadQuarters, // done
        AlertAttack, // done
        AlertReposition, // done
        AlertFollowUp, // done
        Patrol, // done
        PatrolMarchToEnd, // done
        PatrolMarchToStart, // done
        PatrolLookAround, // done
        Suspicion,
        SuspicionCallHeadQuarters,
        SuspicionFollowUp,
        SuspicionLookAround,
        SuspicionShrugOff,
        Searching,
        SearchingFollowUpPointOfInterest,
        SearchingLookAroundPointOfInterest,
        SearchingFollowUpPlayerLastSeen,
        SearchingLookAroundPlayerLastSeen,
        HeldUp, // done
        HeldUpDemandMarkAmmo, // done
        HeldUpDemandMarkEnemies, // done
        HeldUpRefuse, // done
        HeldUpGetDown, // done
        Hurt, // done
        Disabled // done
    }
}
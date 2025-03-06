using HECSFramework.Core;

namespace Commands
{
    [Documentation(Doc.Commands, "RewardedAdvClosedCommand")]
    public struct RewardedAdvClosedCommand : IGlobalCommand
    {
        public int RewardID;
    }
}

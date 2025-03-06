using HECSFramework.Core;

namespace Commands
{
    [Documentation(Doc.Commands, "GetAdvRewardCommand")]
    public struct GetAdvRewardCommand : IGlobalCommand
    {
        public int RewardID;
    }
}

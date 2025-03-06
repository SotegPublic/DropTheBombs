using HECSFramework.Core;

namespace Commands
{
    [Documentation(Doc.Commands, "GetAdvRewardCommand")]
    public struct LoadExternCommand : IGlobalCommand
    {
        public string LoadJSON;
    }
}

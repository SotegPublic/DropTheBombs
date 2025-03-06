using HECSFramework.Core;

namespace Commands
{
    [Documentation(Doc.Commands, "LoadExternCallbackCommand")]
    public struct LoadExternCallbackCommand : IGlobalCommand
    {
        public string JsonData;
    }
}

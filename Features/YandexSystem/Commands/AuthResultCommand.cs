using HECSFramework.Core;

namespace Commands
{
    [Documentation(Doc.Commands, "AuthResultCommand")]
    public struct AuthResultCommand : IGlobalCommand
    {
        public bool IsAuthenticated;
    }
}

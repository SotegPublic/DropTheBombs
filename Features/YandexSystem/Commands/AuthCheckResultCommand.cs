using HECSFramework.Core;

namespace Commands
{
    [Documentation(Doc.Commands, "AuthCheckResultCommand")]
    public struct AuthCheckResultCommand : IGlobalCommand
    {
        public bool IsAuthenticated;
    }
}


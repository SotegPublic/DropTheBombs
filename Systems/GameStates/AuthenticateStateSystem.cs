using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.State, "AuthenticateStateSystem")]
    public sealed class AuthenticateStateSystem : BaseGameStateSystem, IReactGlobalCommand<AuthResultCommand>, IReactGlobalCommand<AuthCheckResultCommand>
    {
        [Single]
        private YandexReceiverSystem yandexSystem;

        protected override int State => GameStateIdentifierMap.AuthenticateState;

        public void CommandGlobalReact(AuthResultCommand command)
        {
            Owner.World.GetSingleComponent<AuthenticateStatusComponent>().IsAuthenticated = command.IsAuthenticated;
            EndState();
        }

        public void CommandGlobalReact(AuthCheckResultCommand command)
        {
            if(command.IsAuthenticated)
            {
                Owner.World.GetSingleComponent<AuthenticateStatusComponent>().IsAuthenticated = true;
                Owner.World.Command(new SetLoadScreenLoginState { IsNeeded = false });
                EndState();
            }
            else
            {
                Owner.World.Command(new SetLoadScreenLoginState { IsNeeded = true });
            }
        }

        public override void InitSystem()
        {
        }

        protected override void ProcessState(int from, int to)
        {
            yandexSystem.YandexReceiver.YandexDebug("We check auth");
            yandexSystem.YandexReceiver.CheckAuthentificated();
        }
    }
}
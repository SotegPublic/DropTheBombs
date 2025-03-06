using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.State, "EndBombLevelStateSystem")]
    public sealed class EndBombLevelStateSystem : BaseGameStateSystem, IReactGlobalCommand<StartReloadProcedureCommand>
    {
        [Single]
        private YandexReceiverSystem yandexSystem;

        protected override int State => GameStateIdentifierMap.EndBombLevelState;

        public void CommandGlobalReact(StartReloadProcedureCommand command)
        {
            if (Owner.World.GetSingleComponent<GameStateComponent>().CurrentState != State)
                return;

            var currentLevelProgress = Owner.World.GetSingleComponent<CurrentLevelProgressComponent>();
            var playerProgress = Owner.World.GetSingleComponent<PlayerProgressComponent>();

            if (currentLevelProgress.IsBombLevelBossDie)
            {
                playerProgress.BossIndex++;
            }

            playerProgress.BombLevelIndex++;
            playerProgress.TotalLevelsIndex++;
            yandexSystem.YandexReceiver.SetLeaderBoardValue("MaximumLevel", playerProgress.TotalLevelsIndex);
            yandexSystem.YandexReceiver.YandexDebug("we send leader board info");

            Owner.World.Command(new SaveCommand());
            EndState();
        }

        public override void InitSystem()
        {
        }

        protected override void ProcessState(int from, int to)
        {
            var currentLevelProgress = Owner.World.GetSingleComponent<CurrentLevelProgressComponent>();

            if (currentLevelProgress.LastKilledPilonID != 0)
            {
                var pilonXModifier = Owner.World.GetSingleComponent<PilonHealthCalculationsConfigComponent>().GetPilonXModifier(currentLevelProgress.LastKilledPilonID);

                currentLevelProgress.GainedCurrency = (float)Math.Round(currentLevelProgress.GainedCurrency * pilonXModifier);
            }

            //if (currentLevelProgress.IsBombLevelBossDie)
            //{
            //    Owner.World.Command(new ShowUICommand { UIViewType = UIIdentifierMap.BossDefeatedPanel_UIIdentifier });
            //}
            //else
            //{
                Owner.World.Command(new ShowUICommand { UIViewType = UIIdentifierMap.RewardPanel_UIIdentifier });
            //}            
        }
    }
}
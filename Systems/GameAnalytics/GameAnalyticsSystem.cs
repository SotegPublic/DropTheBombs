using System;
using HECSFramework.Core;
using UnityEngine;
using Components;
using GameAnalyticsSDK;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.GameAnalytics, "Main game analitycs system")]
    public sealed class GameAnalyticsSystem : BaseSystem, IUpdatable, IReactGlobalCommand<TransitionGameStateCommand>, IOnApplicationQuit
    {
        [Required]
        private SessionPlayTimeComponent sessionPlayTime;
        [Required]
        private LevelPlayTimeComponent levelPlayTime;

        private bool isLevelStarted;
        private string startTimeString;

        public override void InitSystem()
        {
            GameAnalytics.Initialize();

            sessionPlayTime.SetValue(0);

            var startTime = DateTime.Now;
            startTimeString = startTime.ToString("dd.MM.yy hh.mm.ss");
        }

        public void UpdateLocal()
        {
            sessionPlayTime.ChangeValue(Time.deltaTime);

            if (isLevelStarted)
            {
                levelPlayTime.ChangeValue(Time.deltaTime);
            }
        }

        public void CommandGlobalReact(TransitionGameStateCommand command)
        {
            if(command.To == GameStateIdentifierMap.PlaneGameState || command.To == GameStateIdentifierMap.BossHubToGameTransitionState)
            {
                levelPlayTime.SetValue(0);
                isLevelStarted = true;
                SendStartLevelEvent();
            }

            if(command.To == GameStateIdentifierMap.BossFightRewardState || command.To == GameStateIdentifierMap.EndBombLevelState)
            {
                SendEndLevelEvent();

                if(command.To == GameStateIdentifierMap.EndBombLevelState)
                {
                    var currentLevelProgress = Owner.World.GetSingleComponent<CurrentLevelProgressComponent>();

                    if(currentLevelProgress.IsBombLevelBossDie)
                    {
                        SendBombLevelBossKilledEvent();
                    }
                }
            }

            if(command.To == GameStateIdentifierMap.BombLevelHub || command.To == GameStateIdentifierMap.BossLevelHubState)
            {
                SendLoadEndEvent();
            }
        }

        private void SendLoadEndEvent()
        {
            var seconds = (float)Math.Round(sessionPlayTime.Value, 2);
            GameAnalytics.NewDesignEvent($"LoadTime:{startTimeString}:Time", seconds);
            Debug.Log("Load time event sended");
        }

        private void SendStartLevelEvent()
        {
            var playerPlaneCustomisation = Owner.World.GetSingleComponent<PlayerPlaneCustomisationComponent>();
            var playerProgress = Owner.World.GetSingleComponent<PlayerProgressComponent>();

            GameAnalytics.NewProgressionEvent(
                GAProgressionStatus.Start,
                playerProgress.TotalLevelsIndex.ToString(),
                playerPlaneCustomisation.CurrentPlaneID.ToString(),
                playerPlaneCustomisation.CurrentPlanePaintID.ToString()
                );

            Debug.Log("Start level event sended");
        }

        private void SendEndLevelEvent()
        {
            isLevelStarted = false;

            var playerProgress = Owner.World.GetSingleComponent<PlayerProgressComponent>();
            var seconds = (int)levelPlayTime.Value;

            GameAnalytics.NewProgressionEvent(
                GAProgressionStatus.Complete,
                playerProgress.TotalLevelsIndex.ToString(),
                seconds
                );

            Debug.Log("End level event sended");
        }

        private void SendBombLevelBossKilledEvent()
        {
            var playerProgress = Owner.World.GetSingleComponent<PlayerProgressComponent>();

            GameAnalytics.NewDesignEvent(
                $"BombBossKilled:{playerProgress.BossIndex}:TotalLevelsCount",
                playerProgress.TotalLevelsIndex
                );

            Debug.Log("Boss killed event sended");
        }

        public void OnApplicationExit()
        {
            var minutes = (float)Math.Round(sessionPlayTime.Value / 60, 2);
            GameAnalytics.NewDesignEvent($"Session:{startTimeString}:EndTime", minutes);
            Debug.Log("Session time event sended");
        }
    }
}
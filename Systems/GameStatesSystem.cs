using System;
using HECSFramework.Core;
using Components;
using Commands;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

namespace Systems
{
    [Serializable]
    [Documentation(Doc.GameLogic, "MainGameLogicSystem")]
    public sealed class GameStatesSystem : BaseMainGameLogicSystem
    {
        private PlayerProgressComponent playerProgress;

        [Single]
        private YandexReceiverSystem yandexSystem;

        [Single]
        private CursorControlSystem cursorController;

        public async override void GlobalStart()
        {
            playerProgress = Owner.World.GetSingleComponent<PlayerProgressComponent>();
            //playerProgress.TotalLevelsIndex = 5;
            //playerProgress.BombLevelIndex = 4;
            //playerProgress.BossIndex = 7;
            //playerProgress.BossLevelIndex = 5;

            await Owner.World.Request<UniTask<Entity>, ShowUICommand>(new ShowUICommand { UIViewType = UIIdentifierMap.LoadScreenPanel_UIIdentifier });
            await UniTask.Delay(300);
            Owner.World.Command(new HideUICommand { UIViewType = UIIdentifierMap.PreloadScreenPanel_UIIdentifier});

            yandexSystem.YandexReceiver.YandexDebug("Global start: " + DateTime.Now.ToString("HH:mm:ss"));
            ChangeGameState(GameStateIdentifierMap.InitYndexState);
        }

        public override void InitSystem()
        {
        }

        protected override void ProcessEndState(EndGameStateCommand endGameStateCommand)
        {
            switch (endGameStateCommand.GameState)
            {
                case GameStateIdentifierMap.InitYndexState:
                    ChangeGameState(GameStateIdentifierMap.AuthenticateState);
                    yandexSystem.YandexReceiver.YandexDebug("Auth state: " + DateTime.Now.ToString("HH:mm:ss"));
                    break;
                case GameStateIdentifierMap.AuthenticateState:
                    ChangeGameState(GameStateIdentifierMap.LoadPlayer);
                    yandexSystem.YandexReceiver.YandexDebug("LoadPlayer state: " + DateTime.Now.ToString("HH:mm:ss"));
                    break;
                case GameStateIdentifierMap.LoadPlayer:
                    Owner.World.Command(new AddLoadScreenProgressCommand { Value = 0.2f });
                    ChangeGameState(GameStateIdentifierMap.WarmUpState);
                    yandexSystem.YandexReceiver.YandexDebug("WarmUp state: " + DateTime.Now.ToString("HH:mm:ss"));
                    break;

                case GameStateIdentifierMap.WarmUpState:
                    Owner.World.Command(new AddLoadScreenProgressCommand { Value = 0.1f });
                    ChangeGameState(GameStateIdentifierMap.ForkState);
                    yandexSystem.YandexReceiver.YandexDebug("Fork state: " + DateTime.Now.ToString("HH:mm:ss"));
                    break;

                case GameStateIdentifierMap.ForkState:

                    if (playerProgress.TotalLevelsIndex % 3 == 0)
                    {
                        ChangeGameState(GameStateIdentifierMap.LoadBossLevel);
                        yandexSystem.YandexReceiver.YandexDebug("Load boss level state: " + DateTime.Now.ToString("HH:mm:ss"));
                    }
                    else
                    {
                        ChangeGameState(GameStateIdentifierMap.LoadBombLevel);
                        yandexSystem.YandexReceiver.YandexDebug("Load bomb level state: " + DateTime.Now.ToString("HH:mm:ss"));
                    }
                    break;
            }

            BombLevelBranch(endGameStateCommand);
            BossLevelBranch(endGameStateCommand);
        }

        private void BossLevelBranch(EndGameStateCommand endGameStateCommand)
        {
            switch (endGameStateCommand.GameState)
            {
                case GameStateIdentifierMap.LoadBossLevel:
                    Owner.World.Command(new AddLoadScreenProgressCommand { Value = 0.1f });
                    ChangeGameState(GameStateIdentifierMap.SpawnPlayerOnBossLevel);
                    yandexSystem.YandexReceiver.YandexDebug("Spawn player state: " + DateTime.Now.ToString("HH:mm:ss"));
                    break;
                case GameStateIdentifierMap.SpawnPlayerOnBossLevel:
                    Owner.World.Command(new AddLoadScreenProgressCommand { Value = 0.2f });
                    ChangeGameState(GameStateIdentifierMap.BossLevelHubState);
                    yandexSystem.YandexReceiver.YandexDebug("Boss hub state: " + DateTime.Now.ToString("HH:mm:ss"));
                    break;
                case GameStateIdentifierMap.BossLevelHubState:
                    ChangeGameState(GameStateIdentifierMap.BossHubToGameTransitionState);
                    cursorController.LockCursor();
                    cursorController.HideCursor();
                    yandexSystem.YandexReceiver.YandexDebug("BossHubToGameTransition state: " + DateTime.Now.ToString("HH:mm:ss"));
                    break;
                case GameStateIdentifierMap.BossHubToGameTransitionState:
                    ChangeGameState(GameStateIdentifierMap.ShootingGameState);
                    yandexSystem.YandexReceiver.YandexDebug("ShootingGame state: " + DateTime.Now.ToString("HH:mm:ss"));
                    break;
                case GameStateIdentifierMap.ShootingGameState:
                    ChangeGameState(GameStateIdentifierMap.SpawnBossState);
                    yandexSystem.YandexReceiver.YandexDebug("SpawnBoss state: " + DateTime.Now.ToString("HH:mm:ss"));
                    break;
                case GameStateIdentifierMap.SpawnBossState:
                    ChangeGameState(GameStateIdentifierMap.BossFightState);
                    yandexSystem.YandexReceiver.YandexDebug("BossFight state: " + DateTime.Now.ToString("HH:mm:ss"));
                    break;
                case GameStateIdentifierMap.BossFightState:
                    ChangeGameState(GameStateIdentifierMap.BossFightRewardState);
                    cursorController.ShowCursor();
                    cursorController.UnlockCursor();
                    yandexSystem.YandexReceiver.YandexDebug("BossFightReward state: " + DateTime.Now.ToString("HH:mm:ss"));
                    break;
                case GameStateIdentifierMap.BossFightRewardState:
                    ChangeGameState(GameStateIdentifierMap.ClearBossLevelState);
                    yandexSystem.YandexReceiver.YandexDebug("ClearBossLevel state: " + DateTime.Now.ToString("HH:mm:ss"));
                    break;   
                case GameStateIdentifierMap.ClearBossLevelState:
                    ChangeGameState(GameStateIdentifierMap.ForkState);
                    yandexSystem.YandexReceiver.YandexDebug("Fork state: " + DateTime.Now.ToString("HH:mm:ss"));
                    break;
            }
        }

        private void BombLevelBranch(EndGameStateCommand endGameStateCommand)
        {
            switch (endGameStateCommand.GameState)
            {
                case GameStateIdentifierMap.LoadBombLevel:
                    ChangeGameState(GameStateIdentifierMap.LoadBombLevelRings);
                    yandexSystem.YandexReceiver.YandexDebug("LoadBombLevelRings state: " + DateTime.Now.ToString("HH:mm:ss"));
                    break;
                case GameStateIdentifierMap.LoadBombLevelRings:
                    ChangeGameState(GameStateIdentifierMap.LoadBombLevelTraps);
                    yandexSystem.YandexReceiver.YandexDebug("LoadBombLevel state: " + DateTime.Now.ToString("HH:mm:ss"));
                    break;
                case GameStateIdentifierMap.LoadBombLevelTraps:
                    ChangeGameState(GameStateIdentifierMap.LoadBombLevelObjects);
                    yandexSystem.YandexReceiver.YandexDebug("LoadBombLevelObjects state: " + DateTime.Now.ToString("HH:mm:ss"));
                    break;
                case GameStateIdentifierMap.LoadBombLevelObjects:
                    Owner.World.Command(new AddLoadScreenProgressCommand { Value = 0.2f });
                    ChangeGameState(GameStateIdentifierMap.SpawnPlayerOnBombLevel);
                    yandexSystem.YandexReceiver.YandexDebug("SpawnPlayerOnBombLevel state: " + DateTime.Now.ToString("HH:mm:ss"));
                    break;
                case GameStateIdentifierMap.SpawnPlayerOnBombLevel:
                    Owner.World.Command(new AddLoadScreenProgressCommand { Value = 0.2f });
                    ChangeGameState(GameStateIdentifierMap.BombLevelHub);
                    yandexSystem.YandexReceiver.YandexDebug("BombLevelHub state: " + DateTime.Now.ToString("HH:mm:ss"));
                    break;
                case GameStateIdentifierMap.BombLevelHub:
                    ChangeGameState(GameStateIdentifierMap.PlaneGameState);
                    yandexSystem.YandexReceiver.YandexDebug("PlaneGameState state: " + DateTime.Now.ToString("HH:mm:ss"));
                    break;
                case GameStateIdentifierMap.PlaneGameState:
                    ChangeGameState(GameStateIdentifierMap.BombVerticalGameState);
                    yandexSystem.YandexReceiver.YandexDebug("BombVerticalGame State: " + DateTime.Now.ToString("HH:mm:ss"));
                    break;
                case GameStateIdentifierMap.BombVerticalGameState:
                    ChangeGameState(GameStateIdentifierMap.BombHorizontalGameState);
                    yandexSystem.YandexReceiver.YandexDebug("BombHorizontalGame State: " + DateTime.Now.ToString("HH:mm:ss"));
                    break;
                case GameStateIdentifierMap.BombHorizontalGameState:
                    ChangeGameState(GameStateIdentifierMap.EndBombLevelState);
                    yandexSystem.YandexReceiver.YandexDebug("EndBombLevel State: " + DateTime.Now.ToString("HH:mm:ss"));
                    break;
                case GameStateIdentifierMap.EndBombLevelState:
                    ChangeGameState(GameStateIdentifierMap.ClearBombLevelState);
                    yandexSystem.YandexReceiver.YandexDebug("ClearBombLevel State: " + DateTime.Now.ToString("HH:mm:ss"));
                    break;
                case GameStateIdentifierMap.ClearBombLevelState:
                    ChangeGameState(GameStateIdentifierMap.ForkState);
                    yandexSystem.YandexReceiver.YandexDebug("Fork State: " + DateTime.Now.ToString("HH:mm:ss"));
                    break;
            }
        }
    }
}
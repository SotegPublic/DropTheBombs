using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;
using Cysharp.Threading.Tasks;

namespace Systems
{
	[Serializable][Documentation(Doc.Bombs, "this system set and swap bombs view")]
    public sealed class BombsSetAndSwapSystem : BaseSystem, IReactGlobalCommand<SetBombsOnStartCommand>,
        IReactGlobalCommand<ChangeBombsCountCommand>, IReactGlobalCommand<ChangeBombsPowerCommand>, IGlobalStart, IReactGlobalCommand<ResetLevelCommand>
    {
        [Required]
        private BombsPoolComponent bombsPool;
        [Required]
        private BombsConfigsHolder bombsConfigs;
        [Required]
        private ActiveBombsHolderComponent activeBombs;
        [Required]
        private UpgradeSoundComponent upgradeSound;
        [Required]
        private ChangeCountSoundComponent changeCountSound;

        [Single]
        private BombsPositionSystem bombsPositionSystem;

        private MaxBombsCountForLevelComponent maxBombsCount;
        private Transform bombsHolderTransform;
        private int currentBombsID;
        private float currentBombsPower;

        public override void InitSystem()
        {
        }

        public void GlobalStart()
        {
            maxBombsCount = Owner.World.GetSingleComponent<MaxBombsCountForLevelComponent>();
        }

        public void CommandGlobalReact(ResetLevelCommand command)
        {
            currentBombsID = 0;
            currentBombsPower = 0;
            bombsHolderTransform = null;
        }

        public void CommandGlobalReact(SetBombsOnStartCommand command)
        {
            bombsHolderTransform = command.BombsHolder;
            currentBombsID = bombsConfigs.GetIDByPower(command.BombPower);
            currentBombsPower = command.BombPower;

            GetBombs(command.BombsCount);
        }

        private void GetBombs(int bombCount)
        {
            var bombConfig = bombsConfigs.GetConfigByID(currentBombsID);

            for (int i = 0; i < bombCount; i++)
            {
                var bombActor = bombsPool.GetBomb().GetComponent<Actor>();
                bombActor.transform.SetParent(bombsHolderTransform, false);
                BombPositionModel bombPositionModel = null;

                if (activeBombs.ActivebombsPositionsList.Count == 0)
                {
                    bombPositionModel = bombsPositionSystem.GetZeroPosition();
                }
                else
                {
                    bombPositionModel = bombsPositionSystem.GetPositionModelForPlacing();
                }

                var isPlaneState = Owner.World.GetSingleComponent<GameStateComponent>().CurrentState == GameStateIdentifierMap.BombVerticalGameState ? false : true;
                var meshFilter = bombActor.GetComponent<MeshFilter>();
                meshFilter.sharedMesh = bombConfig.BombMesh;

                InitBombActor(bombConfig, bombActor);

                bombPositionModel.TakePosition(bombActor, isPlaneState);
                activeBombs.ActivebombsPositionsList.Add(bombPositionModel);

                bombActor.Entity.Command(new PlayLocalVFXCommand { Enable = true, ID = FXIdentifierMap.UpgradeCount });
            }
        }


        public async void CommandGlobalReact(ChangeBombsPowerCommand command)
        {
            if (Owner.World.GetSingleComponent<GameStateComponent>().CurrentState != GameStateIdentifierMap.PlaneGameState &&
                Owner.World.GetSingleComponent<GameStateComponent>().CurrentState != GameStateIdentifierMap.BombVerticalGameState &&
                Owner.World.GetSingleComponent<GameStateComponent>().CurrentState != GameStateIdentifierMap.BombLevelHub) return;

            var newBombsID = bombsConfigs.GetIDByPower(command.NewBombsPower);

            if(currentBombsID != newBombsID)
            {
                var isUpgrade = command.NewBombsPower > currentBombsPower ? true : false;

                currentBombsID = newBombsID;
                currentBombsPower = command.NewBombsPower;

                var bombConfig = bombsConfigs.GetConfigByID(currentBombsID);

                for (int i = 0; i < activeBombs.ActivebombsPositionsList.Count; i++)
                {
                    var bombActor = activeBombs.ActivebombsPositionsList[i].BombActor;

                    var meshFilter = bombActor.GetComponent<MeshFilter>();
                    meshFilter.sharedMesh = bombConfig.BombMesh;

                    UpdateActor(bombConfig, bombActor);
                }

                await UniTask.Delay(100);

                for (int i = 0; i < activeBombs.ActivebombsPositionsList.Count; i++)
                {
                    var bombActor = activeBombs.ActivebombsPositionsList[i].BombActor;

                    bombActor.Entity.Command(new PlayLocalVFXCommand { Enable = true, ID = FXIdentifierMap.UpgradePower });
                }

                if(isUpgrade)
                {
                    Owner.World.Command(new PlaySoundCommand { AudioType = SoundType.FX, Clip = upgradeSound.Clip, IsRepeatable = false, Owner = Owner.GUID });
                }
            }
        }

        private void UpdateActor(BombConfig bombConfig, Actor bombActor)
        {
            bombActor.Entity.GetComponent<BombTagComponent>().BombID = bombConfig.BombID;
            bombActor.Entity.GetComponent<BombFXIdHolderComponent>().BombFXId = bombConfig.BombFXID;
        }

        private void InitBombActor(BombConfig bombConfig, Actor bombActor)
        {
            bombActor.gameObject.layer = LayerMask.NameToLayer("Default");
            bombActor.Init(initWithContainer: true);

            bombActor.Entity.GetComponent<BombTagComponent>().BombID = bombConfig.BombID;
            bombActor.Entity.GetComponent<BombFXIdHolderComponent>().BombFXId = bombConfig.BombFXID;
        }

        public async void CommandGlobalReact(ChangeBombsCountCommand command)
        {
            if (Owner.World.GetSingleComponent<GameStateComponent>().CurrentState != GameStateIdentifierMap.PlaneGameState &&
                Owner.World.GetSingleComponent<GameStateComponent>().CurrentState != GameStateIdentifierMap.BombVerticalGameState &&
                Owner.World.GetSingleComponent<GameStateComponent>().CurrentState != GameStateIdentifierMap.BombLevelHub) return;

            if (command.NewBombsCount != activeBombs.ActivebombsPositionsList.Count)
            {
                if(command.NewBombsCount > activeBombs.ActivebombsPositionsList.Count)
                {
                    var newCount = command.NewBombsCount;

                    if(newCount > maxBombsCount.Value)
                    {
                        newCount = maxBombsCount.Value;
                    }

                    var dif = newCount - activeBombs.ActivebombsPositionsList.Count;

                    await UniTask.Delay(200);

                    GetBombs(dif);
                    Owner.World.Command(new PlaySoundCommand { AudioType = SoundType.FX, Clip = changeCountSound.Clip, IsRepeatable = false, Owner = Owner.GUID });
                }
                else
                {
                    var dif = activeBombs.ActivebombsPositionsList.Count - command.NewBombsCount;
                    RemoveBombs(dif);
                }
            }
        }

        private void RemoveBombs(int count)
        {
            for(int i = 0; i < count; i++)
            {
                var bombPositionModel = bombsPositionSystem.GetPositionModelForDestroy();
                var bomb = bombPositionModel.BombActor;

                bombPositionModel.FreePosition();

                bombsPool.ReturnBomb(bomb);
                activeBombs.ActivebombsPositionsList.Remove(bombPositionModel);
            }
        }
    }
}
using System;
using HECSFramework.Core;
using Components;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using HECSFramework.Unity;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.Load, Doc.GameState, "LoadBombLevelSystem")]
    public sealed class LoadBombLevelSystem : BaseGameStateSystem 
    {
        [Required]
        private BombLevelHolder levelHolder;
        [Required]
        private BossPilonContainersHolderComponent bossPilonContainers;

        [Single]
        private AssetService assetService;

        private PlayerProgressComponent playerProgress;
        private LevelsHolderComponent levelsHolder;

        protected override int State => GameStateIdentifierMap.LoadBombLevel;

        public override void InitSystem()
        {
            playerProgress = Owner.World.GetSingleComponent<PlayerProgressComponent>();
            levelsHolder = Owner.World.GetSingleComponent<LevelsHolderComponent>();
        }

        protected override async void ProcessState(int from, int to)
        {
            Owner.World.GetEntityBySingleComponent<PlayerTagComponent>().GetOrAddComponent<BombCountComponent>().SetValue(playerProgress.BombsCount);
            Owner.World.GetEntityBySingleComponent<PlayerTagComponent>().GetOrAddComponent<BombPowerComponent>().SetValue(playerProgress.BombPower);

            var levelProgress = Owner.World.GetSingleComponent<CurrentLevelProgressComponent>();
            var levelRef = levelsHolder.GetLevelConfigByIndex(playerProgress.BombLevelIndex - 1).LevelPrefReference;

            if(levelRef == null)
            {
                levelRef = levelsHolder.DefaultLevelPrefReference;
            }

            var view = await assetService.GetAssetInstance(levelRef);
            var levelMonoComponent = view.GetComponent<BombLevelMonoComponent>();

            var bossPilonConfig = bossPilonContainers.GetBossPilonContainerByBossIndex(playerProgress.BossIndex);

            var pilonActor = levelMonoComponent.BossPilon.GetComponent<Actor>();
            pilonActor.ActorContainer.GetComponent<ViewReferenceGameObjectComponent>().ViewReference = bossPilonConfig.BossAssetRef;
            pilonActor.ActorContainer.GetComponent<BossTypeComponent>().Type = bossPilonConfig.BossType;

            pilonActor.ActorContainer.UpdateComponentsCacheData();
            pilonActor.Init(initWithContainer: true);

            levelProgress.CurrentLoadedLevel = view;
            levelHolder.SetBombLevel(levelMonoComponent);
            
            EndState();
        }
    }
}
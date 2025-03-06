using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;
using Cysharp.Threading.Tasks;

namespace Systems
{
	[Serializable][Documentation(Doc.State, Doc.Level, "by this system we clear bomb level")]
    public sealed class ClearBombLevelSystem : BaseGameStateSystem
    {
        [Single]
        private PoolingSystem poolingSystem;
        [Single]
        private AssetService assetService;

        protected override int State => GameStateIdentifierMap.ClearBombLevelState;

        private EntitiesFilter cleanableEntities;

        public override void InitSystem()
        {
            cleanableEntities = Owner.World.GetFilter<CleanableObjectTagComponent>();
        }

        protected override async void ProcessState(int from, int to)
        {
            await Owner.World.Request<UniTask<Entity>, ShowUICommand>(new ShowUICommand { UIViewType = UIIdentifierMap.LoadScreenPanel_UIIdentifier });
            Owner.World.Command(new SetLoadScreenLoginState { IsNeeded = false });

            //todo переделать на группы, должна быть группа лоадинг скрин и группа маин юай
            //await Owner.World.GetSingleSystem<UISystem>().ShowUIGroup()

            await UniTask.Delay(1000); //await open load screen to hide other operations

            Owner.World.Command(new HideUICommand { UIViewType = UIIdentifierMap.MoneyScreenUI_UIIdentifier });
            Owner.World.Command(new HideUICommand { UIViewType = UIIdentifierMap.LevelsCounterPanel_UIIdentifier });
            Owner.World.Command(new HideUICommand { UIViewType = UIIdentifierMap.StarsProgressPanel_UIIdentifier });
            Owner.World.Command(new HideUICommand { UIViewType = UIIdentifierMap.BombsInfoPanel_UIIdentifier });


            var levelProgress = Owner.World.GetSingleComponent<CurrentLevelProgressComponent>();

            Owner.World.Command(new SetCameraFreeCommand());
            Owner.World.Command(new HideUICommand { UIViewType = UIIdentifierMap.RewardPanel_UIIdentifier });

            cleanableEntities.ForceUpdateFilter();
            
            foreach(var entity in cleanableEntities)
            {
                entity.Command(new ResetEntityCommand());
                entity.HecsDestroy();
            }

            poolingSystem.ReleaseView(levelProgress.DropBombGO);
            assetService.Release(levelProgress.CurrentLoadedLevel);
            await UniTask.DelayFrame(1);
            levelProgress.Clear();
            Owner.World.Command(new ResetLevelCommand());
            EndState();
        }
    }
}
using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

namespace Systems
{
	[Serializable][Documentation(Doc.State, Doc.Load, "in this system we warmup all game views")]
    public sealed class WarmUpSystem : BaseGameStateSystem 
    {
        [Required]
        private PlayerPlanePrefabHolderComponent planeHolder;
        [Required]
        private WarmUpVariablesComponent variables;
        [Required]
        private BombsConfigsHolder bombsConfigsHolder;
        [Required]
        private BombsPoolComponent bombsPool;
        [Required]
        private VFXPrefabsHolderComponent vfxPrefabsHolder;

        [Single]
        private PoolingSystem poolingSystem;

        protected override int State => GameStateIdentifierMap.WarmUpState;

        private List<UniTask> taskList = new List<UniTask>(100);

        public override void InitSystem()
        {
        }

        protected override async void ProcessState(int from, int to)
        {
            taskList.Clear();

            for (int i = 0; i < planeHolder.GetCollectionLenth(); i++)
            {
                taskList.Add(poolingSystem.Warmup(planeHolder.GetAssetReferenceByIndex(i), 1));
            }

            taskList.Add(WarmUpBombs(variables.PreloadBombsCount));

            taskList.Add(poolingSystem.Warmup(vfxPrefabsHolder.BulletFXView, variables.PreloadBullets));

            await UniTask.WhenAll(taskList);
            EndState();
        }

        private async UniTask WarmUpBombs(int preloadCount)
        {
            var bombReference = bombsConfigsHolder.BaseBombReference;
            var neededHandler = Addressables.LoadAssetAsync<GameObject>(bombReference);
            var needed = await neededHandler.Task;

            for (int i = 0; i < preloadCount; i++)
            {
                var instance = MonoBehaviour.Instantiate(needed.gameObject);
                instance.layer = LayerMask.NameToLayer("Hiden");
                bombsPool.AddBomb(instance);
            }
        }
    }
}
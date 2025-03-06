using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using UnityEngine.AddressableAssets;

namespace Systems
{
	[Serializable][Documentation(Doc.State, Doc.Load, "here we load bomb level objects")]
    public sealed class LoadBombLevelObjectsSystem : BaseGameStateSystem
    {
        [Required]
        private LevelObjectsHolderComponent objectsHolder;
        [Required]
        private BombLevelVariablesComponent levelVariables;


        protected override int State => GameStateIdentifierMap.LoadBombLevelObjects;

        public override void InitSystem()
        {
        }

        protected override async void ProcessState(int from, int to)
        {
            var startTransform = Owner.World.GetEntityBySingleComponent<PlayerSpawnPointTagComponent>().GetTransform();

            startTransform.position = levelVariables.StartLevelPosition;

            var neededHandler = Addressables.LoadAssetAsync<GameObject>(objectsHolder.DropBombObjectReference);
            var needed = await neededHandler.Task;

            var dropObject = MonoBehaviour.Instantiate(needed.gameObject);
            dropObject.transform.position = levelVariables.DropBombPosition;

            Owner.World.GetSingleComponent<CurrentLevelProgressComponent>().DropBombGO = dropObject;

            EndState();
        }
    }
}
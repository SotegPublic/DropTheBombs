using System;
using System.Threading.Tasks;
using Commands;
using Components;
using Cysharp.Threading.Tasks;
using HECSFramework.Core;
using HECSFramework.Unity;
using UnityEngine;

namespace Systems
{
    [Serializable]
    [Documentation(Doc.Player, Doc.Spawn, "this system spawn player and bomb")]
    public sealed class MyPlayerSpawnSystem : BaseSystem, IRequestProvider<UniTask<PlaneSpawned>> , IRequestProvider<UniTask<BombSpawned>, ConnectBombCommand>,
        IRequestProvider<UniTask<BombReconnected>, ReconnectBombCommand>
    {
        [Required]
        public PlayerPlanePrefabHolderComponent PlanePrefabHolderComponent;

        [Required]
        public PlayerBombContainerHolderComponent BombPrefabHolderComponent;

        [Single]
        private YandexReceiverSystem yandexSystem;


        public override void InitSystem()
        {
        }

        public async UniTask<PlaneSpawned> Request()
        {
            var spawnPoint = GetSpawnPoint();
            var plane = await SpawnPlanePlayerCharacter(spawnPoint);

            plane.Init();
            plane.Entity.RemoveComponent<InputListenerTagComponent>();

            await new WaitFor<ViewReadyTagComponent>(plane.Entity).RunJob();

            await CheckAndChangePlaneView(plane);

            return new PlaneSpawned { Plane = plane.Entity };
        }

        public async UniTask<BombSpawned> Request(ConnectBombCommand command)
        {
            var spawnPoint = GetSpawnPoint();
            var bomb = await SpawnBombPlayerCharacter(spawnPoint);
            bomb.Init();
            bomb.Entity.RemoveComponent<InputListenerTagComponent>();

            ConnectBomb(command.Plane, bomb);

            return new BombSpawned { Bomb = bomb.Entity };
        }

        private Transform GetSpawnPoint()
        {
            var spawnPoints = Owner.World.GetFilter<PlayerSpawnPointTagComponent>();
            spawnPoints.ForceUpdateFilter();

            var spawnPointTransform = spawnPoints[0].GetComponent<UnityTransformComponent>().Transform;
            return spawnPointTransform;
        }

        private async UniTask<Actor> SpawnPlanePlayerCharacter(Transform pont)
        {
            if (PlanePrefabHolderComponent.TryGetContainerByID(EntityContainersMap._DefaultPlanePlayerActorContainer, out var container))
            {
                return await container.GetActor(position: pont.position);
            }
            else
            {
                throw new System.Exception("Can't spawn palyerPlaneCharacter");
            }
        }

        private async UniTask<Actor> SpawnBombPlayerCharacter(Transform pont)
        {
            return await BombPrefabHolderComponent.BombEntity.GetActor(position: pont.position);
        }

        public async UniTask<BombReconnected> Request(ReconnectBombCommand command)
        {
            var bomb = Owner.World.GetEntityBySingleComponent<PlayerBombTagComponent>().AsActor();
            await ReconnectBomb(command.Plane, bomb);

            return new BombReconnected();
        }


        private async void ConnectBomb(Entity plane, Actor bomb)
        {
            await new WaitFor<ViewReadyTagComponent>(plane).RunJob();

            var bombTransform = bomb.Entity.GetTransform();
            var planeView = plane.GetComponent<ViewReadyTagComponent>().View;
            bombTransform.SetParent(planeView.transform);
            bombTransform.localPosition = new Vector3(0, -2, 0);
            bombTransform.rotation = Quaternion.Euler(90, 0, 0);

            await new WaitFor<ViewReadyTagComponent>(bomb.Entity).RunJob();

            var bombsHolder = bomb.Entity.GetComponent<ViewReadyTagComponent>().View.transform;
            var playerBombPower = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>().GetComponent<BombPowerComponent>().Value;
            var playerBombsCount = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>().GetComponent<BombCountComponent>().Value;

            Owner.World.Command(new SetBombsOnStartCommand { BombPower = playerBombPower, BombsCount = playerBombsCount, BombsHolder = bombsHolder });
        }

        private async UniTask ReconnectBomb(Entity plane, Actor bomb)
        {
            await new WaitFor<ViewReadyTagComponent>(plane).RunJob();

            var bombTransform = bomb.Entity.GetTransform();
            bombTransform.SetParent(plane.GetComponent<ViewReadyTagComponent>().View.transform);
            bombTransform.localPosition = new Vector3(0, -2, 0);
            bombTransform.rotation = Quaternion.Euler(90, 0, 0);
        }


        private async UniTask CheckAndChangePlaneView(Actor plane)
        {
            var planeID = Owner.World.GetSingleComponent<PlayerPlaneCustomisationComponent>().CurrentPlaneID;
            var entityPlaneID = plane.Entity.GetComponent<PlayerPlaneTagComponent>().PlayerPlaneId;

            yandexSystem.YandexReceiver.YandexDebug($"planeID: {planeID}");

            if (entityPlaneID != planeID)
            {
                var textureID = Owner.World.GetSingleComponent<PlayerPlaneCustomisationComponent>().CurrentPlanePaintID;

                await Owner.World.Request<UniTask<PlaneViewSwaped>, ChangePlaneViewCommand>(new ChangePlaneViewCommand { PlaneID = planeID });
                Owner.World.Command(new ChangePlaneTextureCommand { TextureID = textureID });
            }
        }
    }
}


using System;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Cysharp.Threading.Tasks;
using Commands;
using System.Threading.Tasks;
using System.Numerics;

namespace Systems
{
    [Serializable][Documentation(Doc.Plane, "this system swaping plane")]
    public sealed class ChangePlaneViewSystem : BaseSystem, IRequestProvider<UniTask<PlaneViewSwaped>, ChangePlaneViewCommand>, IReactGlobalCommand<ChangePlaneTextureCommand> 
    {
        [Required]
        private PlanesViewsHolderComponent planesViews;
        [Required]
        private PlaneTexturesHolderComponent textures;

        public override void InitSystem()
        {
        }

        public async UniTask<PlaneViewSwaped> Request(ChangePlaneViewCommand command)
        {
            var playerPlane = Owner.World.GetEntityBySingleComponent<PlayerPlaneTagComponent>();
            var planeAssetRef = planesViews.GetPlaneViewConfigByID(command.PlaneID).GameViewReferense;

            var isBombSpawned = await CheckSpawnedBombsHolder();

            playerPlane.GetComponent<ViewReferenceGameObjectComponent>().ViewReference = planeAssetRef;
            playerPlane.Command(new RespawnViewCommand());

            await new WaitFor<ViewReadyTagComponent>(playerPlane).RunJob();

            playerPlane.GetComponent<PlayerPlaneTagComponent>().SetNewPlaneID(command.PlaneID);

            if (isBombSpawned)
            {
                await Owner.World.Request<UniTask<BombReconnected>, ReconnectBombCommand>(new ReconnectBombCommand { Plane = playerPlane });
            }

            var viewReady = playerPlane.GetComponent<ViewReadyTagComponent>();
            var planeView = viewReady.View;

            var texture = textures.GetPlaneTextureByID(PlanePaintIdentifierMap.Base).PlaneTexture;
            ChangeTexture(PlanePaintIdentifierMap.Base, texture, planeView);

            return new PlaneViewSwaped();
        }

        public void CommandGlobalReact(ChangePlaneTextureCommand command)
        {
            var texture = textures.GetPlaneTextureByID(command.TextureID).PlaneTexture;

            var playerPlane = Owner.World.GetEntityBySingleComponent<PlayerPlaneTagComponent>();
            var viewReady = playerPlane.GetComponent<ViewReadyTagComponent>();
            var planeView = viewReady.View;

            ChangeTexture(command.TextureID, texture, planeView);
        }

        private async UniTask<bool> CheckSpawnedBombsHolder()
        {
            if (Owner.World.TryGetSingleComponent<PlayerBombTagComponent>(out var bombTagComponent))
            {
                await new WaitFor<ViewReadyTagComponent>(bombTagComponent.Owner).RunJob();

                bombTagComponent.Owner.GetTransform().SetParent(null);
                return true;
            }

            return false;
        }

        private void ChangeTexture(int paintID, Texture2D texture, GameObject planeView)
        {
            planeView.GetComponent<IChangableColorView>().ChangeColor(paintID, texture);
        }
    }
}
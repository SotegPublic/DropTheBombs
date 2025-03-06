using Commands;
using Components;
using DG.Tweening;
using HECSFramework.Core;
using System;
using UnityEngine;

namespace Systems
{
    [Serializable][Documentation(Doc.GameState, Doc.Location, "PlaneGameStateSystem")]
    public sealed class PlaneGameStateSystem : BaseGameStateSystem, IUpdatable
    {
        [Required]
        public PlaneFlyDistance PlaneFlyDistance;
        [Required]
        private BombLevelVariablesComponent levelVariables;
        [Required]
        private PlanesSoundsHolderComponent planesSounds;

        [Single]
        private YandexReceiverSystem yandexSystem;

        private Transform planeTransform;
        private Entity planeEntity;
        private Vector3 planeStartPosition;
        private bool isStateEnd = true;
 
        protected override int State => GameStateIdentifierMap.PlaneGameState;
        
        public override void InitSystem()
        {
        }

        public void UpdateLocal()
        {
            if (isStateEnd)
                return;

            if (planeTransform == null)
                return;

            if (Math.Abs(planeTransform.position.z - planeStartPosition.z) >= PlaneFlyDistance.Distance)
            {
                Owner.World.Command(new SetBombVerticalGameStageCommand { });
                planeEntity.RemoveComponent<InputListenerTagComponent>();
                isStateEnd = true;

                var sequence = DOTween.Sequence();
                sequence.Append(planeTransform.DOMoveY(planeTransform.position.y + 5, 0.5f));
                sequence.Append(planeTransform.DOMoveZ(planeTransform.position.z + 5, 0.5f));

                planeEntity.RemoveComponent<CanAttackTagComponent>();

                var plane = Owner.World.GetEntityBySingleComponent<PlayerPlaneTagComponent>();
                var planeID = Owner.World.GetSingleComponent<PlayerPlaneCustomisationComponent>().CurrentPlaneID;

                if (planesSounds.TryGetSoundByID(planeID, out var clip))
                {
                    Owner.World.Command(new StopSoundCommand { Owner = plane.GUID, Clip = clip });
                }

                EndState();
            }
        }

        protected override void ProcessState(int from, int to)
        {
            if (!Owner.World.TryGetEntityByComponent<PlayerPlaneTagComponent>(out var entity))
                throw new Exception("no entity with PlayerPlaneTagComponent");

            var playerProgress = Owner.World.GetSingleComponent<PlayerProgressComponent>();
            yandexSystem.YandexReceiver.SetLeaderBoardValue("BombPower", playerProgress.BombPower);
            yandexSystem.YandexReceiver.YandexDebug("we send leader board power info");

            Owner.World.Command(new HideUICommand { UIViewType = UIIdentifierMap.UpgradesPanel_UIIdentifier });
            Owner.World.Command(new HideUICommand { UIViewType = UIIdentifierMap.StartBombButtonScreen_UIIdentifier });

            Owner.World.Command(new CustomHideUICommand { UIViewType = UIIdentifierMap.SettingsButton_UIIdentifier });            
            Owner.World.Command(new CustomHideUICommand { UIViewType = UIIdentifierMap.LevelProgressPanel_UIIdentifier });
            Owner.World.Command(new CustomHideUICommand { UIViewType = UIIdentifierMap.HubElementsPanel_UIIdentifier });

            Owner.World.Command(new ShowUICommand { UIViewType = UIIdentifierMap.TutorialPanel_UIIdentifier });
            Owner.World.Command(new ShowUICommand { UIViewType = UIIdentifierMap.StarsProgressPanel_UIIdentifier });
            Owner.World.Command(new ShowUICommand { UIViewType = UIIdentifierMap.BombsInfoPanel_UIIdentifier });
            Owner.World.Command(new CalculateStarsProgressStepCommand { GatesCount = 8 });   // todo положить ворота сюда

            PlaneFlyDistance.Distance = Vector3.Distance(levelVariables.DropBombPosition, levelVariables.StartLevelPosition);

            isStateEnd = false;

            Owner.World.Command(new SetPlaneGameStageCommand { });

            planeEntity = entity;

            planeTransform = planeEntity.GetComponent<UnityTransformComponent>().Transform;
            planeEntity.AddComponent<InputListenerTagComponent>();
            planeStartPosition = planeTransform.position;

            planeEntity.AddComponent<CanAttackTagComponent>();
        }
    }
}
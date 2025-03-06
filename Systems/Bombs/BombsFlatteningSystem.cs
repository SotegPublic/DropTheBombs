using System;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.Bombs, Doc.Movement, "This system flattens bombs as the player slides left or right")]
    public sealed class BombsFlatteningSystem : BaseSystem, ILateUpdatable, IReactGlobalCommand<TransitionGameStateCommand>, IGlobalStart
    {
        [Required]
        private ProgressComponent progress;
        [Required]
        private FlatteningSystemParametersComponent parameters;

        private EdgeComponent edge;
        private EntitiesFilter bombs;
        private float currentTime;
        private Vector3 bombsHolderStartPosition;

        private bool isFallFlatteningActive;
        private bool isHorizontalFlatteningActive;
        private bool isScatteringActive;

        public void GlobalStart()
        {
            edge = Owner.World.GetSingleComponent<EdgeComponent>();
        }

        public void CommandGlobalReact(TransitionGameStateCommand command)
        {
            if(command.To == GameStateIdentifierMap.BombVerticalGameState)
            {
                isScatteringActive = true;
            }

            if(command.From == GameStateIdentifierMap.BombVerticalGameState)
            {
                isFallFlatteningActive = false;
                Owner.RemoveComponent<InputListenerTagComponent>();
                isHorizontalFlatteningActive = true;
            }
        }

        public override void InitSystem()
        {
            bombs = Owner.World.GetFilter(Filter.Get<BombTagComponent>());
        }

        public void UpdateLateLocal()
        {
            if (isScatteringActive)
            {
                currentTime += Time.deltaTime;
                progress.SetValue(currentTime / parameters.DropScatteringTime);

                foreach (var bomb in bombs)
                {
                    var positions = bomb.GetComponent<BombEngagePositionComponent>();

                    var newPosition = Vector3.Lerp(positions.UnderPlanePosition, positions.FallPosition, progress.Value);
                    bomb.GetTransform().localPosition = newPosition;
                }

                if (progress.Value >= 1)
                {
                    isScatteringActive = false;
                    isFallFlatteningActive = true;
                    currentTime = 0;
                    progress.SetValue(0);

                    Owner.AddComponent<InputListenerTagComponent>();
                }
            }

            if(isFallFlatteningActive)
            {
                var bombHolderView = Owner.GetComponent<ViewReadyTagComponent>().View;
                var currentHolderPosition = bombHolderView.transform.localPosition;

                var normalized = Math.Abs(currentHolderPosition.y / edge.Edge) * parameters.FlatteringCoeff;

                foreach (var bomb in bombs)
                {
                    var startPos = bomb.GetComponent<BombEngagePositionComponent>().FallPosition;

                    bomb.GetTransform().localPosition = new Vector3
                    (
                        startPos.x *= (1 + normalized / 2),
                        startPos.y *= (1 - normalized / 2),
                        startPos.z
                    );
                }
            }

            if (isHorizontalFlatteningActive)
            {
                var bombsHolderView = Owner.GetComponent<ViewReadyTagComponent>().View;

                if (bombsHolderStartPosition == null)
                {
                    bombsHolderStartPosition = bombsHolderView.transform.localPosition;
                }

                currentTime += Time.deltaTime;
                progress.SetValue(currentTime / parameters.FinishScatteringTime);
                var endHolderPosition = new Vector3(0, 0, 0);

                var newPosition = Vector3.Lerp(bombsHolderStartPosition, endHolderPosition, progress.Value);
                bombsHolderView.transform.localPosition = newPosition;

                var currentFlatteningY = edge.Edge * progress.Value;

                var normalized = Math.Abs(currentFlatteningY / edge.Edge) * parameters.FlatteringCoeff;

                foreach (var bomb in bombs)
                {
                    var startPos = bomb.GetComponent<BombEngagePositionComponent>().FallPosition;

                    bomb.GetTransform().localPosition = new Vector3
                    (
                        startPos.x *= (1 + (normalized)),
                        startPos.y *= (1 - normalized / 1.6f),
                        startPos.z
                    );
                }

                if (progress.Value >= 1)
                {
                    isHorizontalFlatteningActive = false;
                    currentTime = 0;
                    progress.SetValue(0);
                }
            }
        }
    }
}
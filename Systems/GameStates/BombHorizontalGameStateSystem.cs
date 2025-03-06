using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;
using DG.Tweening;

namespace Systems
{
	[Serializable][Documentation(Doc.GameState, Doc.Location, "BombHorizontalGameStateSystem")]
    public sealed class BombHorizontalGameStateSystem : BaseGameStateSystem, IReactGlobalCommand<GoToEndStateCommand>
    {
        [Required]
        public BombHorizontalFlyDistance BombHorizontalFlyDistance;
        [Required]
        private FinishSpeedComponent finishSpeed;

        private Transform bombTransform;
        private Entity bombEntity;

        protected override int State => GameStateIdentifierMap.BombHorizontalGameState;

        public override void InitSystem()
        {
        }

        protected override void ProcessState(int from, int to)
        {
            Owner.World.GetEntityBySingleComponent<PlayerBombTagComponent>().GetComponent<MoveSpeedComponent>().MoveSpeed = finishSpeed.MoveSpeed;

            if (Owner.World.TryGetEntityByComponent<PlayerBombTagComponent>(out var entity))
            {
                bombEntity = entity;
            }
            else
            {
                throw new Exception("no entity with PlayerBombTagComponent");
            }

            bombTransform = bombEntity.GetComponent<UnityTransformComponent>().Transform;
            bombTransform.DORotate(Vector3.zero, 0.5f);

            var bombParticle = bombEntity.AsActor().GetComponentInChildren<ParticleSystem>();
            bombParticle.Stop();
        }

        public void CommandGlobalReact(GoToEndStateCommand command)
        {
            EndState();
        }
    }
}
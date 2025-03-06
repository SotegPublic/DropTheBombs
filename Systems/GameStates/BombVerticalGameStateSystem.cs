using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;
using DG.Tweening;

namespace Systems
{
	[Serializable][Documentation(Doc.GameState, Doc.Location, "BombVerticalGameStateSystem")]
    public sealed class BombVerticalGameStateSystem : BaseGameStateSystem, IUpdatable
    {
        [Required]
        public BombVerticalFlyDistance BombVerticalFlyDistance;
        [Required]
        private BombLevelVariablesComponent levelVariables;
        [Required]
        private BombLevelHolder levelHolder;
        [Required]
        private DropDownSoundComponent dropDownSound;

        private Transform bombTransform;
        private Vector3 bombStartPosition;
        private Entity bombEntity;
        private bool isStateEnd = true;

        protected override int State => GameStateIdentifierMap.BombVerticalGameState;
                
        public override void InitSystem()
        {

        }

        public void UpdateLocal()
        {
            if (isStateEnd)
                return;

            if (bombTransform == null)
                return;

            if (Math.Abs(bombTransform.position.y - bombStartPosition.y) >= BombVerticalFlyDistance.Distance)
            {
                Owner.World.Command(new SetBombHorisontalGameStateCommand { });
                isStateEnd = true;

                EndState();
            }
        }

        protected override void ProcessState(int from, int to)
        {
            Owner.World.Command(new PlaySoundCommand { AudioType = SoundType.FX, IsRepeatable = false, Clip = dropDownSound.Clip, Owner = Owner.GUID });

            var endPosition = levelHolder.BombLevel.GetComponent<BombLevelMonoComponent>().FinishTransform.position;
            BombVerticalFlyDistance.Distance = Vector3.Distance(endPosition, levelVariables.DropBombPosition);

            isStateEnd = false;

            if (Owner.World.TryGetEntityByComponent<PlayerBombTagComponent>(out var entity))
            {
                bombEntity = entity;
            }
            else
            {
                throw new Exception("no entity with PlayerBombTagComponent");
            }

            bombTransform = bombEntity.GetComponent<UnityTransformComponent>().Transform;
            bombStartPosition = bombTransform.position;
            bombTransform.DOLocalMoveX(0, 2f);
            bombTransform.DOLocalMoveZ(levelVariables.DropBombPosition.z, 2f);
            var sequence = bombTransform.DORotate(new Vector3(90,0,0), 2f);
            sequence.OnComplete(() =>
            {
                var bombParticle = bombEntity.AsActor().GetComponentInChildren<ParticleSystem>();
                bombParticle.Play();
            });

            bombTransform.parent = null;
        }
    }
}
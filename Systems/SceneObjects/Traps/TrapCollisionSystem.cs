using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;
using Helpers;

namespace Systems
{
	[Serializable][Documentation(Doc.Traps, "TrapCollisionSystem")]
    public sealed class TrapCollisionSystem : BaseSystem, IReactCommand<TriggerEnterCommand>
    {
        [Required]
        private TargetTagHolderComponent targetTag;
        [Required]
        private TrapParametersHolder trapParameters;
        [Required]
        private SoundHolderComponent soundHolder;

        public void CommandReact(TriggerEnterCommand command)
        {
            if (command.Collider.TryGetActorFromCollision(out var actor))
            {
                var currentState = Owner.World.GetSingleComponent<GameStateComponent>().CurrentState;

                if ((actor.Entity.ContainsMask<PlayerPlaneTagComponent>() && currentState == GameStateIdentifierMap.PlaneGameState) ||
                    (actor.Entity.ContainsMask<PlayerBombTagComponent>() && currentState == GameStateIdentifierMap.BombVerticalGameState))
                {
                    Owner.World.GetEntityBySingleComponent<PlayerTagComponent>().Command(new UpdateBombsCountCommand
                    {
                        Count = trapParameters.DestroyBombsCount,
                        CalculationType = ModifierCalculationType.Subtract
                    });

                    Owner.World.Command(new PlaySoundCommand { AudioType = SoundType.FX, Clip = soundHolder.Clip, IsRepeatable = false, Owner = Owner.GUID });
                    Owner.World.Command(new SpawnFXToCoordCommand { Coord = command.Collider.transform.position, FXId = FXIdentifierMap.LandHit });

                    Owner.Command(new ResetEntityCommand());
                    Owner.World.Command(new DestroyEntityWorldCommand { Entity = Owner });
                }
            }
        }

        public override void InitSystem()
        {
        }
    }
}
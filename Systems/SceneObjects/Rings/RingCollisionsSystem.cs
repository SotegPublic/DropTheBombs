using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;
using Helpers;

namespace Systems
{
	[Serializable][Documentation(Doc.Rings, "this system check player collisions with rings")]
    public sealed class RingCollisionsSystem : BaseSystem, IReactCommand<TriggerEnterCommand>
    {
        [Required]
        private RingTagComponent tagComponent;
        [Required]
        private TargetTagHolderComponent targetTag;
        [Required]
        private RingParametersHolderComponent ringParameters;
        [Required]
        private RingSoundsComponent sounds;

        public void CommandReact(TriggerEnterCommand command)
        {
            if(command.Collider.TryGetActorFromCollision(out var actor))
            {
                var currentState = Owner.World.GetSingleComponent<GameStateComponent>().CurrentState;

                if ((actor.Entity.ContainsMask<PlayerPlaneTagComponent>() && currentState == GameStateIdentifierMap.PlaneGameState) ||
                    (actor.Entity.ContainsMask<PlayerBombTagComponent>() && currentState == GameStateIdentifierMap.BombVerticalGameState))
                {

                    if (tagComponent.RingId == RingIdentifierMap.Power || tagComponent.RingId == RingIdentifierMap.BadPower)
                    {
                        Owner.World.GetEntityBySingleComponent<PlayerTagComponent>().Command(new UpdatePowerCommand
                        {
                            Value = ringParameters.RingOperationValue,
                            CalculationType = ringParameters.RightRingOperatoinType
                        });
                    }

                    if (tagComponent.RingId == RingIdentifierMap.Count || tagComponent.RingId == RingIdentifierMap.BadCount)
                    {
                        Owner.World.GetEntityBySingleComponent<PlayerTagComponent>().Command(new UpdateBombsCountCommand
                        {
                            Count = ringParameters.RingOperationValue,
                            CalculationType = ringParameters.RightRingOperatoinType
                        });
                    }

                    if (tagComponent.RingId == RingIdentifierMap.Power || tagComponent.RingId == RingIdentifierMap.Count)
                    {
                        Owner.World.Command(new PlaySoundCommand { AudioType = SoundType.FX, IsRepeatable = false, Clip = sounds.Positive, Owner = Owner.GUID });
                        Owner.World.Command(new AddStarsProgressCommand());
                    }
                    else
                    {
                        Owner.World.Command(new PlaySoundCommand { AudioType = SoundType.FX, IsRepeatable = false, Clip = sounds.Negative, Owner = Owner.GUID });
                    }

                    Owner.Command(new BumpRingCommand());
                }

                if (actor.Entity.ContainsMask<PlaneBulletTagComponent>())
                {
                    Owner.World.Command(new PlaySoundCommand { AudioType = SoundType.FX, IsRepeatable = false, Clip = sounds.Hit, Owner = Owner.GUID });
                    actor.Entity.Command(new DestroyBulletCommand());
                    Owner.Command(new UpdateRingCommand());

                }
            }
        }

        public override void InitSystem()
        {
        }
    }
}
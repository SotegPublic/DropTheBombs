using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;
using Helpers;

namespace Systems
{
	[Serializable][Documentation(Doc.Pilons, "by this sistem we react on collisions")]
    public sealed class CollisionsPilonSystem : BaseSystem, IReactCommand<TriggerEnterCommand>
    {
        [Required]
        private PilonTagComponent tag;
        [Required]
        private PilonMonoComponentHolderComponent monobehHolder;
        [Required]
        private HealthComponent healthComponent;

        public void CommandReact(TriggerEnterCommand command)
        {
            if(command.Collider.TryGetActorFromCollision(out var actor))
            {
                if (!actor.Entity.ContainsMask<PlayerBombTagComponent>())
                    return;

                var bombsEntity = actor.Entity;
                bombsEntity.Command(new StartBombingCommand { TargetHealth = healthComponent.Value, BombsTargets = monobehHolder.Monocomponent.TargetTransforms, Target = Owner });
                monobehHolder.Monocomponent.Canvas.sortingOrder = 7;

                if(tag.PilonID == PilonIdentifierMap.Pilon_12)
                {
                    Owner.World.Command(new ZoomOutCameraCommand());
                    Owner.World.Command(new BossTauntCommand());
                }
            }
        }

        public override void InitSystem()
        {
        }
    }
}
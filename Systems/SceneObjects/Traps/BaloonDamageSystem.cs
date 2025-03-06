using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;
using Unity.Mathematics;
using Helpers;

namespace Systems
{
	[Serializable][Documentation(Doc.Traps, Doc.Damage, "this system registers damages to the baloon")]
    public sealed class BaloonDamageSystem : BaseSystem, IReactCommand<TriggerEnterCommand>, IReactCommand<DamageForVisualFXCommand>
    {
        [Required]
        private HealthComponent healthComponent;

        public void CommandReact(TriggerEnterCommand command)
        {
            if (command.Collider.TryGetActorFromCollision(out var actor))
            {
                if (!actor.Entity.ContainsMask<PlaneBulletTagComponent>())
                    return;

                if (Owner.GetComponent<TrapTagComponent>().TrapID == TrapIdentifierMap.TNTBaloon)
                {
                    var maxHP = healthComponent.MaxValue;
                    Owner.Command(new DamageCommand<float> { DamageValue = maxHP / 3 });
                }
            }
        }

        public void CommandReact(DamageForVisualFXCommand command)
        {
            var amount = healthComponent.Value / healthComponent.MaxValue;

            Owner.AsActor().GetComponentInChildren<BalloondMonoComponent>().ChangeFillAmount(amount);
        }

        public override void InitSystem()
        {
        }
    }
}
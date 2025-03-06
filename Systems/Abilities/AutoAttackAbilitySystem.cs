using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;

namespace Systems
{
	[Serializable][Documentation(Doc.Abilities, Doc.Plane, "plane auto attack ability")]
    public sealed class AutoAttackAbilitySystem : BaseAbilitySystem
    {
        [Required]
        private ActionsHolderComponent actionsHolderComponent;
        [Required]
        private BulletContainerHolderComponent bulletContainerHolder;
        [Required]
        private ParticleIdentifierHolderComponent particleIdentifierHolderComponent;

        public override void Execute(Entity owner = null, Entity target = null, bool enable = true)
        {
            actionsHolderComponent.ExecuteAction(ActionIdentifierMap.OnExecute);
            GetBulletFromPoolAcync();
        }

        public override void InitSystem()
        {
        }

        private async void GetBulletFromPoolAcync()
        {
            var spawnPositions = Owner.GetComponent<AbilityOwnerComponent>().AbilityOwner.AsActor().GetComponentsInChildren<SpawnParticlePositionMonoComponent>();
            Vector3 spawnPosition = Vector3.zero;

            for (int i = 0; i < spawnPositions.Length; i++)
            {
                if (spawnPositions[i].ParticleIdentifier == particleIdentifierHolderComponent.ParticleIdentifier)
                {
                    spawnPosition = spawnPositions[i].Transform.position;
                    break;
                }
            }

            var bullet = await bulletContainerHolder.BulletContainer.GetActor(position: spawnPosition);

            bullet.GetHECSComponent<StartPositionHolderComponent>().StartPosition = spawnPosition;

            bullet.Init();
        }
    }
}
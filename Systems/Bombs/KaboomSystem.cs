using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.Bombs, "blow up this, Rico")]
    public sealed class KaboomSystem : BaseSystem, IReactCommand<MoveByCurveEndedCommand>
    {
        [Required]
        private BombTagComponent tag;
        [Required]
        private BombTargetComponent bombTargetHolder;
        [Required]
        private BombFXIdHolderComponent bombFX;

        public void CommandReact(MoveByCurveEndedCommand command)
        {
            var damage = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>().GetComponent<BombPowerComponent>().Value;

            if (bombTargetHolder.Target.IsAliveAndNotDead()) 
            {
                bombTargetHolder.Target.Command(new DamageCommand<float> { DamageValue = damage });
            }

            Owner.World.Command(new SpawnFXToCoordCommand { Coord = Owner.GetPosition(), FXId = bombFX.BombFXId });

            var soundSystem = Owner.World.GetEntityBySingleComponent<PlayerBombTagComponent>().GetSystem<KaboomSoundSystem>();
            soundSystem.PlayBombFX(tag.BombID);
            Owner.Command(new PlayLocalVFXCommand { Enable = false, ID = FXIdentifierMap.FireTrail });

            Owner.World.GetEntityBySingleComponent<PlayerBombTagComponent>().GetComponent<VisualLocalLockComponent>().Remove();

            var bombsPool = Owner.World.GetSingleComponent<BombsPoolComponent>();
            bombsPool.ReturnBomb(Owner.AsActor());
        }

        public override void InitSystem()
        {
        }
    }
}
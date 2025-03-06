using System;
using HECSFramework.Core;
using Components;
using Commands;
using HECSFramework.Unity;

namespace Systems
{
    [Serializable][Documentation(Doc.Damage, Doc.Traps, "this system killing baloon")]
    public sealed class BaloonDieSystem : BaseSystem, IReactCommand<IsDeadCommand> 
    {
        [Required]
        private SoundHolderComponent soundHolder;

        public void CommandReact(IsDeadCommand command)
        {
            Owner.World.Command(new PlaySoundCommand { AudioType = SoundType.FX, Clip = soundHolder.Clip, IsRepeatable = false, Owner = Owner.GUID });
            Owner.World.Command(new SpawnFXToCoordCommand { Coord = Owner.GetPosition(), FXId = FXIdentifierMap.LandHit });

            Owner.Command(new ResetEntityCommand());

            Owner.World.Command(new DestroyEntityWorldCommand { Entity = Owner });
        }

        public override void InitSystem()
        {
        }
    }
}
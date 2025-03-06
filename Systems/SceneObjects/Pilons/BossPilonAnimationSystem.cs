using System;
using HECSFramework.Core;
using Commands;
using Components;

namespace Systems
{
    [Serializable][Documentation(Doc.Animation, Doc.Pilons, "this system controll boss animation")]
    public sealed class BossPilonAnimationSystem : BaseSystem, IReactCommand<DamageForVisualFXCommand>, IReactCommand<IsDeadCommand>, IReactGlobalCommand<BossTauntCommand>
    {
        public void CommandGlobalReact(BossTauntCommand command)
        {
            Owner.Command(new TriggerAnimationCommand { Index = AnimParametersMap.taunt });

            var bossType = Owner.GetComponent<BossTypeComponent>().Type;
            var animSound = Owner.World.GetSingleComponent<SoundsHolderComponent>().GetSoundByID((int)bossType);

            Owner.World.Command(new PlaySoundCommand { AudioType = SoundType.FX, Clip = animSound, IsRepeatable = false, Owner = Owner.GUID });
        }

        public void CommandReact(DamageForVisualFXCommand command)
        {
            Owner.Command(new TriggerAnimationCommand { Index = AnimParametersMap.hurt });
        }

        public void CommandReact(IsDeadCommand command)
        {
            Owner.Command(new TriggerAnimationCommand { Index = AnimParametersMap.dead });
        }

        public override void InitSystem()
        {
        }
    }
}
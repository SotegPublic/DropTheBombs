using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.Sound, "MuteSoundControllerSystem")]
    public sealed class MuteSoundControllerSystem : BaseSystem, IReactGlobalCommand<ShowAdvCommand>,
        IReactGlobalCommand<AdvClosedCommand>, IReactGlobalCommand<RewardedAdvClosedCommand>, IReactGlobalCommand<ChangeSoundSettingsCommand>
    {
        private PlayerProgressComponent playerProgress;

        public void CommandGlobalReact(ShowAdvCommand command)
        {
            if (playerProgress.IsSoundOff)
                return;

            AudioListener.volume = 0f;
        }

        public void CommandGlobalReact(AdvClosedCommand command)
        {
            if(playerProgress.IsSoundOff)
                return;

            AudioListener.volume = 1f;
        }

        public void CommandGlobalReact(RewardedAdvClosedCommand command)
        {
            if(playerProgress.IsSoundOff)
                return;

            AudioListener.volume = 1f;
        }

        public void CommandGlobalReact(ChangeSoundSettingsCommand command)
        {
            AudioListener.volume = command.IsSoundOn ? 1 : 0;
        }

        public override void InitSystem()
        {
            playerProgress = Owner.World.GetSingleComponent<PlayerProgressComponent>();
        }
    }
}
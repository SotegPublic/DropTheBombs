using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.Bombs, Doc.Sound, "this system controlls bombs sounds")]
    public sealed class KaboomSoundSystem : BaseSystem, IUpdatable
    {
        [Required]
        private BombsSoundsHolderComponent bombsSounds;

        private float delay;
        private bool isCanStartSound = true;
        private float currentTime;

        public override void InitSystem()
        {
        }

        public void PlayBombFX(int bombID)
        {
            if(isCanStartSound)
            {
                if(bombsSounds.TryGetClipByBombID(bombID, out var clip))
                {
                    delay = clip.length * 0.3f;
                    isCanStartSound = false;

                    Owner.World.Command(new PlaySoundCommand { AudioType = SoundType.FX, IsRepeatable = false, Clip = clip, Owner = Owner.GUID });
                }
            }
        }

        public void UpdateLocal()
        {
            if (isCanStartSound)
                return;

            currentTime += Time.deltaTime;

            if(currentTime >= delay)
            {
                currentTime = 0;
                isCanStartSound = true;
            }
        }
    }
}
using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.UI, Doc.Pilons, "this system controlled pilon ui")]
    public sealed class PilonUISystem : BaseSystem, IReactCommand<DamageForVisualFXCommand>, IReactCommand<PilonRiseEndCommand>, IReactCommand<IsDeadCommand>, IUpdatable
    {
        [Required]
        private PilonTagComponent tag;
        [Required]
        private PilonMonoComponentHolderComponent monobehHolder;
        [Required]
        private HealthComponent healthComponent;
        [Required]
        private PilonUIParametersComponent parameters;

        private bool isActive;
        private Vector3 startPosition;
        private float currentTime;

        public void CommandReact(DamageForVisualFXCommand command)
        {
            var newValue = healthComponent.Value < 0 ? "0" : ((int)healthComponent.Value).ToString();
            monobehHolder.Monocomponent.HealthText.text = newValue;
            monobehHolder.Monocomponent.HealthImage.fillAmount = healthComponent.Value / healthComponent.MaxValue;
        }

        public void CommandReact(PilonRiseEndCommand command)
        {
            monobehHolder.Monocomponent.CanvasGroup.alpha = 1;
            monobehHolder.Monocomponent.CanvasGroup.blocksRaycasts = true;
        }

        public void CommandReact(IsDeadCommand command)
        {
            int reward = 0;

            var bossIndex = Owner.World.GetSingleComponent<PlayerProgressComponent>().BossIndex;

            if (tag.PilonID == PilonIdentifierMap.BossPilon)
            {
                reward = (int)Owner.World.GetSingleComponent<CurrencyCalculationsConfigComponent>().GetBossReward(bossIndex);
            }
            else
            {
                reward = (int)Owner.World.GetSingleComponent<CurrencyCalculationsConfigComponent>().GetPilonReward(bossIndex);
            }
            

            startPosition = monobehHolder.Monocomponent.GainedCurrencyRect.position;

            monobehHolder.Monocomponent.GainedCurrencyText.text = "+" + reward.ToString();

            monobehHolder.Monocomponent.GainedCurrencyRect.gameObject.SetActive(true);
            isActive = true;
        }

        public override void InitSystem()
        {
        }

        public void UpdateLocal()
        {
            if (!isActive)
                return;

            currentTime += Time.deltaTime;
            var newPos = monobehHolder.Monocomponent.GainedCurrencyRect.position;
            newPos.y += parameters.RiseSpeed * Time.deltaTime;
            monobehHolder.Monocomponent.GainedCurrencyRect.position = newPos;

            if(currentTime > parameters.RiseTime)
            {
                isActive = false;
                currentTime = 0;
                monobehHolder.Monocomponent.GainedCurrencyRect.gameObject.SetActive(false);
                monobehHolder.Monocomponent.GainedCurrencyRect.position = startPosition;
            }
        }
    }
}
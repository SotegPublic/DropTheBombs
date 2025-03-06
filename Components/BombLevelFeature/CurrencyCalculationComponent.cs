using HECSFramework.Core;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Level, Doc.Config, Doc.Currency, "here we hold base parameters for currency calculation system")]
    public sealed class CurrencyCalculationsConfigComponent : BaseComponent, IWorldSingleComponent
    {
        [Header("Pilons settings")]
        [SerializeField] private float baseRewardForPilon;
        [SerializeField] private float baseRewardForBoss;
        [SerializeField] private CurrencyModifier[] pilonModifiers;
        [SerializeField] private CurrencyModifier[] bossModifiers;

        [Header("BossLevel Enemies settings")]
        [SerializeField] private int baseRewardForKill;

        public int BaseRewardForKill => baseRewardForKill;

        public float GetPilonReward(int bossIndex)
        {
            var reward = baseRewardForPilon;

            for (int i = 1; i <= bossIndex; i++)
            {
                var modifier = GetRewardModifier(pilonModifiers, i);
                reward += modifier;
            }

            return reward;
        }

        public float GetBossReward(int level)
        {
            var reward = baseRewardForBoss;

            for (int i = 1; i <= level; i++)
            {
                var modifier = GetRewardModifier(bossModifiers, i);
                reward += modifier;
            }

            return reward;
        }

        private float GetRewardModifier(CurrencyModifier[] modifierConfigs, int level)
        {
            for (int i = modifierConfigs.Length - 1; i >= 0; i--)
            {
                if (level >= modifierConfigs[i].LevelEdge)
                {
                    return modifierConfigs[i].Modifier;
                }
            }

            return modifierConfigs[1].Modifier;
        }
    }
}
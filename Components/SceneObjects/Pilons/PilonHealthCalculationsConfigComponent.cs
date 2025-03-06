using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Level, Doc.Config, Doc.Pilons, "here we hold base parameters for pilon health calculation system")]
    public sealed class PilonHealthCalculationsConfigComponent : BaseComponent, IWorldSingleComponent
    {
        [SerializeField] private PilonBaseConfig[] pilonConfigs;
        [SerializeField] private PilonHealthModifierConfig[] pilonModifiers;

        public float GetPilonHealth(int pilonID, int bossIndex)
        {
            var health = 0f;
            var baseHealth = GetBaseHealth(pilonID);
            health += baseHealth;

            for(int i = 1; i <= bossIndex; i++)
            {
                var modifier = GetHealthModifier(i);
                health *= modifier;
            }

            return health;
        }

        public float GetPilonXModifier(int pilonID)
        {
            for(int i = 0; i < pilonConfigs.Length; i++)
            {
                if (pilonConfigs[i].PilonID == pilonID)
                {
                    return pilonConfigs[i].PilonXModifier;
                }
            }

            throw new Exception("bad pilon ID = " + pilonID);
        }

        private float GetHealthModifier(int index)
        {
            for(int i = pilonModifiers.Length - 1; i >= 0; i--)
            {
                if(index >= pilonModifiers[i].BossIndex)
                {
                    return pilonModifiers[i].Modifier;
                }
            }

            return pilonModifiers[1].Modifier;
        }

        private float GetBaseHealth(int pilonID)
        {
            for(int i = 0; i < pilonConfigs.Length; i++)
            {
                if (pilonConfigs[i].PilonID == pilonID)
                {
                    return pilonConfigs[i].PilonHealth;
                }
            }

            throw new Exception("unknown pilon id");
        }
    }
}
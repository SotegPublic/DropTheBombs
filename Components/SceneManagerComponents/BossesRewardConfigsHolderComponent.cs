using HECSFramework.Core;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Holder, Doc.Level, Doc.Rewards, "here we setup rewards for bosses in bomb and boss levels")]
    public sealed class BossesRewardConfigsHolderComponent : BaseComponent
    {
        [SerializeField] private int simpleBossReward;
        [SerializeField] private RewardConfig[] bossLevelRewards;
        [SerializeField] private RewardConfig[] bombLevelRewards;

        public int SimpleBossReward => simpleBossReward;
        public RewardConfig[] BossLevelRewards => bossLevelRewards;
        public RewardConfig[] BombLevelRewards => bombLevelRewards;
    }
}
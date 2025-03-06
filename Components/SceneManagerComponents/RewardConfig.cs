using Helpers;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Components
{
    [Serializable]
    public class RewardConfig
    {
        [SerializeField] private RewardTypes rewardItemType;
        [SerializeField][IdentifierDropDown(nameof(PlaneIdentifier))][ShowIf(nameof(IsPlaneView))] int planeID;
        [SerializeField][IdentifierDropDown(nameof(PlanePaintIdentifier))][ShowIf(nameof(IsPlaneWithTexture))] int textureID;
        [SerializeField][ShowIf(nameof(IsCurrency))] private int currencyRewardCount;

        public int PlaneID => planeID;
        public int TextureID => textureID;
        public RewardTypes RewardItemType => rewardItemType;
        public int CurrencyRewardCount => currencyRewardCount;

        private bool IsPlaneView()
        {
            return (rewardItemType == RewardTypes.Plane) || (rewardItemType == RewardTypes.PlaneWithSkin) ? true : false;
        }

        private bool IsPlaneWithTexture()
        {
            return rewardItemType == RewardTypes.PlaneWithSkin ? true : false;
        }

        private bool IsCurrency()
        {
            return rewardItemType == RewardTypes.Currency ? true : false;
        }
    }
}
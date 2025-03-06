using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Components
{
    [Serializable]
    public class BossPilonConfig
    {
        [SerializeField] private AssetReferenceGameObject bossAssetRef;
        [SerializeField] private BossesTypes bossType;

        public AssetReferenceGameObject BossAssetRef => bossAssetRef;
        public BossesTypes BossType => bossType;
    }
}
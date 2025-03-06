using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Components
{
    [Serializable][Documentation(Doc.Level, Doc.Holder, "here we hold level configs")]
    public sealed class LevelsHolderComponent : BaseComponent, IWorldSingleComponent
    {
        [SerializeField] private LevelConfig[] levelConfigs;
        [SerializeField] private AssetReference defaultLevelPrefReference;

        public AssetReference DefaultLevelPrefReference => defaultLevelPrefReference;

        public LevelConfig GetLevelConfigByIndex(int index)
        {
            return levelConfigs[index];
        }
    }
}
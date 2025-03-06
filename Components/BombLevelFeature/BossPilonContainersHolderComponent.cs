using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Holder, Doc.Pilons, "here we hold boss pilon containers")]
    public sealed class BossPilonContainersHolderComponent : BaseComponent
    {
        [SerializeField] private BossPilonConfig[] bossPilonConfigs;

       public BossPilonConfig GetBossPilonContainerByBossIndex(int index)
        {
            var newIndex = index % bossPilonConfigs.Length == 0 ? bossPilonConfigs.Length - 1 : (index % bossPilonConfigs.Length) - 1;

            return bossPilonConfigs[newIndex];
        }
    }
}
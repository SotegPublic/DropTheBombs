using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Holder, "here we hold traps containers")]
    public sealed class TrapsContainersHolderComponent : BaseContainerHolderComponent<TrapTagComponent>
    {
        public EntityContainer GetTrapByID(int trapID)
        {
            for (int i = 0; i < containers.Length; i++)
            {
                if (containers[i].GetComponent<TrapTagComponent>().TrapID == trapID)
                {
                    return containers[i];
                }
            }

            return null;
        }
    }
}
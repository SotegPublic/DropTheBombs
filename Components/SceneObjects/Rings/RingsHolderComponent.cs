using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Holder, "here we hold ring containers")]
    public sealed class RingsHolderComponent : BaseContainerHolderComponent<RingTagComponent>
    {
        public EntityContainer GetRingByID(int ringID, ModifierCalculationType operationType)
        {
            var validRingID = ValidateRingID(ringID, operationType);

            for (int i = 0; i < containers.Length; i++)
            {
                if (containers[i].GetComponent<RingTagComponent>().RingId == validRingID)
                {
                    return containers[i];
                }
            }

            return null;
        }

        private int ValidateRingID(int ringID, ModifierCalculationType operationType)
        {
            var isBadOperation = operationType == ModifierCalculationType.Subtract || operationType == ModifierCalculationType.Divide ? true : false;

            if(isBadOperation)
            {
                if(ringID == RingIdentifierMap.Power)
                {
                    return RingIdentifierMap.BadPower;
                }

                if(ringID == RingIdentifierMap.Count)
                {
                    return RingIdentifierMap.BadCount;
                }
            }
            else
            {
                if (ringID == RingIdentifierMap.BadPower)
                {
                    return RingIdentifierMap.Power;
                }

                if (ringID == RingIdentifierMap.BadCount)
                {
                    return RingIdentifierMap.Count;
                }
            }

            return ringID;
        }
    }
}
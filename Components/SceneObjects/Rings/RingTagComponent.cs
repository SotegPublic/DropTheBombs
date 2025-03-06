using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;
using Helpers;

namespace Components
{
    [Serializable][Documentation(Doc.Tag, "ring tag component")]
    public sealed class RingTagComponent : BaseComponent
    {
        [SerializeField, IdentifierDropDown(nameof(RingIdentifier))] private int ringId;

        public int RingId => ringId;

        public void ChangeRingID(int newRingId)
        {
            ringId = newRingId;
        }
    }
}
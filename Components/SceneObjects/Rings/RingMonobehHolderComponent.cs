using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Holder, "here we hold ring monocomponent")]
    public sealed class RingMonobehHolderComponent : BaseComponent
    {
        public RingMonoComponent Monocomponent;
    }
}
using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Rings, "here we hold target tag name")]
    public sealed class TargetTagHolderComponent : BaseComponent
    {
        public string TargetTag;
    }
}
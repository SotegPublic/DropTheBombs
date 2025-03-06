using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Level, "by this tag we mark cleanable sceen objects")]
    public sealed class CleanableObjectTagComponent : BaseComponent
    {
    }
}
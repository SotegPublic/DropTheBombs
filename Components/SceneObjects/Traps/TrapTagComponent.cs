using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;
using Helpers;

namespace Components
{
    [Serializable][Documentation(Doc.Tag, "TrapTagComponent")]
    public sealed class TrapTagComponent : BaseComponent
    {
        [SerializeField, IdentifierDropDown(nameof(TrapIdentifier))] int trapID;

        public int TrapID => trapID;
    }
}
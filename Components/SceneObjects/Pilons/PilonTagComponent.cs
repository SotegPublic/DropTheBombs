using HECSFramework.Core;
using HECSFramework.Unity;
using Helpers;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Tag, "by this tag we mark pilons")]
    public sealed class PilonTagComponent : BaseComponent
    {
        [SerializeField][IdentifierDropDown(nameof(PilonIdentifier))] private int pilonID;

        public int PilonID => pilonID;
    }
}